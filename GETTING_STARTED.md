# AgPay+ 快速上手指南

> 快速部署、更新和管理 AgPay+ 系统的完整指南

---

## 📋 目录

1. [系统要求](#1-系统要求)
2. [快速部署](#2-快速部署)
3. [环境配置](#3-环境配置)
4. [常用命令](#4-常用命令)
5. [典型场景](#5-典型场景)
6. [故障排查](#6-故障排查)
7. [进阶使用](#7-进阶使用)

---

## 1. 系统要求

### 硬件要求
- **CPU**: 4核或更高
- **内存**: 8GB 或更高
- **磁盘空间**: 20GB 或更高
- **网络**: 稳定的网络连接

### 软件要求
- **Docker**: 20.10+ 
- **Docker Compose**: 2.0+ 
- **Git**: 2.0+ (可选，用于代码更新)

### 操作系统
- **Linux**: CentOS 7+, Ubuntu 18.04+
- **Windows**: Windows 10+ 或 Windows Server 2016+
- **macOS**: macOS 10.14+ (Mojave 或更高)

---

## 2. 快速部署

### 2.1 首次部署（推荐）

#### Linux/macOS

```bash
# 步骤 1: 克隆项目（如果尚未克隆）
git clone https://github.com/agpayplus/agpayplus.git
cd agpayplus

# 步骤 2: 一键初始化（自动配置环境、生成证书）
./deploy.sh --env production --init --skip-backup

# 步骤 3: 执行数据库初始化（首次部署时执行）
./deploy.sh --env production --init-db
```

#### Windows

```powershell
# 步骤 1: 克隆项目（如果尚未克隆）
git clone https://github.com/agpayplus/agpayplus.git
cd agpayplus

# 步骤 2: 一键初始化（自动配置环境、生成证书）
.\deploy.ps1 -Environment production -Init -SkipBackup

# 步骤 3: 执行数据库初始化（首次部署时执行）
.\deploy.ps1 -Environment production -InitDb
```

### 2.2 验证部署

部署完成后，访问以下地址：

| 服务 | 地址 | 默认账号 |
|------|------|----------|
| **运营平台** | https://localhost:8817 | agpayadmin / agpay123 |
| **代理商系统** | https://localhost:8816 | - / agpay666 |
| **商户系统** | https://localhost:8818 | - / agpay666 |
| **支付网关 API** | https://localhost:9819 | - |
| **收银台** | https://localhost:9819/cashier | - |
| **RabbitMQ 管理** | http://localhost:15672 | admin / admin |

---

## 3. 环境配置

### 3.1 环境文件

| 文件 | 环境 | 说明 |
|------|------|------|
| `.env.development` | 开发 | 本地开发、测试 |
| `.env.staging` | 预发布 | 预生产测试 |
| `.env.production` | 生产 | 正式生产环境 |

### 3.2 核心配置项

```bash
# ========== 基础配置 ==========
ENVIRONMENT=production            # 环境名称
COMPOSE_PROJECT_NAME=agpayplus    # 项目名称
IMAGE_PREFIX=agpay                # 镜像前缀
IMAGE_TAG=latest                 # 镜像标签

# ========== 域名/IP ==========
IPORDOMAIN=yourdomain.com        # 改为实际域名或IP

# ========== MySQL 配置 ==========
MYSQL_SERVER_NAME=host.docker.internal  # Windows/macOS
# MYSQL_SERVER_NAME=172.17.0.1          # Linux
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_secure_password  # 改为安全密码

# ========== 数据路径 ==========
DATA_PATH_HOST=/var/agpayplus       # Linux
# DATA_PATH_HOST=E:/app/agpayplus     # Windows

# ========== SSL 证书 ==========
CERT_PATH=/root/.aspnet/https       # Linux
# CERT_PATH=${USERPROFILE}/.aspnet/https  # Windows
CERT_PASSWORD=123456

# ========== Cashier 构建 ==========
BUILD_CASHIER=false               # 是否构建收银台

# ========== Redis 配置 ==========
REDIS_HOST=redis
REDIS_PORT=6379
REDIS_PASSWORD=
REDIS_DB=0

# ========== 健康检查配置 ==========
HEALTH_CHECK_ENABLED=true
HEALTH_CHECK_INTERVAL=30s
HEALTH_CHECK_TIMEOUT=10s
HEALTH_CHECK_RETRIES=3

# ========== 备份配置 ==========
BACKUP_ENABLED=true
BACKUP_RETENTION=5
BACKUP_PATH=/var/agpayplus/backup
```

### 3.3 配置示例

#### 开发环境配置

```bash
# .env.development
ENVIRONMENT=development
COMPOSE_PROJECT_NAME=agpayplus-dev
IPORDOMAIN=localhost
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PASSWORD=123456
DATA_PATH_HOST=/var/agpayplus-dev
BUILD_CASHIER=true  # 开发环境建议构建收银台
```

#### 生产环境配置

```bash
# .env.production
ENVIRONMENT=production
COMPOSE_PROJECT_NAME=agpayplus
IPORDOMAIN=yourdomain.com
MYSQL_SERVER_NAME=192.168.1.100  # 生产环境使用独立数据库
MYSQL_PASSWORD=your_secure_password
DATA_PATH_HOST=/var/agpayplus
BUILD_CASHIER=false  # 生产环境不频繁构建收银台
```

---

## 4. 常用命令

### 4.1 部署命令

#### Linux/macOS

```bash
# 首次部署（开发环境）
./deploy.sh --env development --skip-backup

# 首次部署（生产环境）
./deploy.sh --env production --skip-backup

# 一键初始化（推荐）
./deploy.sh --env production --init --skip-backup

# 数据库初始化
./deploy.sh --env production --init-db

# 验证配置
./deploy.sh --validate-config

# 构建收银台
./deploy.sh --services agpay-payment-api --build-cashier
```

#### Windows

```powershell
# 首次部署（开发环境）
.\deploy.ps1 -Environment development -SkipBackup

# 首次部署（生产环境）
.\deploy.ps1 -Environment production -SkipBackup

# 一键初始化（推荐）
.\deploy.ps1 -Environment production -Init -SkipBackup

# 数据库初始化
.\deploy.ps1 -Environment production -InitDb

# 验证配置
.\deploy.ps1 -ValidateConfig

# 构建收银台
.\deploy.ps1 -Services "agpay-payment-api" -BuildCashier
```

### 4.2 更新命令

#### Linux/macOS

```bash
# 更新所有服务
./update.sh

# 更新单个服务
./update.sh --services agpay-manager-api

# 更新多个服务
./update.sh --services "agpay-manager-api agpay-agent-api"

# 更新支付网关并构建收银台
./update.sh --services agpay-payment-api --build-cashier
```

#### Windows

```powershell
# 更新所有服务
.\update.ps1

# 更新单个服务
.\update.ps1 -Services "agpay-manager-api"

# 更新多个服务
.\update.ps1 -Services "agpay-manager-api","agpay-agent-api"

# 更新支付网关并构建收银台
.\update.ps1 -Services "agpay-payment-api" -BuildCashier
```

### 4.3 回滚命令

#### Linux/macOS

```bash
# 查看所有备份
./rollback.sh --list

# 回滚到最新备份
./rollback.sh

# 回滚到指定版本
./rollback.sh --backup 20260415_120000

# 回滚指定服务
./rollback.sh --services agpay-manager-api
```

#### Windows

```powershell
# 查看所有备份
.\rollback.ps1 -List

# 回滚到最新备份
.\rollback.ps1

# 回滚到指定版本
.\rollback.ps1 -Backup "20260415_120000"

# 回滚指定服务
.\rollback.ps1 -Services "agpay-manager-api"
```

### 4.4 日常管理命令

```bash
# 查看服务状态
docker compose ps

# 查看日志
docker compose logs -f

# 查看特定服务日志
docker compose logs -f agpay-manager-api

# 重启服务
docker compose restart

# 停止服务
docker compose stop

# 启动服务
docker compose start

# 清理服务
docker compose down
```

---

## 5. 典型场景

### 场景 1: 首次生产环境部署

**目标**: 在生产服务器上部署 AgPay+ 系统

**步骤**:

```bash
# 1. 准备配置
cp .env.production .env
vi .env  # 修改 IPORDOMAIN、MYSQL_PASSWORD 等

# 2. 生成 SSL 证书
./generate-cert-linux.sh

# 3. 执行部署
./deploy.sh --env production --skip-backup --build-cashier

# 4. 初始化数据库
./deploy.sh --env production --init-db

# 5. 验证部署
docker compose ps
# 访问 https://yourdomain.com:8817
```

### 场景 2: 日常更新单服务

**目标**: 更新运营平台 API 服务

**步骤**:

```bash
# 1. 执行更新
./update.sh --services agpay-manager-api

# 2. 验证更新
docker compose logs -f agpay-manager-api
# 访问运营平台验证功能
```

### 场景 3: 更新多服务

**目标**: 更新运营平台前后端服务

**步骤**:

```bash
# 1. 执行更新
./update.sh --services "agpay-ui-manager agpay-manager-api"

# 2. 验证更新
docker compose logs -f agpay-ui-manager agpay-manager-api
# 访问运营平台验证功能
```

### 场景 4: 紧急回滚

**目标**: 更新失败后回滚到之前的版本

**步骤**:

```bash
# 1. 查看备份
./rollback.sh --list

# 2. 回滚到最新备份
./rollback.sh

# 3. 验证回滚
docker compose ps
# 访问系统验证功能
```

### 场景 5: 开发环境频繁更新

**目标**: 在开发环境中频繁更新前端服务

**步骤**:

```bash
# 1. 执行更新
./update.sh --env development --services "agpay-ui-manager agpay-ui-agent agpay-ui-merchant"

# 2. 验证更新
docker compose logs -f agpay-ui-manager
# 访问前端服务验证功能
```

### 场景 6: 更新支付网关

**目标**: 更新支付网关并构建收银台

**步骤**:

```bash
# 1. 执行更新
./update.sh --services agpay-payment-api --build-cashier

# 2. 验证更新
docker compose logs -f agpay-payment-api
# 访问收银台验证功能
```

### 场景 7: 灰度发布

**目标**: 逐步更新服务，降低风险

**步骤**:

```bash
# 1. 先更新一个服务
./update.sh --services agpay-manager-api

# 2. 观察运行
docker compose logs -f agpay-manager-api

# 3. 确认后更新其他服务
./update.sh --services "agpay-agent-api agpay-merchant-api agpay-payment-api"
```

---

## 6. 故障排查

### 6.1 常见问题

#### 服务启动失败
- **原因**: 端口被占用、配置错误、资源不足
- **解决**: 检查端口占用、查看日志、增加资源

#### 数据库连接失败
- **原因**: 数据库服务未运行、连接信息错误、网络问题
- **解决**: 检查数据库服务状态、验证连接信息、检查网络连接

#### 证书问题
- **原因**: 证书文件不存在、密码错误、权限问题
- **解决**: 重新生成证书、验证密码、检查文件权限

#### Cashier 构建失败
- **原因**: 网络问题、依赖安装失败、Node.js 版本问题
- **解决**: 配置 npm 镜像源、检查网络连接、验证 Node.js 版本

#### 镜像拉取缓慢
- **原因**: 网络问题、Docker 镜像源未配置
- **解决**: 配置 Docker 镜像源加速

### 6.2 排查命令

```bash
# 查看服务状态
docker compose ps

# 查看详细日志
docker compose logs -f

# 查看特定服务日志
docker compose logs -f agpay-manager-api

# 进入容器
docker exec -it agpayplus-manager-api-1 /bin/bash

# 检查容器健康状态
docker inspect --format '{{json .State.Health}}' agpayplus-manager-api-1

# 检查数据库连接
docker exec -it agpayplus-manager-api-1 curl -f http://localhost:5817/health

# 清理并重新部署
docker compose down
docker system prune -a
docker compose up -d
```

### 6.3 镜像源配置

#### Docker 镜像源加速

**Linux 系统**:
```bash
sudo mkdir -p /etc/docker
sudo tee /etc/docker/daemon.json <<-'EOF'
{
  "registry-mirrors": [
    "https://dockerhub.azk8s.cn",
    "https://reg-mirror.qiniu.com",
    "https://hub-mirror.c.163.com",
    "https://mirror.baidubce.com"
  ]
}
EOF
sudo systemctl daemon-reload
sudo systemctl restart docker
```

**Windows 系统**:
1. 打开 Docker Desktop 设置
2. 选择 Docker Engine
3. 添加镜像源
4. 应用并重启 Docker

#### npm 镜像源配置

```bash
# 临时使用
npm install --registry=https://registry.npmmirror.com

# 永久设置
npm config set registry https://registry.npmmirror.com
```

---

## 7. 进阶使用

### 7.1 多环境并行部署

```bash
# 开发环境
export COMPOSE_PROJECT_NAME=agpayplus-dev
docker compose --env-file .env.development up -d

# 预发布环境
export COMPOSE_PROJECT_NAME=agpayplus-staging
docker compose --env-file .env.staging up -d

# 生产环境
export COMPOSE_PROJECT_NAME=agpayplus
docker compose --env-file .env.production up -d
```

### 7.2 备份管理

```bash
# 查看备份列表
./rollback.sh --list

# 手动清理旧备份
ls -1t .backup/ | grep production | tail -n +6 | xargs -I {} rm -rf .backup/{}

# 备份目录结构
# .backup/
# ├── production_update_20260415_120000/
# │   ├── agpay-manager-api.tar.gz
# │   ├── agpay-agent-api.tar.gz
# │   ├── containers.json
# │   ├── images.json
# │   ├── .env.backup
# │   └── docker-compose.yml.backup
# ├── latest_production
# └── latest_development
```

### 7.3 性能优化

1. **跳过 Cashier 构建**：设置 `BUILD_CASHIER=false` 节省构建时间
2. **指定服务更新**：仅更新必要的服务
3. **使用镜像缓存**：Docker 会自动使用构建缓存
4. **并行构建**：Docker Compose 会自动并行构建独立的服务

### 7.4 安全建议

1. **修改默认密码**：更新数据库密码、证书密码等
2. **配置防火墙**：限制端口访问
3. **使用正式 SSL 证书**：生产环境使用 Let's Encrypt 证书
4. **定期备份**：定期检查和备份数据
5. **监控服务**：设置服务监控和告警

---

## 📚 相关文档

- **[DEPLOYMENT_USAGE_GUIDE.md](DEPLOYMENT_USAGE_GUIDE.md)** - 完整部署与更新指南
- **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** - 常用命令速查
- **[ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md)** - 环境变量说明

---

**版本**: 2.1  
**更新日期**: 2026-04-15  
**维护者**: AgPay+ Team