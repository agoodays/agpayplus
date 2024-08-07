# AgPay UI
AgPay对应的前端项目，包括运营平台、代理商系统、商户系统、聚合码收银台。前端技术以Vue为主，框架使用Ant Design Vue开发。

服务端项目：https://github.com/agoodays/agpayplus/tree/main/aspnet-core

> 目录结构

```lua
ant-design-vue
├── agpay-ui-cashier -- 聚合收银台项目
├── agpay-ui-manager -- 运营平台web管理端
├── agpay-ui-agent -- 代理商系统web管理端
└── agpay-ui-merchant -- 商户系统web管理端
```
> 参考命令

node版本要求：`<= 16 `

#### 参考命令

``` bash
# 拉取源码完毕后请先安装依赖, 进入项目根目录命令行执行:

> npm install

# 本地启动项目（开发环境）:

1. 打开根目录下文件".env.development", 修改请求服务器地址"VUE_APP_API_BASE_URL"；

2. 在项目根目录命令行执行:

> npm run serve

# 打包（生产环境）：

1. 打开根目录下文件".env", 修改请求服务器地址"VUE_APP_API_BASE_URL"；

2. 在项目根目录命令行执行：

> npm run build

3. 文件将输出到 [/dist]目录， 拷贝到web服务器即可。
```
