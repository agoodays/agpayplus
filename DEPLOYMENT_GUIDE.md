# 🚀 AgPay+ 部署实战指南

本文档提供**超级详细**的部署和更新步骤，包含每一步的输出示例和当前工作目录说明。

---

## 📚 目录

- [首次部署完整流程](#首次部署完整流程)
  - [Windows 部署](#windows-部署)
  - [Linux/macOS 部署](#linuxmacos-部署)
- [服务更新流程](#服务更新流程)
- [常见问题排查](#常见问题排查)
- [验证部署结果](#验证部署结果)

---

## 首次部署完整流程

### Windows 部署

#### 步骤 1：准备环境

**当前目录**：任意目录

```powershell
# 检查 Docker 是否安装
PS C:\> docker --version
# 输出示例：
# Docker version 24.0.6, build ed223bc

PS C:\> docker compose version
# 输出示例：
# Docker Compose version v2.23.0

# 检查 Git 是否安装
PS C:\> git --version
# 输出示例：
# git version 2.42.0.windows.1

# 检查 .NET SDK（用于生成证书）
PS C:\> dotnet --version
# 输出示例：
# 8.0.100
```

如果任何命令失败，请先安装对应的软件：
- Docker Desktop: https://www.docker.com/products/docker-desktop
- Git: https://git-scm.com/download/win
- .NET SDK: https://dotnet.microsoft.com/download

---

#### 步骤 2：克隆项目

**当前目录**：您的工作目录（例如 `E:\projects`）

```powershell
# 进入您的工作目录
PS C:\> cd E:\agoodays

# 克隆项目（如果还没克隆）
PS E:\agoodays> git clone https://github.com/agoodays/agpayplus.git
# 或使用 Gitee
PS E:\agoodays> git clone https://gitee.com/agoodays/agpayplus.git

# 输出示例：
# Cloning into 'agpayplus'...
# remote: Enumerating objects: 1234, done.
# remote: Counting objects: 100% (1234/1234), done.
# remote: Compressing objects: 100% (567/567), done.
# remote: Total 1234 (delta 890), reused 1100 (delta 800)
# Receiving objects: 100% (1234/1234), 15.20 MiB | 2.50 MiB/s, done.
# Resolving deltas: 100% (890/890), done.

# 进入项目目录
PS E:\agoodays> cd agpayplus
PS E:\agoodays\agpayplus>
```

---

#### 步骤 3：配置环境变量

**当前目录**：`E:\agoodays\agpayplus`

```powershell
PS E:\agoodays\agpayplus> ls .env*
# 输出示例：
#     目录: E:\agoodays\agpayplus
# Mode                 LastWriteTime         Length Name
# ----                 -------------         ------ ----
# -a----         2024/1/1     10:00            520 .env.linux
# -a----         2024/1/1     10:00            530 .env.windows

# 复制环境变量模板
PS E:\agoodays\agpayplus> Copy-Item .env.windows .env
# （没有输出表示成功）

# 编辑配置文件
PS E:\agoodays\agpayplus> notepad .env
# 或使用 VS Code
PS E:\agoodays\agpayplus> code .env
```

**编辑 `.env` 文件**，修改以下配置：

```env
# 基础配置
IPORDOMAIN=localhost  # 生产环境改为实际 IP 或域名

# MySQL 配置（使用宿主机 MySQL）
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # （请修改为实际密码）

# 数据路径
DATA_PATH_HOST=E:/app/agpayplus  # （请修改为实际路径）

# 证书路径
CERT_PATH=${USERPROFILE}/.aspnet/https
```

**保存并关闭文件**。

---

#### 步骤 4：准备 MySQL 数据库

**当前目录**：`E:\agoodays\agpayplus`

##### 4.1 连接 MySQL

```powershell
PS E:\agoodays\agpayplus> mysql -u root -p
# 输出示例：
# Enter password: ********
# Welcome to the MySQL monitor.  Commands end with ; or \g.
# Your MySQL connection id is 8
# Server version: 8.0.36 MySQL Community Server - GPL
#
# Type 'help;' or '\h' for help. Type '\c' to clear the current input statement.
#
# mysql>
```

##### 4.2 创建数据库

```sql
mysql> CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
-- 输出示例：
-- Query OK, 1 row affected (0.01 sec)

mysql> SHOW DATABASES LIKE 'agpayplusdb';
-- 输出示例：
-- +------------------------+
-- | Database (agpayplusdb) |
-- +------------------------+
-- | agpayplusdb            |
-- +------------------------+
-- 1 row in set (0.00 sec)

mysql> EXIT;
-- 输出示例：
-- Bye
```

##### 4.3 导入初始化脚本

```powershell
PS E:\agoodays\agpayplus> mysql -u root -p agpayplusdb < aspnet-core\docs\sql\agpayplusinit.sql
# 输出示例：
# Enter password: ********
# （导入过程可能需要 10-30 秒，成功后没有输出）

# 验证导入结果
PS E:\agoodays\agpayplus> mysql -u root -p agpayplusdb -e "SHOW TABLES;"
# 输出示例：
# Enter password: ********
# +-------------------------+
# | Tables_in_agpayplusdb   |
# +-------------------------+
# | t_sys_config            |
# | t_sys_user              |
# | t_mch_info              |
# | t_pay_order             |
# ...
# +-------------------------+
# 24 rows in set (0.01 sec)
```

##### 4.4 配置 MySQL 远程访问（Docker 容器访问）

```powershell
PS E:\agoodays\agpayplus> mysql -u root -p
# 输入密码后进入 MySQL
```

```sql
mysql> CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
-- 输出示例：
-- Query OK, 0 rows affected (0.01 sec)

mysql> GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
-- 输出示例：
-- Query OK, 0 rows affected (0.00 sec)

mysql> FLUSH PRIVILEGES;
-- 输出示例：
-- Query OK, 0 rows affected (0.01 sec)

mysql> SELECT host, user FROM mysql.user WHERE user='root';
-- 输出示例：
-- +-----------+------+
-- | host      | user |
-- +-----------+------+
-- | %         | root |
-- | localhost | root |
-- +-----------+------+
-- 2 rows in set (0.00 sec)

mysql> EXIT;
```

---

#### 步骤 5：创建数据目录

**当前目录**：`E:\agoodays\agpayplus`

```powershell
# 创建日志和上传文件目录
PS E:\agoodays\agpayplus> New-Item -ItemType Directory -Force -Path E:\app\agpayplus\logs
# 输出示例：
#     目录: E:\app\agpayplus
# Mode                 LastWriteTime         Length Name
# ----                 -------------         ------ ----
# d-----         2024/1/1     11:00                logs

PS E:\agoodays\agpayplus> New-Item -ItemType Directory -Force -Path E:\app\agpayplus\upload
# 输出示例：
#     目录: E:\app\agpayplus
# Mode                 LastWriteTime         Length Name
# ----                 -------------         ------ ----
# d-----         2024/1/1     11:00                upload
```

---

#### 步骤 6：执行自动化部署脚本

**当前目录**：`E:\agoodays\agpayplus`

```powershell
PS E:\agoodays\agpayplus> .\deploy-windows.ps1
```

**完整输出示例**：

```
========================================
   AgPay+ Docker 部署脚本 (Windows)
========================================

[1/6] 检查 Docker 环境...
ℹ️ Docker 版本: 24.0.6
ℹ️ Docker Compose 版本: v2.23.0

[2/6] 配置环境变量...
ℹ️ 已创建 .env 文件

[3/6] 生成 SSL 证书...
ℹ️ 证书目录已存在: C:\Users\YourName\.aspnet\https
检测到现有证书...
是否重新生成证书? (y/n) [默认: n]: n
ℹ️ 使用现有证书

[4/6] 创建数据目录...
ℹ️ 创建目录: E:\app\agpayplus\logs
ℹ️ 创建目录: E:\app\agpayplus\upload

[5/6] 构建 Docker 镜像...
[+] Building 145.2s (62/62) FINISHED
 => [ui-manager internal] load build definition from Dockerfile                    0.1s
 => => transferring dockerfile: 543B                                               0.0s
 => [ui-manager internal] load .dockerignore                                       0.0s
 => => transferring context: 2B                                                    0.0s
 => [ui-manager internal] load metadata for docker.io/library/nginx:stable-alpine 1.2s
 => [ui-manager internal] load metadata for docker.io/library/node:16-alpine      1.3s
 ...
 => [ui-manager] exporting to image                                                2.1s
 => => exporting layers                                                            2.0s
 => => writing image sha256:abc123...                                              0.0s
 => => naming to docker.io/library/agpayplus-ui-manager                            0.0s

[+] Building 152.3s (58/58) FINISHED
 => [manager-api internal] load build definition from Dockerfile                   0.0s
 ...
 => [manager-api] exporting to image                                               3.2s

[+] 镜像构建成功

[6/6] 启动服务...
[+] Running 10/10
 • Network agpayplus_app-network           Created                                 0.1s
 • Container agpayplus-redis-1             Started                                 1.2s
 • Container agpayplus-rabbitmq-1          Started                                 1.3s
 • Container agpayplus-ui-manager-1        Started                                 2.1s
 • Container agpayplus-ui-agent-1          Started                                 2.2s
 • Container agpayplus-ui-merchant-1       Started                                 2.1s
 • Container agpayplus-manager-api-1       Started                                 3.5s
 • Container agpayplus-agent-api-1         Started                                 3.6s
 • Container agpayplus-merchant-api-1      Started                                 3.4s
 • Container agpayplus-payment-api-1       Started                                 3.7s
✅ 服务启动成功

========================================
   部署完成！
========================================

✅ 所有服务已成功启动

🔗 服务访问地址：
   运营平台:     https://localhost:8817
   代理商系统:   https://localhost:8816
   商户系统:     https://localhost:8818
   支付网关 API: https://localhost:9819
   收银台:       https://localhost:9819/cashier
   RabbitMQ:     http://localhost:15672

🔐 默认账号密码：
   运营平台:     agpayadmin / agpay123
   代理商/商户:  (创建后) / agpay666
   RabbitMQ:     admin / admin

📄 查看日志: docker compose logs -f
📊 查看状态: docker compose ps

✅ 部署成功！
========================================
```

---

#### 步骤 7：验证部署

**当前目录**：`E:\agoodays\agpayplus`

```powershell
# 查看容器状态
PS E:\agoodays\agpayplus> docker compose ps
# 输出示例：
# NAME                          IMAGE                   STATUS         PORTS
# agpayplus-agent-api-1         agpayplus-agent-api     Up 2 minutes   0.0.0.0:5816->5816/tcp, 0.0.0.0:9816->9816/tcp
# agpayplus-manager-api-1       agpayplus-manager-api   Up 2 minutes   0.0.0.0:5817->5817/tcp, 0.0.0.0:9817->9817/tcp
# agpayplus-merchant-api-1      agpayplus-merchant-api  Up 2 minutes   0.0.0.0:5818->5818/tcp, 0.0.0.0:9818->9818/tcp
# agpayplus-payment-api-1       agpayplus-payment-api   Up 2 minutes   0.0.0.0:5819->5819/tcp, 0.0.0.0:9819->9819/tcp
# agpayplus-rabbitmq-1          rabbitmq:3-management   Up 2 minutes   0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
# agpayplus-redis-1             redis                   Up 2 minutes   0.0.0.0:6379->6379/tcp
# agpayplus-ui-agent-1          agpayplus-ui-agent      Up 2 minutes   0.0.0.0:8816->80/tcp
# agpayplus-ui-manager-1        agpayplus-ui-manager    Up 2 minutes   0.0.0.0:8817->80/tcp
# agpayplus-ui-merchant-1       agpayplus-ui-merchant   Up 2 minutes   0.0.0.0:8818->80/tcp

# 查看日志（实时）
PS E:\agoodays\agpayplus> docker compose logs -f payment-api
# 输出示例（持续滚动）：
# payment-api-1  | info: Microsoft.Hosting.Lifetime[14]
# payment-api-1  |       Now listening on: https://[::]:9819
# payment-api-1  | info: Microsoft.Hosting.Lifetime[14]
# payment-api-1  |       Now listening on: http://[::]:5819
# payment-api-1  | info: Microsoft.Hosting.Lifetime[0]
# payment-api-1  |       Application started. Press Ctrl+C to shutdown.
# payment-api-1  | info: Microsoft.Hosting.Lifetime[0]
# payment-api-1  |       Hosting environment: Production
# payment-api-1  | info: Microsoft.Hosting.Lifetime[0]
# payment-api-1  |       Content root path: /app

# 按 Ctrl+C 退出日志查看

# 测试服务访问
PS E:\agoodays\agpayplus> curl -k https://localhost:9817/api/health
# 输出示例：
# StatusCode        : 200
# StatusDescription : OK
# Content           : {"status":"Healthy"}
```

---

### Linux/macOS 部署

#### 步骤 1：准备环境

**当前目录**：任意目录

```bash
# 检查 Docker 是否安装
$ docker --version
# 输出示例：
# Docker version 24.0.6, build ed223bc

$ docker compose version
# 输出示例：
# Docker Compose version v2.23.0

# 检查 Git 是否安装
$ git --version
# 输出示例：
# git version 2.39.2

# 检查 .NET SDK（用于生成证书）
$ dotnet --version
# 输出示例：
# 8.0.100
```

---

#### 步骤 2：克隆项目

**当前目录**：您的工作目录（例如 `/opt`）

```bash
# 进入工作目录
$ cd /opt

# 克隆项目
$ git clone https://github.com/agoodays/agpayplus.git
# 或使用 Gitee
$ git clone https://gitee.com/agoodays/agpayplus.git

# 输出示例：
# Cloning into 'agpayplus'...
# remote: Enumerating objects: 1234, done.
# remote: Counting objects: 100% (1234/1234), done.
# remote: Compressing objects: 100% (567/567), done.
# remote: Total 1234 (delta 890), reused 1100 (delta 800)
# Receiving objects: 100% (1234/1234), 15.20 MiB | 2.50 MiB/s, done.
# Resolving deltas: 100% (890/890), done.

# 进入项目目录
$ cd agpayplus
$ pwd
# 输出示例：
# /opt/agpayplus
```

---

#### 步骤 3：配置环境变量

**当前目录**：`/opt/agpayplus`

```bash
$ ls -la .env*
# 输出示例：
# -rw-r--r-- 1 root root  450 Jan  1 10:00 .env.app
# -rw-r--r-- 1 root root  520 Jan  1 10:00 .env.linux
# -rw-r--r-- 1 root root  530 Jan  1 10:00 .env.windows

# 复制环境变量模板
$ cp .env.linux .env

# 编辑配置文件
$ vim .env
# 或使用 nano
$ nano .env
```

**编辑 `.env` 文件**：

```env
# 基础配置
IPORDOMAIN=localhost  # 生产环境改为实际 IP 或域名

# MySQL 配置（使用宿主机 MySQL）
MYSQL_SERVER_NAME=172.17.0.1  # Docker 网桥 IP
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # （请修改为实际密码）

# 数据路径
DATA_PATH_HOST=/opt/agpayplus  # （请修改为实际路径）

# 证书路径
CERT_PATH=~/.aspnet/https
```

**保存并退出**：
- vim: 按 `ESC`，输入 `:wq`，按 `Enter`
- nano: 按 `Ctrl+X`，按 `Y`，按 `Enter`

---

#### 步骤 4：获取 Docker 网桥 IP

**当前目录**：`/opt/agpayplus`

```bash
# 获取 Docker 网桥 IP
$ ip addr show docker0 | grep inet
# 输出示例：
#     inet 172.17.0.1/16 brd 172.17.255.255 scope global docker0
#     inet6 fe80::42:c0ff:fea8:9f1/64 scope link

# 使用上面显示的 IP（172.17.0.1）更新 .env 文件
$ vim .env
# 修改 MYSQL_SERVER_NAME=172.17.0.1
```

---

#### 步骤 5：准备 MySQL 数据库

**当前目录**：`/opt/agpayplus`

##### 5.1 创建数据库

```bash
$ mysql -u root -p
# 输出示例：
# Enter password: 
# Welcome to the MySQL monitor.  Commands end with ; or \g.
# Your MySQL connection id is 8
# Server version: 8.0.36 MySQL Community Server - GPL
#
# mysql>
```

```sql
mysql> CREATE DATABASE agpayplusdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
-- 输出示例：
-- Query OK, 1 row affected (0.01 sec)

mysql> EXIT;
```

##### 5.2 导入初始化脚本

```bash
$ mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql
# 输出示例：
# Enter password: 
# （导入过程可能需要 10-30 秒，成功后没有输出）

# 验证导入结果
$ mysql -u root -p agpayplusdb -e "SHOW TABLES;"
# 输出示例：
# Enter password: 
# +-------------------------+
# | Tables_in_agpayplusdb   |
# +-------------------------+
# | t_sys_config            |
# | t_sys_user              |
# | t_mch_info              |
# | t_pay_order             |
# ...
# +-------------------------+
```

##### 5.3 配置 MySQL 远程访问

```bash
$ mysql -u root -p
```

```sql
mysql> CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
mysql> GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
mysql> FLUSH PRIVILEGES;
mysql> EXIT;
```

---

#### 步骤 6：创建数据目录

**当前目录**：`/opt/agpayplus`

```bash
# 创建日志和上传文件目录
$ mkdir -p /opt/agpayplus/logs
$ mkdir -p /opt/agpayplus/upload

# 验证目录
$ ls -la /opt/agpayplus/
# 输出示例：
# total 12
# drwxr-xr-x  4 root root 4096 Jan  1 11:00 .
# drwxr-xr-x 10 root root 4096 Jan  1 11:00 ..
# drwxr-xr-x  2 root root 4096 Jan  1 11:00 logs
# drwxr-xr-x  2 root root 4096 Jan  1 11:00 upload
```

---

#### 步骤 7：添加脚本执行权限

**当前目录**：`/opt/agpayplus`

```bash
$ chmod +x deploy-linux.sh
$ chmod +x update-linux.sh
$ chmod +x generate-cert-linux.sh

# 验证权限
$ ls -la *.sh
# 输出示例：
# -rwxr-xr-x 1 root root 3456 Jan  1 10:00 deploy-linux.sh
# -rwxr-xr-x 1 root root 2345 Jan  1 10:00 generate-cert-linux.sh
# -rwxr-xr-x 1 root root 2789 Jan  1 10:00 update-linux.sh
```

---

#### 步骤 8：执行自动化部署脚本

**当前目录**：`/opt/agpayplus`

```bash
$ ./deploy-linux.sh
```

**完整输出示例**：

```
========================================
   AgPay+ Docker 部署脚本 (Linux/macOS)
========================================

[1/6] 检查 Docker 环境...
ℹ️ Docker 版本: 24.0.6
ℹ️ Docker Compose 版本: v2.23.0

[2/6] 配置环境变量...
ℹ️ 已创建 .env 文件

[3/6] 生成 SSL 证书...
ℹ️ 证书目录已存在: /root/.aspnet/https
检测到现有证书...
是否重新生成证书? (y/n) [默认: n]: n
ℹ️ 使用现有证书

[4/6] 创建数据目录...
ℹ️ 创建目录: /opt/agpayplus/logs
ℹ️ 创建目录: /opt/agpayplus/upload

[5/6] 构建 Docker 镜像...
[+] Building 145.2s (62/62) FINISHED
 => [ui-manager internal] load build definition from Dockerfile                    0.1s
 ...
 => [manager-api] exporting to image                                               3.2s

[+] 镜像构建成功

[6/6] 启动服务...
[+] Running 10/10
 • Network agpayplus_app-network           Created                                 0.1s
 • Container agpayplus-redis-1             Started                                 1.2s
 • Container agpayplus-rabbitmq-1          Started                                 1.3s
 • Container agpayplus-ui-manager-1        Started                                 2.1s
 • Container agpayplus-ui-agent-1          Started                                 2.2s
 • Container agpayplus-ui-merchant-1       Started                                 2.1s
 • Container agpayplus-manager-api-1       Started                                 3.5s
 • Container agpayplus-agent-api-1         Started                                 3.6s
 • Container agpayplus-merchant-api-1      Started                                 3.4s
 • Container agpayplus-payment-api-1       Started                                 3.7s
✅ 服务启动成功

========================================
   部署完成！
========================================

✅ 所有服务已成功启动

🔗 服务访问地址：
   运营平台:     https://localhost:8817
   代理商系统:   https://localhost:8816
   商户系统:     https://localhost:8818
   支付网关 API: https://localhost:9819
   收银台:       https://localhost:9819/cashier
   RabbitMQ:     http://localhost:15672

🔐 默认账号密码：
   运营平台:     agpayadmin / agpay123
   代理商/商户:  (创建后) / agpay666
   RabbitMQ:     admin / admin

📄 查看日志: docker compose logs -f
📊 查看状态: docker compose ps

✅ 部署成功！
========================================
```

---

#### 步骤 9：验证部署

**当前目录**：`/opt/agpayplus`

```bash
# 查看容器状态
$ docker compose ps
# 输出示例：
# NAME                          IMAGE                   STATUS         PORTS
# agpayplus-agent-api-1         agpayplus-agent-api     Up 2 minutes   0.0.0.0:5816->5816/tcp, 0.0.0.0:9816->9816/tcp
# agpayplus-manager-api-1       agpayplus-manager-api   Up 2 minutes   0.0.0.0:5817->5817/tcp, 0.0.0.0:9817->9817/tcp
# agpayplus-merchant-api-1      agpayplus-merchant-api  Up 2 minutes   0.0.0.0:5818->5818/tcp, 0.0.0.0:9818->9818/tcp
# agpayplus-payment-api-1       agpayplus-payment-api   Up 2 minutes   0.0.0.0:5819->5819/tcp, 0.0.0.0:9819->9819/tcp
# agpayplus-rabbitmq-1          rabbitmq:3-management   Up 2 minutes   0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
# agpayplus-redis-1             redis                   Up 2 minutes   0.0.0.0:6379->6379/tcp
# agpayplus-ui-agent-1          agpayplus-ui-agent      Up 2 minutes   0.0.0.0:8816->80/tcp
# agpayplus-ui-manager-1        agpayplus-ui-manager    Up 2 minutes   0.0.0.0:8817->80/tcp
# agpayplus-ui-merchant-1       agpayplus-ui-merchant   Up 2 minutes   0.0.0.0:8818->80/tcp

# 查看特定服务日志
$ docker compose logs -f payment-api
# 按 Ctrl+C 退出

# 测试服务访问
$ curl -k https://localhost:9817/api/health
# 输出示例：
# {"status":"Healthy"}
```

---

## 服务更新流程

### Windows 更新

**当前目录**：`E:\agoodays\agpayplus`

#### 场景 1：更新所有应用服务

```powershell
PS E:\agoodays\agpayplus> .\update-windows.ps1
```

**输出示例**：

```
========================================
   AgPay+ 服务更新脚本 (Windows)
========================================

即将更新以下服务：
  • ui-manager
  • ui-agent
  • ui-merchant
  • manager-api
  • agent-api
  • merchant-api
  • payment-api

是否继续? (y/n) [默认: n]: y

[1/4] 检查 Docker 环境...
ℹ️ Docker 正在运行

[2/4] 检查代码更新...
是否拉取最新代码? (y/n) [默认: n]: y
From https://github.com/agoodays/agpayplus
 * branch            main       -> FETCH_HEAD
Already up to date.
ℹ️ 代码已是最新

[3/4] 重新构建镜像...
这可能需要几分钟时间，请耐心等待...
[+] Building 89.3s (62/62) FINISHED
 => [ui-manager] exporting to image                                                2.1s
 ...
✅ 镜像重新構建完成

[4/4] 更新服务...
[+] Running 7/7
 • Container agpayplus-ui-manager-1    Started                                    2.1s
 • Container agpayplus-ui-agent-1      Started                                    2.2s
 • Container agpayplus-ui-merchant-1   Started                                    2.3s
 • Container agpayplus-manager-api-1   Started                                    3.1s
 • Container agpayplus-agent-api-1     Started                                    3.2s
 • Container agpayplus-merchant-api-1  Started                                    3.3s
 • Container agpayplus-payment-api-1   Started                                    3.4s
✅ 所有服务更新成功

========================================
   更新完成！
========================================

📄 查看日志: docker compose logs -f [service_name]
📊 查看状态: docker compose ps

✅ 更新成功！
========================================
```

---

#### 场景 2：更新指定服务

```powershell
PS E:\agoodays\agpayplus> .\update-windows.ps1 -Services "payment-api"
```

**输出示例**：

```
========================================
   AgPay+ 服务更新脚本 (Windows)
========================================

即将更新以下服务：
  • payment-api

是否继续? (y/n) [默认: n]: y

[1/4] 检查 Docker 环境...
ℹ️ Docker 正在运行

[2/4] 检查代码更新...
是否拉取最新代码? (y/n) [默认: n]: n
⏭️ 跳过代码拉取

[3/4] 重新构建镜像...
这可能需要几分钟时间，请耐心等待...
[+] Building 34.2s (28/28) FINISHED
 => [payment-api] exporting to image                                               1.8s
✅ 镜像重新构建完成

[4/4] 更新服务...
[+] Running 1/1
 • Container agpayplus-payment-api-1   Started                                    2.3s
✅ payment-api 更新成功

========================================
   更新完成！
========================================

📄 查看日志: docker compose logs -f payment-api
📊 查看状态: docker compose ps

✅ 更新成功！
========================================
```

---

#### 场景 3：快速更新（跳过构建）

```powershell
PS E:\agoodays\agpayplus> .\update-windows.ps1 -Services "payment-api" -NoBuild
```

**输出示例**：

```
========================================
   AgPay+ 服务更新脚本 (Windows)
========================================


即将更新以下服务：
  • payment-api

是否继续? (y/n) [默认: n]: y

[1/4] 检查 Docker 环境...
ℹ️ Docker 正在运行

[2/4] 检查代码更新...
是否拉取最新代码? (y/n) [默认: n]: n
⏭️ 跳过代码拉取

[3/4] 重新构建镜像...
⏭️ 跳过镜像构建（使用现有镜像）

[4/4] 更新服务...
[+] Running 1/1
 • Container agpayplus-payment-api-1   Started                                    2.1s
✅ payment-api 更新成功

========================================
   更新完成！
========================================
```

---

### Linux/macOS 更新

**当前目录**：`/opt/agpayplus`

#### 场景 1：更新所有应用服务

```bash
$ ./update-linux.sh
```

输出与 Windows 类似。

---

#### 场景 2：更新指定服务

```bash
$ ./update-linux.sh --services "payment-api,manager-api"
```

**输出示例**：

```
========================================
   AgPay+ 服务更新脚本 (Linux/macOS)
========================================

即将更新以下服务：
  • payment-api
  • manager-api

是否继续? (y/n) [默认: n]: y

[1/4] 检查 Docker 环境...
ℹ️ Docker 正在运行

[2/4] 检查代码更新...
是否拉取最新代码? (y/n) [默认: n]: y
From https://github.com/agoodays/agpayplus
 * branch            main       -> FETCH_HEAD
Already up to date.
ℹ️ 代码已是最新

[3/4] 重新构建镜像...
这可能需要几分钟时间，请耐心等待...
[+] Building 56.7s (44/44) FINISHED
 => [payment-api] exporting to image                                               2.1s
 => [manager-api] exporting to image                                               2.3s
✅ 镜像重新构建完成

[4/4] 更新服务...
[+] Running 2/2
 • Container agpayplus-payment-api-1   Started                                    2.3s
 • Container agpayplus-manager-api-1   Started                                    2.4s
✅ payment-api, manager-api 更新成功

========================================
   更新完成！
========================================

📄 查看日志: docker compose logs -f [service_name]
📊 查看状态: docker compose ps

✅ 更新成功！
========================================
```

---

## 常见问题排查

### 问题 1：容器启动失败

**当前目录**：项目根目录

```bash
# 查看容器状态
$ docker compose ps
# 输出示例（问题）：
# NAME                          STATUS
# agpayplus-payment-api-1       Restarting (1) 10 seconds ago

# 查看日志
$ docker compose logs payment-api
# 输出示例（错误）：
# payment-api-1  | Unhandled exception. System.InvalidOperationException: 
# payment-api-1  | Unable to connect to MySQL server at '172.17.0.1:3306'
# payment-api-1  | at MySql.Data.MySqlClient.MySqlConnection.Open()
```

**解决方案**：
1. 检查 MySQL 是否运行
2. 检查 `.env` 中的 MySQL 配置
3. 验证 MySQL 远程访问权限

```bash
# 测试 MySQL 连接
$ mysql -h 172.17.0.1 -u root -p
```

---

### 问题 2：证书错误

**症状**：浏览器提示证书不受信任

**解决方案**：

**Windows:**
```powershell
PS E:\agoodays\agpayplus> .\generate-cert-windows.ps1
# 选择 'y' 重新生成并信任证书
```

**Linux/macOS:**
```bash
$ ./generate-cert-linux.sh
# 选择 'y' 重新生成证书
```

---

### 问题 3：端口被占用

```bash
$ docker compose up -d
# 输出示例（错误）：
# Error response from daemon: Ports are not available: 
# exposing port TCP 0.0.0.0:8817 -> 0.0.0.0:0: 
# listen tcp 0.0.0.0:8817: bind: address already in use
```

**解决方案**：

**Windows:**
```powershell
# 查找占用端口的进程
PS> netstat -ano | findstr :8817
# 输出示例：
#   TCP    0.0.0.0:8817           0.0.0.0:0              LISTENING       12345

# 结束进程
PS> taskkill /PID 12345 /F
```

**Linux/macOS:**
```bash
# 查找占用端口的进程
$ sudo lsof -i :8817
# 输出示例：
# COMMAND  PID USER   FD   TYPE DEVICE SIZE/OFF NODE NAME
# nginx   1234 root    6u  IPv4  12345      0t0  TCP *:8817 (LISTEN)

# 结束进程
$ sudo kill -9 1234
```

---

### 问题 4：收银台无法访问

```bash
# 进入 payment-api 容器
$ docker exec -it agpayplus-payment-api-1 /bin/bash

# 检查收银台文件
root@container:/app# ls -la wwwroot/cashier/
# 输出示例（问题）：
# ls: cannot access 'wwwroot/cashier/': No such file or directory

# 退出容器
root@container:/app# exit
```

**解决方案**：
```bash
# 重新构建 payment-api（包含收银台）
$ docker compose build --no-cache payment-api
$ docker compose up -d payment-api
```

---

## 验证部署结果

**当前目录**：项目根目录

### 1. 检查容器状态

```bash
$ docker compose ps
```

**预期输出**：所有服务状态为 `Up`

```
NAME                          STATUS
agpayplus-agent-api-1         Up 5 minutes (healthy)
agpayplus-manager-api-1       Up 5 minutes (healthy)
agpayplus-merchant-api-1      Up 5 minutes (healthy)
agpayplus-payment-api-1       Up 5 minutes (healthy)
agpayplus-rabbitmq-1          Up 5 minutes (healthy)
agpayplus-redis-1             Up 5 minutes (healthy)
agpayplus-ui-agent-1          Up 5 minutes
agpayplus-ui-manager-1        Up 5 minutes
agpayplus-ui-merchant-1       Up 5 minutes
```

---

### 2. 测试服务访问

#### 测试后端 API

```bash
# 测试管理平台 API
$ curl -k https://localhost:9817/api/health
# 预期输出：
# {"status":"Healthy"}

# 测试支付网关 API
$ curl -k https://localhost:9819/api/health
# 预期输出：
# {"status":"Healthy"}
```

#### 访问前端页面

在浏览器中访问：

| 服务 | 地址 | 预期 |
|------|------|------|
| 运营平台 | https://localhost:8817 | ✅ 显示登录页面 |
| 代理商系统 | https://localhost:8816 | ✅ 显示登录页面 |
| 商户系统 | https://localhost:8818 | ✅ 显示登录页面 |
| 收银台 | https://localhost:9819/cashier | ✅ 显示收银台页面 |

---

### 3. 检查 RabbitMQ 延时插件

```bash
$ docker exec agpayplus-rabbitmq-1 rabbitmq-plugins list | grep delayed
# 预期输出：
# [E*] rabbitmq_delayed_message_exchange 3.13.0
# 
# 说明：
# [E] = Explicitly enabled（显式启用）
# [*] = Running（正在运行）
```

---

### 4. 检查 Redis 连接

```bash
$ docker exec agpayplus-redis-1 redis-cli ping
# 预期输出：
# PONG
```

---

### 5. 登录运营平台测试

1. 访问：https://localhost:8817
2. 账号：`agpayadmin`
3. 密码：`agpay123`
4. 点击登录

**预期**：✅ 成功登录，进入运营平台首页

---

## 🎉 恭喜！

部署完成！现在您可以：

1. ✅ 访问各个系统界面
2. ✅ 进行系统配置和测试
3. ✅ 查看服务日志和监控
4. ✅ 根据需要更新服务

### 下一步

- 📖 阅读 [完整部署文档](DOCKER_DEPLOYMENT.md) 了解更多配置
- 🗄️ 查看 [数据库搭建指南](DATABASE_SETUP.md) 优化数据库
- ✅ 参考 [部署检查清单](DEPLOYMENT_CHECKLIST.md) 准备生产环境
- 🔧 了解 [收银台部署说明](CASHIER_DEPLOYMENT.md) 自定义收银台

---

## 📎 相关文档

- [README_DOCKER.md](README_DOCKER.md) - 快速入门
- [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) - 完整部署文档
- [DATABASE_SETUP.md](DATABASE_SETUP.md) - 数据库搭建
- [DEPLOYMENT_CHECKLIST.md](DEPLOYMENT_CHECKLIST.md) - 部署检查清单
- [CASHIER_DEPLOYMENT.md](CASHIER_DEPLOYMENT.md) - 收银台部署

---

**祝您使用愉快！** 🎉
