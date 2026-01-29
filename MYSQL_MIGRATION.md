# MySQL 数据迁移说明

## 📋 概述

本文档说明如何将 Docker MySQL 数据从旧的 **named volume** 方式迁移到新的 **host bind mount** 方式。

**变更说明**：
- **旧方式**：使用 Docker named volume (`db-data`)，数据存储在 Docker 管理的卷中
- **新方式**：使用 host bind mount (`${DATA_PATH_HOST}/mysql`)，数据直接存储在宿主机指定目录

**变更优势**：
- ✅ 统一管理：所有持久化数据集中在 `${DATA_PATH_HOST}` 目录
- ✅ 便于备份：可直接备份整个数据目录
- ✅ 易于迁移：复制目录即可迁移数据
- ✅ 位置透明：数据位置明确，便于监控

---

## ⚠️ 重要提示

**生产环境建议**：
- 生产环境强烈建议使用**宿主机 MySQL** 或**云数据库（RDS）**
- Docker MySQL 仅适合开发/测试环境
- 如果已在使用宿主机 MySQL，无需进行本迁移操作

---

## 🔄 迁移步骤

### 方式一：使用 mysqldump 导出/导入（推荐）

这是最安全的方式，适用于所有场景。

#### 1. 导出现有数据

```bash
# 检查当前运行的 MySQL 容器
docker compose ps

# 导出所有数据库
docker exec <mysql-container-name> mysqldump \
  -u root -p<your-password> \
  --all-databases \
  --single-transaction \
  --quick \
  --lock-tables=false \
  > mysql_backup_$(date +%Y%m%d_%H%M%S).sql

# 验证备份文件
ls -lh mysql_backup_*.sql
```

#### 2. 停止并移除旧容器

```bash
# 停止所有服务
docker compose down

# 可选：删除旧的 named volume（确认备份后）
# docker volume rm agpayplus_db-data
```

#### 3. 更新配置

确认 `docker-compose.yml` 中 MySQL 配置已更新为：

```yaml
#   volumes:
#     - ./aspnet-core/docs/sql/agpayplusinit.sql:/docker-entrypoint-initdb.d/init.sql:ro
#     - ${DATA_PATH_HOST}/mysql:/var/lib/mysql
```

#### 4. 创建 MySQL 数据目录

```bash
# Linux/macOS
sudo mkdir -p /opt/agpayplus/mysql
sudo chown -R $(whoami):$(whoami) /opt/agpayplus/mysql

# 或使用部署脚本自动创建
./deploy-linux.sh --skip-cert --skip-env
```

#### 5. 启动新的 MySQL 容器

```bash
# 取消 docker-compose.yml 中 db 服务的注释
# 修改 .env 中 MYSQL_SERVER_NAME=db

# 启动 MySQL 服务
docker compose up -d db

# 等待 MySQL 初始化完成（约 30-60 秒）
docker compose logs -f db
```

#### 6. 导入数据

```bash
# 导入备份数据
docker exec -i <new-mysql-container-name> mysql \
  -u root -p<your-password> \
  < mysql_backup_<timestamp>.sql

# 验证数据
docker exec <new-mysql-container-name> mysql \
  -u root -p<your-password> \
  -e "SHOW DATABASES;"
```

#### 7. 启动应用服务

```bash
# 启动所有服务
docker compose up -d

# 检查服务状态
docker compose ps
docker compose logs -f manager-api
```

---

### 方式二：直接复制数据文件（高级用户）

⚠️ **风险较高，仅适合测试环境**

#### 1. 停止 MySQL 容器

```bash
docker compose stop db
```

#### 2. 找到旧 volume 位置

```bash
# 查看 volume 详情
docker volume inspect agpayplus_db-data

# 输出类似：
# "Mountpoint": "/var/lib/docker/volumes/agpayplus_db-data/_data"
```

#### 3. 复制数据

```bash
# 复制数据到新位置（需要 sudo）
sudo cp -rp /var/lib/docker/volumes/agpayplus_db-data/_data/* \
  /opt/agpayplus/mysql/

# 修改权限
sudo chown -R 999:999 /opt/agpayplus/mysql
```

#### 4. 更新配置并启动

```bash
# 更新 docker-compose.yml 中的 volumes 配置
# 启动服务
docker compose up -d db
```

---

## 🔍 验证迁移成功

### 1. 检查容器状态

```bash
# 查看容器是否正常运行
docker compose ps

# 查看 MySQL 日志
docker compose logs db | tail -50
```

### 2. 验证数据库连接

```bash
# 测试连接
docker exec <mysql-container-name> mysql \
  -u root -p<your-password> \
  -e "SELECT VERSION();"

# 检查数据库列表
docker exec <mysql-container-name> mysql \
  -u root -p<your-password> \
  -e "SHOW DATABASES;"
```

### 3. 验证应用连接

```bash
# 检查 API 服务日志
docker compose logs manager-api | grep -i mysql
docker compose logs manager-api | grep -i "database"

# 测试 API 接口
curl -k https://localhost:9817/api/health
```

### 4. 检查数据文件

```bash
# 查看数据目录
ls -lh /opt/agpayplus/mysql/

# 应该看到：
# - ibdata1（InnoDB 系统表空间）
# - ib_logfile*（InnoDB 日志文件）
# - mysql/（系统数据库）
# - agpayplusdb/（应用数据库）
```

---

## 🔙 回滚方案

如果迁移后遇到问题，可以回滚到旧配置：

### 1. 停止服务

```bash
docker compose down
```

### 2. 恢复旧配置

在 `docker-compose.yml` 中恢复：

```yaml
#   volumes:
#     - ./aspnet-core/docs/sql/agpayplusinit.sql:/docker-entrypoint-initdb.d/init.sql:ro
#     - db-data:/var/lib/mysql

# 恢复 volumes 定义
volumes:
  db-data:
    driver: local
```

### 3. 重新启动

```bash
docker compose up -d
```

---

## 📝 注意事项

### 权限问题

MySQL 容器通常以 UID `999` 运行，确保数据目录权限正确：

```bash
# Linux/macOS
sudo chown -R 999:999 /opt/agpayplus/mysql

# 或设置宽松权限（仅测试环境）
sudo chmod -R 777 /opt/agpayplus/mysql
```

### SELinux（CentOS/RHEL）

如果启用了 SELinux，需要设置正确的安全上下文：

```bash
sudo chcon -R -t svirt_sandbox_file_t /opt/agpayplus/mysql
```

### 磁盘空间

确保宿主机有足够空间：

```bash
# 检查磁盘空间
df -h /opt/agpayplus

# 检查 MySQL 数据大小
du -sh /opt/agpayplus/mysql
```

### 备份建议

迁移前后都应做好备份：

```bash
# 迁移前：导出备份
docker exec <old-container> mysqldump -u root -p --all-databases > before_migration.sql

# 迁移后：导出备份
docker exec <new-container> mysqldump -u root -p --all-databases > after_migration.sql

# 保留旧 volume 一段时间
# 确认迁移成功后再删除：docker volume rm agpayplus_db-data
```

---

## 🆘 故障排查

### 问题 1：容器无法启动

**症状**：`docker compose up -d db` 后容器立即退出

**原因**：
- 数据目录权限不正确
- 数据目录不为空且包含损坏文件
- 端口冲突

**解决**：
```bash
# 查看日志
docker compose logs db

# 检查权限
ls -la /opt/agpayplus/mysql

# 清空目录重新初始化（⚠️ 数据会丢失）
sudo rm -rf /opt/agpayplus/mysql/*
docker compose up -d db
```

### 问题 2：应用无法连接数据库

**症状**：API 日志显示 "Unable to connect to MySQL"

**原因**：
- 数据库未完全初始化
- 连接字符串配置错误
- 网络问题

**解决**：
```bash
# 检查 MySQL 是否就绪
docker exec <mysql-container> mysqladmin -u root -p<password> ping

# 检查网络连通性
docker compose exec manager-api ping db

# 验证连接字符串
docker compose config | grep ConnectionStrings
```

### 问题 3：数据丢失

**症状**：数据库中没有应用数据

**原因**：
- 导入步骤未执行
- 导入失败但未报错
- 使用了错误的备份文件

**解决**：
```bash
# 重新导入备份
docker exec -i <mysql-container> mysql -u root -p < mysql_backup_<timestamp>.sql

# 验证导入
docker exec <mysql-container> mysql -u root -p -e "USE agpayplusdb; SHOW TABLES;"
```

---

## 📚 相关文档

- [Docker 部署说明](DOCKER_DEPLOYMENT.md)
- [数据库设置说明](DATABASE_SETUP.md)
- [部署指南](DEPLOYMENT_GUIDE.md)

---

## 📞 获取帮助

如果迁移过程中遇到问题：

1. 查看日志：`docker compose logs db`
2. 检查本文档的"故障排查"部分
3. 在 GitHub Issues 提问
4. 保留备份文件和日志，便于排查

---
