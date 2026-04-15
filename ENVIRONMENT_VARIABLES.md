# AgPay+ 环境变量说明

本文档详细说明 AgPay+ 系统的所有环境变量配置选项，帮助您根据实际环境进行正确配置。

## 📋 目录

1. [Docker Compose 配置](#docker-compose-配置)
2. [服务器配置](#服务器配置)
3. [MySQL 数据库配置](#mysql-数据库配置)
4. [SSL 证书配置](#ssl-证书配置)
5. [消息队列配置](#消息队列配置)
6. [日志配置](#日志配置)
7. [数据持久化路径](#数据持久化路径)
8. [Redis 配置](#redis-配置)
9. [健康检查配置](#健康检查配置)
10. [备份配置](#备份配置)
11. [其他配置](#其他配置)
12. [配置示例](#配置示例)

## Docker Compose 配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `COMPOSE_PROJECT_NAME` | `agpayplus` | Docker Compose 项目名称，用于区分不同环境 | `agpayplus-dev` (开发环境) |
| `IMAGE_PREFIX` | `agpay` | 镜像前缀，用于构建镜像名称 | `agpay-dev` |
| `IMAGE_TAG` | `latest` | 镜像标签，用于指定镜像版本 | `v1.0.0` 或 `dev-20240315` |

## 服务器配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `IPORDOMAIN` | `localhost` | 服务器 IP 或域名，用于生成访问地址和证书 | `192.168.1.100` 或 `agpay.example.com` |

## MySQL 数据库配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `MYSQL_SERVER_NAME` | `host.docker.internal` | MySQL 服务器地址 | Windows: `host.docker.internal`<br>Linux: `172.17.0.1` |
| `MYSQL_PORT` | `3306` | MySQL 端口 | `3306` |
| `MYSQL_USER` | `root` | MySQL 用户名 | `root` |
| `MYSQL_PASSWORD` | `123456` | MySQL 密码 | 安全的密码 |
| `MYSQL_DATABASE` | `agpayplusdb` | 数据库名称 | `agpayplusdb` |

## SSL 证书配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `CERT_PASSWORD` | `123456` | 证书密码 | 安全的密码 |
| `CERT_PATH` | `~/.aspnet/https` | 证书存储路径 | Linux: `~/.aspnet/https`<br>Windows: `%USERPROFILE%\.aspnet\https` |
| `CERT_PATH_IN_CONTAINER` | `/https/agpayplusapi.pfx` | 容器内证书路径 | `/https/agpayplusapi.pfx` |

## 消息队列配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `MQ_VENDER` | `RabbitMQ` | 消息队列供应商 | `RabbitMQ` |
| `MQ_HOSTNAME` | `rabbitmq` | 消息队列主机名 | `rabbitmq` |
| `MQ_USERNAME` | `admin` | 消息队列用户名 | `admin` |
| `MQ_PASSWORD` | `admin` | 消息队列密码 | 安全的密码 |
| `MQ_PORT` | `5672` | 消息队列端口 | `5672` |

## 日志配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `SEQ_URL` | `http://seq:80` | Seq 日志服务器地址 | `http://seq:80` |
| `ENABLE_SEQ` | `true` | 是否启用 Seq 日志 | `true` 或 `false` |
| `SEQ_API_KEY` | `` | Seq API 密钥（可选） | `your-api-key` |

## 数据持久化路径

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `DATA_PATH_HOST` | `/opt/agpayplus` | 数据持久化路径 | Linux: `/opt/agpayplus`<br>Windows: `E:\app\agpayplus` |

## Redis 配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `REDIS_HOST` | `redis` | Redis 主机名 | `redis` |
| `REDIS_PORT` | `6379` | Redis 端口 | `6379` |
| `REDIS_PASSWORD` | `` | Redis 密码（可选） | 安全的密码 |
| `REDIS_DB` | `0` | Redis 数据库编号 | `0` |

## 健康检查配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `HEALTH_CHECK_ENABLED` | `true` | 是否启用健康检查 | `true` 或 `false` |
| `HEALTH_CHECK_INTERVAL` | `30s` | 健康检查间隔 | `30s` |
| `HEALTH_CHECK_TIMEOUT` | `10s` | 健康检查超时时间 | `10s` |
| `HEALTH_CHECK_RETRIES` | `3` | 健康检查重试次数 | `3` |

## 备份配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `BACKUP_ENABLED` | `true` | 是否启用备份 | `true` 或 `false` |
| `BACKUP_RETENTION` | `5` | 备份保留数量 | `5` |
| `BACKUP_PATH` | `/var/agpayplus/backup` | 备份存储路径 | `/var/agpayplus/backup` |

## 其他配置

| 变量名 | 默认值 | 说明 | 示例 |
|-------|-------|------|------|
| `BUILD_CASHIER` | `false` | 是否构建 Cashier | `true` 或 `false` |

## 配置示例

### 开发环境配置

```env
# Docker Compose 配置
COMPOSE_PROJECT_NAME=agpayplus-dev
IMAGE_PREFIX=agpay-dev
IMAGE_TAG=dev

# 服务器配置
IPORDOMAIN=localhost

# MySQL 配置
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_USER=root
MYSQL_PASSWORD=123456
MYSQL_DATABASE=agpayplusdb

# 数据路径
DATA_PATH_HOST=E:\app\agpayplus-dev

# 其他配置保持默认
```

### 生产环境配置

```env
# Docker Compose 配置
COMPOSE_PROJECT_NAME=agpayplus
IMAGE_PREFIX=agpay
IMAGE_TAG=latest

# 服务器配置
IPORDOMAIN=agpay.example.com

# MySQL 配置
MYSQL_SERVER_NAME=192.168.1.100
MYSQL_PORT=3306
MYSQL_USER=agpayuser
MYSQL_PASSWORD=your_secure_password
MYSQL_DATABASE=agpayplusdb

# 数据路径
DATA_PATH_HOST=/opt/agpayplus

# 备份配置
BACKUP_PATH=/opt/agpayplus/backup
```

## 配置说明

1. **复制配置文件**：将 `.env.example` 复制为 `.env` 并根据实际环境修改
2. **敏感信息**：不要将包含敏感信息的 `.env` 文件提交到版本控制系统
3. **路径格式**：Windows 使用反斜杠 `\`，Linux/macOS 使用正斜杠 `/`
4. **环境区分**：使用不同的 `COMPOSE_PROJECT_NAME` 区分不同环境
5. **MySQL 连接**：
   - Windows: 使用 `host.docker.internal`
   - Linux: 使用 Docker 网桥 IP (`172.17.0.1`) 或宿主机 IP

## 验证配置

部署前可以使用以下命令验证配置：

```bash
# Windows
.eploy.ps1 -ValidateConfig

# Linux/macOS
./deploy.sh --validate-config
```

## 常见问题

### Q: 数据库连接失败怎么办？
A: 检查 `MYSQL_SERVER_NAME` 是否正确，确保 MySQL 服务正在运行，并且允许远程连接。

### Q: 证书生成失败怎么办？
A: 确保 `.NET SDK` 已安装，并且 `CERT_PATH` 目录存在且有写入权限。

### Q: 数据持久化路径如何选择？
A: 选择有足够空间的磁盘分区，确保有读写权限，建议使用独立的挂载点。

### Q: 健康检查失败怎么办？
A: 检查服务日志，确保服务正常启动，网络连接正常。