# ========================================
# AgPay+ Agent UI Dockerfile
# ========================================
# 多阶段构建：前端构建 + Nginx 部署
# ========================================
# 使用方法：
#
# 推荐方式 1：使用 docker-compose（自动处理 context）
#   docker compose build agpay-ui-agent
#
# 推荐方式 2：使用部署脚本
#   .\deploy.ps1 -Environment dev          # Windows
#   ./deploy.sh -e dev                     # Linux
#
# 直接使用 docker build（context 是 ant-design-vue 目录）：
#   cd <项目根目录>
#   docker build -f ant-design-vue/agpay-ui-agent/Dockerfile -t agpay-ui-agent ./ant-design-vue
# ========================================

# 构建阶段
# 使用 Node 16 镜像构建
FROM node:16-alpine AS build-stage

WORKDIR /app

# 设置 npm 镜像和超时时间
RUN npm config set registry https://registry.npmmirror.com && \
    npm config set timeout 120000 && \
    npm config set fetch-timeout 120000 && \
    npm config set fetch-retries 3

# 复制 package.json 和 package-lock.json
COPY agpay-ui-agent/package*.json ./

# 安装依赖
RUN npm install

# 复制项目文件
COPY agpay-ui-agent/. .

# 确保 vue-cli-service 是可执行的
RUN chmod +x ./node_modules/.bin/vue-cli-service

# 构建项目
RUN npm run build

# 生产环境阶段
# 使用 Nginx 镜像运行
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# 替换默认的 Nginx 配置文件
COPY agpay-ui-agent/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]