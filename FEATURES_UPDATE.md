# AgPay+ 多环境部署方案 - 新功能说明

> 完整的多环境、多系统部署、更新和回滚方案

---

## 🎉 新增功能

### ✅ 1. 多环境支持

支持三个独立环境，每个环境使用独立的配置文件：

- **development** - 开发环境
- **staging** - 预发布环境  
- **production** - 生产环境

```bash
# 使用不同环境
./deploy.sh --env development
./deploy.sh --env staging
./deploy.sh --env production
```

### ✅ 2. 统一部署脚本

**单一脚本，双重功能**：
- 首次部署：自动检测并初始化环境
- 更新部署：自动备份、支持回滚

```bash
# 首次部署
./deploy.sh --skip-backup

# 更新部署（自动备份）
./deploy.sh
```

### ✅ 3. 独立更新脚本

专门用于服务更新的脚本，支持：
- 指定单个或多个服务更新
- 自动备份当前版本
- 健康检查
- 失败自动回滚

```bash
# 更新单个服务
./update.sh --services manager-api

# 更新多个服务
./update.sh --services "manager-api agent-api"
```

### ✅ 4. 完整回滚功能

支持回滚到任意备份版本：
- 列出所有可用备份
- 回滚到指定版本
- 支持指定服务回滚
- 自动恢复配置文件

```bash
# 查看所有备份
./rollback.sh --list

# 回滚到最新备份
./rollback.sh

# 回滚到指定版本
./rollback.sh --backup 20240315_143022
```

### ✅ 5. 可选 Cashier 构建

Cashier（收银台）构建可选，节省构建时间：

**默认模式（推荐）**: 使用现有 cashier
```bash
BUILD_CASHIER=false
./update.sh --services payment-api
```

**构建模式**: 重新构建 cashier
```bash
BUILD_CASHIER=true
./update.sh --services payment-api --build-cashier
```

### ✅ 6. 自动备份管理

- **自动备份**: 每次更新前自动备份
- **版本管理**: 保留最近 5 个备份
- **自动清理**: 超过限制自动删除旧备份
- **独立存储**: 按环境分别存储备份

### ✅ 7. 健康检查和自动回滚

部署/更新后自动健康检查：
- 验证所有服务运行状态
- 失败服务自动回滚
- 显示详细错误日志
- 零停机时间（回滚时）

### ✅ 8. 跨平台支持

**Linux/macOS**: Shell 脚本 (.sh)
```bash
./deploy.sh
./update.sh
./rollback.sh
```

**Windows**: PowerShell 脚本 (.ps1)
```powershell
.\deploy.ps1
.\update.ps1
.\rollback.ps1
```

---

## 📁 新增文件

### 脚本文件

| 文件 | 说明 | 系统 |
|------|------|------|
| `deploy.sh` | 统一部署脚本 | Linux/macOS |
| `deploy.ps1` | 统一部署脚本 | Windows |
| `update.sh` | 服务更新脚本 | Linux/macOS |
| `update.ps1` | 服务更新脚本 | Windows |
| `rollback.sh` | 版本回滚脚本 | Linux/macOS |
| `rollback.ps1` | 版本回滚脚本 | Windows |

### 配置文件

| 文件 | 说明 |
|------|------|
| `.env.development` | 开发环境配置模板 |
| `.env.staging` | 预发布环境配置模板 |
| `.env.production` | 生产环境配置模板 |

### 文档文件

| 文件 | 说明 |
|------|------|
| `DEPLOYMENT_USAGE_GUIDE.md` | 完整使用文档 |
| `QUICK_REFERENCE.md` | 快速参考手册 |
| `FEATURES_UPDATE.md` | 本文档 |

### Dockerfile

| 文件 | 说明 |
|------|------|
| `aspnet-core/src/AGooday.AgPay.Payment.Api/Dockerfile.flexible` | 支持可选 cashier 构建的 Dockerfile |

---

## 🚀 使用方法

### 1. 首次部署

```bash
# 1. 配置环境
cp .env.production .env
vi .env  # 修改 IPORDOMAIN 等配置

# 2. 生成证书
./generate-cert-linux.sh  # Linux
.\generate-cert-windows.ps1  # Windows

# 3. 执行部署
./deploy.sh --env production --skip-backup  # Linux
.\deploy.ps1 -Environment production -SkipBackup  # Windows
```

### 2. 日常更新

```bash
# 更新单个服务
./update.sh --services manager-api  # Linux
.\update.ps1 -Services "manager-api"  # Windows

# 更新多个服务
./update.sh --services "manager-api agent-api"  # Linux
.\update.ps1 -Services "manager-api","agent-api"  # Windows
```

### 3. 版本回滚

```bash
# 查看备份
./rollback.sh --list  # Linux
.\rollback.ps1 -List  # Windows

# 回滚到最新
./rollback.sh  # Linux
.\rollback.ps1  # Windows
```

---

## 📖 文档导航

### 快速入门
- [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - 快速参考手册（推荐新手）

### 完整文档
- [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md) - 完整使用指南

### 专题文档
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - 故障排查指南
- [DOCKER_MIRROR_GUIDE.md](./DOCKER_MIRROR_GUIDE.md) - Docker 镜像源配置
- [CASHIER_DEPLOYMENT.md](./CASHIER_DEPLOYMENT.md) - Cashier 部署说明

---

## 🎯 典型场景

### 场景 1: 生产环境首次部署

```bash
# Linux
cp .env.production .env && \
./generate-cert-linux.sh && \
./deploy.sh --env production --skip-backup --build-cashier

# Windows
Copy-Item .env.production .env; `
.\generate-cert-windows.ps1; `
.\deploy.ps1 -Environment production -SkipBackup -BuildCashier
```

### 场景 2: 仅更新运营平台

```bash
# Linux
./update.sh --services "ui-manager manager-api"

# Windows
.\update.ps1 -Services "ui-manager","manager-api"
```

### 场景 3: 更新失败自动回滚

更新脚本会自动检测服务健康状态，失败时自动回滚：

```bash
./update.sh --services payment-api

# 输出示例：
# [6/7] 更新服务...
#   ✅ payment-api 更新成功
# [7/7] 健康检查...
#   ❌ payment-api: exited
#   
# 开始自动回滚...
# [1/5] 检查备份...
#   ✅ 找到备份: production_update_20240315_143022
# ...
# ✅ 回滚成功！
```

### 场景 4: 多环境并行部署

```bash
# 终端 1: 开发环境
./deploy.sh --env development

# 终端 2: 预发布环境  
./deploy.sh --env staging

# 终端 3: 生产环境
./deploy.sh --env production
```

### 场景 5: 灰度发布

```bash
# 1. 先更新一个服务
./update.sh --services manager-api

# 2. 观察运行状况
docker compose logs -f manager-api

# 3. 确认无问题后更新其他服务
./update.sh --services "agent-api merchant-api payment-api"
```

---

## 🔍 关键特性对比

| 特性 | 旧方案 | 新方案 |
|------|--------|--------|
| 多环境支持 | ❌ | ✅ 3 个环境 |
| 统一部署脚本 | ❌ 分散 | ✅ 单一脚本 |
| 指定服务更新 | ❌ | ✅ 支持 |
| 自动备份 | ❌ | ✅ 自动 |
| 版本回滚 | ❌ | ✅ 完整支持 |
| 健康检查 | ❌ | ✅ 自动检查 |
| 自动回滚 | ❌ | ✅ 失败自动回滚 |
| Cashier 可选构建 | ❌ 总是构建 | ✅ 可选 |
| 跨平台 | 部分 | ✅ 完整支持 |
| 备份管理 | ❌ | ✅ 自动管理 |

---

## 💡 最佳实践

### 1. 环境隔离

每个环境使用独立配置文件和项目名称：

```bash
# .env.development
COMPOSE_PROJECT_NAME=agpayplus-dev
IMAGE_PREFIX=agpay-dev

# .env.production
COMPOSE_PROJECT_NAME=agpayplus-prod
IMAGE_PREFIX=agpay-prod
```

### 2. 备份策略

- **开发环境**: 可以跳过备份（`--skip-backup`）
- **预发布环境**: 建议备份
- **生产环境**: 必须备份（默认行为）

### 3. Cashier 构建

- **首次部署**: 使用 `--build-cashier`
- **Cashier 有变更**: 使用 `--build-cashier`
- **仅后端变更**: 不使用（默认）

### 4. 更新策略

- **小范围更新**: 使用 `update.sh` 指定服务
- **大范围更新**: 使用 `deploy.sh`
- **测试后发布**: 先更新开发环境，再更新生产环境

### 5. 回滚准备

定期检查备份：
```bash
./rollback.sh --list
```

保持足够磁盘空间存储备份（建议至少 10GB）

---

## 🔧 配置说明

### 必须配置项

```bash
# 域名或 IP
IPORDOMAIN=yourdomain.com

# MySQL 配置
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_USER=root
MYSQL_PASSWORD=your_secure_password

# 数据路径
DATA_PATH_HOST=/var/agpayplus  # Linux
DATA_PATH_HOST=E:/app/agpayplus  # Windows
```

### 可选配置项

```bash
# 项目名称（用于多环境隔离）
COMPOSE_PROJECT_NAME=agpayplus-prod

# 镜像前缀（用于多环境隔离）
IMAGE_PREFIX=agpay-prod

# Cashier 构建
BUILD_CASHIER=false

# 日志配置
ENABLE_SEQ=true
```

---

## 📊 性能优化

### 1. 跳过 Cashier 构建

不构建 cashier 可以节省 3-5 分钟构建时间：

```bash
BUILD_CASHIER=false
```

### 2. 指定服务更新

仅更新必要的服务：

```bash
./update.sh --services manager-api  # 仅更新一个服务
```

### 3. 并行构建

Docker Compose 会自动并行构建独立的服务。

### 4. 使用镜像缓存

Docker 会自动使用构建缓存，除非使用 `--no-cache`。

---

## 🛡️ 安全建议

### 1. 环境文件权限

```bash
chmod 600 .env*
```

### 2. 密码管理

不要在配置文件中使用默认密码：

```bash
MYSQL_PASSWORD=your_secure_password_here  # 改为强密码
CERT_PASSWORD=your_cert_password  # 改为强密码
```

### 3. 证书管理

定期更新 SSL 证书：

```bash
./generate-cert-linux.sh  # Linux
.\generate-cert-windows.ps1  # Windows
```

### 4. 备份加密

生产环境建议加密备份文件。

---

## 📞 获取帮助

### 查看脚本帮助

```bash
# Linux
./deploy.sh --help
./update.sh --help
./rollback.sh --help

# Windows
Get-Help .\deploy.ps1 -Full
Get-Help .\update.ps1 -Full
Get-Help .\rollback.ps1 -Full
```

### 查看文档

- **快速开始**: [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)
- **完整指南**: [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md)
- **故障排查**: [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

### 提交问题

遇到问题时，请提供：
1. 操作系统和版本
2. Docker 版本
3. 错误信息完整输出
4. 相关日志
5. 操作步骤

---

## 🎓 学习路径

### 新手推荐

1. 阅读 [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)
2. 使用开发环境测试：`./deploy.sh --env development --skip-backup`
3. 尝试更新服务：`./update.sh --services manager-api`
4. 尝试回滚：`./rollback.sh --list` 和 `./rollback.sh`

### 进阶使用

1. 阅读 [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md)
2. 理解多环境配置
3. 掌握指定服务更新
4. 了解备份和回滚机制

### 高级应用

1. 多环境并行部署
2. 灰度发布策略
3. 自定义构建参数
4. 自动化集成

---

**版本**: 2.0  
**发布日期**: 2024-03-15  
**维护者**: AgPay+ Team  
**许可**: 与项目主体一致
