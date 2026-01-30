# 部署脚本回滚机制使用指南

> 支持 **Windows**、**Linux** 和 **macOS** 平台

---

## 📋 目录

- [问题说明](#问题说明)
- [解决方案](#解决方案)
- [使用指南](#使用指南)
  - [部署脚本](#部署脚本)
  - [更新脚本](#更新脚本)
- [回滚场景](#回滚场景)
- [最佳实践](#最佳实践)
- [故障排查](#故障排查)

---

## ❌ 问题说明

### 原脚本的风险

**deploy-windows.ps1** 和 **deploy-linux.sh** 没有回滚机制：

```bash
[5/7] 清理旧容器...        # ✅ 旧容器已删除
      ↓
[6/7] 构建 Docker 镜像...   # ❌ 构建失败
      ↓
[7/7] 启动服务...           # ⚠️ 不会执行
```

**后果**：
- ❌ 旧服务被删除
- ❌ 新服务构建失败
- ❌ **系统完全不可用！**

---

## ✅ 解决方案

### 新增带回滚机制的脚本

#### 部署脚本

| 平台 | 部署脚本 | 部署回滚 |
|------|---------|---------|
| Windows | `deploy-windows-with-rollback.ps1` | `rollback-deployment.ps1` |
| Linux/macOS | `deploy-linux-with-rollback.sh` | `rollback-deployment.sh` |

#### 更新脚本

| 平台 | 更新脚本 | 更新回滚 |
|------|---------|---------|
| Windows | `update-windows-with-rollback.ps1` | `rollback-update.ps1` |
| Linux/macOS | `update-linux-with-rollback.sh` | `rollback-update.sh` |

### 核心功能

1. **自动备份** - 部署前自动保存当前状态
2. **状态跟踪** - 记录每个部署步骤的状态
3. **自动回滚** - 失败时自动恢复到部署前状态
4. **手动回滚** - 支持回滚到任意历史版本
5. **备份管理** - 自动清理旧备份（默认保留 7 天）

---

## 🚀 使用指南

### Windows

#### 方案一：使用带回滚的部署脚本（推荐）

```powershell
# 基本部署
.\deploy-windows-with-rollback.ps1

# 跳过交互
.\deploy-windows-with-rollback.ps1 -SkipCert -SkipEnv

# 不创建备份（快速部署，不推荐生产环境）
.\deploy-windows-with-rollback.ps1 -NoBackup
```

#### 方案二：手动回滚

**列出所有备份：**
```powershell
.\rollback-deployment.ps1
```

输出示例：
```
可用的备份：

  [1] 2024-01-29 14:30:52 - 20240129_143052
  [2] 2024-01-29 10:15:33 - 20240129_101533
  [3] 2024-01-28 16:45:21 - 20240128_164521

请选择要回滚的备份编号 (1-3):
```

**回滚到指定版本：**
```powershell
# 交互式回滚
.\rollback-deployment.ps1 .backup\20240129_143052

# 强制回滚（不询问确认）
.\rollback-deployment.ps1 .backup\20240129_143052 -Force
```

---

### Linux/macOS

#### 方案一：使用带回滚的部署脚本（推荐）

```bash
# 添加执行权限（首次运行）
chmod +x deploy-linux-with-rollback.sh
chmod +x rollback-deployment.sh

# 基本部署
./deploy-linux-with-rollback.sh

# 跳过交互
./deploy-linux-with-rollback.sh --skip-cert --skip-env

# 不创建备份
./deploy-linux-with-rollback.sh --no-backup
```

#### 方案二：手动回滚

**列出所有备份：**
```bash
./rollback-deployment.sh
```

输出示例：
```
可用的备份：

  [1] 2024-01-29 14:30:52 - 20240129_143052
  [2] 2024-01-29 10:15:33 - 20240129_101533
  [3] 2024-01-28 16:45:21 - 20240128_164521

请选择要回滚的备份编号 (1-3):
```

**回滚到指定版本：**
```bash
# 交互式回滚
./rollback-deployment.sh .backup/20240129_143052

# 强制回滚
./rollback-deployment.sh .backup/20240129_143052 --force
```

---

## 🎬 回滚场景演示

### 场景 1：镜像构建失败（自动回滚）

```bash
[7/8] 构建 Docker 镜像...
  ✗ 镜像构建失败: manager-api build error

========================================
  部署失败，开始回滚...
  原因: 镜像构建失败
========================================

[回滚 1/3] 恢复配置文件...
  ✓ 已恢复 docker-compose.yml
  ✓ 已恢复 .env

[回滚 2/3] 清理失败的容器...
  ✓ 已清理失败的容器

[回滚 3/3] 尝试恢复旧服务...
  ✓ 已恢复旧服务

========================================
  回滚完成
========================================
```

**结果**：
- ✅ 系统自动恢复到部署前状态
- ✅ 旧服务继续运行
- ✅ **零停机时间**

---

### 场景 2：部分服务启动失败

```bash
[8/8] 启动服务...
  等待服务启动...
  ✗ 部分服务启动失败: redis, rabbitmq

========================================
  部署失败，开始回滚...
  原因: 部分服务启动失败
========================================

[回滚] ...（同场景1）
```

---

### 场景 3：配置错误需要手动回滚

```bash
# 部署成功，但发现配置有问题
$ ./rollback-deployment.sh

可用的备份：

  [1] 2024-01-29 14:30:52 - 20240129_143052
  [2] 2024-01-29 10:15:33 - 20240129_101533

请选择要回滚的备份编号: 1

[1/4] 停止当前服务...
  ✓ 已停止当前服务

[2/4] 恢复配置文件...
  ✓ 已恢复 docker-compose.yml
  ✓ 已恢复 .env

[3/4] 检查镜像...
  ✓ 镜像已存在

[4/4] 启动服务...
  ✓ 服务启动成功

========================================
  回滚成功！
========================================
```

---

### 场景 4：更新单个服务失败（自动回滚）

#### Windows
```powershell
# 更新 manager-api
.\update-windows-with-rollback.ps1 -Services "manager-api"

  更新 manager-api...
    [1/4] 停止服务...
    [2/4] 构建镜像...
    [3/4] 启动服务...
    [4/4] 检查服务状态...
  ✗ manager-api 未正常运行

========================================
  更新失败，开始回滚...
========================================

[回滚 1/2] 恢复服务镜像...
  ✓ 已恢复 manager-api 镜像

[回滚 2/2] 重启服务...
  ✓ 已恢复 manager-api

========================================
  回滚完成
========================================
```

#### Linux/macOS
```bash
# 更新多个服务
./update-linux-with-rollback.sh --services "manager-api,agent-api"

  更新 manager-api...
    [1/4] 停止服务...
    [2/4] 构建镜像...
    ✗ manager-api 镜像构建失败

========================================
  更新失败，开始回滚...
  原因: manager-api 镜像构建失败
========================================

[回滚 1/2] 恢复服务镜像...
  ✓ 已恢复 manager-api 镜像

[回滚 2/2] 重启服务...
  ✓ 已恢复 manager-api

========================================
  回滚完成
========================================
```

**结果**：
- ✅ 自动恢复到更新前状态
- ✅ 服务继续正常运行
- ✅ **零停机时间**

---

### 场景 5：手动回滚更新

#### Windows
```powershell
# 更新成功，但发现新版本有问题
.\rollback-update.ps1

可用的更新备份：

  [1] 2024-01-29 15:30:22
      服务: manager-api, agent-api
      路径: update_20240129_153022

  [2] 2024-01-29 14:15:10
      服务: payment-api
      路径: update_20240129_141510

请选择要回滚的备份编号 (1-2): 1

[1/3] 停止当前服务...
  ✓ 已停止所有服务

[2/3] 恢复镜像...
  ✓ 已恢复 manager-api 镜像
  ✓ 已恢复 agent-api 镜像

[3/3] 启动服务...
  ✓ manager-api 启动成功
  ✓ agent-api 启动成功

========================================
  回滚成功！
========================================
```

#### Linux/macOS
```bash
# 直接指定备份路径回滚
./rollback-update.sh .backup/update_20240129_153022 --force

[1/3] 停止当前服务...
  ✓ 已停止所有服务

[2/3] 恢复镜像...
  ✓ 已恢复 manager-api 镜像
  ✓ 已恢复 agent-api 镜像

[3/3] 启动服务...
  ✓ manager-api 启动成功
  ✓ agent-api 启动成功

========================================
  回滚成功！
========================================
```

---

## 🔍 流程对比

### 部署流程对比

#### 原脚本（无回滚）

```
[1/7] 检查 Docker 环境
[2/7] 配置环境变量
[3/7] 配置 SSL 证书
[4/7] 创建数据目录
[5/7] 清理旧容器        ← 旧容器被删除
      ↓
[6/7] 构建 Docker 镜像  ← ⚠️ 如果失败，系统不可用！
      ↓
[7/7] 启动服务
```

#### 新脚本（带回滚）

```
[1/8] 检查 Docker 环境
[2/8] 创建备份           ← ✅ 保存当前状态
[3/8] 配置环境变量
[4/8] 配置 SSL 证书
[5/8] 创建数据目录
[6/8] 停止旧容器
      ↓
[7/8] 构建 Docker 镜像  ← 如果失败
      ↓                     ↓
[8/8] 启动服务            ✅ 自动回滚，恢复旧服务
      ↓
    ✅ 成功
```

---

### 更新流程对比

#### 原更新脚本（无回滚）

```
[1/2] 更新服务
      ↓
  停止 manager-api        ← 服务已停止
      ↓
  构建镜像                ← ⚠️ 如果失败，服务不可用！
      ↓
  启动服务
      
[2/2] 验证
```

#### 新更新脚本（带回滚）

```
[1/4] 检查 Docker 环境
[2/4] 创建备份           ← ✅ 保存镜像状态
[3/4] 更新服务
      ↓
  停止 manager-api
      ↓
  构建镜像                ← 如果失败
      ↓                     ↓
  启动服务                ✅ 自动回滚，恢复旧镜像
      ↓
  检查状态
      
[4/4] 验证
```

---

## 💡 最佳实践

### 1. 部署前

#### Windows
```powershell
# 备份数据库
docker exec mysql mysqldump -u root -p --all-databases > backup.sql

# 使用带回滚的脚本
.\deploy-windows-with-rollback.ps1
```

#### Linux/macOS
```bash
# 备份数据库
docker exec mysql mysqldump -u root -p --all-databases > backup.sql

# 使用带回滚的脚本
./deploy-linux-with-rollback.sh
```

---

### 2. 部署中

- ✅ 监控控制台输出
- ✅ 注意错误和警告信息
- ✅ 如有问题，脚本会自动回滚

---

### 3. 部署后验证

#### Windows
```powershell
# 检查服务状态
docker compose ps

# 检查服务日志
docker compose logs -f manager-api

# 如有问题，立即回滚
.\rollback-deployment.ps1
```

#### Linux/macOS
```bash
# 检查服务状态
docker compose ps

# 检查服务日志
docker compose logs -f manager-api

# 如有问题，立即回滚
./rollback-deployment.sh
```

---

### 4. 服务更新

#### Windows

```powershell
# 更新单个服务
.\update-windows-with-rollback.ps1 -Services "manager-api"

# 更新多个服务
.\update-windows-with-rollback.ps1 -Services "manager-api,agent-api,merchant-api"

# 强制更新（不询问确认）
.\update-windows-with-rollback.ps1 -Services "manager-api" -Force

# 跳过构建（使用现有镜像）
.\update-windows-with-rollback.ps1 -Services "redis,rabbitmq" -NoBuild
```

#### Linux/macOS

```bash
# 更新单个服务
./update-linux-with-rollback.sh --services "manager-api"

# 更新多个服务
./update-linux-with-rollback.sh --services "manager-api,agent-api,merchant-api"

# 强制更新
./update-linux-with-rollback.sh --services "manager-api" --force

# 跳过构建
./update-linux-with-rollback.sh --services "redis,rabbitmq" --no-build
```

**更新后验证：**
```bash
# 检查服务状态
docker compose ps manager-api

# 查看服务日志
docker compose logs -f manager-api

# 如有问题，回滚更新
./rollback-update.sh
```

---

### 5. 生产环境部署策略

#### 蓝绿部署
```bash
# 1. 在新环境部署
./deploy-linux-with-rollback.sh

# 2. 验证新环境
curl -k https://new-env:8817/api/health

# 3. 切换流量到新环境
# 4. 监控一段时间
# 5. 如有问题，切回旧环境
```

#### 灰度发布
```bash
# 1. 只更新部分服务
./update-linux-with-rollback.sh --services "manager-api"

# 2. 观察日志和指标
docker compose logs -f manager-api

# 3. 逐步扩大范围
./update-linux-with-rollback.sh --services "agent-api,merchant-api"
```

---

## 🔧 高级配置

### 自定义备份保留时间

#### Windows (`deploy-windows-with-rollback.ps1`)
```powershell
function Cleanup {
    # 保留 30 天
    $cutoffDate = (Get-Date).AddDays(-30)
    Get-ChildItem "$ScriptDir\.backup" | 
        Where-Object { $_.LastWriteTime -lt $cutoffDate } | 
        Remove-Item -Recurse -Force
}
```

#### Linux (`deploy-linux-with-rollback.sh`)
```bash
cleanup_old_backups() {
    if [ -d "$SCRIPT_DIR/.backup" ]; then
        # 保留 30 天
        find "$SCRIPT_DIR/.backup" -type d -mtime +30 -exec rm -rf {} + 2>/dev/null || true
    fi
}
```

---

### 添加健康检查

#### Windows
```powershell
function Check-ServiceHealth {
    param([string]$DockerCompose)
    
    Write-Host "  检查服务健康状态..." -ForegroundColor Gray
    
    $services = @("manager-api", "agent-api", "merchant-api")
    foreach ($service in $services) {
        $healthy = docker inspect --format='{{.State.Health.Status}}' $service 2>&1
        if ($healthy -ne "healthy") {
            throw "服务 $service 健康检查失败"
        }
    }
    
    Write-Host "  ✓ 所有服务健康" -ForegroundColor Green
}
```

#### Linux
```bash
check_service_health() {
    local docker_compose="$1"
    
    echo -e "${GRAY}  检查服务健康状态...${NC}"
    
    local services=("manager-api" "agent-api" "merchant-api")
    for service in "${services[@]}"; do
        local healthy=$(docker inspect --format='{{.State.Health.Status}}' "$service" 2>&1)
        if [ "$healthy" != "healthy" ]; then
            echo -e "${RED}  服务 $service 健康检查失败${NC}"
            return 1
        fi
    done
    
    echo -e "${GREEN}  ✓ 所有服务健康${NC}"
    return 0
}
```

---

### 添加数据库备份

#### Windows
```powershell
function Backup-Database {
    Write-Host "  备份数据库..." -ForegroundColor Gray
    
    $mysqlPassword = Get-EnvValue -Key "MYSQL_ROOT_PASSWORD"
    $backupFile = "$($DeploymentState.BackupPath)\mysql_backup.sql"
    
    docker exec mysql mysqldump -u root -p$mysqlPassword --all-databases > $backupFile
    
    Write-Host "  ✓ 数据库备份完成" -ForegroundColor Green
}
```

#### Linux
```bash
backup_database() {
    echo -e "${GRAY}  备份数据库...${NC}"
    
    local mysql_password=$(get_env_value "MYSQL_ROOT_PASSWORD")
    local backup_file="$BACKUP_PATH/mysql_backup.sql"
    
    docker exec mysql mysqldump -u root -p"$mysql_password" --all-databases > "$backup_file"
    
    echo -e "${GREEN}  ✓ 数据库备份完成${NC}"
}
```

---

## 🆘 故障排查

### Q1: 回滚后服务无法启动？

**A**: 检查镜像是否存在

**Windows:**
```powershell
# 查看镜像列表
docker images

# 如果镜像不存在，重新构建
docker compose build
```

**Linux:**
```bash
# 查看镜像列表
docker images

# 如果镜像不存在，重新构建
docker compose build
```

---

### Q2: 备份目录太大？

**A**: 手动清理旧备份

**Windows:**
```powershell
# 只保留最近 3 个备份
Get-ChildItem .backup | 
    Sort-Object CreationTime -Descending | 
    Select-Object -Skip 3 | 
    Remove-Item -Recurse -Force
```

**Linux:**
```bash
# 只保留最近 3 个备份
cd .backup
ls -t | tail -n +4 | xargs rm -rf
```

---

### Q3: 如何查看备份内容？

**A**: 备份目录包含以下文件

```
.backup/20240129_143052/
├── docker-compose.yml    # 配置文件
├── .env                  # 环境变量
└── containers.json       # 容器状态
```

查看备份：

**Windows:**
```powershell
Get-Content .backup\20240129_143052\.env
Get-Content .backup\20240129_143052\containers.json
```

**Linux:**
```bash
cat .backup/20240129_143052/.env
cat .backup/20240129_143052/containers.json | jq .
```

---

### Q4: 构建时无法连接到 mcr.microsoft.com？

**A**: 网络问题，配置国内镜像源

这是中国大陆用户的常见问题。错误信息：
```
failed to do request: Get "https://mcr.microsoft.com/..."
dial tcp 150.171.69.10:443: connectex: A connection attempt failed
```

**快速解决**：

#### Windows (Docker Desktop)
1. 打开 Docker Desktop → Settings → Docker Engine
2. 添加镜像源配置：
```json
{
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live",
    "https://docker.awsl9527.cn"
  ]
}
```
3. 点击 **Apply & Restart**
4. 重新运行部署脚本

#### Linux
```bash
# 配置镜像源
sudo nano /etc/docker/daemon.json

# 添加配置（同上JSON）

# 重启 Docker
sudo systemctl restart docker

# 重新部署
./deploy-linux-with-rollback.sh
```

**详细说明**：参见 `DOCKER_MIRROR_GUIDE.md`

---

### Q5: 如何完全禁用回滚？

**A**: 使用原脚本或添加参数

**Windows:**
```powershell
# 使用原脚本
.\deploy-windows.ps1

# 或跳过备份
.\deploy-windows-with-rollback.ps1 -NoBackup
```

**Linux:**
```bash
# 使用原脚本
./deploy-linux.sh

# 或跳过备份
./deploy-linux-with-rollback.sh --no-backup
```

---

## ⚠️ 注意事项

### 1. 数据安全

- ⚠️ 回滚**不会**恢复数据库数据
- ⚠️ 回滚**不会**恢复上传的文件
- ✅ 仅恢复配置和容器状态

建议在重要操作前手动备份数据库：
```bash
docker exec mysql mysqldump -u root -p --all-databases > full_backup.sql
```

---

### 2. 磁盘空间

备份会占用额外磁盘空间：
- 每个备份约 1-10 MB
- 默认保留 7 天
- 可手动清理 `.backup` 目录

---

### 3. 回滚限制

以下情况无法回滚：
- ❌ 数据库结构已变更
- ❌ 已删除的数据卷
- ❌ 依赖的外部服务变更

---

## 📊 对比总结

| 特性 | 原脚本 | 新脚本（带回滚） |
|------|--------|-----------------|
| 自动备份 | ❌ | ✅ |
| 失败回滚 | ❌ | ✅ |
| 状态跟踪 | ❌ | ✅ |
| 手动回滚 | ❌ | ✅ |
| 备份管理 | ❌ | ✅ (自动清理) |
| 零停机风险 | ❌ 高风险 | ✅ 低风险 |
| 跨平台 | ✅ | ✅ |

---

## 🎯 脚本文件清单

| 平台 | 部署脚本（原） | 部署脚本（回滚） | 回滚脚本 | 状态 |
|------|--------------|-----------------|---------|------|
| Windows | `deploy-windows.ps1` | `deploy-windows-with-rollback.ps1` | `rollback-deployment.ps1` | ✅ |
| Linux/macOS | `deploy-linux.sh` | `deploy-linux-with-rollback.sh` | `rollback-deployment.sh` | ✅ |

---

## 📞 获取帮助

### 报告问题

提供以下信息：
1. 操作系统和版本
2. Docker 版本：`docker --version`
3. Docker Compose 版本
4. 脚本输出日志
5. 备份目录内容

### 相关文档

- **项目主页**: https://github.com/agoodays/agpayplus
- **部署指南**: `DEPLOYMENT_GUIDE.md`
- **MySQL 迁移**: `MYSQL_MIGRATION.md`

---

**建议**：
- 🎯 **生产环境**：使用带回滚的脚本
- ⚡ **开发环境**：可继续使用原脚本
- 💡 **测试环境**：优先测试新脚本

**维护**: AgPay+ Team  
**更新**: 2024
