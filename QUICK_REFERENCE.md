# AgPay+ 部署快速参考

> 快速查找命令和操作指南

---

## 🚀 快速命令

### 首次部署

```bash
# Linux/macOS
./deploy.sh --env production --skip-backup

# Windows
.\deploy.ps1 -Environment production -SkipBackup
```

### 日常更新

```bash
# 更新所有服务
./update.sh                               # Linux
.\update.ps1                              # Windows

# 更新单个服务
./update.sh --services agpay-manager-api        # Linux
.\update.ps1 -Services "agpay-manager-api"      # Windows

# 更新多个服务
./update.sh --services "agpay-manager-api agpay-agent-api"              # Linux
.\update.ps1 -Services "agpay-manager-api","agpay-agent-api"            # Windows
```

### 回滚

```bash
# 查看备份列表
./rollback.sh --list                      # Linux
.\rollback.ps1 -List                      # Windows

# 回滚到最新备份
./rollback.sh                             # Linux
.\rollback.ps1                            # Windows

# 回滚到指定备份
./rollback.sh --backup 20240315_143022    # Linux
.\rollback.ps1 -Backup "20240315_143022"  # Windows
```

---

## 📋 服务列表

| 服务名 | 说明 | 端口 |
|-------|------|------|
| `agpay-ui-manager` | 运营平台前端 | 8817 |
| `agpay-ui-agent` | 代理商系统前端 | 8816 |
| `agpay-ui-merchant` | 商户系统前端 | 8818 |
| `agpay-manager-api` | 运营平台 API | 5817/9817 |
| `agpay-agent-api` | 代理商系统 API | 5816/9816 |
| `agpay-merchant-api` | 商户系统 API | 5818/9818 |
| `agpay-payment-api` | 支付网关 API | 5819/9819 |

---

## ⚙️ 环境选项

```bash
--env development    # 开发环境
--env staging        # 预发布环境
--env production     # 生产环境（默认）
```

---

## 🎯 常用场景

### 场景 1: 仅更新运营平台

```bash
# Linux
./update.sh --services "agpay-ui-manager agpay-manager-api"

# Windows
.\update.ps1 -Services "agpay-ui-manager","agpay-manager-api"
```

### 场景 2: 更新所有 API

```bash
# Linux
./update.sh --services "agpay-manager-api agpay-agent-api agpay-merchant-api agpay-payment-api"

# Windows
.\update.ps1 -Services "agpay-manager-api","agpay-agent-api","agpay-merchant-api","agpay-payment-api"
```

### 场景 3: 更新所有前端

```bash
# Linux
./update.sh --services "agpay-ui-manager agpay-ui-agent agpay-ui-merchant"

# Windows
.\update.ps1 -Services "agpay-ui-manager","agpay-ui-agent","agpay-ui-merchant"
```

### 场景 4: 更新支付网关（含 Cashier）

```bash
# Linux
./update.sh --services agpay-payment-api --build-cashier

# Windows
.\update.ps1 -Services "agpay-payment-api" -BuildCashier
```

### 场景 5: 开发环境快速更新

```bash
# Linux
./update.sh --env development --services "agpay-ui-manager" --force

# Windows
.\update.ps1 -Environment development -Services "agpay-ui-manager" -Force
```

---

## 🔍 查看和管理

### 查看服务状态

```bash
docker compose ps
```

### 查看服务日志

```bash
# 所有服务
docker compose logs -f

# 特定服务
docker compose logs -f agpay-manager-api

# 最近 100 行
docker compose logs --tail=100 -f agpay-manager-api
```

### 重启服务

```bash
# 单个服务
docker compose restart agpay-manager-api

# 所有服务
docker compose restart
```

### 停止服务

```bash
# 单个服务
docker compose stop agpay-manager-api

# 所有服务
docker compose stop
```

### 查看备份

```bash
# Linux
./rollback.sh --list

# Windows
.\rollback.ps1 -List
```

---

## 🔧 故障处理

### 构建失败

```bash
# 清理缓存重新构建
docker builder prune -f
docker compose build --no-cache
```

### 端口冲突

```bash
# Linux - 查找占用端口的进程
sudo lsof -ti:8817

# Windows - 查找占用端口的进程
netstat -ano | findstr :8817

# 修改 .env 文件中的端口配置
```

### 服务启动失败

```bash
# 查看详细日志
docker compose logs --tail=100 service-name

# 进入容器调试
docker exec -it service-name /bin/bash

# 重启服务
docker compose restart service-name
```

### 数据库连接失败

```bash
# 检查 MySQL 容器
docker compose logs mysql

# 检查配置
cat .env | grep MYSQL

# 进入 MySQL 容器测试
docker exec -it mysql mysql -u root -p
```

### 清理和重置

```bash
# 停止所有服务
docker compose down

# 清理未使用的资源
docker system prune -a --volumes

# 重新部署
./deploy.sh
```

---

## 📱 访问地址

假设 `IPORDOMAIN=yourdomain.com`：

- **运营平台**: https://yourdomain.com:8817
- **代理商系统**: https://yourdomain.com:8816
- **商户系统**: https://yourdomain.com:8818
- **支付网关**: https://yourdomain.com:9819
- **收银台**: https://yourdomain.com:9819/cashier
- **日志查看**: http://yourdomain.com:5341 (Seq)
- **RabbitMQ**: http://yourdomain.com:15672

---

## 🔐 证书管理

### 生成证书

```bash
# Linux
./generate-cert-linux.sh

# Windows
.\generate-cert-windows.ps1
```

### 证书位置

- **Linux**: `~/.aspnet/https/agpayplusapi.pfx`
- **Windows**: `%USERPROFILE%\.aspnet\https\agpayplusapi.pfx`

---

## 📂 文件和目录

### 配置文件

```
.env                    # 当前环境配置（自动生成）
.env.development        # 开发环境配置
.env.staging            # 预发布环境配置
.env.production         # 生产环境配置
docker-compose.yml      # Docker Compose 配置
```

### 脚本文件

```
deploy.sh / deploy.ps1              # 部署脚本
update.sh / update.ps1              # 更新脚本
rollback.sh / rollback.ps1          # 回滚脚本
generate-cert-linux.sh              # 证书生成（Linux）
generate-cert-windows.ps1           # 证书生成（Windows）
```

### 备份目录

```
.backup/
├── production_update_20240315_143022/
├── production_update_20240315_120530/
├── development_update_20240315_140000/
├── latest_production
└── latest_development
```

---

## 💡 提示和技巧

### 1. 使用环境变量覆盖

```bash
# 临时修改镜像标签
IMAGE_TAG=v2.0 ./deploy.sh
```

### 2. 跳过确认快速部署

```bash
# Linux
./deploy.sh --force

# Windows
.\deploy.ps1 -Force
```

### 3. 仅构建不部署

```bash
docker compose build service-name
```

### 4. 查看镜像列表

```bash
docker compose images
```

### 5. 清理旧备份

```bash
# 仅保留最近 2 个备份
ls -1t .backup/ | grep production | tail -n +3 | xargs -I {} rm -rf .backup/{}
```

### 6. 监控资源使用

```bash
# 实时监控
docker stats

# 查看磁盘使用
docker system df
```

### 7. 批量操作

```bash
# 重启所有 API 服务
docker compose restart agpay-manager-api agpay-agent-api agpay-merchant-api agpay-payment-api

# 查看所有 API 日志
docker compose logs -f agpay-manager-api agpay-agent-api merchant-api agpay-payment-api
```

---

## 🔗 帮助和文档

- **完整文档**: [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md)
- **故障排查**: [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)
- **镜像源配置**: [DOCKER_MIRROR_GUIDE.md](./DOCKER_MIRROR_GUIDE.md)
- **Cashier 说明**: [CASHIER_DEPLOYMENT.md](./CASHIER_DEPLOYMENT.md)

### 查看脚本帮助

```bash
# Linux
./deploy.sh --help
./update.sh --help
./rollback.sh --help

# Windows
Get-Help .\deploy.ps1
Get-Help .\update.ps1
Get-Help .\rollback.ps1
```

---

## ⚡ 一键命令

### 完整首次部署

```bash
# Linux
cp .env.production .env && \
./generate-cert-linux.sh && \
./deploy.sh --env production --skip-backup

# Windows
Copy-Item .env.production .env; `
.\generate-cert-windows.ps1; `
.\deploy.ps1 -Environment production -SkipBackup
```

### 快速更新和查看日志

```bash
# Linux
./update.sh --services agpay-manager-api && docker compose logs -f agpay-manager-api

# Windows
.\update.ps1 -Services "agpay-manager-api"; docker compose logs -f agpay-manager-api
```

### 完整重置和重新部署

```bash
# Linux
docker compose down -v && \
docker system prune -a -f && \
./deploy.sh --env production

# Windows
docker compose down -v; `
docker system prune -a -f; `
.\deploy.ps1 -Environment production
```

---

**快速参考版本**: 2.0  
**最后更新**: 2024-03-15
