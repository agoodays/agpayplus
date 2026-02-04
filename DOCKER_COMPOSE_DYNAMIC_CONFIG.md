# AgPay+ 动态环境配置使用指南

> **版本**: 1.0  
> **更新日期**: 2024

---

## 📋 概述

现在 `docker-compose.yml` 已完全动态化，支持：
- ✅ 多环境部署（开发/测试/生产）
- ✅ 多实例并行运行
- ✅ 自定义镜像名称和标签
- ✅ CI/CD 集成
- ✅ 多分支开发

---

## 🎯 环境变量说明

### 核心变量

```sh
# 项目名称（影响容器名、网络名）
COMPOSE_PROJECT_NAME=agpayplus

# 镜像前缀和标签
IMAGE_PREFIX=agpay
IMAGE_TAG=latest
```

### 变量影响范围

| 变量 | 默认值 | 影响范围 |
|------|--------|---------|
| `COMPOSE_PROJECT_NAME` | `agpayplus` | 项目名、容器名、网络名 |
| `IMAGE_PREFIX` | `agpay` | 所有应用镜像名 |
| `IMAGE_TAG` | `latest` | 所有应用镜像标签 |

---

## 🚀 使用场景

### 1. 生产环境（默认）

**配置文件**: `.env`

```sh
COMPOSE_PROJECT_NAME=agpayplus
IMAGE_PREFIX=agpay
IMAGE_TAG=latest
```

**启动**:
```sh
docker compose up -d
```

**容器名称**:
```
agpayplus-ui-manager
agpayplus-manager-api
agpayplus-redis
...
```

**镜像名称**:
```
agpay-ui-manager:latest
agpay-manager-api:latest
...
```

---

### 2. 开发环境

**配置文件**: `.env.dev`

```sh
COMPOSE_PROJECT_NAME=agpayplus-dev
IMAGE_PREFIX=agpay-dev
IMAGE_TAG=dev
```

**启动**:
```sh
docker compose --env-file .env.dev up -d
```

**容器名称**:
```
agpayplus-dev-ui-manager
agpayplus-dev-manager-api
agpayplus-dev-redis
...
```

**镜像名称**:
```
agpay-dev-ui-manager:dev
agpay-dev-manager-api:dev
...
```

---

### 3. 测试环境

**配置文件**: `.env.test`

```sh
COMPOSE_PROJECT_NAME=agpayplus-test
IMAGE_PREFIX=agpay-test
IMAGE_TAG=test
```

**启动**:
```sh
docker compose --env-file .env.test up -d
```

---

### 4. 多实例并行

```sh
# 实例 1
export COMPOSE_PROJECT_NAME=agpayplus-instance1
docker compose up -d

# 实例 2
export COMPOSE_PROJECT_NAME=agpayplus-instance2
docker compose up -d

# 不冲突！
```

---

### 5. 多分支开发

```sh
# feature-payment 分支
export COMPOSE_PROJECT_NAME=agpayplus-feature-payment
export IMAGE_TAG=feature-payment
docker compose up -d

# feature-oauth 分支
export COMPOSE_PROJECT_NAME=agpayplus-feature-oauth
export IMAGE_TAG=feature-oauth
docker compose up -d
```

---

### 6. CI/CD 集成

**GitHub Actions 示例**:
```yaml
name: Deploy

on:
  push:
    branches: [main, develop]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v3

      - name: Set environment
        run: |
          if [ "${{ github.ref }}" == "refs/heads/main" ]; then
            echo "COMPOSE_PROJECT_NAME=agpayplus" >> $GITHUB_ENV
            echo "IMAGE_TAG=latest" >> $GITHUB_ENV
          else
            echo "COMPOSE_PROJECT_NAME=agpayplus-dev" >> $GITHUB_ENV
            echo "IMAGE_TAG=dev" >> $GITHUB_ENV
          fi

      - name: Deploy
        run: |
          docker compose build
          docker compose up -d
```

---

## 📖 常用命令

### 查看当前配置

```sh
# 查看项目名
docker compose config --project-name

# 查看完整配置
docker compose config

# 查看镜像名
docker compose config --images
```

### 管理容器

```sh
# 启动（使用默认 .env）
docker compose up -d

# 启动（指定环境文件）
docker compose --env-file .env.dev up -d

# 查看容器
docker compose ps

# 停止
docker compose down

# 查看日志
docker compose logs -f
```

### 按环境管理

```sh
# 开发环境
docker compose --env-file .env.dev up -d
docker compose --env-file .env.dev ps
docker compose --env-file .env.dev down

# 测试环境
docker compose --env-file .env.test up -d
docker compose --env-file .env.test ps
docker compose --env-file .env.test down
```

### 按标签筛选

```sh
# 查看所有前端服务
docker ps --filter "label=app.component=frontend"

# 查看所有后端服务
docker ps --filter "label=app.component=backend"

# 查看特定项目的容器
docker ps --filter "label=com.docker.compose.project=agpayplus-dev"
```

---

## 🔧 高级用法

### 自定义镜像仓库

```sh
# .env.prod
COMPOSE_PROJECT_NAME=agpayplus
IMAGE_PREFIX=registry.example.com/agpay
IMAGE_TAG=v1.0.0

# 构建后推送
docker compose build
docker compose push
```

### 版本化部署

```sh
# .env.v1
COMPOSE_PROJECT_NAME=agpayplus-v1
IMAGE_TAG=v1.0.0

# .env.v2
COMPOSE_PROJECT_NAME=agpayplus-v2
IMAGE_TAG=v2.0.0

# 蓝绿部署
docker compose --env-file .env.v2 up -d
# 测试通过后
docker compose --env-file .env.v1 down
```

### 按时间戳标记

```sh
# 构建时动态生成标签
export IMAGE_TAG=$(date +%Y%m%d-%H%M%S)
docker compose build
docker compose up -d
```

---

## ⚙️ 脚本集成

### 部署脚本已兼容

现有的部署脚本（`deploy.sh`, `deploy.ps1` 等）已自动支持动态配置，因为它们会读取 `.env` 文件。

**使用示例**:
```sh
# 生产环境（使用默认 .env）
./deploy.sh

# 开发环境
cp .env.development .env
./deploy.sh

# 或直接指定
export $(cat .env.development | xargs)
./deploy.sh
```

---

## 🆘 故障排查

### 问题 1: 容器名称冲突

**症状**:
```
Error: Conflict. The container name "/agpayplus-ui-manager" is already in use
```

**解决**:
```sh
# 使用不同的项目名
export COMPOSE_PROJECT_NAME=agpayplus-test
docker compose up -d
```

---

### 问题 2: 环境变量未生效

**症状**: 容器名仍然是默认值

**解决**:
```sh
# 检查环境变量
echo $COMPOSE_PROJECT_NAME
echo $IMAGE_PREFIX
echo $IMAGE_TAG

# 验证配置
docker compose config

# 确保使用正确的环境文件
docker compose --env-file .env.dev config
```

---

### 问题 3: 镜像名称不匹配

**症状**: 找不到镜像

**解决**:
```sh
# 查看期望的镜像名
docker compose config --images

# 查看实际的镜像
docker images

# 确保镜像名称一致
docker compose build
```

---

## 📝 最佳实践

### 1. 环境隔离

```sh
# 为每个环境创建独立配置
.env          # 生产（不提交到 Git）
.env.example  # 示例（提交到 Git）
.env.dev      # 开发（可选提交）
.env.test     # 测试（可选提交）
```

### 2. 镜像管理

```sh
# 使用语义化版本
IMAGE_TAG=v1.2.3

# 或使用 Git 信息
IMAGE_TAG=$(git rev-parse --short HEAD)

# 或使用日期时间
IMAGE_TAG=$(date +%Y%m%d-%H%M%S)
```

### 3. 数据隔离

```sh
# 不同环境使用不同数据路径
# .env.dev
DATA_PATH_HOST=/opt/agpayplus-dev

# .env.test
DATA_PATH_HOST=/opt/agpayplus-test

# .env
DATA_PATH_HOST=/opt/agpayplus
```

### 4. .gitignore 配置

```
# .gitignore
.env
.env.local
.env.*.local
data/
logs/
```

---

## 🔗 相关文档

- [Docker Compose 文档](https://docs.docker.com/compose/)
- [环境变量优先级](https://docs.docker.com/compose/environment-variables/set-environment-variables/)
- [部署脚本指南](DEPLOYMENT_SCRIPT_GUIDE.md)

---

**维护**: AgPay+ Team  
**许可**: MIT
