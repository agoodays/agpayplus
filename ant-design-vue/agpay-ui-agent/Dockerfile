# Dockerfile in ant-design-vue/agpay-ui-agent/
# 构建阶段
# 使用 Node 16 镜像构建
FROM node:16-alpine AS build-stage
WORKDIR /app
COPY agpay-ui-agent/package*.json ./
RUN npm install
COPY agpay-ui-agent/. .
RUN npm run build

# 生产环境阶段
# 使用 Nginx 镜像运行
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# 替换默认的 Nginx 配置文件
COPY agpay-ui-agent/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]