# AgPay+ Docker 部署指南

> 本指南提供完整的 Docker 部署方案，支持 Windows、Linux 和 macOS 平台，实现一键部署和灵活的更新策略。

## 📑 目录

- [系统要求](#系统要求)
- [快速开始](#快速开始)
- [部署步骤](#部署步骤)
- [更新服务](#更新服务)
- [配置说明](#配置说明)
- [常见问题](#常见问题)
- [服务管理](#服务管理)

---

## 💻 系统要求

### 所有平台通用
- Docker Engine 20.10+
- Docker Compose 2.0+
- 至少 4GB RAM
- 至少 20GB 磁盘空间

### Windows 特定要求
- Windows 10/11 Pro/Enterprise/Education
- Docker Desktop for Windows
- PowerShell 5.1 或更高版本
- .NET SDK 6.0+ (用于生成 SSL 证书)

### Linux 特定要求
- Ubuntu 20.04+, Debian 11+, CentOS 8+, 或其他发行版
- Docker Engine
- Docker Compose
- .NET SDK 6.0+ (用于生成 SSL 证书)
- Bash Shell

### macOS 特定要求
- macOS 11 (Big Sur) 或更高版本
- Docker Desktop for Mac
- .NET SDK 6.0+ (用于生成 SSL 证书)
- Bash Shell

### 外部依赖
- **MySQL 8.0+**: 安装在宿主机上
  - 数据库名：`agpayplusdb`
  - 字符集：`utf8mb4`
  - 初始化脚本：`aspnet-core/docs/sql/agpayplusinit.sql`

---

## 🚀 快速开始

### Windows 环境

```powershell
# 1. 克隆项目（如果还没有）
git clone https://github.com/agoodays/agpayplus.git
cd agpayplus

# 2. 配置环境变量
# 复制 Windows 环境配置模板
Copy-Item .env.windows .env

# 编辑 .env 文件，修改必要的配置
notepad .env

# 3. 一键部署
.\deploy-windows.ps1
```

### Linux/macOS 环境

```bash
# 1. 克隆项目（如果还没有）
git clone https://github.com/agoodays/agpayplus.git
cd agpayplus

# 2. 添加执行权限
chmod +x *.sh

# 3. 配置环境变量
# 复制 Linux 环境配置模板
cp .env.linux .env

# 编辑 .env 文件，修改必要的配置
vim .env  # 或使用其他编辑器

# 4. 一键部署
./deploy-linux.sh
```

---

## 📋 部署步骤

### 第一步：准备 MySQL 数据库

在宿主机上安装并配置 MySQL：

```bash
# 1. 创建数据库
mysql -u root -p
CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# 2. 导入初始化脚本
mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql

# 3. 创建数据库用户（可选）
CREATE USER 'agpayplus'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'agpayplus'@'%';
FLUSH PRIVILEGES;
```

### 第二步：配置环境变量

根据您的操作系统，编辑对应的环境配置文件：

#### Windows 环境 (`.env.windows`)

```env
# 服务器IP或域名
IPORDOMAIN=localhost

# MySQL 配置（Windows Docker 使用 host.docker.internal）
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_mysql_password

# 数据持久化路径（Windows 路径格式）
DATA_PATH_HOST=E:/app/agpayplus

# SSL 证书路径
CERT_PATH=${USERPROFILE}/.aspnet/https
```

#### Linux 环境 (`.env.linux`)

```env
# 服务器IP或域名
IPORDOMAIN=localhost

# MySQL 配置（Linux 需要使用宿主机IP）
# 获取宿主机IP: ip addr show docker0 | grep inet
MYSQL_SERVER_NAME=172.17.0.1
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_mysql_password

# 数据持久化路径（Linux 路径格式）
DATA_PATH_HOST=/opt/agpayplus

# SSL 证书路径
CERT_PATH=~/.aspnet/https
```

#### macOS 环境 (`.env.linux`)

```env
# 服务器IP或域名
IPORDOMAIN=localhost

# MySQL 配置（macOS 使用 host.docker.internal）
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_mysql_password

# 数据持久化路径（macOS 路径格式）
DATA_PATH_HOST=/opt/agpayplus

# SSL 证书路径
CERT_PATH=~/.aspnet/https
```

### 第三步：生成 SSL 证书（可选）

如果启用 HTTPS，需要生成开发证书：

**Windows:**
```powershell
.\generate-cert-windows.ps1
```

**Linux/macOS:**
```bash
./generate-cert-linux.sh
```

证书信息：
- 证书名称：`agpayplusapi`
- 证书密码：`123456`
- 证书路径：`~/.aspnet/https/agpayplusapi.pfx`

### 第四步：执行部署

**Windows:**
```powershell
# 完整部署（包含环境检查、证书生成、镜像构建、服务启动）
.\deploy-windows.ps1

# 跳过证书生成
.\deploy-windows.ps1 -SkipCert

# 跳过环境配置
.\deploy-windows.ps1 -SkipEnv
```

**Linux/macOS:**
```bash
# 完整部署
./deploy-linux.sh

# 跳过证书生成
./deploy-linux.sh --skip-cert

# 跳过环境配置
./deploy-linux.sh --skip-env
```

### 第五步：验证部署

部署完成后，访问以下地址验证服务：

- 🖥️ **运营平台**: https://localhost:8817
- 👥 **代理商系统**: https://localhost:8816
- 🏪 **商户系统**: https://localhost:8818
- 💳 **支付网关**: https://localhost:9819
- 🐰 **RabbitMQ 管理**: http://localhost:15672 (admin/admin)

查看服务状态：
```bash
docker compose ps
```

---

## 🔄 更新服务

项目提供了灵活的更新策略，支持更新全部服务或指定服务。

### Windows 环境

```powershell
# 更新所有应用服务（不包括 redis 和 rabbitmq）
.\update-windows.ps1

# 更新指定服务
.\update-windows.ps1 -Services "ui-manager,manager-api"

# 跳过镜像构建（仅重启服务）
.\update-windows.ps1 -NoBuild

# 强制更新（不询问确认）
.\update-windows.ps1 -Force

# 组合使用
.\update-windows.ps1 -Services "manager-api" -Force
```

### Linux/macOS 环境

```bash
# 更新所有应用服务
./update-linux.sh

# 更新指定服务
./update-linux.sh --services "ui-manager,manager-api"

# 跳过镜像构建
./update-linux.sh --no-build

# 强制更新
./update-linux.sh --force

# 组合使用
./update-linux.sh --services "manager-api" --force
```

### 可用的服务名称

- `ui-manager` - 运营平台前端
- `ui-agent` - 代理商系统前端
- `ui-merchant` - 商户系统前端
- `manager-api` - 运营平台后端 API
- `agent-api` - 代理商系统后端 API
- `merchant-api` - 商户系统后端 API
- `payment-api` - 支付网关后端 API（包含收银台前端）
- `redis` - Redis 缓存服务
- `rabbitmq` - RabbitMQ 消息队列

**特别说明**：
- 收银台（cashier）前端在构建 `payment-api` 时会自动打包到 `wwwroot/cashier` 目录
- 更新 `payment-api` 服务会同时更新收银台前端
- 收银台通过 `https://localhost:9819/cashier` 访问

### 更新工作流程

1. **检查 Docker 环境**：确认 Docker 正在运行
2. **拉取最新代码**：询问是否从 Git 仓库拉取最新代码
3. **重新构建镜像**：使用 `--no-cache` 确保使用最新代码
4. **更新服务**：停止旧容器，启动新容器
5. **验证状态**：显示服务运行状态

---

## ⚙️ 配置说明

### 环境变量详解

| 变量名 | 说明 | 示例值 |
|--------|------|--------|
| `IPORDOMAIN` | 服务器IP或域名，用于CORS和JWT配置 | `localhost` / `your-domain.com` |
| `MYSQL_SERVER_NAME` | MySQL服务器地址 | `host.docker.internal` (Win/Mac) / `172.17.0.1` (Linux) |
| `MYSQL_PORT` | MySQL端口 | `3306` |
| `MYSQL_DATABASE` | 数据库名称 | `agpayplusdb` |
| `MYSQL_USER` | 数据库用户名 | `root` |
| `MYSQL_PASSWORD` | 数据库密码 | `your_password` |
| `DATA_PATH_HOST` | 宿主机数据存储路径 | Windows: `E:/app/agpayplus`<br>Linux: `/opt/agpayplus` |
| `CERT_PATH` | SSL证书路径 | `${USERPROFILE}/.aspnet/https` (Win)<br>`~/.aspnet/https` (Linux/Mac) |

### 服务端口映射

| 服务 | 容器端口 | 宿主机端口 | 协议 |
|------|----------|------------|------|
| 运营平台前端 | 80 | 8817 | HTTP |
| 代理商前端 | 80 | 8816 | HTTP |
| 商户前端 | 80 | 8818 | HTTP |
| 运营平台API | 9817 (HTTPS)<br>5817 (HTTP) | 9817<br>5817 | HTTPS/HTTP |
| 代理商API | 9816 (HTTPS)<br>5816 (HTTP) | 9816<br>5816 | HTTPS/HTTP |
| 商户API | 9818 (HTTPS)<br>5818 (HTTP) | 9818<br>5818 | HTTPS/HTTP |
| 支付网关API | 9819 (HTTPS)<br>5819 (HTTP) | 9819<br>5819 | HTTPS/HTTP |
| Redis | 6379 | 6379 | TCP |
| RabbitMQ | 5672<br>15672 | 5672<br>15672 | AMQP<br>HTTP |

### 数据持久化

以下数据会持久化到宿主机：

```
${DATA_PATH_HOST}/
├── logs/          # 应用日志
└── upload/        # 上传文件
```

Docker 卷持久化：

- `redis-data`: Redis 数据
- `rabbitmq-data`: RabbitMQ 数据

---

## 📦 服务管理

### 查看服务状态

```bash
docker compose ps
```

### 查看服务日志

```bash
# 查看所有服务日志
docker compose logs

# 查看特定服务日志
docker compose logs manager-api

# 实时跟踪日志
docker compose logs -f manager-api

# 查看最近100行日志
docker compose logs --tail=100 manager-api
```

### 启动/停止服务

```bash
# 启动所有服务
docker compose up -d

# 启动特定服务
docker compose up -d manager-api

# 停止所有服务
docker compose stop

# 停止特定服务
docker compose stop manager-api

# 重启服务
docker compose restart manager-api
```

### 进入容器

```bash
# 进入容器 Shell
docker compose exec manager-api /bin/bash

# 以 root 用户进入
docker compose exec -u root manager-api /bin/bash
```

### 清理资源

```bash
# 停止并删除所有容器
docker compose down

# 停止并删除容器、网络、卷
docker compose down -v

# 清理未使用的镜像
docker image prune -a

# 清理所有未使用的资源
docker system prune -a --volumes
```

---

## ? 常见问题

### 1. Windows 环境下 MySQL 连接失败

**问题**: 后端 API 无法连接到宿主机的 MySQL。

**解决方案**:
- 确保 MySQL 配置允许远程连接
- 检查 MySQL 用户权限：`GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';`
- 确认 `.env` 中 `MYSQL_SERVER_NAME=host.docker.internal`
- 检查 Windows 防火墙是否允许 3306 端口

### 2. Linux 环境下 host.docker.internal 不可用

**问题**: Linux 不支持 `host.docker.internal`。

**解决方案**:
```bash
# 获取 Docker 网桥 IP
ip addr show docker0 | grep inet

# 或使用默认网桥 IP
MYSQL_SERVER_NAME=172.17.0.1

# 或使用宿主机 IP
MYSQL_SERVER_NAME=192.168.1.100
```

### 3. SSL 证书错误

**问题**: 浏览器显示证书不受信任。

**解决方案**:
- 重新生成证书：`.\generate-cert-windows.ps1` 或 `./generate-cert-linux.sh`
- 确保执行了 `--trust` 参数（Windows/macOS）
- Linux 需要手动添加证书到系统信任列表
- 浏览器选择"继续访问"（开发环境）

### 4. 端口被占用

**问题**: 启动失败，提示端口已被使用。

**解决方案**:
```bash
# Windows
netstat -ano | findstr :8817

# Linux/macOS
lsof -i :8817

# 修改 docker-compose.yml 中的端口映射
ports:
  - "18817:80"  # 使用其他端口
```

### 5. RabbitMQ 插件未加载

**问题**: RabbitMQ 延迟消息插件未启用。

**解决方案**:
```bash
# 进入 RabbitMQ 容器
docker compose exec rabbitmq bash

# 启用插件
rabbitmq-plugins enable rabbitmq_delayed_message_exchange

# 重启 RabbitMQ
docker compose restart rabbitmq
```

### 6. Node 版本不兼容

**问题**: 前端构建失败，Node 版本过高。

**解决方案**:
- Dockerfile 已指定 `node:16-alpine`
- 如需本地构建，使用 nvm 切换到 Node 16：
  ```bash
  nvm install 16
  nvm use 16
  ```

### 7. 磁盘空间不足

**问题**: 构建失败，提示磁盘空间不足。

**解决方案**:
```bash
# 清理未使用的镜像
docker image prune -a

# 清理构建缓存
docker builder prune

# 清理所有未使用的资源
docker system prune -a --volumes
```

### 8. 服务健康检查失败

**问题**: Redis 或 RabbitMQ 健康检查一直失败。

**解决方案**:
```bash
# 查看服务日志
docker compose logs redis
docker compose logs rabbitmq

# 手动测试连接
docker compose exec redis redis-cli ping
docker compose exec rabbitmq rabbitmqctl status

# 增加健康检查重试次数
# 编辑 docker-compose.yml
healthcheck:
  retries: 10
  interval: 30s
```

---

## 🔐 生产环境部署建议

### 1. 修改默认密码

```env
# MySQL
MYSQL_PASSWORD=<strong-password>

# RabbitMQ
# 编辑 docker-compose.yml
environment:
  RABBITMQ_DEFAULT_USER: admin
  RABBITMQ_DEFAULT_PASS: <strong-password>

# 证书密码
# 编辑 generate-cert-*.* 脚本
CERT_PASSWORD=<strong-password>
```

### 2. 配置域名和 SSL

```env
# 使用实际域名
IPORDOMAIN=your-domain.com

# 使用正式 SSL 证书（Let's Encrypt）
# 替换开发证书为生产证书
CERT_PATH=/etc/ssl/certs/your-domain
```

### 3. 启用反向代理

建议使用 Nginx 或 Traefik 作为反向代理：

```nginx
# nginx.conf 示例
server {
    listen 443 ssl http2;
    server_name your-domain.com;
    
    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;
    
    location / {
        proxy_pass https://localhost:8817;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

### 4. 配置日志轮转

```bash
# /etc/logrotate.d/agpayplus
/opt/agpayplus/logs/*.log {
    daily
    rotate 30
    compress
    delaycompress
    notifempty
    create 0644 root root
}
```

### 5. 设置资源限制

编辑 `docker-compose.yml`：

```yaml
services:
  manager-api:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 2G
        reservations:
          cpus: '0.5'
          memory: 512M
```

### 6. 启用监控

- 使用 Prometheus + Grafana 监控容器
- 集成 ELK 堆栈收集日志
- 配置告警通知

---

## 🏭️ 项目结构

```
agpayplus/
├── aspnet-core/              # .NET 后端项目
│   ├── src/
│   │   ├── AGooday.AgPay.Manager.Api/
│   │   ├── AGooday.AgPay.Agent.Api/
│   │   ├── AGooday.AgPay.Merchant.Api/
│   │   └── AGooday.AgPay.Payment.Api/
│   └── docs/
│       ├── sql/
│       │   └── agpayplusinit.sql
│       └── rabbitmq_plugin/
│           └── rabbitmq_delayed_message_exchange-3.13.0.ez
├── ant-design-vue/           # Vue 前端项目
│   ├── agpay-ui-manager/
│   ├── agpay-ui-agent/
│   └── agpay-ui-merchant/
├── docker-compose.yml        # Docker Compose 配置
├── .env                      # 环境变量（实际使用）
├── .env.windows              # Windows 环境模板
├── .env.linux                # Linux/macOS 环境模板
├── deploy-windows.ps1        # Windows 部署脚本
├── deploy-linux.sh           # Linux/macOS 部署脚本
├── update-windows.ps1        # Windows 更新脚本
├── update-linux.sh           # Linux/macOS 更新脚本
├── generate-cert-windows.ps1 # Windows 证书生成脚本
├── generate-cert-linux.sh    # Linux/macOS 证书生成脚本
└── DOCKER_DEPLOYMENT.md      # 本文档
```

---

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

---

## 📄 许可证

[LICENSE](LICENSE)

---

## 📧 联系我们

- 项目主页: https://github.com/agoodays/agpayplus
- 问题反馈: https://github.com/agoodays/agpayplus/issues
- 邮箱: support@agpayplus.com

---

**祝您部署顺利！** 🎉
