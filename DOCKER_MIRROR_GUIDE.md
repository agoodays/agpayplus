# Docker 镜像源配置指南

## 问题说明

构建时出现错误：
```
failed to do request: Get "https://mcr.microsoft.com/..."
dial tcp 150.171.69.10:443: connectex: A connection attempt failed
```

**原因**：无法访问 Microsoft Container Registry (mcr.microsoft.com)

---

## 解决方案

### 方案一：配置 Docker 使用国内镜像（推荐）✅

#### Windows (Docker Desktop)

1. 打开 Docker Desktop
2. 点击 **Settings** (设置)
3. 选择 **Docker Engine**
4. 添加以下配置：

```json
{
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live",
    "https://docker.awsl9527.cn",
    "https://dockerpull.org",
    "https://dockerhub.icu"
  ],
  "insecure-registries": [],
  "debug": false,
  "experimental": false
}
```

5. 点击 **Apply & Restart**

#### Linux

编辑或创建 `/etc/docker/daemon.json`：

```bash
sudo nano /etc/docker/daemon.json
```

添加：
```json
{
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live",
    "https://docker.awsl9527.cn",
    "https://dockerpull.org",
    "https://dockerhub.icu"
  ]
}
```

重启 Docker：
```bash
sudo systemctl daemon-reload
sudo systemctl restart docker
```

#### macOS (Docker Desktop)

与 Windows 相同，在 Docker Desktop 中配置。

---

### 方案二：使用代理

#### Windows

```powershell
# 设置 Docker Desktop 代理
# Settings -> Resources -> Proxies
# 填入代理地址，例如：
# HTTP Proxy:  http://127.0.0.1:7890
# HTTPS Proxy: http://127.0.0.1:7890
```

#### Linux

```bash
# 创建 Docker 服务配置目录
sudo mkdir -p /etc/systemd/system/docker.service.d

# 创建代理配置文件
sudo nano /etc/systemd/system/docker.service.d/http-proxy.conf
```

添加：
```ini
[Service]
Environment="HTTP_PROXY=http://127.0.0.1:7890"
Environment="HTTPS_PROXY=http://127.0.0.1:7890"
Environment="NO_PROXY=localhost,127.0.0.1"
```

重启 Docker：
```bash
sudo systemctl daemon-reload
sudo systemctl restart docker
```

---

### 方案三：修改 Dockerfile 使用阿里云镜像

如果以上方法都不行，可以直接修改 Dockerfile：

#### 创建 `Dockerfile.cn`（中国版）

```dockerfile
# 使用阿里云镜像
FROM registry.cn-hangzhou.aliyuncs.com/dotnet/aspnet:9.0 AS base
ENV LANG=C.UTF-8 \
    LC_ALL=C.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Asia/Shanghai

RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

USER app
WORKDIR /app
EXPOSE 5816
EXPOSE 9816
ENV ASPNETCORE_URLS=http://+:5816;https://+:9816

# 构建阶段也使用阿里云镜像
FROM registry.cn-hangzhou.aliyuncs.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ... 其余不变 ...
```

然后修改 `docker-compose.yml`：

```yaml
services:
  agpay-agent-api:
    build:
      context: ./aspnet-core/src
      dockerfile: AGooday.AgPay.Agent.Api/Dockerfile.cn  # 使用中国版 Dockerfile
```

---

### 方案四：预先拉取镜像

```bash
# 手动拉取基础镜像（使用代理或镜像源）
docker pull mcr.microsoft.com/dotnet/aspnet:9.0
docker pull mcr.microsoft.com/dotnet/sdk:9.0

# 或使用国内镜像
docker pull registry.cn-hangzhou.aliyuncs.com/dotnet/aspnet:9.0
docker pull registry.cn-hangzhou.aliyuncs.com/dotnet/sdk:9.0

# 然后重新构建
docker compose build
```

---

## 验证配置

### 测试镜像源是否生效

```bash
# Windows/Linux/macOS
docker info | grep -A 10 "Registry Mirrors"

# 应该看到配置的镜像地址
```

### 测试拉取速度

```bash
# 拉取测试镜像
docker pull hello-world

# 查看拉取速度
docker pull nginx:latest
```

---

## 常用国内镜像源

### Docker Hub 镜像

| 提供商 | 镜像地址 | 说明 |
|--------|---------|------|
| DaoCloud | `https://docker.m.daocloud.io` | 推荐 |
| 1Panel | `https://docker.1panel.live` | 稳定 |
| AWSL | `https://docker.awsl9527.cn` | 快速 |
| DockerPull | `https://dockerpull.org` | 备用 |
| DockerHub ICU | `https://dockerhub.icu` | 备用 |

### Microsoft 镜像

| 提供商 | 镜像地址 | 说明 |
|--------|---------|------|
| 阿里云 | `registry.cn-hangzhou.aliyuncs.com/dotnet/` | 官方合作 |
| 腾讯云 | `ccr.ccs.tencentyun.com/dotnet/` | 官方合作 |

---

## 完整部署流程（使用镜像源）

### Windows

```powershell
# 1. 配置 Docker 镜像源（在 Docker Desktop 中）

# 2. 重启 Docker Desktop

# 3. 验证配置
docker info

# 4. 重新部署
.\deploy.ps1
```

### Linux

```bash
# 1. 配置 Docker 镜像源
sudo nano /etc/docker/daemon.json

# 2. 重启 Docker
sudo systemctl restart docker

# 3. 验证配置
docker info | grep -A 10 "Registry Mirrors"

# 4. 重新部署
./deploy.sh
```

---

## 故障排查

### Q1: 镜像源配置后还是很慢？

**A**: 尝试多个镜像源，选择最快的：

```bash
# 测试速度
time docker pull hello-world
```

### Q2: 提示 "unauthorized: authentication required"？

**A**: 某些镜像需要认证，使用原始镜像或切换到其他源。

### Q3: 构建时卡在某个步骤不动？

**A**: 可能是网络超时，增加超时时间：

```bash
# Linux
export DOCKER_CLIENT_TIMEOUT=300
export COMPOSE_HTTP_TIMEOUT=300

# 然后重新构建
docker compose build --no-cache
```

### Q4: Docker Desktop 无法启动？

**A**: 检查 `daemon.json` 格式是否正确（必须是有效的 JSON）。

---

## 推荐配置（中国大陆用户）

### Docker Desktop (Windows/macOS)

```json
{
  "builder": {
    "gc": {
      "defaultKeepStorage": "20GB",
      "enabled": true
    }
  },
  "experimental": false,
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live",
    "https://docker.awsl9527.cn"
  ]
}
```

### Linux

```json
{
  "registry-mirrors": [
    "https://docker.m.daocloud.io",
    "https://docker.1panel.live",
    "https://docker.awsl9527.cn"
  ],
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "10m",
    "max-file": "3"
  }
}
```

---

## 相关资源

- [Docker 官方文档 - 镜像加速器](https://docs.docker.com/registry/recipes/mirror/)
- [阿里云容器镜像服务](https://cr.console.aliyun.com/)
- [DaoCloud 镜像站](https://github.com/DaoCloud/public-image-mirror)

---

**提示**：配置完成后，记得重启 Docker 服务使配置生效！
