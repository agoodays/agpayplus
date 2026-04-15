# AgPay+ 完整部署和更新使用指南

> 多环境、多系统部署、更新和回滚完整方案

***

## 📖 文档概述

本文档提供 AgPay+ 系统的完整部署、更新和回滚方案，支持：

✅ **多环境部署** - development/staging/production\
✅ **多系统支持** - Linux/macOS/Windows\
✅ **指定服务更新** - 支持单个或多个服务\
✅ **自动备份回滚** - 更新失败自动回滚\
✅ **可选 Cashier 构建** - 按需构建收银台\
✅ **健康检查** - 自动验证服务状态

***

## 📋 目录

1. [快速开始](#1-快速开始)
2. [脚本说明](#2-脚本说明)
3. [环境配置](#3-环境配置)
4. [部署流程](#4-部署流程)
5. [更新流程](#5-更新流程)
6. [回滚流程](#6-回滚流程)
7. [Cashier 管理](#7-Cashier-管理)
8. [使用场景](#8-使用场景)
9. [故障排查](#9-故障排查)
10. [数据库设置](#10-数据库设置)
11. [部署前检查清单](#11-部署前检查清单)
12. [镜像源配置](#12-镜像源配置)

***

## 1. 快速开始

### 1.1 系统要求

- **Docker**: 20.10+
- **Docker Compose**: 2.0+
- **磁盘空间**: 20GB+
- **内存**: 8GB+

### 1.2 首次部署（2 步，推荐）

#### Linux/macOS

```bash
# 步骤 1: 一键初始化（自动配置环境、生成证书）
./deploy.sh --env production --init --skip-backup

# 步骤 2: 执行数据库初始化（首次部署时执行）
./deploy.sh --env production --init-db
```

#### Windows

```powershell
# 步骤 1: 一键初始化（自动配置环境、生成证书）
.\deploy.ps1 -Environment production -Init -SkipBackup

# 步骤 2: 执行数据库初始化（首次部署时执行）
.\deploy.ps1 -Environment production -InitDb
```

### 1.3 传统首次部署（3 步）

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

***

## 2. 脚本说明

### 2.1 部署脚本

| 脚本           | 系统          | 功能        |
| ------------ | ----------- | --------- |
| `deploy.sh`  | Linux/macOS | 首次部署和更新部署 |
| `deploy.ps1` | Windows     | 首次部署和更新部署 |

**主要功能**:

- 首次部署：自动初始化环境
- 更新部署：自动备份、支持回滚
- 多环境支持
- 指定服务部署
- 健康检查和自动回滚

### 2.2 更新脚本

| 脚本           | 系统          | 功能   |
| ------------ | ----------- | ---- |
| `update.sh`  | Linux/macOS | 服务更新 |
| `update.ps1` | Windows     | 服务更新 |

**主要功能**:

- 指定服务更新
- 自动备份
- 健康检查
- 自动回滚

### 2.3 回滚脚本

| 脚本             | 系统          | 功能   |
| -------------- | ----------- | ---- |
| `rollback.sh`  | Linux/macOS | 版本回滚 |
| `rollback.ps1` | Windows     | 版本回滚 |

**主要功能**:

- 回滚到指定备份
- 支持指定服务回滚
- 列出所有可用备份

***

## 3. 环境配置

### 3.1 环境文件

| 文件                 | 环境  | 说明      |
| ------------------ | --- | ------- |
| `.env.development` | 开发  | 本地开发、测试 |
| `.env.staging`     | 预发布 | 预生产测试   |
| `.env.production`  | 生产  | 正式生产环境  |

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

### 3.3 关键配置说明

#### IPORDOMAIN

- 开发环境: `localhost`
- 生产环境: `yourdomain.com` 或服务器 IP

#### MYSQL\_SERVER\_NAME

- 容器内 MySQL: `mysql`
- 宿主机 MySQL: `host.docker.internal`
- 远程 MySQL: `192.168.1.100`

#### DATA\_PATH\_HOST

- 确保有足够空间和读写权限
- Linux 推荐: `/var/agpayplus`, `/data/agpayplus`
- Windows 推荐: `E:/app/agpayplus`, `D:/agpayplus`

***

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
  --init                    一键初始化（自动配置环境）
  --init-db                 执行数据库初始化
  --validate-config         验证配置
  --verbose-output          详细输出
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
  -Init                    一键初始化（自动配置环境）
  -InitDb                  执行数据库初始化
  -ValidateConfig          验证配置
  -VerboseOutput           详细输出
```

### 4.2 可用服务

| 服务名                  | 说明       | 访问端口 |
| -------------------- | -------- | ---- |
| `agpay-ui-manager`   | 运营平台前端   | 8817 |
| `agpay-ui-agent`     | 代理商前端    | 8816 |
| `agpay-ui-merchant`  | 商户前端     | 8818 |
| `agpay-manager-api`  | 运营平台 API | 9817 |
| `agpay-agent-api`    | 代理商 API  | 9816 |
| `agpay-merchant-api` | 商户 API   | 9818 |
| `agpay-payment-api`  | 支付网关     | 9819 |

### 4.3 部署示例

#### 示例 1: 首次生产环境部署（传统方式）

```bash
# Linux
./deploy.sh --env production --skip-backup

# Windows
.\deploy.ps1 -Environment production -SkipBackup
```

#### 示例 2: 一键初始化（推荐）

```bash
# Linux
./deploy.sh --env production --init --skip-backup

# Windows
.\deploy.ps1 -Environment production -Init -SkipBackup
```

#### 示例 3: 数据库初始化

```bash
# Linux
./deploy.sh --env production --init-db

# Windows
.\deploy.ps1 -Environment production -InitDb
```

#### 示例 4: 配置验证

```bash
# Linux
./deploy.sh --validate-config

# Windows
.\deploy.ps1 -ValidateConfig
```

#### 示例 5: 开发环境部署（构建 cashier）

```bash
# Linux
./deploy.sh --env development --build-cashier

# Windows
.\deploy.ps1 -Environment development -BuildCashier
```

#### 示例 6: 仅部署运营平台

```bash
# Linux
./deploy.sh --services "agpay-ui-manager agpay-manager-api"

# Windows
.\deploy.ps1 -Services "agpay-ui-manager","agpay-manager-api"
```

#### 示例 7: 更新多个服务

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

***

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

***

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

***

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

***

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

***

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

***

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

***

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

***

## 📊 对比要点

| 特性   | 首次部署  | 更新部署   |
| ---- | ----- | ------ |
| 旧服务  | ❌ 无   | ✅ 有    |
| 备份   | ⏭️ 跳过 | ✅ 自动创建 |
| 失败后  | 🧹 清理 | ♻️ 回滚  |
| 系统状态 | 无服务   | 服务可用   |
| 操作建议 | 修复后重试 | 自动恢复   |

***

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

***

## 📝 常见问题

Q: 如何判断是否为首次部署？

A: 脚本会自动检测；也可手动检查 `docker compose ps` 输出，若没有 agpayplus 容器则为首次部署。

Q: 首次部署失败后是否需要回滚？

A: 不需要，首次部署失败只需清理失败资源并重试。

***

## 💡 最佳实践

- 首次部署前：确认 `.env` 配置、网络和磁盘空间；建议构建时开启 `--build-cashier`（若首次需要收银台）。
- 更新部署前：务必备份，避免使用 `--skip-backup`。
- 对于关键服务，先在测试环境执行更新并观察健康检查后再在生产环境逐步推广（灰度）。

***

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

详见 DEPLOYMENT_USAGE_GUIDE.md 中的故障排查章节

***

## 📚 相关文档

- [README.md](./README.md) - 项目主文档
- [GETTING_STARTED.md](./GETTING_STARTED.md) - 快速上手指南
- [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - 常用命令速查
- [ENVIRONMENT_VARIABLES.md](./ENVIRONMENT_VARIABLES.md) - 环境变量说明

***

**版本**: 2.1\
**最后更新**: 2026-04-15\
**维护者**: AgPay+ Team

## 🆕 版本更新内容

### 2.1 版本新增功能

1. **健康检查增强**
   - 为所有 API 服务添加了健康检查配置
   - 自动检测服务状态并在失败时触发回滚
2. **服务可靠性**
   - 为所有服务添加了 `restart: unless-stopped` 策略
   - 提高服务的自动恢复能力
3. **备份目录统一**
   - 统一 Windows 和 Linux 脚本的备份目录为 `.backup`
   - 优化备份管理策略
4. **错误处理优化**
   - 增强脚本的错误捕获和处理机制
   - 提供更详细的错误信息和日志输出
5. **脚本语法修复**
   - 修复了 `rollback.ps1` 中的语法错误
   - 确保跨平台脚本的一致性
6. **新环境变量配置**
   - 添加了 Redis 配置选项
   - 添加了健康检查配置选项
   - 添加了备份配置选项
   - 支持通过环境变量自定义备份路径和保留数量

### 健康检查配置

所有 API 服务现在都配置了健康检查：

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:5817/health"]
  interval: 30s
  timeout: 10s
  retries: 3
```

### 服务重启策略

所有服务都添加了重启策略：

```yaml
restart: unless-stopped
```

### 备份目录结构

```
.backup/
├── production_update_20241113_120000/
│   ├── agpay-manager-api.tar.gz
│   ├── agpay-agent-api.tar.gz
│   ├── containers.json
│   ├── images.json
│   ├── .env.backup
│   └── docker-compose.yml.backup
├── latest_production
└── latest_development
```

### 故障排除新增方法

1. **检查健康状态**
   ```bash
   # 查看服务健康状态
   docker compose ps --format "{{.Service}}: {{.State}}"
   ```
2. **查看健康检查日志**
   ```bash
   # 查看健康检查详情
   docker inspect --format '{{json .State.Health}}' <container_id>
   ```
3. **手动触发健康检查**
   ```bash
   # 手动检查服务健康状态
   docker exec <container_id> curl -f http://localhost:5817/health
   ```
4. **服务依赖检查**
   ```bash
   # 检查服务依赖状态
   docker compose ps redis rabbitmq
   ```

***

## 10. 数据库设置

### 10.1 数据库部署方式

**重要提示：生产环境不建议使用 Docker MySQL！**

**原因**：性能损失、数据安全风险、备份恢复复杂、运维成本高

**生产环境请使用**：宿主机 MySQL 或云数据库（阿里云 RDS、AWS RDS 等）

#### 方式对比

| 特性        | 宿主机 MySQL | Docker MySQL    |
| --------- | --------- | --------------- |
| **性能**    | ⭐⭐⭐⭐⭐ 最佳  | ⭐⭐⭐⭐ 良好         |
| **数据持久化** | ✅ 原生文件系统  | ✅ Docker Volume |
| **管理工具**  | ✅ 任意工具    | ✅ 通过端口映射        |
| **备份恢复**  | ✅ 直接操作    | ⚠️ 需通过容器        |
| **资源消耗**  | 📉 独立进程   | 📈 额外容器         |
| **适用场景**  | 🏢 生产环境   | 🧪 开发/测试        |
| **部署复杂度** | ⚠️ 需手动安装  | ✅ 一键部署          |

##### 推荐选择

- **生产环境** → 宿主机 MySQL
- **开发环境** → Docker MySQL
- **快速体验** → Docker MySQL

### 10.2 宿主机 MySQL（推荐生产环境）

#### 步骤 1：安装 MySQL 8.0+

**Windows**：

```powershell
# 下载 MySQL Installer
# https://dev.mysql.com/downloads/installer/

# 或使用 Chocolatey
choco install mysql

# 启动 MySQL 服务
net start MySQL80

# 设置 root 密码
mysql -u root -p
ALTER USER 'root'@'localhost' IDENTIFIED BY 'your_password';
```

**Linux (Ubuntu/Debian)**：

```bash
# 安装 MySQL
sudo apt update
sudo apt install mysql-server

# 启动服务
sudo systemctl start mysql
sudo systemctl enable mysql

# 安全配置
sudo mysql_secure_installation

# 设置 root 密码
sudo mysql
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'your_password';
FLUSH PRIVILEGES;
EXIT;
```

**macOS**：

```bash
# 使用 Homebrew
brew install mysql

# 启动服务
brew services start mysql

# 设置 root 密码
mysql -u root
ALTER USER 'root'@'localhost' IDENTIFIED BY 'your_password';
FLUSH PRIVILEGES;
EXIT;
```

#### 步骤 2：创建数据库

```sql
-- 连接 MySQL
mysql -u root -p

-- 创建数据库
CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 验证
SHOW DATABASES LIKE 'agpayplusdb';

-- 退出
EXIT;
```

#### 步骤 3：导入初始化脚本

```bash
# 导入 SQL 脚本
mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql

# 验证表是否创建成功
mysql -u root -p agpayplusdb -e "SHOW TABLES;"
```

#### 步骤 4：配置远程访问（Docker 容器访问）

```sql
-- 连接 MySQL
mysql -u root -p

-- 创建远程访问用户（或授权 root）
CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';

-- 刷新权限
FLUSH PRIVILEGES;

-- 验证用户
SELECT host, user FROM mysql.user WHERE user='root';

EXIT;
```

#### 步骤 5：配置 .env 文件

**Windows**：

```env
# 使用宿主机 MySQL
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # 修改为实际密码
```

**Linux**：

```env
# 获取 Docker 网桥 IP
# ip addr show docker0 | grep inet

MYSQL_SERVER_NAME=172.17.0.1  # 使用上面获取的 IP
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # 修改为实际密码
```

**macOS**：

```env
# macOS 使用 Docker Desktop 内置的 host
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # 修改为实际密码
```

### 10.3 Docker MySQL（仅开发/测试）

#### 步骤 1：配置 .env 文件

```env
# 使用 Docker MySQL
MYSQL_SERVER_NAME=db  # 使用服务名
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=mysql123456  # 设置密码
```

#### 步骤 2：启动服务

```bash
# 完整部署（包含 MySQL）
./deploy.ps1  # Windows
./deploy.sh   # Linux/macOS

# 或单独启动 MySQL
docker compose up -d db

# 查看日志
docker compose logs -f db
```

#### 步骤 3：验证部署

```bash
# 查看容器状态
docker compose ps db

# 进入容器检查
docker exec -it agpayplus-db-1 mysql -u root -p

# 查看数据库
SHOW DATABASES;
USE agpayplusdb;
SHOW TABLES;
EXIT;
```

### 10.4 数据库管理

#### 使用 MySQL Workbench

```bash
# 下载安装
https://dev.mysql.com/downloads/workbench/

# 连接配置
Host: localhost (或 172.17.0.1)
Port: 3306
Username: root
Password: your_password
```

#### 使用 Adminer（Web 界面）

添加到 docker-compose.yml：

```yaml
adminer:
  image: adminer
  ports:
    - "8080:8080"
  networks:
    - app-network
```

访问：<http://localhost:8080>

#### 命令行管理

```bash
# 进入 MySQL
docker exec -it agpayplus-db-1 mysql -u root -p  # Docker
mysql -u root -p  # 宿主机

# 常用命令
SHOW DATABASES;
USE agpayplusdb;
SHOW TABLES;
DESCRIBE table_name;
SELECT COUNT(*) FROM table_name;
```

### 10.5 故障排查

#### 问题 1：无法连接数据库

**症状**：

```
Can't connect to MySQL server on 'xxx.xxx.xxx.xxx'
```

**解决方案**：

```bash
# 1. 检查 MySQL 服务状态
# 宿主机
systemctl status mysql  # Linux
brew services list  # macOS
net start MySQL80  # Windows

# Docker
docker compose ps db

# 2. 检查端口
netstat -an | grep 3306  # Linux/macOS
netstat -ano | findstr :3306  # Windows

# 3. 测试连接
telnet localhost 3306
mysql -h localhost -u root -p

# 4. 检查防火墙
sudo ufw allow 3306  # Linux
# Windows: 控制面板 → 防火墙 → 允许应用
```

#### 问题 2：权限错误

**症状**：

```
ERROR 1045 (28000): Access denied for user 'root'@'xxx'
```

**解决方案**：

```sql
-- 重新授权
CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
FLUSH PRIVILEGES;
```

### 10.6 性能优化

**宿主机 MySQL**：

编辑 `my.cnf` 或 `my.ini`：

```ini
[mysqld]
# 基础配置
max_connections = 200
max_allowed_packet = 64M
default-storage-engine = InnoDB

# InnoDB 优化
innodb_buffer_pool_size = 1G  # 设置为物理内存的 50-70%
innodb_log_file_size = 256M
innodb_flush_log_at_trx_commit = 2

# 查询缓存
query_cache_type = 1
query_cache_size = 64M

# 字符集
character-set-server = utf8mb4
collation-server = utf8mb4_unicode_ci
```

**Docker MySQL**：

通过 command 参数：

```yaml
db:
  image: mysql:8.0
  command:
    - --max-connections=200
    - --max-allowed-packet=64M
    - --innodb-buffer-pool-size=1G
    - --character-set-server=utf8mb4
    - --collation-server=utf8mb4_unicode_ci
```

## 11. 部署前检查清单

在执行部署前，请按照此清单逐项检查，确保所有前置条件都已满足。

### 11.1 系统环境检查

#### 1. Docker 环境

- [ ] Docker Desktop 已安装并运行（Windows/macOS）
  ```bash
  docker --version
  # 预期输出：Docker version 20.10.x 或更高
  ```
- [ ] Docker Engine 已安装并运行（Linux）
  ```bash
  docker --version
  sudo systemctl status docker
  ```
- [ ] Docker Compose 可用
  ```bash
  docker compose version
  # 预期输出：Docker Compose version v2.x.x 或更高
  ```
- [ ] Docker 有足够的资源配置
  - **CPU**：至少 4 核
  - **内存**：至少 4GB
  - **磁盘空间**：至少 20GB

#### 2. .NET SDK（用于生成证书）

- [ ] .NET SDK 6.0 或更高版本已安装
  ```bash
  dotnet --version
  # 预期输出：6.0.x 或更高
  ```
- [ ] 如果未安装，请访问：
  - Windows/macOS: <https://dotnet.microsoft.com/download>
  - Linux: <https://docs.microsoft.com/dotnet/core/install/linux>

#### 3. MySQL 数据库

- [ ] MySQL 8.0+ 已安装在宿主机上
  ```bash
  mysql --version
  # 预期输出：mysql Ver 8.0.x
  ```
- [ ] MySQL 服务正在运行
  ```bash
  # Windows
  Get-Service MySQL* | Select Status

  # Linux
  sudo systemctl status mysql

  # macOS
  brew services list | grep mysql
  ```
- [ ] 已创建数据库
  ```sql
  CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
  ```
- [ ] 已导入初始化脚本
  ```bash
  mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql
  ```
- [ ] MySQL 允许远程连接（Docker 容器访问）
  ```sql
  -- 检查用户权限
  SELECT host, user FROM mysql.user WHERE user='root';

  -- 如果没有 '%' 或 '172.%'，执行：
  CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
  GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
  FLUSH PRIVILEGES;
  ```

### 11.2 项目文件检查

#### 1. 代码完整性

- [ ] 项目已克隆到本地
  ```bash
  git clone https://github.com/agoodays/agpayplus.git
  cd agpayplus
  ```
- [ ] 所有必需的目录存在
  ```
  agpayplus/
  ├── aspnet-core/
  │   ├── src/
  │   └── docs/
  └── ant-design-vue/
      ├── agpay-ui-manager/
      ├── agpay-ui-agent/
      ├── agpay-ui-merchant/
      └── agpay-ui-cashier/
  ```
- [ ] RabbitMQ 延迟插件文件存在
  ```bash
  ls aspnet-core/docs/rabbitmq_plugin/rabbitmq_delayed_message_exchange-3.13.0.ez
  ```

#### 2. 部署脚本

- [ ] 部署脚本存在并有执行权限（Linux/macOS）
  ```bash
  ls -l *.sh
  chmod +x *.sh  # 如果需要
  ```
- [ ] PowerShell 脚本存在（Windows）
  ```powershell
  Get-ChildItem *.ps1
  ```

#### 3. 配置文件

- [ ] 环境变量模板文件存在

```bash
ls -l .env.development .env.staging .env.production .env.example
```

### 11.3 环境配置

#### 1. 环境变量配置

- [ ] 已复制并编辑 `.env` 文件

```bash
# Windows
Copy-Item .env.production .env
  
# Linux/macOS
cp .env.production .env
```

- [ ] 核心配置已正确设置
  #### Windows 配置
  ```env
  IPORDOMAIN=localhost                    # ✅ 已设置
  MYSQL_SERVER_NAME=host.docker.internal  # ✅ Windows/macOS 使用此值
  MYSQL_PORT=3306                         # ✅ 已设置
  MYSQL_DATABASE=agpayplusdb              # ✅ 已设置
  MYSQL_USER=root                         # ✅ 已设置
  MYSQL_PASSWORD=your_actual_password     # ⚠️ 需要修改为实际密码
  DATA_PATH_HOST=E:/app/agpayplus         # ⚠️ 修改为实际路径
  CERT_PATH=${USERPROFILE}/.aspnet/https  # ✅ Windows 默认值
  ```
  #### Linux 配置
  ```env
  IPORDOMAIN=localhost                    # ✅ 已设置
  MYSQL_SERVER_NAME=172.17.0.1            # ⚠️ 需要确认宿主机IP
  MYSQL_PORT=3306                         # ✅ 已设置
  MYSQL_DATABASE=agpayplusdb              # ✅ 已设置
  MYSQL_USER=root                         # ✅ 已设置
  MYSQL_PASSWORD=your_actual_password     # ⚠️ 需要修改为实际密码
  DATA_PATH_HOST=/opt/agpayplus           # ✅ Linux 默认值
  CERT_PATH=~/.aspnet/https               # ✅ Linux 默认值
  ```
  #### macOS 配置
  ```env
  IPORDOMAIN=localhost                    # ✅ 已设置
  MYSQL_SERVER_NAME=host.docker.internal  # ✅ macOS 使用此值
  MYSQL_PORT=3306                         # ✅ 已设置
  MYSQL_DATABASE=agpayplusdb              # ✅ 已设置
  MYSQL_USER=root                         # ✅ 已设置
  MYSQL_PASSWORD=your_actual_password     # ⚠️ 需要修改为实际密码
  DATA_PATH_HOST=/opt/agpayplus           # ✅ macOS 默认值
  CERT_PATH=~/.aspnet/https               # ✅ macOS 默认值
  ```

#### 2. Linux 特殊配置

- [ ] 获取 Docker 网桥 IP（Linux）
  ```bash
  ip addr show docker0 | grep inet
  # 或使用默认 IP
  # MYSQL_SERVER_NAME=172.17.0.1
  ```
- [ ] 确认宿主机 MySQL 可从 Docker 网络访问
  ```bash
  # 测试连接
  docker run --rm mysql:8.0 mysql -h 172.17.0.1 -u root -p
  ```

#### 3. 数据目录

- [ ] 数据存储目录已创建（或部署脚本会自动创建）
  ```bash
  # Windows
  New-Item -ItemType Directory -Path E:\app\agpayplus -Force

  # Linux/macOS
  sudo mkdir -p /opt/agpayplus
  sudo chown -R $(whoami):$(whoami) /opt/agpayplus
  ```

### 11.4 SSL 证书

#### 选项 1：自动生成（推荐）

- [ ] 部署脚本会自动生成开发证书
  - 证书名称：agpayplusapi
  - 证书密码：123456
  - 证书路径：`~/.aspnet/https/agpayplusapi.pfx`

#### 选项 2：手动生成

- [ ] 执行证书生成脚本
  ```bash
  # Windows
  .\generate-cert-windows.ps1

  # Linux/macOS
  ./generate-cert-linux.sh
  ```
- [ ] 验证证书文件存在
  ```bash
  # Windows
  Test-Path $env:USERPROFILE\.aspnet\https\agpayplusapi.pfx

  # Linux/macOS
  ls -l ~/.aspnet/https/agpayplusapi.pfx
  ```

#### 选项 3：使用生产证书

- [ ] 准备正式 SSL 证书（.pfx 格式）
- [ ] 复制到证书目录
- [ ] 更新 docker-compose.yml 中的证书密码

### 11.5 网络和端口

#### 1. 端口占用检查

确保以下端口未被占用：

- [ ] **8817**（运营平台前端）
  ```bash
  # Windows
  netstat -ano | findstr :8817

  # Linux/macOS
  lsof -i :8817
  ```
- [ ] **8816**（代理商前端）
- [ ] **8818**（商户前端）
- [ ] **9817/5817**（运营平台 API）
- [ ] **9816/5816**（代理商 API）
- [ ] **9818/5818**（商户 API）
- [ ] **9819/5819**（支付网关 API + 收银台）
- [ ] **6379**（Redis）
- [ ] **5672/15672**（RabbitMQ）

#### 2. 防火墙规则

- [ ] Windows 防火墙允许 Docker 通信
- [ ] Linux iptables/firewalld 允许相关端口
- [ ] 云服务器安全组规则已配置

### 11.6 部署前最终检查

#### 1. 验证 Docker Compose 配置

- [ ] 检查配置语法
  ```bash
  docker compose config
  ```
- [ ] 查看将使用的配置
  ```bash
  docker compose config --services
  # 预期输出：
  # agpay-ui-manager
  # agpay-ui-agent
  # agpay-ui-merchant
  # agpay-manager-api
  # agpay-agent-api
  # agpay-merchant-api
  # agpay-payment-api
  # redis
  # rabbitmq
  ```

#### 2. 磁盘空间

- [ ] 检查可用磁盘空间
  ```bash
  # Windows
  Get-PSDrive C

  # Linux/macOS
  df -h
  ```
- [ ] 至少有 20GB 可用空间（用于 Docker 镜像和容器）

#### 3. 内存和 CPU

- [ ] 系统有足够的可用内存（至少 4GB）
  ```bash
  # Windows
  Get-ComputerInfo | Select-Object CsTotalPhysicalMemory, CsPhyicallyInstalledMemory

  # Linux
  free -h

  # macOS
  top -l 1 | grep PhysMem
  ```

### 11.7 准备开始部署

所有检查项都完成后，您可以开始部署：

#### Windows 部署

```powershell
# 完整部署
.deploy.ps1

# 如果已有证书，跳过证书生成
.deploy.ps1 -SkipCert

# 如果已配置 .env，跳过环境配置
.deploy.ps1 -SkipEnv
```

#### Linux/macOS 部署

```bash
# 完整部署
./deploy.sh

# 跳过证书生成
./deploy.sh --skip-cert

# 跳过环境配置
./deploy.sh --skip-env
```

### 11.8 预期部署时间

- **首次部署**：15-25 分钟（包括下载镜像、构建）
- **后续更新**：5-10 分钟

最慢的部分：

1. Payment API 构建（包含收银台前端）：5-7 分钟
2. 前端构建：每个 3-5 分钟
3. 后端 API 构建：每个 2-3 分钟

## 12. 镜像源配置

### 11.1 Docker 镜像源加速

#### Linux 系统

1. **编辑 Docker 配置文件**
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
   ```
2. **重启 Docker 服务**
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart docker
   ```

#### Windows 系统

1. **打开 Docker Desktop 设置**
2. **选择 Docker Engine**
3. **添加镜像源**
   ```json
   {
     "registry-mirrors": [
       "https://dockerhub.azk8s.cn",
       "https://reg-mirror.qiniu.com",
       "https://hub-mirror.c.163.com",
       "https://mirror.baidubce.com"
     ]
   }
   ```
4. **应用并重启 Docker**

### 10.2 npm 镜像源配置

在前端构建过程中，可以配置 npm 镜像源加速依赖安装：

```bash
# 临时使用
npm install --registry=https://registry.npmmirror.com

# 永久设置
npm config set registry https://registry.npmmirror.com
```

### 10.3 验证镜像源

```bash
# 检查 Docker 镜像源
docker info | grep Registry

# 检查 npm 镜像源
npm config get registry
```

