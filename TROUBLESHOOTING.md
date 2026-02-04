# 常见问题快速解决指南

> 部署和更新过程中可能遇到的问题及解决方案

---

## 🌐 网络问题

### ❌ 无法连接到 mcr.microsoft.com

**错误信息**：
```
failed to do request: Get "https://mcr.microsoft.com/..."
dial tcp: connectex: A connection attempt failed
```

**原因**：无法访问 Microsoft Container Registry

**快速解决**：配置国内镜像源

#### Windows
```powershell
# 1. Docker Desktop → Settings → Docker Engine
# 2. 添加配置：
{
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live"
  ]
}
# 3. Apply & Restart
# 4. 重新部署
.\deploy.ps1
```

#### Linux
```bash
sudo nano /etc/docker/daemon.json
# 添加上述 JSON 配置
sudo systemctl restart docker
./deploy.sh
```

**详细说明**：`DOCKER_MIRROR_GUIDE.md`

---

### ❌ docker compose 命令未找到

**错误信息**：
```
docker compose: command not found
```

**解决方案**：

```bash
# 安装 Docker Compose v2
sudo curl -SL https://github.com/docker/compose/releases/download/v2.24.0/docker-compose-linux-x86_64 \
  -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# 或使用包管理器
sudo apt install docker-compose-plugin  # Ubuntu/Debian
sudo yum install docker-compose-plugin  # CentOS/RHEL
```

---

## 🏗️ 构建问题

### ❌ 镜像构建失败

**错误信息**：
```
failed to solve: failed to compute cache key
```

**解决方案**：

```bash
# 清理缓存重新构建
docker builder prune -f
docker compose build --no-cache

# 如果还是失败，检查磁盘空间
df -h
```

---

### ❌ 权限被拒绝

**错误信息**：
```
permission denied while trying to connect to Docker daemon
```

**解决方案**：

```bash
# 将当前用户添加到 docker 组
sudo usermod -aG docker $USER

# 重新登录或刷新组
newgrp docker

# 或临时使用 sudo
sudo docker compose up -d
```

---

## 🚀 启动问题

### ❌ 端口已被占用

**错误信息**：
```
Error: bind: address already in use
```

**解决方案**：

#### Windows
```powershell
# 查找占用端口的进程
netstat -ano | findstr :8817

# 终止进程
taskkill /PID <PID> /F
```

#### Linux
```bash
# 查找并终止
sudo lsof -ti:8817 | xargs sudo kill -9

# 或修改 .env 文件使用其他端口
```

---

### ❌ 容器启动后立即退出

**解决方案**：

```bash
# 查看容器日志
docker compose logs agpay-manager-api

# 查看最近的日志
docker compose logs --tail=100 -f agpay-manager-api

# 检查容器状态
docker compose ps -a
```

---

## 💾 数据库问题

### ❌ MySQL 连接失败

**错误信息**：
```
Can't connect to MySQL server
```

**解决方案**：

```bash
# 1. 检查 MySQL 容器是否运行
docker compose ps mysql

# 2. 查看 MySQL 日志
docker compose logs mysql

# 3. 检查 .env 配置
cat .env | grep MYSQL

# 4. 进入 MySQL 容器测试
docker exec -it mysql mysql -u root -p
```

---

### ❌ 数据库初始化失败

**解决方案**：

```bash
# 删除数据卷重新初始化
docker compose down -v
docker volume rm agpayplus_mysql-data

# 重新部署
./deploy.sh

```

---

## 🔐 证书问题

### ❌ HTTPS 证书错误

**错误信息**：
```
ERR_CERT_AUTHORITY_INVALID
```

**解决方案**：

#### Windows
```powershell
# 重新生成证书
.\generate-cert-windows.ps1

# 信任证书
# Chrome：高级 → 继续访问
# 或在证书管理器中导入 agpayplusapi.pfx
```

#### Linux
```bash
# 重新生成证书
./generate-cert-linux.sh

# 信任证书
sudo cp ~/.aspnet/https/agpayplusapi.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
```

---

## 🔄 回滚问题

### ❌ 回滚后服务无法启动

**解决方案**：

```bash
# 1. 检查镜像是否存在
docker images

# 2. 查看备份内容
ls -la .backup/

# 3. 手动恢复
docker compose down
docker compose up -d

# 4. 查看日志
docker compose logs -f
```

---

### ❌ 找不到备份

**解决方案**：

```bash
# 检查备份目录
ls -la .backup/

# 如果备份被删除，使用原脚本重新部署
./deploy.sh

```

---

## 📦 更新问题

### ❌ 更新单个服务失败

**解决方案**：

```bash
# 1. 使用更新脚本
./update.sh --services "agpay-manager-api"

# 2. 如果自动回滚失败，手动回滚
./rollback.sh

# 3. 查看日志排查问题
docker compose logs agpay-manager-api

# 4. 修复问题后重新更新
```

---

## 💻 系统资源问题

### ❌ 内存不足

**错误信息**：
```
Cannot allocate memory
```

**解决方案**：

```bash
# 清理未使用的资源
docker system prune -a --volumes

# 检查资源使用
docker stats

# 调整 Docker 内存限制（Docker Desktop）
# Settings → Resources → Memory: 4GB+
```

---

### ❌ 磁盘空间不足

**解决方案**：

```bash
# 查看磁盘使用
df -h
docker system df

# 清理
docker system prune -a --volumes -f

# 清理旧备份
rm -rf .backup/*

# 清理构建缓存
docker builder prune -a -f
```

---

## 🔧 配置问题

### ❌ 环境变量未生效

**解决方案**：

```bash
# 1. 检查 .env 文件
cat .env

# 2. 重新加载配置
docker compose down
docker compose up -d

# 3. 验证环境变量
docker exec agpay-manager-api env | grep MYSQL
```

---

### ❌ 服务间无法通信

**解决方案**：

```bash
# 检查网络
docker network ls
docker network inspect agpayplus_default

# 测试服务连接
docker exec agpay-manager-api ping mysql
docker exec agpay-manager-api nc -zv mysql 3306

# 重建网络
docker compose down
docker network prune
docker compose up -d
```

---

## 🐛 调试技巧

### 查看所有容器状态

```bash
docker compose ps -a
```

### 实时查看日志

```bash
# 所有服务
docker compose logs -f

# 特定服务
docker compose logs -f agpay-manager-api

# 最近100行
docker compose logs --tail=100 -f
```

### 进入容器调试

```bash
# 进入容器 Shell
docker exec -it agpay-manager-api /bin/bash

# 执行命令
docker exec agpay-manager-api ls -la /app
```

### 检查资源使用

```bash
# 实时监控
docker stats

# 查看详细信息
docker inspect agpay-manager-api
```

---

## 📞 获取帮助

### 收集诊断信息

```bash
# 系统信息
docker version
docker compose version
docker info

# 容器状态
docker compose ps -a

# 最近日志
docker compose logs --tail=200 > logs.txt

# 配置文件
cat .env > config.txt
cat docker-compose.yml >> config.txt
```

### 报告问题

提供以下信息：
1. 操作系统和版本
2. Docker 版本
3. 错误信息完整输出
4. 相关日志
5. 操作步骤

---

## 🔗 相关文档

- [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) - 快速参考手册
- [DEPLOYMENT_USAGE_GUIDE.md](./DEPLOYMENT_USAGE_GUIDE.md) - 完整使用指南
- [README_DEPLOYMENT.md](./README_DEPLOYMENT.md) - 部署方案说明
- [DOCKER_MIRROR_GUIDE.md](./DOCKER_MIRROR_GUIDE.md) - Docker 镜像源配置
- [MYSQL_MIGRATION.md](./MYSQL_MIGRATION.md) - MySQL 迁移指南

---

**提示**：大多数问题可以通过查看日志快速定位：`docker compose logs -f`
