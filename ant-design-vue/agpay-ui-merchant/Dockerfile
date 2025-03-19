# Dockerfile in ant-design-vue/agpay-ui-merchant/
# 构建阶段
# 使用 Node 16 镜像构建
FROM node:16-alpine AS build-stage

WORKDIR /app

# 设置 npm 镜像和超时时间
RUN npm config set registry https://registry.npmmirror.com && \
    npm config set timeout 60000

# 复制 package.json 和 package-lock.json
COPY agpay-ui-merchant/package*.json ./

# 安装依赖
RUN npm install

# 复制项目文件
COPY agpay-ui-merchant/. .

# 构建项目
RUN npm run build

# 生产环境阶段
# 使用 Nginx 镜像运行
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# 替换默认的 Nginx 配置文件
COPY agpay-ui-merchant/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]