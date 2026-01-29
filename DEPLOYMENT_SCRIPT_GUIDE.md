# AgPay+ 部署和更新脚本优化指南

> **版本**: 2.0  
> **状态**: ✅ 所有优化已完成  

---

## 📋 目录

- [快速开始](#快速开始)
- [改进概述](#改进概述)
- [使用指南](#使用指南)
- [技术实现](#技术实现)
- [兼容性](#兼容性)
- [故障排查](#故障排查)
- [MySQL 数据迁移](#mysql-数据迁移)
- [最佳实践](#最佳实践)

---

## 🚀 快速开始

### Linux/macOS

```bash
# 部署
./deploy-linux.sh

# 更新指定服务
./update-linux.sh --services "manager-api,agent-api"
```

### Windows

```powershell
# 部署
.\deploy-windows.ps1

# 更新指定服务
.\update-windows.ps1 -Services "manager-api,agent-api"
```

---

## ✅ 改进概述

### 完成状态

| 文件 | 改进项 | 状态 |
|------|--------|------|
| `deploy-linux.sh` | 7 | ✅ 100% |
| `update-linux.sh` | 5 | ✅ 100% |
| `deploy-windows.ps1` | 8 | ✅ 100% |
| `update-windows.ps1` | 7 | ✅ 100% |
| **总计** | **27** | **✅ 100%** |

### 三大核心改进

#### 1. .env 解析健壮性 ✅

**问题**：旧方法 `grep "KEY=" .env | cut -d'=' -f2` 无法处理注释、空格、引号。

**解决方案**：

**Linux/macOS:**
```bash
get_env_value() {
    local key="$1"
    local env_file="${2:-$SCRIPT_DIR/.env}"
    
    if [ ! -f "$env_file" ]; then
        echo ""
        return 1
    fi
    
    local value=$(grep -E "^\s*${key}\s*=" "$env_file" | \
        head -n 1 | \
        sed -e 's/^[[:space:]]*//' \
            -e 's/[[:space:]]*$//' \
            -e "s/^${key}=//" \
            -e 's/^["'\'']//' \
            -e 's/["'\'']*$//' \
            -e 's/#.*//')
    
    # 展开 ~ 为 $HOME
    value="${value/#\~/$HOME}"
    echo "$value"
}
```

**Windows:**
```powershell
function Get-EnvValue {
    param(
        [string]$Key,
        [string]$EnvFile = "$ScriptDir\.env"
    )
    
    if (-not (Test-Path $EnvFile)) {
        return $null
    }
    
    $content = Get-Content $EnvFile -ErrorAction SilentlyContinue
    $line = $content | Where-Object { $_ -match "^\s*$Key\s*=" } | Select-Object -First 1
    
    if ($line) {
        $value = $line -replace "^\s*$Key\s*=", "" `
                      -replace "^[`"']", "" `
                      -replace "[`"']*\s*#.*$", "" `
                      -replace "[`"']*$", ""
        
        $value = $value -replace '\$\{USERPROFILE\}', $env:USERPROFILE
        $value = $value -replace '\$env:USERPROFILE', $env:USERPROFILE
        
        return $value.Trim()
    }
    
    return $null
}
```

**功能**：
- ✅ 处理注释（`# comment`）
- ✅ 处理空格（`KEY = value`）
- ✅ 处理引号（`KEY="value"` 或 `KEY='value'`）
- ✅ 处理多个等号（`KEY=val=ue`）
- ✅ 展开路径（`~` → `$HOME`，`${USERPROFILE}`）

---

#### 2. 避免强制 sudo ✅

**问题**：旧方法强制使用 `sudo`，对 root 用户或有权限的用户不友好。

**解决方案**：

```bash
for dir in "${directories[@]}"; do
    if [ ! -d "$dir" ]; then
        # 先尝试不用 sudo
        if mkdir -p "$dir" 2>/dev/null; then
            echo -e "${GREEN}  ✅ 创建目录: $dir${NC}"
        else
            # 失败时提示需要 sudo
            echo -e "${YELLOW}  需要 sudo 权限创建目录: $dir${NC}"
            sudo mkdir -p "$dir"
            sudo chown -R $(id -u):$(id -g) "$dir"
            echo -e "${GREEN}  ✅ 创建目录: $dir${NC}"
        fi
    else
        # 检查已存在目录的写权限
        if [ -w "$dir" ]; then
            echo -e "${GRAY}  ℹ️ 目录已存在: $dir${NC}"
        else
            echo -e "${YELLOW}  ⚠️ 目录存在但无写权限: $dir${NC}"
            sudo chown -R $(id -u):$(id -g) "$dir" 2>/dev/null || true
        fi
    fi
done
```

**改进**：
- ✅ 先尝试无 sudo 创建
- ✅ 明确提示何时需要 sudo
- ✅ 使用 `$(id -u):$(id -g)` 而非 `$(whoami)`（更可靠）
- ✅ 检查已存在目录权限

---

#### 3. Docker Compose 命令兼容 ✅

**问题**：只检查 `docker compose` (v2)，不兼容 `docker-compose` (v1)。

**解决方案**：

**Linux/macOS:**
```bash
DOCKER_COMPOSE=""
if command -v docker &> /dev/null && docker compose version &> /dev/null 2>&1; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
fi

if [ -z "$DOCKER_COMPOSE" ]; then
    echo -e "${RED}  ❌ Docker Compose 未安装${NC}"
    echo -e "${GRAY}  请安装 Docker Compose v2 (docker compose) 或 v1 (docker-compose)${NC}"
    exit 1
fi
```

**Windows:**
```powershell
$DockerCompose = ""
if (Get-Command docker -ErrorAction SilentlyContinue) {
    try {
        docker compose version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $DockerCompose = "docker compose"
        }
    } catch {}
}

if (-not $DockerCompose) {
    try {
        docker-compose version 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $DockerCompose = "docker-compose"
        }
    } catch {}
}
```

**兼容性**：
- ✅ Docker Compose v2 (内置)
- ✅ Docker Compose v1 (独立安装)
- ✅ Ubuntu 18.04+、CentOS 7+、Windows 10+

---

## 📖 使用指南

### 部署脚本

#### Linux/macOS

```bash
# 完整部署（交互式）
./deploy-linux.sh

# 跳过证书生成
./deploy-linux.sh --skip-cert

# 跳过环境配置
./deploy-linux.sh --skip-env

# 完全非交互（CI/CD 友好）
./deploy-linux.sh --skip-cert --skip-env
```

#### Windows

```powershell
# 完整部署
.\deploy-windows.ps1

# 跳过证书生成
.\deploy-windows.ps1 -SkipCert

# 跳过环境配置
.\deploy-windows.ps1 -SkipEnv

# 完全非交互
.\deploy-windows.ps1 -SkipCert -SkipEnv
```

**部署步骤**：
1. 检查 Docker 环境
2. 配置环境变量（.env）
3. 生成 SSL 证书
4. 创建数据目录
5. 清理旧容器
6. 构建 Docker 镜像
7. 启动所有服务

---

### 更新脚本

#### Linux/macOS

```bash
# 更新所有应用服务（不包括 redis、rabbitmq、seq）
./update-linux.sh

# 更新指定服务
./update-linux.sh --services "manager-api,agent-api,merchant-api"

# 强制更新（不询问确认）
./update-linux.sh --force

# 跳过构建（使用现有镜像）
./update-linux.sh --no-build

# 更新基础设施服务
./update-linux.sh --services "redis,rabbitmq,seq"
```

#### Windows

```powershell
# 更新所有应用服务
.\update-windows.ps1

# 更新指定服务
.\update-windows.ps1 -Services "manager-api,agent-api"

# 强制更新
.\update-windows.ps1 -Force

# 跳过构建
.\update-windows.ps1 -NoBuild

# 更新基础设施
.\update-windows.ps1 -Services "redis,rabbitmq,seq"
```

**可用服务**：
- **应用服务**: `ui-manager`, `ui-agent`, `ui-merchant`, `manager-api`, `agent-api`, `merchant-api`, `payment-api`
- **基础设施**: `redis`, `rabbitmq`, `seq`

**更新逻辑**：
- **应用服务**: 重新构建镜像 → 重启容器
- **基础设施**: 拉取最新镜像 → 重启容器

---

## 🔧 技术实现

### 代码改进统计

| 类别 | Linux/macOS | Windows | 总计 |
|------|-------------|---------|------|
| .env 解析 | 3 | 3 | 6 |
| sudo 智能化 | 4 | - | 4 |
| Docker Compose 兼容 | 5 | 7 | 12 |
| 错误处理 | 3 | 2 | 5 |
| **总计** | **15** | **12** | **27** |

### 脚本流程

#### 部署流程图

```
[开始] 
  ↓
[检查 Docker] → 失败 → [退出(1)]
  ↓ 成功
[配置 .env] → 跳过/完成
  ↓
[生成证书] → 跳过/完成
  ↓
[创建目录] → 权限检查 → 需要 sudo?
  ↓
[清理旧容器]
  ↓
[构建镜像] → 失败 → [退出(1)]
  ↓ 成功
[启动服务] → 失败 → [退出(1)]
  ↓ 成功
[显示状态]
  ↓
[完成]
```

#### 更新流程图

```
[开始]
  ↓
[检查 Docker] → 失败 → [退出(1)]
  ↓ 成功
[拉取代码] → 可选
  ↓
[区分服务类型] → 应用 / 基础设施
  ↓
[构建/拉取镜像]
  ↓
[逐个更新服务] → 停止 → 删除 → 启动
  ↓
[显示状态]
  ↓
[完成]
```

---

## ✅ 兼容性

### 操作系统

| 系统 | 版本 | 状态 | 说明 |
|------|------|------|------|
| Ubuntu | 18.04/20.04/22.04/24.04 | ✅ 完全支持 | 推荐 |
| CentOS | 7/8/9 | ✅ 完全支持 | Stream 版本也支持 |
| Debian | 10/11/12 | ✅ 完全支持 | |
| Fedora | 35+ | ✅ 完全支持 | |
| RHEL | 7/8/9 | ✅ 完全支持 | 与 CentOS 兼容 |
| macOS | 11+ (Big Sur+) | ✅ 完全支持 | Docker Desktop |
| Windows | 10/11 | ✅ 完全支持 | Docker Desktop |
| Windows Server | 2016/2019/2022 | ✅ 完全支持 | |

### Docker Compose

| 版本 | 命令 | Linux/macOS | Windows | 说明 |
|------|------|-------------|---------|------|
| v2 | `docker compose` | ✅ | ✅ | 推荐（内置于 Docker） |
| v1 | `docker-compose` | ✅ | ✅ | 兼容（独立安装） |

### PowerShell

| 版本 | Windows | 说明 |
|------|---------|------|
| PowerShell 5.1 | ✅ | 默认版本 |
| PowerShell 7+ | ✅ | PowerShell Core |

---

## 🆘 故障排查

### 常见问题

#### 1. .env 解析失败

**症状**：
```
❌ .env 文件中未找到 DATA_PATH_HOST 配置
```

**原因**：
- `.env` 文件不存在
- 键名拼写错误
- 值为空或格式错误

**解决**：
```bash
# 检查 .env 文件
cat .env | grep DATA_PATH_HOST

# 确保格式正确
DATA_PATH_HOST=/opt/agpayplus

# 或使用引号
DATA_PATH_HOST="/opt/agpayplus"
```

---

#### 2. Docker Compose 未检测到

**症状**：
```
❌ Docker Compose 未安装
```

**原因**：
- Docker 未安装
- Docker Compose 未安装
- 命令不在 PATH 中

**解决**：

**Ubuntu/Debian:**
```bash
# 安装 Docker
curl -fsSL https://get.docker.com | sh

# 安装 Compose v2（推荐）
sudo apt-get update
sudo apt-get install docker-compose-plugin

# 或安装 Compose v1
sudo curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" \
  -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

**CentOS/RHEL:**
```bash
# 安装 Docker
sudo yum install -y yum-utils
sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
sudo yum install docker-ce docker-ce-cli containerd.io docker-compose-plugin

# 启动 Docker
sudo systemctl start docker
sudo systemctl enable docker
```

**Windows:**
- 下载并安装 [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop)

---

#### 3. 权限不足

**症状**：
```
❌ 创建目录失败
Permission denied
```

**原因**：
- 无权限创建目录
- SELinux 限制（CentOS/RHEL）
- 目录被占用

**解决**：

**方案 1：使用 sudo**
```bash
sudo ./deploy-linux.sh
```

**方案 2：修改目标路径**
```bash
# 编辑 .env
DATA_PATH_HOST=$HOME/agpayplus
```

**方案 3：禁用 SELinux（临时）**
```bash
sudo setenforce 0
```

**方案 4：设置 SELinux 上下文**
```bash
sudo chcon -R -t container_file_t /opt/agpayplus
```

---

#### 4. 证书生成失败

**症状**：
```
❌ 证书生成失败
```

**原因**：
- dotnet dev-certs 未安装
- 权限不足
- 证书目录不存在

**解决**：

**安装 .NET SDK:**
```bash
# Ubuntu
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Windows
# 下载并安装 .NET SDK from https://dotnet.microsoft.com/download
```

**手动生成证书:**
```bash
# Linux/macOS
dotnet dev-certs https -ep ~/.aspnet/https/agpayplusapi.pfx -p 123456
dotnet dev-certs https --trust

# Windows
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\agpayplusapi.pfx -p 123456
dotnet dev-certs https --trust
```

**或跳过证书生成:**
```bash
./deploy-linux.sh --skip-cert
```

---

#### 5. 容器启动失败

**症状**：
```
❌ 服务启动失败
```

**原因**：
- 端口被占用
- 依赖服务未就绪
- 配置错误

**解决**：

**检查端口占用:**
```bash
# Linux/macOS
sudo lsof -i :9817
sudo netstat -tlnp | grep 9817

# Windows
netstat -ano | findstr :9817
```

**查看容器日志:**
```bash
docker compose logs manager-api
docker compose logs -f manager-api  # 实时查看
```

**检查服务状态:**
```bash
docker compose ps
docker compose ps manager-api
```

**重启服务:**
```bash
docker compose restart manager-api
```

---

#### 6. MySQL 连接失败

**症状**：
```
Unable to connect to MySQL
Connection refused
```

**原因**：
- MySQL 未启动
- 连接字符串错误
- 网络问题

**解决**：

**检查 MySQL 状态:**
```bash
# 如果使用宿主机 MySQL
sudo systemctl status mysql

# 如果使用 Docker MySQL
docker compose ps db
docker compose logs db
```

**测试连接:**
```bash
# 从容器内测试
docker compose exec manager-api ping db

# 从宿主机测试
mysql -h 172.17.0.1 -u root -p
```

**检查配置:**
```bash
# 查看解析后的连接字符串
docker compose config | grep ConnectionStrings
```

---

## 💾 MySQL 数据迁移

### 迁移概述

如果之前使用 Docker named volume (`db-data`)，现在改为 host bind mount (`${DATA_PATH_HOST}/mysql`)。

**变更优势**：
- ✅ 统一管理所有数据
- ✅ 便于备份
- ✅ 易于迁移

### 迁移步骤

#### 方式一：mysqldump（推荐）

```bash
# 1. 导出现有数据
docker exec <mysql-container> mysqldump \
  -u root -p<password> \
  --all-databases \
  --single-transaction \
  --quick \
  > mysql_backup_$(date +%Y%m%d).sql

# 2. 停止服务
docker compose down

# 3. 创建新目录
mkdir -p /opt/agpayplus/mysql

# 4. 更新 docker-compose.yml
# 确认 MySQL volumes 为：
#   - ${DATA_PATH_HOST}/mysql:/var/lib/mysql

# 5. 启动 MySQL
docker compose up -d db

# 6. 导入数据
docker exec -i <mysql-container> mysql \
  -u root -p<password> \
  < mysql_backup_<date>.sql

# 7. 验证数据
docker exec <mysql-container> mysql \
  -u root -p<password> \
  -e "SHOW DATABASES;"

# 8. 启动应用
docker compose up -d
```

#### 方式二：直接复制（高级用户）

```bash
# 1. 停止 MySQL
docker compose stop db

# 2. 找到旧 volume 位置
docker volume inspect agpayplus_db-data
# 输出: "Mountpoint": "/var/lib/docker/volumes/agpayplus_db-data/_data"

# 3. 复制数据
sudo cp -rp /var/lib/docker/volumes/agpayplus_db-data/_data/* \
  /opt/agpayplus/mysql/

# 4. 修改权限
sudo chown -R 999:999 /opt/agpayplus/mysql

# 5. 启动服务
docker compose up -d
```

### 验证迁移

```bash
# 检查容器
docker compose ps db

# 检查日志
docker compose logs db | tail -50

# 测试连接
docker exec <mysql-container> mysql -u root -p -e "SELECT VERSION();"

# 检查数据文件
ls -lh /opt/agpayplus/mysql/
```

### 回滚方案

```bash
# 1. 停止服务
docker compose down

# 2. 恢复 docker-compose.yml
# volumes:
#   - db-data:/var/lib/mysql

# 3. 重启
docker compose up -d
```

---

## 💡 最佳实践

### 1. 环境配置

**使用环境特定的 .env 文件：**
```bash
# Linux/macOS
cp .env.linux .env

# Windows
copy .env.windows .env
```

**不要提交敏感信息：**
```bash
# 添加到 .gitignore
echo ".env" >> .gitignore
echo "*.pfx" >> .gitignore
```

---

### 2. 数据备份

**定期备份：**
```bash
# Linux/macOS
tar -czf backup-$(date +%Y%m%d).tar.gz /opt/agpayplus

# Windows
Compress-Archive -Path E:\app\agpayplus -DestinationPath backup-$(Get-Date -Format yyyyMMdd).zip
```

**自动备份脚本：**
```bash
#!/bin/bash
# backup-daily.sh
BACKUP_DIR="/backup"
DATA_DIR="/opt/agpayplus"
DATE=$(date +%Y%m%d)

# 备份数据
tar -czf "$BACKUP_DIR/agpay-data-$DATE.tar.gz" "$DATA_DIR"

# 保留最近 7 天
find "$BACKUP_DIR" -name "agpay-data-*.tar.gz" -mtime +7 -delete

# 备份数据库
docker exec agpayplus-db-1 mysqldump -u root -p<password> --all-databases \
  > "$BACKUP_DIR/agpay-mysql-$DATE.sql"
```

---

### 3. 更新策略

**测试更新：**
```bash
# 1. 备份数据
./backup-data.sh

# 2. 先更新单个服务测试
./update-linux.sh --services "manager-api"

# 3. 验证功能
curl -k https://localhost:9817/api/health

# 4. 成功后更新其他服务
./update-linux.sh
```

**分批更新：**
```bash
# UI 服务
./update-linux.sh --services "ui-manager,ui-agent,ui-merchant"

# API 服务
./update-linux.sh --services "manager-api,agent-api,merchant-api,payment-api"

# 基础设施
./update-linux.sh --services "redis,rabbitmq,seq"
```

---

### 4. 监控和日志

**查看服务状态：**
```bash
docker compose ps
docker compose ps --format json
```

**查看日志：**
```bash
# 所有服务
docker compose logs

# 指定服务
docker compose logs manager-api

# 实时跟踪
docker compose logs -f manager-api

# 最近 100 行
docker compose logs --tail=100 manager-api

# 带时间戳
docker compose logs --timestamps manager-api
```

**监控资源使用：**
```bash
docker stats
docker stats manager-api
```

---

### 5. 安全建议

**生产环境：**
- ✅ 使用强密码
- ✅ 不要使用默认端口
- ✅ 启用防火墙
- ✅ 定期更新镜像
- ✅ 使用 HTTPS
- ✅ 限制容器权限

**敏感信息管理：**
```bash
# 使用环境变量
export MYSQL_PASSWORD="strong-password"
./deploy-linux.sh

# 或使用 Docker secrets
docker secret create mysql_password mysql_password.txt
```

---

## 📞 获取帮助

### 报告问题

提供以下信息：
1. 操作系统和版本：`uname -a` (Linux) 或 `ver` (Windows)
2. Docker 版本：`docker --version`
3. Docker Compose 版本：`docker compose version` 或 `docker-compose --version`
4. 脚本输出日志
5. .env 文件内容（去除敏感信息）
6. 容器日志：`docker compose logs`

### 相关资源

- **项目主页**: https://github.com/agoodays/agpayplus
- **Docker Hub**: https://hub.docker.com/u/agooday
- **文档**: https://agpayplus.com/docs

---

## 📝 更新日志

### v2.0 (2024)
- ✅ 添加健壮的 .env 解析
- ✅ 智能 sudo 权限管理
- ✅ Docker Compose v1/v2 兼容
- ✅ 改进错误提示和处理
- ✅ 统一 MySQL 数据持久化方式
- ✅ 完善文档和故障排查

### v1.0
- 初始版本

---

**完成日期**: 2024  
**状态**: ✅ 所有优化已完成  
**维护**: AgPay+ Team
