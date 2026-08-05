# AgPay 收银台前端（agpay-ui-cashier）

AgPay Plus 聚合收银台前端项目，用于用户支付时的交互页面。

## 技术栈

- Vue 2.x
- Vue CLI
- Ant Design Vue 2.x

## 开发

开发端口：`8819`

```bash
# 安装依赖
npm install

# 本地启动开发环境
npm run serve

# 打包生产环境
npm run build
```

## 环境配置

修改 `.env.development` 中的 API 地址：

```
VUE_APP_API_BASE_URL=https://localhost:9819
```

## 部署

构建产物部署到 `Payment.Api` 项目的 `wwwroot/cashier` 目录，由支付网关统一提供静态资源服务。

部署路径：`aspnet-core/src/AGooday.AgPay.Payment.Api/wwwroot/cashier`

## 说明

- 本项目为收银台界面，用于用户支付时的交互页面
- 服务端项目：https://github.com/agoodays/agpayplus/tree/main/aspnet-core
