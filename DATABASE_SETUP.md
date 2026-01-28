# 📊 AgPay+ 数据库环境搭建指南

> **⚠️ 重要提示：生产环境不建议使用 Docker MySQL！**
> 
> **原因**：性能损失、数据安全风险、备份恢复复杂、运维成本高
> 
> **生产环境请使用**：宿主机 MySQL 或云数据库（阿里云 RDS、AWS RDS 等）

本文档提供 **两种数据库部署方式**，请根据实际场景选择：

1. **🏠 宿主机 MySQL**（✅ **强烈推荐生产环境**）- 使用已安装的 MySQL 服务
2. **🐳 Docker MySQL**（⚠️ **仅用于开发/测试**）- 使用容器化 MySQL 服务

---

## 📋 目录

- [方式对比](#方式对比)
- [方式 1：宿主机 MySQL](#方式-1宿主机-mysql)
- [方式 2：Docker MySQL](#方式-2docker-mysql)
- [数据库初始化](#数据库初始化)
- [数据库管理](#数据库管理)
- [故障排查](#故障排查)
- [性能优化](#性能优化)

---

## 方式对比

| 特性 | 宿主机 MySQL | Docker MySQL |
|------|-------------|-------------|
| **性能** | ⭐⭐⭐⭐⭐ 最佳 | ⭐⭐⭐⭐ 良好 |
| **数据持久化** | ✅ 原生文件系统 | ✅ Docker Volume |
| **管理工具** | ✅ 任意工具 | ✅ 通过端口映射 |
| **备份恢复** | ✅ 直接操作 | ⚠️ 需通过容器 |
| **资源消耗** | 📉 独立进程 | 📈 额外容器 |
| **适用场景** | 🏢 生产环境 | 🧪 开发/测试 |
| **部署复杂度** | ⚠️ 需手动安装 | ✅ 一键部署 |

### 🎯 推荐选择

- **生产环境** → 方式 1（宿主机 MySQL）
- **开发环境** → 方式 2（Docker MySQL）
- **快速体验** → 方式 2（Docker MySQL）

---

## 方式 1：宿主机 MySQL

使用宿主机上已安装的 MySQL 服务。

### 步骤 1：安装 MySQL 8.0+

#### Windows

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

#### Linux (Ubuntu/Debian)

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

#### macOS

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

### 步骤 2：创建数据库

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

### 步骤 3：导入初始化脚本

```bash
# 导入 SQL 脚本
mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql

# 验证表是否创建成功
mysql -u root -p agpayplusdb -e "SHOW TABLES;"
```

### 步骤 4：配置远程访问（Docker 容器访问）

```sql
-- 连接 MySQL
mysql -u root -p

-- 创建远程访问用户（或授权 root）
-- 方案 A：授权现有 root 用户
CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';

-- 或方案 B：创建专用用户
CREATE USER 'agpayuser'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'agpayuser'@'%';

-- 刷新权限
FLUSH PRIVILEGES;

-- 验证用户
SELECT host, user FROM mysql.user WHERE user='root';

EXIT;
```

#### 修改 MySQL 配置（如需要）

**Linux:**
```bash
# 编辑配置
sudo vim /etc/mysql/mysql.conf.d/mysqld.cnf

# 注释掉 bind-address
# bind-address = 127.0.0.1

# 重启 MySQL
sudo systemctl restart mysql
```

**Windows:**
```ini
; 编辑 my.ini (通常在 C:\ProgramData\MySQL\MySQL Server 8.0\)
; 注释掉 bind-address
; bind-address = 127.0.0.1

; 重启 MySQL 服务
net stop MySQL80
net start MySQL80
```

### 步骤 5：配置 .env 文件

**Windows (.env.windows → .env)**

```env
# 使用宿主机 MySQL
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # 修改为实际密码
```

**Linux (.env.linux → .env)**

```env
# 获取 Docker 网桥 IP
# ip addr show docker0 | grep inet
# 输出示例: inet 172.17.0.1/16

MYSQL_SERVER_NAME=172.17.0.1  # 使用上面获取的 IP
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # 修改为实际密码
```

**macOS (.env.linux → .env)**

```env
# macOS 使用 Docker Desktop 内置的 host
MYSQL_SERVER_NAME=host.docker.internal
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=your_actual_password  # 修改为实际密码
```

### 步骤 6：测试连接

```bash
# 从 Docker 网络测试连接
docker run --rm mysql:8.0 mysql -h host.docker.internal -u root -p  # Windows/macOS
docker run --rm mysql:8.0 mysql -h 172.17.0.1 -u root -p  # Linux

# 输入密码后应该能成功连接
```

---

## 方式 2：Docker MySQL

使用 Docker 容器运行 MySQL 服务（**最简单的方式**）。

### 步骤 1：配置 .env 文件

```bash
# 复制模板
cp .env.windows .env  # Windows
cp .env.linux .env    # Linux/macOS

# 编辑配置
vim .env
```

**配置内容：**

```env
# 使用 Docker MySQL
MYSQL_SERVER_NAME=db  # 使用服务名
MYSQL_PORT=3306
MYSQL_DATABASE=agpayplusdb
MYSQL_USER=root
MYSQL_PASSWORD=mysql123456  # 设置密码
```

### 步骤 2：部署 MySQL 容器

Docker Compose 配置已包含 MySQL 服务：

```yaml
services:
  db:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_PASSWORD}
      MYSQL_DATABASE: ${MYSQL_DATABASE}
      MYSQL_USER: ${MYSQL_USER}
      MYSQL_PASSWORD: ${MYSQL_PASSWORD}
      TZ: Asia/Shanghai
    ports:
      - "${MYSQL_PORT}:3306"
    volumes:
      - ./aspnet-core/docs/sql/agpayplusinit.sql:/docker-entrypoint-initdb.d/init.sql:ro
      - db-data:/var/lib/mysql
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
    networks:
      - app-network

volumes:
  db-data:
    driver: local
```

### 步骤 3：启动服务

```bash
# 完整部署（包含 MySQL）
./deploy-windows.ps1  # Windows
./deploy-linux.sh     # Linux/macOS

# 或单独启动 MySQL
docker compose up -d db

# 查看日志
docker compose logs -f db
```

### 步骤 4：验证部署

```bash
# 查看容器状态
docker compose ps db

# 输出示例：
# NAME               STATUS        PORTS
# agpayplus-db-1     Up (healthy)  0.0.0.0:3306->3306/tcp

# 进入容器检查
docker exec -it agpayplus-db-1 mysql -u root -p

# 查看数据库
SHOW DATABASES;
USE agpayplusdb;
SHOW TABLES;
EXIT;
```

### 步骤 5：数据持久化

数据存储在 Docker Volume `db-data` 中：

```bash
# 查看卷信息
docker volume ls | grep db-data
docker volume inspect agpayplus_db-data

# 备份数据
docker exec agpayplus-db-1 mysqldump -u root -p agpayplusdb > backup.sql

# 恢复数据
docker exec -i agpayplus-db-1 mysql -u root -p agpayplusdb < backup.sql
```

---

## 数据库初始化

### 初始化脚本说明

位置：`aspnet-core/docs/sql/agpayplusinit.sql`

**包含内容：**
- 数据库表结构
- 初始数据（管理员账号、系统配置等）
- 索引和约束

### 自动初始化（Docker MySQL）

使用 Docker MySQL 时，脚本会自动执行：

```yaml
volumes:
  - ./aspnet-core/docs/sql/agpayplusinit.sql:/docker-entrypoint-initdb.d/init.sql:ro
```

### 手动初始化（宿主机 MySQL）

```bash
# 方式 1：命令行导入
mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql

# 方式 2：MySQL Shell 内执行
mysql -u root -p agpayplusdb
SOURCE /path/to/agpayplusinit.sql;

# 方式 3：使用 MySQL Workbench
# File → Run SQL Script → 选择 agpayplusinit.sql
```

### 验证初始化

```sql
-- 检查表数量
SELECT COUNT(*) FROM information_schema.tables 
WHERE table_schema = 'agpayplusdb';

-- 检查管理员账号
USE agpayplusdb;
SELECT * FROM sys_user WHERE login_username = 'admin';

-- 检查系统配置
SELECT * FROM sys_config;
```

---

## 数据库管理

### 使用 MySQL Workbench

```bash
# 下载安装
https://dev.mysql.com/downloads/workbench/

# 连接配置
Host: localhost (或 172.17.0.1)
Port: 3306
Username: root
Password: your_password
```

### 使用 Adminer（Web 界面）

添加到 docker-compose.yml：

```yaml
adminer:
  image: adminer
  ports:
    - "8080:8080"
  networks:
    - app-network
```

访问：http://localhost:8080

### 命令行管理

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

# 查看表大小
SELECT 
  table_name AS `Table`,
  ROUND(((data_length + index_length) / 1024 / 1024), 2) AS `Size (MB)`
FROM information_schema.TABLES
WHERE table_schema = 'agpayplusdb'
ORDER BY (data_length + index_length) DESC;
```

---

## 故障排查

### 问题 1：无法连接数据库

**症状：**
```
Can't connect to MySQL server on 'xxx.xxx.xxx.xxx'
```

**解决方案：**

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

# 5. 检查 MySQL 用户权限
mysql -u root -p
SELECT host, user FROM mysql.user;
```

### 问题 2：权限错误

**症状：**
```
ERROR 1045 (28000): Access denied for user 'root'@'xxx'
```

**解决方案：**

```sql
-- 重新授权
CREATE USER 'root'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON agpayplusdb.* TO 'root'@'%';
FLUSH PRIVILEGES;

-- 或修改现有用户
UPDATE mysql.user SET host='%' WHERE user='root' AND host='localhost';
FLUSH PRIVILEGES;
```

### 问题 3：Docker MySQL 无法启动

**症状：**
```
Container keeps restarting
```

**解决方案：**

```bash
# 1. 查看日志
docker compose logs db

# 2. 检查卷权限
docker volume inspect agpayplus_db-data

# 3. 清理并重建
docker compose down -v
docker compose up -d db

# 4. 手动创建卷
docker volume create agpayplus_db-data
```

### 问题 4：初始化脚本未执行

**症状：**
数据库创建了但表为空

**解决方案：**

```bash
# Docker MySQL 只在首次创建时执行初始化脚本
# 如需重新初始化：

# 1. 删除数据卷
docker compose down
docker volume rm agpayplus_db-data

# 2. 重新启动
docker compose up -d db

# 或手动导入
docker exec -i agpayplus-db-1 mysql -u root -p agpayplusdb < aspnet-core/docs/sql/agpayplusinit.sql
```

---

## 性能优化

### MySQL 配置优化

**宿主机 MySQL：**

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

**Docker MySQL：**

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

### 索引优化

```sql
-- 查看慢查询
SHOW VARIABLES LIKE 'slow_query%';

-- 分析表
ANALYZE TABLE table_name;

-- 优化表
OPTIMIZE TABLE table_name;

-- 查看索引使用情况
SHOW INDEX FROM table_name;
```

### 定期维护

```bash
# 备份脚本
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/opt/mysql_backup"

# Docker
docker exec agpayplus-db-1 mysqldump -u root -p${MYSQL_PASSWORD} agpayplusdb > $BACKUP_DIR/agpayplus_$DATE.sql

# 宿主机
mysqldump -u root -p agpayplusdb > $BACKUP_DIR/agpayplus_$DATE.sql

# 压缩
gzip $BACKUP_DIR/agpayplus_$DATE.sql

# 清理 7 天前的备份
find $BACKUP_DIR -name "*.sql.gz" -mtime +7 -delete
```

---

## 📚 相关文档

- [快速部署文档](README_DOCKER.md)
- [完整部署指南](DOCKER_DEPLOYMENT.md)
- [部署检查清单](DEPLOYMENT_CHECKLIST.md)

---

**祝您数据库搭建顺利！** 🎉
