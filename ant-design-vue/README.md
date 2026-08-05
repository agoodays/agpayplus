# AgPay UI（Vue2 遗留项目）

> ⚠️ **本目录为 Vue2 遗留项目，仅做必要维护。新功能请在 [ant-design-vue3/agpay-ui-manager](../ant-design-vue3/agpay-ui-manager) 开发。**

AgPay 对应的前端项目，包括运营平台、代理商系统、商户系统、聚合码收银台。前端技术以 Vue 2 为主，框架使用 Ant Design Vue 开发。

服务端项目：https://github.com/agoodays/agpayplus/tree/main/aspnet-core

## 目录结构

```lua
ant-design-vue
├── agpay-ui-cashier   -- 聚合收银台项目（端口：8819）
├── agpay-ui-manager   -- 运营平台web管理端（端口：8817）
├── agpay-ui-agent     -- 代理商系统web管理端（端口：8816）
└── agpay-ui-merchant  -- 商户系统web管理端（端口：8818）
```

## 参考命令

node 版本要求：`<= 16`

```bash
# 安装依赖
npm install

# 本地启动项目（开发环境）
# 1. 修改 .env.development 中的 VUE_APP_API_BASE_URL
# 2. 执行：
npm run serve

# 打包（生产环境）
# 1. 修改 .env 中的 VUE_APP_API_BASE_URL
# 2. 执行：
npm run build
# 产物输出到 dist 目录，拷贝到 web 服务器即可
```

## Vue3 目标项目

新功能开发请使用 Vue3 项目：[ant-design-vue3/agpay-ui-manager](../ant-design-vue3/agpay-ui-manager)
