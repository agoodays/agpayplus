# AgPay+ 完整部署和更新使用指南

> 多环境、多系统部署、更新和回滚完整方案

---

## 📖 文档概述

本文档提供 AgPay+ 系统的完整部署、更新和回滚方案，支持：

✅ **多环境部署** - development/staging/production  
✅ **多系统支持** - Linux/macOS/Windows  
✅ **指定服务更新** - 支持单个或多个服务  
✅ **自动备份回滚** - 更新失败自动回滚  
✅ **可选 Cashier 构建** - 按需构建收银台  
✅ **健康检查** - 自动验证服务状态

---

## 📋 目录

1. [快速开始](#1-快速开始)
2. [脚本说明](#2-脚本说明)
3. [环境配置](#3-环境配置)
4. [部署流程](#4-部署流程)
5. [更新流程](#5-更新流程)
6. [回滚流程](#6-回滚流程)
7. [Cashier 管理](#7-cashier-管理)
8. [使用场景](#8-使用场景)
9. [故障排查](#9-故障排查)

---

## 1. 快速开始

### 1.1 系统要求

- **Docker**: 20.10+
- **Docker Compose**: 2.0+
- **磁盘空间**: 20GB+
- **内存**: 8GB+

### 1.2 首次部署（3 步）

#### Linux/macOS

```bash
# 步骤 1: 配置环境
cp .env.production .env
vi .env  # 修改 IPORDOMAIN 等配置

# 步骤 2: 生成证书
./generate-cert-linux.sh

# 步骤 3: 执行部署
./deploy.sh --env production --skip-backup
```

#### Windows

```powershell
# 步骤 1: 配置环境
Copy-Item .env.production .env
notepad .env  # 修改配置

# 步骤 2: 生成证书
.\generate-cert-windows.ps1

# 步骤 3: 执行部署
.\deploy.ps1 -Environment production -SkipBackup
```

---

## 2. 脚本说明

### 2.1 部署脚本

| 脚本 | 系统 | 功能 |
|------|------|------|
| `deploy.sh` | Linux/macOS | 首次部署和更新部署 |
| `deploy.ps1` | Windows | 首次部署和更新部署 |

**主要功能**:
- 首次部署：自动初始化环境
- 更新部署：自动备份、支持回滚
- 多环境支持
- 指定服务部署
- 健康检查和自动回滚

### 2.2 更新脚本

| 脚本 | 系统 | 功能 |
|------|------|------|
| `update.sh` | Linux/macOS | 服务更新 |
| `update.ps1` | Windows | 服务更新 |

**主要功能**:
- 指定服务更新
- 自动备份
- 健康检查
- 自动回滚

### 2.3 回滚脚本

| 脚本 | 系统 | 功能 |
|------|------|------|
| `rollback.sh` | Linux/macOS | 版本回滚 |
| `rollback.ps1` | Windows | 版本回滚 |

**主要功能**:
- 回滚到指定备份
- 支持指定服务回滚
- 列出所有可用备份

---

## 3. 环境配置

### 3.1 环境文件

| 文件 | 环境 | 说明 |
|------|------|------|
| `.env.development` | 开发 | 本地开发、测试 |
| `.env.staging` | 预发布 | 预生产测试 |
| `.env.production` | 生产 | 正式生产环境 |

### 3.2 配置模板

```bash
# ========== 基础配置 ==========
ENVIRONMENT=production
COMPOSE_PROJECT_NAME=agpayplus-prod
IMAGE_PREFIX=agpay-prod
IMAGE_TAG=latest

# ========== 域名/IP ==========
IPORDOMAIN=yourdomain.com  # 改为实际域名或IP

# ========== MySQL 配置 ==========
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_secure_password  # 改为安全密码

# ========== 数据路径 ==========
# Linux: /var/agpayplus
# Windows: E:/app/agpayplus
DATA_PATH_HOST=/var/agpayplus

# ========== SSL 证书 ==========
# Linux: /root/.aspnet/https
# Windows: ${USERPROFILE}/.aspnet/https
CERT_PATH=/root/.aspnet/https
CERT_PASSWORD=123456

# ========== Cashier 构建 ==========
# true: 每次重新构建 cashier
# false: 使用现有 cashier（推荐）
BUILD_CASHIER=false
```

### 3.3 关键配置说明

#### IPORDOMAIN
- 开发环境: `localhost`
- 生产环境: `yourdomain.com` 或服务器 IP

#### MYSQL_SERVER_NAME
- 容器内 MySQL: `mysql`
- 宿主机 MySQL: `host.docker.internal`
- 远程 MySQL: `192.168.1.100`

#### DATA_PATH_HOST
- 确保有足够空间和读写权限
- Linux 推荐: `/var/agpayplus`, `/data/agpayplus`
- Windows 推荐: `E:/app/agpayplus`, `D:/agpayplus`

---

## 4. 部署流程

### 4.1 部署命令

#### Linux/macOS

```bash
./deploy.sh [选项]

选项：
  --env <环境>              环境名称
  --services <服务列表>     指定服务（空格分隔）
  --build-cashier           强制构建 cashier
  --skip-backup             跳过备份
  --skip-cert               跳过证书生成
  --force                   跳过确认
  --help                    帮助信息
```

#### Windows

```powershell
.\deploy.ps1 [参数]

参数：
  -Environment <环境>       环境名称
  -Services <服务列表>      指定服务
  -BuildCashier            强制构建 cashier
  -SkipBackup              跳过备份
  -SkipCert                跳过证书生成
  -Force                   跳过确认
```

### 4.2 可用服务

| 服务名 | 说明 | 访问端口 |
|-------|------|----------|
| `agpay-ui-manager` | 运营平台前端 | 8817 |
| `agpay-ui-agent` | 代理商前端 | 8816 |
| `agpay-ui-merchant` | 商户前端 | 8818 |
| `agpay-manager-api` | 运营平台 API | 9817 |
| `agpay-agent-api` | 代理商 API | 9816 |
| `agpay-merchant-api` | 商户 API | 9818 |
| `agpay-payment-api` | 支付网关 | 9819 |

### 4.3 部署示例

#### 示例 1: 首次生产环境部署

```bash
# Linux
./deploy.sh --env production --skip-backup

# Windows
.\deploy.ps1 -Environment production -SkipBackup
```

#### 示例 2: 开发环境部署（构建 cashier）

```bash
# Linux
./deploy.sh --env development --build-cashier

# Windows
.\deploy.ps1 -Environment development -BuildCashier
```

#### 示例 3: 仅部署运营平台

```bash
# Linux
./deploy.sh --services "agpay-ui-manager agpay-manager-api"

# Windows
.\deploy.ps1 -Services "agpay-ui-manager","agpay-manager-api"
```

#### 示例 4: 更新多个服务

```bash
# Linux
./deploy.sh --services "agpay-manager-api agpay-agent-api agpay-merchant-api"

# Windows
.\deploy.ps1 -Services "agpay-manager-api","agpay-agent-api","agpay-merchant-api"
```

### 4.4 部署步骤

```
[1/9] 检查 Docker 环境
      ├─ Docker 版本检测
      ├─ Docker Compose 检测
      └─ Docker 运行状态验证

[2/9] 检查现有部署
      ├─ 判断部署类型（首次/更新）
      └─ 统计容器数量

[3/9] 检查 SSL 证书
      ├─ 验证证书存在性
      └─ 自动生成证书

[4/9] 初始化数据目录
      ├─ 日志目录
      ├─ 上传目录
      └─ 数据库/Redis/MQ 目录

[5/9] 备份当前部署（更新时）
      ├─ 保存容器信息
      ├─ 导出镜像
      ├─ 备份配置
      └─ 清理旧备份

[6/9] 准备构建参数
      └─ 确定 cashier 构建策略

[7/9] 构建镜像
      └─ 构建指定或所有服务

[8/9] 部署服务
      ├─ 停止旧服务
      └─ 启动新服务

[9/9] 健康检查
      ├─ 验证服务状态
      ├─ 显示失败日志
      └─ 自动回滚（如失败）
```

---

## 5. 更新流程

### 5.1 更新命令

#### Linux/macOS

```bash
./update.sh [选项]

选项：
  --env <环境>              环境名称
  --services <服务列表>     指定服务
  --build-cashier           强制构建 cashier
  --force                   跳过确认
  --help                    帮助信息
```

#### Windows

```powershell
.\update.ps1 [参数]

参数：
  -Environment <环境>       环境名称
  -Services <服务列表>      指定服务
  -BuildCashier            强制构建 cashier
  -Force                   跳过确认
```

### 5.2 更新示例

#### 示例 1: 更新所有服务

```bash
# Linux
./update.sh

# Windows
.\update.ps1
```

#### 示例 2: 更新单个服务

```bash
# Linux
./update.sh --services agpay-manager-api

# Windows
.\update.ps1 -Services "agpay-manager-api"
```

#### 示例 3: 更新多个服务

```bash
# Linux
./update.sh --services "agpay-manager-api agpay-agent-api"

# Windows
.\update.ps1 -Services "agpay-manager-api","agpay-agent-api"
```

#### 示例 4: 更新支付网关并构建 cashier

```bash
# Linux
./update.sh --services agpay-payment-api --build-cashier

# Windows
.\update.ps1 -Services "agpay-payment-api" -BuildCashier
```

### 5.3 更新步骤

```
[1/7] 检查 Docker 环境
[2/7] 检查现有部署
[3/7] 备份当前部署
      ├─ 保存容器和镜像信息
      ├─ 导出镜像
      └─ 备份配置
[4/7] 准备构建参数
[5/7] 构建新镜像
[6/7] 更新服务
      ├─ 停止旧服务
      ├─ 删除旧容器
      └─ 启动新服务
[7/7] 健康检查
      └─ 自动回滚（如失败）
```

---

## 6. 回滚流程

### 6.1 回滚命令

#### Linux/macOS

```bash
./rollback.sh [选项]

选项：
  --env <环境>              环境名称
  --services <服务列表>     指定服务
  --backup <版本>           备份版本号
  --list                    列出所有备份
  --auto                    自动模式
  --help                    帮助信息
```

#### Windows

```powershell
.\rollback.ps1 [参数]

参数：
  -Environment <环境>       环境名称
  -Services <服务列表>      指定服务
  -Backup <版本>           备份版本号
  -List                    列出所有备份
  -Auto                    自动模式
```

### 6.2 回滚示例

#### 示例 1: 列出所有备份

```bash
# Linux
./rollback.sh --list

# Windows
.\rollback.ps1 -List
```

输出示例：
```
========================================
  可用备份列表
========================================

production 环境:
  20240315_143022  [1.2 GB] (最新)
    服务: agpay-manager-api agpay-agent-api agpay-merchant-api agpay-payment-api
  20240315_120530  [1.2 GB]
    服务: agpay-manager-api agpay-agent-api agpay-merchant-api agpay-payment-api

development 环境:
  20240315_140000  [800 MB] (最新)
    服务: agpay-ui-manager agpay-ui-agent
```

#### 示例 2: 回滚到最新备份

```bash
# Linux
./rollback.sh

# Windows
.\rollback.ps1
```

#### 示例 3: 回滚指定服务

```bash
# Linux
./rollback.sh --services agpay-manager-api

# Windows
.\rollback.ps1 -Services "agpay-manager-api"
```

#### 示例 4: 回滚到指定版本

```bash
# Linux
./rollback.sh --backup 20240315_143022

# Windows
.\rollback.ps1 -Backup "20240315_143022"
```

### 6.3 回滚步骤

```
[1/5] 检查备份
      ├─ 查找可用备份
      ├─ 确定回滚版本
      └─ 列出备份服务

[2/5] 恢复环境配置
      └─ 恢复 .env 文件

[3/5] 加载备份镜像
      └─ 从备份加载镜像

[4/5] 重启服务
      ├─ 停止当前服务
      └─ 启动备份服务

[5/5] 健康检查
      └─ 验证服务状态
```

### 6.4 备份管理

#### 备份目录结构

```
.backup/
├── production_update_20240315_143022/
│   ├── agpay-manager-api.tar.gz
│   ├── agpay-agent-api.tar.gz
│   ├── containers.json
│   ├── images.json
│   ├── .env.backup
│   └── docker-compose.yml.backup
├── latest_production
└── latest_development
```

#### 备份策略

- **自动保留**: 最近 5 个备份
- **自动清理**: 超过 5 个会自动删除旧备份
- **手动管理**: 可手动删除 `.backup` 目录

---

## 7. Cashier 管理

### 7.1 Cashier 说明

Cashier（收银台）集成在 `payment-api` 中，位于 `wwwroot/cashier`。

### 7.2 构建策略

#### 不构建（默认，推荐）

**适用场景**:
- Cashier 无变更
- 仅更新后端
- 节省构建时间

**配置**:
```bash
BUILD_CASHIER=false
```

#### 构建 Cashier

**适用场景**:
- Cashier 有变更
- 首次部署
- 需要更新收银台

**方式 1**: 环境变量
```bash
BUILD_CASHIER=true
```

**方式 2**: 命令行
```bash
# Linux
./deploy.sh --services agpay-payment-api --build-cashier

# Windows
.\deploy.ps1 -Services "agpay-payment-api" -BuildCashier
```

### 7.3 访问地址

- 本地: `https://localhost:9819/cashier`
- 生产: `https://yourdomain.com:9819/cashier`

---

## 8. 使用场景

### 场景 1: 首次生产部署

```bash
# 1. 准备配置
cp .env.production .env
vi .env

# 2. 生成证书
./generate-cert-linux.sh

# 3. 执行部署
./deploy.sh --env production --skip-backup --build-cashier
```

### 场景 2: 日常更新单服务

```bash
./update.sh --services agpay-manager-api
```

### 场景 3: 更新多服务

```bash
./update.sh --services "agpay-ui-manager agpay-manager-api"
```

### 场景 4: 紧急回滚

```bash
# 查看备份
./rollback.sh --list

# 回滚
./rollback.sh
```

### 场景 5: 开发环境频繁更新

```bash
./update.sh --env development --services "agpay-ui-manager agpay-ui-agent agpay-ui-merchant"
```

### 场景 6: 更新支付网关

```bash
./update.sh --services agpay-payment-api --build-cashier
```

### 场景 7: 灰度发布

```bash
# 1. 先更新一个服务
./update.sh --services agpay-manager-api

# 2. 观察运行
docker compose logs -f agpay-manager-api

# 3. 确认后更新其他
./update.sh --services "agpay-agent-api agpay-merchant-api agpay-payment-api"
```

---

### 首次部署 vs 更新部署

# 首次部署 vs 更新部署说明

## 🎯 重要区别

### 首次部署（First Deployment）

**定义**：系统中没有任何运行中的 AgPay+ 容器

**特点**：
- ✅ 没有旧服务
- ✅ 没有历史备份
- ✅ 失败时只需清理，无需回滚

**失败处理**：
```
构建失败 → 清理失败的资源 → 修复问题 → 重新部署
```

---

### 更新部署（Update Deployment）

**定义**：系统中已有运行中的 AgPay+ 容器，需要更新

**特点**：
- ✅ 有旧服务运行
- ✅ 有配置可备份
- ✅ 失败时需要回滚到旧版本

**失败处理**：
```
构建失败 → 自动回滚 → 恢复旧服务 → 系统继续可用
```

---

## 🔍 脚本自动识别

新版部署脚本会**自动检测**是首次部署还是更新部署并采取相应策略（跳过备份或自动创建备份、回滚等）。

#### 首次部署时（示例输出）

```powershell
[2/8] 跳过备份（首次部署）
[3/8] 配置环境变量...
[4/8] 配置 SSL 证书...
...

# 如果失败
[清理] 这是首次部署，清理失败的资源...
  ✓ 已清理失败的容器
  
  首次部署失败，请检查错误信息后重试
  提示：
    1. 检查 .env 配置是否正确
    2. 确保网络连接正常（参考 DOCKER_MIRROR_GUIDE.md）
    3. 查看错误日志定位问题
```

#### 更新部署时（示例输出）

```powershell
[2/8] 创建备份...
  ✓ 备份已保存到: .backup/20240129_143052
[3/8] 配置环境变量...
...

# 如果失败
========================================
  部署失败
  原因: 镜像构建失败
========================================

  开始回滚...

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

---

## 📊 对比要点

| 特性 | 首次部署 | 更新部署 |
|------|---------|---------|
| 旧服务 | ❌ 无 | ✅ 有 |
| 备份 | ⏭️ 跳过 | ✅ 自动创建 |
| 失败后 | 🧹 清理 | ♻️ 回滚 |
| 系统状态 | 无服务 | 服务可用 |
| 操作建议 | 修复后重试 | 自动恢复 |

---

## 🚀 使用与范例

- 首次部署（自动检测）：
  - Windows: `.\\deploy.ps1`
  - Linux: `./deploy.sh`

- 更新部署（自动备份并回滚）：
  - Windows: `.\\deploy.ps1`
  - Linux: `./deploy.sh`

- 跳过备份（不推荐）：
  - Windows: `.\\deploy.ps1 -SkipBackup`
  - Linux: `./deploy.sh --skip-backup`

---

## 📝 常见问题

Q: 如何判断是否为首次部署？

A: 脚本会自动检测；也可手动检查 `docker compose ps` 输出，若没有 agpayplus 容器则为首次部署。

Q: 首次部署失败后是否需要回滚？

A: 不需要，首次部署失败只需清理失败资源并重试。

---

## 💡 最佳实践

- 首次部署前：确认 `.env` 配置、网络和磁盘空间；建议构建时开启 `--build-cashier`（若首次需要收银台）。
- 更新部署前：务必备份，避免使用 `--skip-backup`。
- 对于关键服务，先在测试环境执行更新并观察健康检查后再在生产环境逐步推广（灰度）。

---

## 9. 故障排查

### 9.1 查看日志

```bash
# 所有服务
docker compose logs -f

# 特定服务
docker compose logs -f agpay-manager-api

# 最近 100 行
docker compose logs --tail=100 -f agpay-manager-api
```

### 9.2 查看状态

```bash
# 服务状态
docker compose ps

# 详细状态
docker compose ps -a
```

### 9.3 重启服务

```bash
# 单个服务
docker compose restart agpay-manager-api

# 所有服务
docker compose restart
```

### 9.4 清理重建

```bash
# 停止服务
docker compose down

# 清理资源
docker system prune -a

# 重新部署
./deploy.sh
```

### 9.5 常见问题

详见 [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

---

## 📚 相关文档

- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - 故障排查指南
- [DOCKER_MIRROR_GUIDE.md](./DOCKER_MIRROR_GUIDE.md) - 镜像源配置
- [CASHIER_DEPLOYMENT.md](./CASHIER_DEPLOYMENT.md) - Cashier 部署说明
- [README.md](./README.md) - 项目主文档

---

**版本**: 2.0  
**最后更新**: 2024-03-15  
**维护者**: AgPay+ Team
