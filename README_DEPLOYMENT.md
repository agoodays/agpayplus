# AgPay+ 部署方案使用说明

> 🎉 新的多环境部署方案已就绪！

---

## 🚀 立即开始

### 首次部署（3 步）

```bash
# 1. 配置环境
cp .env.production .env
vi .env  # 修改 IPORDOMAIN 等配置

# 2. 生成证书
./generate-cert-linux.sh  # Linux/macOS
.\generate-cert-windows.ps1  # Windows

# 3. 执行部署
./deploy.sh --env production --skip-backup  # Linux/macOS
.\deploy.ps1 -Environment production -SkipBackup  # Windows
```

### 日常更新

```bash
# 更新单个服务
./update.sh --services agpay-manager-api  # Linux/macOS
.\update.ps1 -Services "agpay-manager-api"  # Windows

# 更新多个服务
./update.sh --services "agpay-manager-api agpay-agent-api"  # Linux/macOS
.\update.ps1 -Services "agpay-manager-api","agpay-agent-api"  # Windows
```

### 版本回滚

```bash
# 查看备份列表
./rollback.sh --list  # Linux/macOS
.\rollback.ps1 -List  # Windows

# 回滚到最新备份
./rollback.sh  # Linux/macOS
.\rollback.ps1  # Windows
```

---

## 📚 文档索引

### 🔰 新手必读

**[QUICK_REFERENCE.md](./QUICK_REFERENCE.md)** - 快速参考手册
- 常用命令速查
- 典型使用场景
- 快速故障处理

### 📖 完整指南

**[DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md)** - 完整使用指南
- 详细的环境配置说明
- 完整的部署流程
- 更新和回滚步骤
- Cashier 管理指南

### 🆕 功能说明

**[FEATURES_UPDATE.md](./FEATURES_UPDATE.md)** - 新功能说明
- 新增功能列表
- 特性对比
- 最佳实践
- 性能优化建议

### ✅ 实施报告

**[IMPLEMENTATION_REPORT.md](./IMPLEMENTATION_REPORT.md)** - 实施完成报告
- 完成情况总结
- 文件清单
- 工作流程说明
- 使用示例

### 🔧 故障排查

**[TROUBLESHOOTING.md](./TROUBLESHOOTING.md)** - 故障排查指南
- 网络问题
- 构建问题
- 启动问题
- 数据库问题
- 证书问题

---

## 🎯 核心功能

### ✅ 多环境支持

支持 3 个独立环境，每个环境独立配置：

```bash
./deploy.sh --env development   # 开发环境
./deploy.sh --env staging       # 预发布环境
./deploy.sh --env production    # 生产环境
```

### ✅ 指定服务更新

支持单个或多个服务更新：

```bash
./update.sh --services agpay-manager-api                        # 单个服务
./update.sh --services "agpay-manager-api agpay-agent-api"            # 多个服务
./update.sh --services "agpay-ui-manager agpay-ui-agent agpay-ui-merchant"  # 所有前端
```

### ✅ 完整备份回滚

自动备份，支持回滚到任意版本：

```bash
./rollback.sh --list                      # 查看所有备份
./rollback.sh                             # 回滚到最新
./rollback.sh --backup 20240315_143022    # 回滚到指定版本
```

### ✅ 可选 Cashier 构建

默认不构建 cashier，节省时间：

```bash
# 不构建（默认，推荐）
./update.sh --services agpay-payment-api

# 强制构建（有变更时）
./update.sh --services agpay-payment-api --build-cashier
```

### ✅ 自动健康检查和回滚

更新失败自动回滚，无需人工干预：

```bash
./update.sh --services agpay-payment-api

# 如果失败，脚本会自动：
# 1. 检测服务状态
# 2. 显示错误日志
# 3. 从备份恢复
# 4. 验证恢复状态
```

---

## 📁 新增文件

### 脚本文件

| 文件 | 用途 | 系统 |
|------|------|------|
| `deploy.sh` | 统一部署脚本 | Linux/macOS |
| `deploy.ps1` | 统一部署脚本 | Windows |
| `update.sh` | 服务更新脚本 | Linux/macOS |
| `update.ps1` | 服务更新脚本 | Windows |
| `rollback.sh` | 版本回滚脚本 | Linux/macOS |
| `rollback.ps1` | 版本回滚脚本 | Windows |

### 配置文件

| 文件 | 用途 |
|------|------|
| `.env.development` | 开发环境配置 |
| `.env.staging` | 预发布环境配置 |
| `.env.production` | 生产环境配置 |

### 文档文件

| 文件 | 用途 |
|------|------|
| `DEPLOYMENT_USAGE_GUIDE.md` | 完整使用指南 |
| `QUICK_REFERENCE.md` | 快速参考手册 |
| `FEATURES_UPDATE.md` | 功能更新说明 |
| `IMPLEMENTATION_REPORT.md` | 实施完成报告 |
| `README_DEPLOYMENT.md` | 本文档 |

---

## 🔍 常用命令

### 部署相关

```bash
# 首次部署
./deploy.sh --env production --skip-backup

# 更新部署（会自动备份）
./deploy.sh --env production

# 指定服务部署
./deploy.sh --services "agpay-manager-api agpay-agent-api"

# 强制部署（跳过确认）
./deploy.sh --force
```

### 更新相关

```bash
# 更新所有服务
./update.sh

# 更新单个服务
./update.sh --services agpay-manager-api

# 更新多个服务
./update.sh --services "agpay-manager-api agpay-agent-api agpay-merchant-api"

# 更新 agpay-payment-api 并构建 cashier
./update.sh --services agpay-payment-api --build-cashier
```

### 回滚相关

```bash
# 列出所有备份
./rollback.sh --list

# 回滚到最新备份
./rollback.sh

# 回滚到指定备份
./rollback.sh --backup 20240315_143022

# 回滚指定服务
./rollback.sh --services "agpay-manager-api agpay-agent-api"
```

### 查看状态

```bash
# 查看服务状态
docker compose ps

# 查看日志（所有服务）
docker compose logs -f

# 查看日志（指定服务）
docker compose logs -f agpay-manager-api

# 查看最近 100 行日志
docker compose logs --tail=100 -f agpay-manager-api
```

### 服务管理

```bash
# 重启服务
docker compose restart agpay-manager-api

# 停止服务
docker compose stop agpay-manager-api

# 启动服务
docker compose start agpay-manager-api

# 删除服务
docker compose down
```

---

## 🎓 学习路径

### Level 1: 快速入门（30 分钟）

1. 阅读 [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)
2. 配置开发环境
3. 首次部署测试

### Level 2: 熟练使用（1 小时）

1. 阅读 [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md)
2. 尝试更新单个服务
3. 尝试回滚操作
4. 理解 Cashier 管理

### Level 3: 深入理解（2 小时）

1. 阅读 [FEATURES_UPDATE.md](./FEATURES_UPDATE.md)
2. 理解多环境配置
3. 掌握备份管理
4. 学习故障排查

### Level 4: 高级应用（3+ 小时）

1. 多环境并行部署
2. 灰度发布策略
3. 自定义构建参数
4. 自动化集成

---

## 💡 最佳实践

### 1. 环境隔离

每个环境使用独立的项目名称和镜像前缀：

```bash
# .env.development
COMPOSE_PROJECT_NAME=agpayplus-dev
IMAGE_PREFIX=agpay-dev

# .env.production
COMPOSE_PROJECT_NAME=agpayplus-prod
IMAGE_PREFIX=agpay-prod
```

### 2. 备份策略

- 开发环境：可跳过备份
- 预发布环境：建议备份
- 生产环境：必须备份

### 3. Cashier 构建

- 首次部署：使用 `--build-cashier`
- Cashier 有变更：使用 `--build-cashier`
- 仅后端变更：不使用

### 4. 更新策略

- 小范围更新：使用 `update.sh` 指定服务
- 大范围更新：使用 `deploy.sh`
- 灰度发布：先更新一个服务观察，再更新其他

### 5. 回滚准备

- 定期检查备份：`./rollback.sh --list`
- 保持足够磁盘空间（建议 10GB+）
- 测试回滚流程

---

## 🔧 配置要点

### 必须配置

```bash
# 域名或 IP
IPORDOMAIN=yourdomain.com

# MySQL 配置
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PASSWORD=your_secure_password

# 数据路径
DATA_PATH_HOST=/var/agpayplus  # Linux
DATA_PATH_HOST=E:/app/agpayplus  # Windows
```

### 可选配置

```bash
# Cashier 构建
BUILD_CASHIER=false  # 默认不构建

# 日志
ENABLE_SEQ=true  # 启用 Seq 日志
```

---

## 📊 服务端口

| 服务 | HTTP | HTTPS | 说明 |
|------|------|-------|------|
| UI Manager | - | 8817 | 运营平台前端 |
| UI Agent | - | 8816 | 代理商系统前端 |
| UI Merchant | - | 8818 | 商户系统前端 |
| Manager API | 5817 | 9817 | 运营平台 API |
| Agent API | 5816 | 9816 | 代理商系统 API |
| Merchant API | 5818 | 9818 | 商户系统 API |
| Payment API | 5819 | 9819 | 支付网关 API |
| Cashier | - | 9819/cashier | 收银台 |
| Seq | 5341 | - | 日志查看 |
| RabbitMQ | 5672 | 15672 | 消息队列 |

---

## 🆘 获取帮助

### 查看脚本帮助

```bash
# Linux/macOS
./deploy.sh --help
./update.sh --help
./rollback.sh --help

# Windows
Get-Help .\deploy.ps1 -Full
Get-Help .\update.ps1 -Full
Get-Help .\rollback.ps1 -Full
```

### 遇到问题

1. 查看 [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)
2. 查看日志：`docker compose logs -f`
3. 查看服务状态：`docker compose ps -a`
4. 提交 Issue

---

## ✨ 特色功能

### 🎯 零配置首次部署

脚本自动检测首次部署，无需额外配置。

### 🔄 智能备份管理

- 自动保留最近 5 个备份
- 自动清理过期备份
- 按环境独立存储

### 🛡️ 失败自动回滚

更新失败时自动回滚，零人工干预。

### ⚡ 可选 Cashier 构建

默认不构建，节省 3-5 分钟。

### 🌐 跨平台一致性

Linux/Windows 功能完全一致。

### 📊 详细日志输出

每步都有清晰的进度提示和彩色输出。

### ✅ 自动健康检查

自动验证服务状态，显示失败日志。

---

## 🎉 开始使用

选择您的入口：

- **新手**: 👉 [QUICK_REFERENCE.md](./QUICK_REFERENCE.md)
- **详细**: 👉 [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md)
- **功能**: 👉 [FEATURES_UPDATE.md](./FEATURES_UPDATE.md)
- **报告**: 👉 [IMPLEMENTATION_REPORT.md](./IMPLEMENTATION_REPORT.md)
- **故障**: 👉 [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

---

**版本**: 2.0  
**发布日期**: 2024-03-15  
**维护者**: AgPay+ Team
