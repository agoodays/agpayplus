# AgPay+ Docker 快速部署

> 一键部署 AgPay+ 支付系统，支持 Windows、Linux 和 macOS

## 🚀 5分钟快速启动

### Windows 环境

```powershell
# 1. 选择环境配置
Copy-Item .env.production .env      # 生产环境
# Copy-Item .env.development .env   # 开发环境
# Copy-Item .env.staging .env       # 预发布环境

# 2. 编辑配置文件
notepad .env  # 修改 MySQL 密码、域名等配置

# 3. 一键部署
.\deploy.ps1
```

### Linux/macOS 环境

```bash
# 1. 添加执行权限
chmod +x *.sh

# 2. 选择环境配置
cp .env.production .env      # 生产环境
# cp .env.development .env   # 开发环境
# cp .env.staging .env       # 预发布环境

# 3. 编辑配置文件
vim .env  # 修改配置

# 4. 一键部署
./deploy.sh
```

## 📋 前置要求

### 所有平台
- ✅ Docker 20.10+
- ✅ Docker Compose 2.0+
- ✅ MySQL 8.0+（安装在宿主机）
- ✅ .NET SDK 6.0+（用于生成证书）

### 准备数据库

```sql
-- 创建数据库
CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 导入初始化脚本
mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql
```

## ⚙️ 配置说明

### 核心配置项（.env 文件）

```env
# 服务器地址
IPORDOMAIN=localhost

# MySQL 配置
MYSQL_SERVER_NAME=host.docker.internal  # Windows/macOS
# MYSQL_SERVER_NAME=172.17.0.1          # Linux
MYSQL_PASSWORD=your_password            # 修改为实际密码

# 数据存储路径
DATA_PATH_HOST=E:/app/agpayplus         # Windows
# DATA_PATH_HOST=/opt/agpayplus         # Linux/macOS
```

### 平台差异

| 配置项 | Windows/macOS | Linux |
|--------|---------------|-------|
| MySQL 地址 | `host.docker.internal` | `172.17.0.1` 或宿主机 IP |
| 数据路径 | `E:/app/agpayplus` | `/opt/agpayplus` |
| 证书路径 | `%USERPROFILE%\.aspnet\https` | `~/.aspnet/https` |

## 🌐 访问地址

部署完成后，访问以下地址：

| 服务 | 地址 | 说明 |
|------|------|------|
| 🖥️ 运营平台 | https://localhost:8817 | 系统管理后台 |
| 👥 代理商系统 | https://localhost:8816 | 代理商管理 |
| 🏪 商户系统 | https://localhost:8818 | 商户管理 |
| 💳 支付网关 | https://localhost:9819 | 支付 API 服务 |
| 💰 收银台 | https://localhost:9819/cashier | 支付收银页面<br>（集成在支付网关中） |
| 🐰 RabbitMQ | http://localhost:15672 | 消息队列管理<br>账号：admin/admin |

## 🔄 更新服务

### 更新所有服务

**Windows:**
```powershell
.\update.ps1
```

**Linux/macOS:**
```bash
./update.sh
```

### 更新指定服务

**Windows:**
```powershell
# 更新运营平台前后端
.\update.ps1 -Services "ui-manager,manager-api"

# 更新所有前端
.\update.ps1 -Services "ui-manager,ui-agent,ui-merchant"

# 更新支付网关（包含收银台前端）
.\update.ps1 -Services "payment-api"

# 更新支付网关并重新构建 Cashier
.\update.ps1 -Services "payment-api" -BuildCashier
```

**Linux/macOS:**
```bash
# 更新运营平台前后端
./update.sh --services "ui-manager,manager-api"

# 更新所有前端
./update.sh --services "ui-manager,ui-agent,ui-merchant"

# 更新支付网关并重新构建 Cashier
./update.sh --services "payment-api" --build-cashier
```

### 可用服务列表

- `ui-manager` - 运营平台前端
- `ui-agent` - 代理商系统前端
- `ui-merchant` - 商户系统前端
- `manager-api` - 运营平台后端
- `agent-api` - 代理商系统后端
- `merchant-api` - 商户系统后端
- `payment-api` - 支付网关后端（包含收银台前端）

**注意**：
- 收银台（cashier）前端已集成到 `payment-api` 服务中
- 通过 `/cashier` 路径访问
- 默认不重新构建 cashier（节省时间）
- 如果 cashier 有变更，使用 `--build-cashier` 参数

## 🔙 版本回滚

### 查看可用备份

**Windows:**
```powershell
.\rollback.ps1 -List
```

**Linux/macOS:**
```bash
./rollback.sh --list
```

### 回滚到最新版本

**Windows:**
```powershell
.\rollback.ps1
```

**Linux/macOS:**
```bash
./rollback.sh
```

### 回滚到指定版本

**Windows:**
```powershell
.\rollback.ps1 -Backup "20240315_143022"
```

**Linux/macOS:**
```bash
./rollback.sh --backup "20240315_143022"
```

### 回滚指定服务

**Windows:**
```powershell
.\rollback.ps1 -Services "manager-api,agent-api"
```

**Linux/macOS:**
```bash
./rollback.sh --services "manager-api,agent-api"
```

## 📦 常用命令

```bash
# 查看服务状态
docker compose ps

# 查看日志
docker compose logs -f [service-name]

# 重启服务
docker compose restart [service-name]

# 停止所有服务
docker compose down

# 停止并删除数据
docker compose down -v
```

## ❓ 常见问题

### 1. Windows MySQL 连接失败

```env
# 确保 .env 配置正确
MYSQL_SERVER_NAME=host.docker.internal

# 确保 MySQL 允许远程连接
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
```

### 2. Linux MySQL 连接失败

```bash
# 获取 Docker 网桥 IP
ip addr show docker0 | grep inet

# 或使用宿主机 IP
MYSQL_SERVER_NAME=192.168.1.100
```

### 3. SSL 证书警告

浏览器首次访问会提示证书不受信任，这是正常的（开发证书）。

**解决方案：**
- 点击"继续访问"（开发环境）
- 生产环境请使用正式 SSL 证书

### 4. 端口被占用

```bash
# 修改 docker-compose.yml 中的端口映射
ports:
  - "18817:80"  # 将 8817 改为其他端口
```

### 5. 构建失败

```bash
# 清理 Docker 缓存
docker system prune -a

# 重新部署
.\deploy.ps1  # Windows
./deploy.sh   # Linux/macOS
```

### 6. 更新失败需要回滚

```bash
# 自动回滚到最新备份
.\rollback.ps1  # Windows
./rollback.sh   # Linux/macOS

# 查看回滚日志
docker compose logs -f
```

## 📚 完整文档

详细的部署文档、配置说明和故障排查，请参考：

- 📖 [快速参考手册](QUICK_REFERENCE.md) - 常用命令速查
- 📘 [完整使用指南](DEPLOYMENT_USAGE_GUIDE.md) - 详细部署步骤
- 🔧 [故障排查指南](TROUBLESHOOTING.md) - 常见问题解决
- 📊 [数据库搭建](DATABASE_SETUP.md) - MySQL 配置
- ✅ [部署检查清单](DEPLOYMENT_CHECKLIST.md) - 上线前检查

## 🔐 生产环境

生产环境部署前，请务必：

1. ✅ 修改所有默认密码
2. ✅ 配置正式 SSL 证书
3. ✅ 使用实际域名替换 localhost
4. ✅ 配置反向代理（Nginx/Traefik）
5. ✅ 启用日志轮转
6. ✅ 配置资源限制
7. ✅ 设置监控告警
8. ✅ 定期备份数据库
9. ✅ 测试回滚流程

详见：[部署检查清单](DEPLOYMENT_CHECKLIST.md)

## 🏭️ 项目架构

```
agpayplus/
├── 🔧 配置文件
│   ├── docker-compose.yml       # Docker 编排配置
│   ├── .env                     # 环境变量（当前使用，从模板复制）
│   ├── .env.development         # 开发环境模板
│   ├── .env.staging             # 预发布环境模板
│   ├── .env.production          # 生产环境模板
│   └── .env.example             # 配置示例
├── 📜 部署脚本
│   ├── deploy.sh / deploy.ps1           # 统一部署脚本
│   ├── update.sh / update.ps1           # 服务更新脚本
│   ├── rollback.sh / rollback.ps1       # 版本回滚脚本
│   ├── generate-cert-linux.sh           # Linux/macOS 证书生成
│   └── generate-cert-windows.ps1        # Windows 证书生成
├── 🔙 后端服务
│   └── aspnet-core/src/
│       ├── AGooday.AgPay.Manager.Api/    # 运营平台 API
│       ├── AGooday.AgPay.Agent.Api/      # 代理商 API
│       ├── AGooday.AgPay.Merchant.Api/   # 商户 API
│       └── AGooday.AgPay.Payment.Api/    # 支付网关 API
└── 🎨 前端服务
    └── ant-design-vue/
        ├── agpay-ui-manager/    # 运营平台前端
        ├── agpay-ui-agent/      # 代理商前端
        ├── agpay-ui-merchant/   # 商户前端
        └── agpay-ui-cashier/    # 收银台前端（打包到 Payment API）
```

## 🤝 获取帮助

- 📖 [快速参考](QUICK_REFERENCE.md) - 常用命令速查
- 📘 [完整指南](DEPLOYMENT_USAGE_GUIDE.md) - 详细部署说明
- 🔧 [故障排查](TROUBLESHOOTING.md) - 问题解决方案
- 🐛 [问题反馈](https://github.com/agoodays/agpayplus/issues)
- 💬 [讨论区](https://github.com/agoodays/agpayplus/discussions)

---

**开始您的支付系统之旅！** 🎉
