# 聚合支付平台
AgPay 是一套适合互联网企业使用的支付系统，支持多渠道服务商和普通商户模式。已对接微信支付官方、支付宝官方、云闪付、随行付、乐刷、嘉联、汇付斗拱、银盛等接口，支持聚合码支付。AgPay 基于.Net 9，使用 Web API 开发，实现权限管理功能，是一套非常实用的 Web 开发框架。AgPay 对应的前端项目，包括运营平台、代理商系统、商户系统、聚合码收银台。前端技术以 Vue 为主，框架使用 Ant Design Vue 开发。

数字化聚合支付平台，多渠道接口、交易、退款、分账、转账。支持 SAAS 模式，支持服务商模式，多个服务商独立运营，支持多级代理商，每级代理支持分润，为商户提供线上线下支付产品，支持 Docker 一键部署。

```
数据库脚本位置根目录 aspnet-core/docs/sql 文件夹 目前仅提供了 MySql 脚本。

后端技术：.Net9、EFCore9、Web API、Swagger、WebSocket、AutoMapper、FluentValidation、Log4Net、MediatR、Redis、RabbitMQ、Quartz.NET、SkiaSharp

前端技术：Vue2.x、Antd Of Vue 2.x

开发工具：Visual Studio 2022、SQLyog、WebStorm

接口文档：https://www.yuque.com/xiangyisheng/agooday/cweewhugp7h7hvml
```

### 工程结构

![输入图片说明](docs/images/project-map.png)

![输入图片说明](docs/images/project-map-2.png)

### 功能列表

![输入图片说明](docs/images/mgr.png)

![输入图片说明](docs/images/agent.png)

![输入图片说明](docs/images/mch.png)

### ✨  部分截图

![输入图片说明](docs/images/main-page.png)

| ![输入图片说明](docs/images/login-page.png) | ![输入图片说明](docs/images/main-page.png) |
|-----------------------------------|---|

| ![输入图片说明](docs/images/users-page.png) | ![输入图片说明](docs/images/sys-config-sms-page.png) |
|-----------------------------------|---|

| ![输入图片说明](docs/images/store-add-page.png) | ![输入图片说明](docs/images/qr-shell-add-page-a.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/qr-shell-add-page-b.png) | ![输入图片说明](docs/images/qr-shell-add-page-b-view.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/qr-shell-page-view.png) | ![输入图片说明](docs/images/qrc-add-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/qrc-add-page-2.png) | ![输入图片说明](docs/images/qrc-add-page-view.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/mch-sys-config-pay-auth.png) | ![输入图片说明](docs/images/payways-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/ifdefines-page.png) | ![输入图片说明](docs/images/ifdefines-page-edit.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/pay-order-page.png) | ![输入图片说明](docs/images/pay-order-view-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/mch-statistic-page.png) | ![输入图片说明](docs/images/mch-statistic-way-page.png)   |
|-----------------------------------|---|

| ![输入图片说明](docs/images/isv-pay-config-page.png) | ![输入图片说明](docs/images/isv-pay-config-2-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/isv-pay-rate-config-page.png) | ![输入图片说明](docs/images/app-pay-config-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/mch-notify-page.png) | ![输入图片说明](docs/images/log-view-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/mch-login-page.png) | ![输入图片说明](docs/images/quick-cashier-page.png)  |
|-----------------------------------|---|

| ![输入图片说明](docs/images/wxpay-page-view.png) | ![输入图片说明](docs/images/wxpay-page-view-remark.png) | ![输入图片说明](docs/images/alipay-page-view.png) | ![输入图片说明](docs/images/ysfpay-page-view.png) |
| ------------ | ------------ | ------------ | ------------ |

### 项目结构
```
agpayplus/
├── aspnet-core/
│   ├── docs/
│   │   ├── sql/
│   │   │   └── agpayplusinit.sql
│   │   └── rabbitmq_plugin/
│   │       └── rabbitmq_delayed_message_exchange-3.13.0.ez
│   ├── src/
│   │   ├── AGooday.AgPay.Manager.Api/ (端口：9817)
│   │   │   ├── wwwroot/
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── AGooday.AgPay.Manager.Api.csproj
│   │   │   ├── log4net.config
│   │   │   ├── Program.cs
│   │   │   ├── Dockerfile
│   │   │   └── ...
│   │   ├── AGooday.AgPay.Agent.Api/ (端口：9816)
│   │   │   ├── wwwroot/
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── AGooday.AgPay.Agent.Api.csproj
│   │   │   ├── log4net.config
│   │   │   ├── Program.cs
│   │   │   ├── Dockerfile
│   │   │   └── ...
│   │   ├── AGooday.AgPay.Merchant.Api/ (端口：9818)
│   │   │   ├── wwwroot/
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── AGooday.AgPay.Merchant.Api.csproj
│   │   │   ├── log4net.config
│   │   │   ├── Program.cs
│   │   │   ├── Dockerfile
│   │   │   └── ...
│   │   └── AGooday.AgPay.Payment.Api/ (端口：9819)
│   │   │   ├── wwwroot/
│   │   │   │   └── cashier/
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── AGooday.AgPay.Payment.Api.csproj
│   │   │   ├── log4net.config
│   │   │   ├── Program.cs
│   │   │   ├── Dockerfile
│   │   │   └── ...
│   │   └── ...
│   ├── test/
│   ├── README.md
│   └── AGooday.AgPay.sln
├── ant-design-vue/
│   ├── agpay-ui-manager/ (vue-app 端口：8817)
│   │   ├── node_modules/
│   │   ├── public/
│   │   ├── src/
│   │   ├── .env
│   │   ├── .env.development
│   │   ├── package.json
│   │   ├── vue.config.js
│   │   ├── nginx.conf
│   │   └── Dockerfile
│   ├── agpay-ui-agent/ (vue-app 端口：8816)
│   │   ├── node_modules/
│   │   ├── public/
│   │   ├── src/
│   │   ├── .env
│   │   ├── .env.development
│   │   ├── package.json
│   │   ├── vue.config.js
│   │   ├── nginx.conf
│   │   └── Dockerfile
│   ├── agpay-ui-merchant/ (vue-app 端口：8818)
│   │   ├── node_modules/
│   │   ├── public/
│   │   ├── src/
│   │   ├── .env
│   │   ├── .env.development
│   │   ├── package.json
│   │   ├── vue.config.js
│   │   ├── nginx.conf
│   │   └── Dockerfile
│   ├── agpay-ui-cashier/ (vue-app 端口：8819)
│   │   ├── node_modules/
│   │   ├── public/
│   │   ├── src/
│   │   ├── .env
│   │   ├── .env.development
│   │   ├── package.json
│   │   └── vue.config.js
│   └── README.md
├── docs/
├── .gitignore
├── README.md
├── LICENSE
├── .env.example
├── .env.development
├── .env.staging
├── .env.production
├── docker-compose-app.yml
└── docker-compose.yml
```

### 环境准备
云服务器推荐购买阿里云（腾讯云 或 华为云）的主机，建议不低于以下配置：

| 操作系统 | CPU | 内存 | 带宽	 | 其他 |
| ------------ | ------------ | ------------ | ------------ | ------------ |
| Linux CentOS 7.X （或以上） | 4核	 | 8G | 2M+（或弹性） | 开发测试环境配置再低些也可以部署 |

### 初始化数据库
初始化数据库，在msyql 8.0.36下创建数据库agpayplusdb，用户agpay，密码123456（数据库和账号密码可自己设定，密码不要过于简单）。

执行项目下 aspnet-core/docs/sql/agpayplusinit.sql ，确保所有语句执行成功。

### 初始账号
系统部署后初始账号密码。

运营平台

账号：agpayadmin 密码：agpay123

代理商系统和商户系统，默认密码：agpay666

---

## 🚀 系统部署

### 📋 快速开始

AgPay+ 提供了完整的 **Docker Compose** 部署方案，支持 **一键部署**。

#### 方式 1：自动化部署脚本（推荐）

**Windows:**
```powershell
# 完整部署（包含环境检查、证书生成、服务启动）
.\deploy.ps1

# 更新服务
.\update.ps1

# 回滚到之前版本
.\rollback.ps1

# 生成 SSL 证书
.\generate-cert-windows.ps1
```

**Linux/macOS:**
```bash
# 添加执行权限
chmod +x deploy.sh update.sh rollback.sh generate-cert-linux.sh

# 完整部署
./deploy.sh

# 更新服务
./update.sh

# 回滚到之前版本
./rollback.sh

# 生成 SSL 证书
./generate-cert-linux.sh
```

#### 方式 2：Docker Compose 命令

```bash
# 1. 选择环境配置
cp .env.development .env    # 开发环境
cp .env.staging .env        # 预发布环境
cp .env.production .env     # 生产环境

# 2. 编辑配置文件
vim .env

# 3. 部署所有服务
docker compose up -d

# 4. 查看状态
docker compose ps

# 5. 查看日志
docker compose logs -f
```

### 📚 详细文档

下列文档为本项目的详细说明与运维手册，建议按照顺序阅读：

| 文档 | 说明 |
|------|------|
| **[DEPLOYMENT_USAGE_GUIDE.md](DEPLOYMENT_USAGE_GUIDE.md)** | 📘 完整部署与更新指南（首选阅读，包含首次部署/更新/回滚/CI/CD） |
| **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** | 📖 常用命令速查：一键命令、示例和操作速查表（运维速查表） |
| **[README_DOCKER.md](README_DOCKER.md)** | 🚀 Docker 快速上手：5 分钟部署示例和常见问题 |
| **[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)** | ✅ 上线前检查清单（生产环境准备、配置与校验） |
| **[DATABASE_SETUP.md](DATABASE_SETUP.md)** | 🗄️ 数据库搭建与初始化（MySQL 配置、初始化脚本、远程访问） |
| **[MYSQL_MIGRATION.md](MYSQL_MIGRATION.md)** | 🔄 MySQL 数据迁移指南与卷到宿主机迁移说明 |
| **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** | 🔧 故障排查指南（日志、常见问题及解决步骤） |
| **[DOCKER_MIRROR_GUIDE.md](DOCKER_MIRROR_GUIDE.md)** | 🌐 镜像源与加速配置，提升拉取速度的建议 |
| **[DOCKER_COMPOSE_DYNAMIC_CONFIG.md](DOCKER_COMPOSE_DYNAMIC_CONFIG.md)** | ⚙️ 动态配置与多环境并行运行说明（CI/CD、标签管理） |
| **[CASHIER_DEPLOYMENT.md](CASHIER_DEPLOYMENT.md)** | 💳 收银台（Cashier）部署与集成说明 |
| **[FEATURES_UPDATE.md](FEATURES_UPDATE.md)** | ✨ 功能变更与更新记录（添加/变更功能摘要） |
| **[IMPLEMENTATION_REPORT.md](IMPLEMENTATION_REPORT.md)** | 📝 实现说明与设计记录（供开发/审计参考） |
| **[README_DEPLOYMENT.md](README_DEPLOYMENT.md)** | 📚 部署脚本说明与示例（轻量参考） |

额外模块与子项目说明：

| 文档 | 说明 |
|------|------|
| **[aspnet-core/README.md](aspnet-core/README.md)** | 后端项目总览与代码结构 |
| **[ant-design-vue/README.md](ant-design-vue/README.md)** | 前端项目总览（Vue 项目结构与运行） |
| **[ant-design-vue/agpay-ui-cashier/README.md](ant-design-vue/agpay-ui-cashier/README.md)** | Cashier 前端工程说明 |

如需导出或打印完整清单，请参阅 `DEPLOYMENT_USAGE_GUIDE.md` 的“文档索引”章节。若要我把 README 中的这些链接按另一种顺序或增加简短概要说明，请告知。

### 🎯 部署架构

AgPay+ 采用微服务架构，基于 Docker Compose 进行容器编排：

#### 服务组成

**前端服务（3个）**
- **ui-manager** (8817) - 运营平台前端
- **ui-agent** (8816) - 代理商系统前端
- **ui-merchant** (8818) - 商户系统前端

**后端服务（4个）**
- **manager-api** (9817/5817) - 运营平台 API
- **agent-api** (9816/5816) - 代理商 API
- **merchant-api** (9818/5818) - 商户 API
- **payment-api** (9819/5819) - 支付网关 API + 收银台

**基础设施（2个）**
- **redis** (6379) - 缓存服务
- **rabbitmq** (5672/15672) - 消息队列

**数据库**
- **MySQL 8.0+** - 使用宿主机 MySQL（推荐生产环境）

#### 架构图

```
宿主机/服务器
│
├─ Docker Compose 容器编排
│  │
│  ├─ 前端服务 (Nginx)
│  │  ├─ ui-manager    :8817
│  │  ├─ ui-agent      :8816
│  │  └─ ui-merchant   :8818
│  │
│  ├─ 后端服务 (.NET 9)
│  │  ├─ manager-api   :9817
│  │  ├─ agent-api     :9816
│  │  ├─ merchant-api  :9818
│  │  └─ payment-api   :9819 (含收银台)
│  │
│  └─ 基础设施
│     ├─ redis         :6379
│     └─ rabbitmq      :5672/15672
│
└─ 宿主机 MySQL 8.0+ (推荐生产环境)
```

#### 服务通信

| 通信类型 | 说明 |
|----------|------|
| **前端 → 后端** | HTTPS 调用后端 API |
| **后端 → 数据库** | TCP 连接 MySQL |
| **后端 → Redis** | 缓存读写 |
| **后端 → RabbitMQ** | 消息队列通信 |
| **服务间** | Docker 内部网络通信 |

### ⚙️ 环境配置

#### 1. 环境变量配置

从环境模板创建配置文件：

```bash
# 开发环境
cp .env.development .env

# 预发布环境
cp .env.staging .env

# 生产环境
cp .env.production .env
```

编辑 `.env` 文件：

```env
# 基础配置
COMPOSE_PROJECT_NAME=agpayplus        # 项目名称
IPORDOMAIN=localhost                  # 生产环境改为实际域名或IP
IMAGE_PREFIX=agpay                    # 镜像前缀
IMAGE_TAG=latest                      # 镜像标签

# MySQL 配置
MYSQL_SERVER_NAME=host.docker.internal  # Windows/macOS
# MYSQL_SERVER_NAME=172.17.0.1          # Linux
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_password            # 修改为实际密码

# 数据路径
DATA_PATH_HOST=E:/app/agpayplus         # Windows
# DATA_PATH_HOST=/opt/agpayplus         # Linux

# 证书路径
CERT_PATH=${USERPROFILE}/.aspnet/https  # Windows
# CERT_PATH=~/.aspnet/https             # Linux
CERT_PASSWORD=123456
```

#### 2. 数据库准备

```sql
-- 创建数据库
CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 导入初始化脚本
mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql
```

详见：[DATABASE_SETUP.md](DATABASE_SETUP.md)

#### 3. SSL 证书生成

```bash
# Windows
.\generate-cert-windows.ps1

# Linux/macOS
./generate-cert-linux.sh

# 或手动生成
dotnet dev-certs https -ep ~/.aspnet/https/agpayplusapi.pfx -p 123456
dotnet dev-certs https --trust
```

### 🌐 服务访问

部署成功后，访问以下地址：

| 服务 | 地址 | 默认账号 |
|------|------|----------|
| **运营平台** | https://localhost:8817 | agpayadmin / agpay123 |
| **代理商系统** | https://localhost:8816 | - / agpay666 |
| **商户系统** | https://localhost:8818 | - / agpay666 |
| **支付网关 API** | https://localhost:9819 | - |
| **收银台** | https://localhost:9819/cashier | - |
| **RabbitMQ 管理** | http://localhost:15672 | admin / admin |

### 🔧 常用命令

```bash
# 启动所有服务
docker compose up -d

# 停止所有服务
docker compose stop

# 重启服务
docker compose restart

# 查看状态
docker compose ps

# 查看日志
docker compose logs -f [service_name]

# 更新服务
docker compose pull
docker compose up -d --build

# 清理
docker compose down
docker compose down -v  # 同时删除数据卷
```

### 📦 更新服务

#### 使用更新脚本

```bash
# 更新所有应用服务
./update.ps1        # Windows
./update.sh         # Linux/macOS

# 更新指定服务
./update.ps1 -Services "payment-api"                      # Windows
./update.sh --services "payment-api,manager-api"          # Linux/macOS

# 强制重新构建
./update.ps1 -Services "payment-api" -Force               # Windows
./update.sh --services "payment-api" --force              # Linux/macOS
```

#### 手动更新

```bash
# 1. 拉取最新代码
git pull

# 2. 重新构建
docker compose build [service_name]

# 3. 重启服务
docker compose up -d [service_name]
```

### 🔄 版本回滚

#### 使用回滚脚本

```bash
# 查看可用备份
./rollback.ps1 -List        # Windows
./rollback.sh --list        # Linux/macOS

# 回滚到最新备份
./rollback.ps1              # Windows
./rollback.sh               # Linux/macOS

# 回滚到指定备份
./rollback.ps1 -Backup "20240315_143022"        # Windows
./rollback.sh --backup "20240315_143022"        # Linux/macOS
```

### 🐛 故障排查

#### 查看日志

```bash
# 查看所有服务日志
docker compose logs -f

# 查看特定服务日志
docker compose logs -f payment-api

# 查看最近 100 行日志
docker compose logs --tail=100 payment-api
```

#### 进入容器

```bash
# 进入容器
docker exec -it agpayplus-payment-api-1 /bin/bash

# 检查文件
ls -la /app
ls -la /app/wwwroot/cashier
```

#### 重新部署

```bash
# 停止并删除所有容器
docker compose down

# 清理并重新构建
docker compose build --no-cache
docker compose up -d
```

详见：[TROUBLESHOOTING.md](TROUBLESHOOTING.md)

---

## 📖 传统部署方式

以下是传统的手动 Docker 部署方式（不推荐，建议使用上面的 Docker Compose 方案）：

<details>
<summary>点击展开查看传统部署命令</summary>

### Docker
```
# 创建网络
docker network create agpay-plus-network

# Docker安装Redis
# Docker搜索redis镜像 命令：docker search <镜像名称>
docker search redis

# Docker拉取镜像 命令：docker pull <镜像名称>:<版本号>
docker pull redis

# 运行 redis 容器
docker run -d --name agpay-plus-redis -p 6379:6379 --network agpay-plus-network redis

# Docker安装RabbitMQ
# 拉去镜像
docker pull rabbitmq:management

# 运行 rabbitmq 容器
docker run -d --hostname agpay-plus-rabbitmq --name agpay-plus-rabbitmq --network agpay-plus-network -p 15672:15672 -p 5672:5672 rabbitmq:3-management

# 根据 RabbitMQ 版本下载「rabbitmq_delayed_message_exchange」插件 https://www.rabbitmq.com/community-plugins.html
# 将刚下载的插件拷贝到容器内的 plugins 目录下
# Window
docker cp E:\agoodays\agpayplus\aspnet-core\docs\rabbitmq_plugin\rabbitmq_delayed_message_exchange-3.13.0.ez agpay-plus-rabbitmq:/plugins
# Linux
docker cp /root/agpayplus/aspnet-core/docs/rabbitmq_plugin/rabbitmq_delayed_message_exchange-3.13.0.ez agpay-plus-rabbitmq:/plugins/

# 进入rabbitmq命令
docker exec -it agpay-plus-rabbitmq /bin/bash

# 查看插件列表
root@agpay-plus-rabbitmq:/# rabbitmq-plugins list

# 查看插件是否存在
root@agpay-plus-rabbitmq:/# cd plugins
root@agpay-plus-rabbitmq:/plugins# ls |grep delay
root@agpay-plus-rabbitmq:/plugins# cd ../
# Enable 插件
root@agpay-plus-rabbitmq:/# rabbitmq-plugins enable rabbitmq_delayed_message_exchange

# 退出容器
root@agpay-plus-rabbitmq:/# exit

# 重启 RabbitMQ
docker restart agpay-plus-rabbitmq

# 手动拉取镜像
docker pull mcr.microsoft.com/dotnet/sdk:9.0
docker pull mcr.microsoft.com/dotnet/aspnet:9.0

# 构建并运行后端容器
# 构建 Docker 镜像
cd agpayplus/aspnet-core/src
agpayplus\aspnet-core\src> docker build --no-cache -t agpay-plus-manager-api -f ./AGooday.AgPay.Manager.Api/Dockerfile .
agpayplus\aspnet-core\src> docker build --no-cache -t agpay-plus-agent-api -f ./AGooday.AgPay.Agent.Api/Dockerfile .
agpayplus\aspnet-core\src> docker build --no-cache -t agpay-plus-merchant-api -f ./AGooday.AgPay.Merchant.Api/Dockerfile .
agpayplus\aspnet-core\src> docker build --no-cache -t agpay-plus-payment-api -f ./AGooday.AgPay.Payment.Api/Dockerfile .

# 将运行的容器连接到指定的网络，运行 docker network inspect agpay-plus-network 命令查看容器是否连接到了该网络
docker network connect agpay-plus-network agpay-plus-manager-api

# 生成证书并配置本地计算机
# https://learn.microsoft.com/zh-cn/aspnet/core/security/docker-https?view=aspnetcore-9.0
# https://www.linkedin.com/pulse/run-aspnet-core-api-docker-https-senthil-kumaran
# 创建目标目录
mkdir $env:USERPROFILE\.aspnet\https
# 生成并导出证书
# dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\agpayplusapi.pfx -p 123456
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\agpayplusapi.pfx -p 123456
# 验证证书是否已正确导出
ls $env:USERPROFILE\.aspnet\https
# 在 Linux 或 macOS 上，替换 $env:USERPROFILE（%USERPROFILE%） 为 ~ 并确保你有适当的权限：
dotnet dev-certs https -ep ~/.aspnet/https/agpayplusapi.pfx -p 123456
# 信任生成的证书
dotnet dev-certs https --trust

# 运行容器
# docker run -d --name agpay-plus-manager-api -p 9817:9817 --network agpay-plus-network agpay-plus-manager-api
# 使用为 HTTPS 配置的 ASP.NET Core 运行容器镜像
# docker run --rm -it -d --name agpay-plus-manager-api --network agpay-plus-network -p 5817:5817 -p 9817:9817 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORTS=9817 -e ASPNETCORE_Kestrel__Certificates__Default__Password="123456" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/agpayplusapi.pfx -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-manager-api
# docker run --rm -it -d --name agpay-plus-agent-api --network agpay-plus-network -p 5816:5816 -p 9816:9816 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORTS=9816 -e ASPNETCORE_Kestrel__Certificates__Default__Password="123456" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/agpayplusapi.pfx -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-agent-api
# docker run --rm -it -d --name agpay-plus-merchant-api --network agpay-plus-network -p 5818:5818 -p 9818:9818 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORTS=9818 -e ASPNETCORE_Kestrel__Certificates__Default__Password="123456" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/agpayplusapi.pfx -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-merchant-api
# docker run --rm -it -d --name agpay-plus-payment-api --network agpay-plus-network -p 5819:5819 -p 9819:9819 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORTS=9819 -e ASPNETCORE_Kestrel__Certificates__Default__Password="123456" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/agpayplusapi.pfx -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-payment-api
# Window
docker run -d --name agpay-plus-manager-api -v /e/app/agpayplus/logs:/app/agpayplus/logs -v /e/app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5817:5817 -p 9817:9817 -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-manager-api
docker run -d --name agpay-plus-agent-api -v /e/app/agpayplus/logs:/app/agpayplus/logs -v /e/app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5816:5816 -p 9816:9816 -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-agent-api
docker run -d --name agpay-plus-merchant-api -v /e/app/agpayplus/logs:/app/agpayplus/logs -v /e/app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5818:5818 -p 9818:9818 -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-merchant-api
docker run -d --name agpay-plus-payment-api -v /e/app/agpayplus/logs:/app/agpayplus/logs -v /e/app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5819:5819 -p 9819:9819 -v $env:USERPROFILE\.aspnet\https:/https/ agpay-plus-payment-api
# Linux
docker run -d --name agpay-plus-manager-api -v /app/agpayplus/logs:/app/agpayplus/logs -v /app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5817:5817 -p 9817:9817 -v ${HOME}/.aspnet/https:/https/ agpay-plus-manager-api
docker run -d --name agpay-plus-agent-api -v /app/agpayplus/logs:/app/agpayplus/logs -v /app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5816:5816 -p 9816:9816 -v ${HOME}/.aspnet/https:/https/ agpay-plus-agent-api
docker run -d --name agpay-plus-merchant-api -v /app/agpayplus/logs:/app/agpayplus/logs -v /app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5818:5818 -p 9818:9818 -v ${HOME}/.aspnet/https:/https/ agpay-plus-merchant-api
docker run -d --name agpay-plus-payment-api -v /app/agpayplus/logs:/app/agpayplus/logs -v /app/agpayplus/upload:/app/agpayplus/upload --network agpay-plus-network -p 5819:5819 -p 9819:9819 -v ${HOME}/.aspnet/https:/https/ agpay-plus-payment-api

# 停止并删除当前正在运行的 agpay-plus-manager-api 容器：
docker stop agpay-plus-manager-api
docker rm agpay-plus-manager-api

# 构建并运行前端容器
# 直接拉取所需的基础镜像
docker pull node:16-alpine
docker pull nginx:stable-alpine

# 构建 Docker 镜像
cd agpayplus/ant-design-vue
agpayplus\ant-design-vue> docker build --no-cache -t agpay-ui-manager -f ./agpay-ui-manager/Dockerfile .
agpayplus\ant-design-vue> docker build --no-cache -t agpay-ui-agent -f ./agpay-ui-agent/Dockerfile .
agpayplus\ant-design-vue> docker build --no-cache -t agpay-ui-merchant -f ./agpay-ui-merchant/Dockerfile .

# 运行容器
docker run -d --name agpay-ui-manager -p 8817:80 --network agpay-plus-network agpay-ui-manager
docker run -d --name agpay-ui-agent -p 8816:80 --network agpay-plus-network agpay-ui-agent
docker run -d --name agpay-ui-merchant -p 8818:80 --network agpay-plus-network agpay-ui-merchant
```

### Docker Compose

**推荐使用上面的自动化部署脚本，以下为手动命令参考：**

```bash
# 构建并启动服务
agpayplus> docker-compose -f docker-compose-app.yml --env-file .env.app up
# 使用应用服务配置文件部署 -d 参数可以在后台运行服务
agpayplus> docker-compose -f docker-compose-app.yml --env-file .env.app up -d

# 检查容器状态
agpayplus> docker-compose -f docker-compose-app.yml --env-file .env.app ps

# 重新构建并启动服务
agpayplus> docker-compose -f docker-compose-app.yml --env-file .env.app build
agpayplus> docker-compose -f docker-compose-app.yml --env-file .env.app up -d
```

</details>

---

## 💡 生产环境建议

### 硬件配置

| 组件 | 最低配置 | 推荐配置 |
|------|----------|----------|
| **CPU** | 4核 | 8核+ |
| **内存** | 8GB | 16GB+ |
| **硬盘** | 100GB SSD | 500GB SSD |
| **带宽** | 2Mbps | 10Mbps+ |

### 安全建议

1. ✅ 修改所有默认密码
2. ✅ 配置防火墙规则
3. ✅ 使用正式 SSL 证书（Let's Encrypt）
4. ✅ 启用日志审计
5. ✅ 定期备份数据库
6. ✅ 配置监控告警

### 性能优化

1. ✅ 使用 Redis 集群
2. ✅ 配置 MySQL 主从复制
3. ✅ 启用 CDN 加速
4. ✅ 优化数据库索引
5. ✅ 配置负载均衡

详见：[DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md)

---

## 🔗 相关链接

- 📖 [快速参考手册](QUICK_REFERENCE.md) - 常用命令速查
- 📘 [完整使用指南](DEPLOYMENT_USAGE_GUIDE.md) - 详细的部署和更新说明
- 📖 [Docker 快速指南](README_DOCKER.md) - Docker 快速上手
- 📊 [数据库搭建](DATABASE_SETUP.md) - MySQL 环境配置
- ✅ [部署检查清单](DEPLOYMENT_CHECKLIST.md) - 上线前检查
- 💳 [收银台部署](CASHIER_DEPLOYMENT.md) - 收银台集成
- 🔧 [故障排查指南](TROUBLESHOOTING.md) - 常见问题解决
- 📝 [接口文档](https://www.yuque.com/xiangyisheng/agooday/cweewhugp7h7hvml) - API 接口说明

---

**部署脚本说明：**
- `deploy.sh` / `deploy.ps1` - 首次部署脚本（包含环境检查、证书生成、服务启动）
- `update.sh` / `update.ps1` - 服务更新脚本（支持选择性更新服务）
- `rollback.sh` / `rollback.ps1` - 版本回滚脚本（支持回滚到历史版本）
- `generate-cert-linux.sh` / `generate-cert-windows.ps1` - SSL 证书生成脚本

**环境配置文件：**
- `.env.development` - 开发环境配置模板
- `.env.staging` - 预发布环境配置模板
- `.env.production` - 生产环境配置模板
- `.env` - 当前使用的配置（从模板复制并修改）