# 收银台（Cashier）部署说明

## 📝 部署方式变更

收银台前端（agpay-ui-cashier）**不再作为独立服务部署**，而是在构建 Payment API 时自动集成。

## 🏭️ 构建流程

### 多阶段构建

Payment API 的 Dockerfile 采用多阶段构建：

1. **阶段 1**：使用 `node:16-alpine` 构建 cashier 前端
   - 安装 npm 依赖
   - 执行 `npm run build` 生成生产构建
   - 输出到 `/cashier/dist`

2. **阶段 2-4**：构建 .NET 后端应用

3. **阶段 5**：最终镜像
   - 复制 .NET 应用
   - 复制 cashier 前端到 `wwwroot/cashier`

### Dockerfile 关键部分

```dockerfile
# 阶段 1: 构建 Cashier 前端
FROM node:16-alpine AS cashier-build
WORKDIR /cashier
COPY ../../ant-design-vue/agpay-ui-cashier/package*.json ./
RUN npm install
COPY ../../ant-design-vue/agpay-ui-cashier/. .
RUN npm run build

# 阶段 5: 最终镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=cashier-build /cashier/dist ./wwwroot/cashier
```

## 🌐 访问方式

### 本地开发环境

- **收银台地址**: `https://localhost:9819/cashier`
- **支付 API**: `https://localhost:9819`

### 生产环境

- **收银台地址**: `https://your-domain.com:9819/cashier`
- **支付 API**: `https://your-domain.com:9819`

## 🔄 更新收银台

要更新收银台前端，需要重新构建 `payment-api` 服务：

### Windows

```powershell
# 更新支付网关（包含收银台）
.\update-windows.ps1 -Services "payment-api"
```

### Linux/macOS

```bash
# 更新支付网关（包含收银台）
./update-linux.sh --services "payment-api"
```

## 📂 文件结构

### 源代码位置

```
agpayplus/
└── ant-design-vue/
    └── agpay-ui-cashier/          # 收银台前端源代码
        ├── src/
        ├── public/
        ├── package.json
        └── vue.config.js
```

### 运行时位置（容器内）

```
/app/
├── AGooday.AgPay.Payment.Api.dll  # Payment API 主程序
└── wwwroot/
    └── cashier/                    # 收银台前端（构建产物）
        ├── index.html
        ├── js/
        ├── css/
        └── assets/
```

## 🔧 配置说明

### ASP.NET Core 静态文件配置

Payment API 需要配置静态文件中间件来提供 cashier 前端：

```csharp
// Program.cs 或 Startup.cs
app.UseStaticFiles(); // 启用静态文件服务

// wwwroot/cashier 下的文件可以通过 /cashier/* 访问
```

### 路由配置

- `/cashier/` → `wwwroot/cashier/index.html`
- `/cashier/assets/*` → `wwwroot/cashier/assets/*`

## ⚙️ 本地开发

### 方式 1：使用 Docker（推荐生产环境配置）

```bash
# 完整部署包含收银台
docker compose up -d payment-api
```

### 方式 2：独立运行（仅用于开发）

如果需要单独开发收银台前端：

```bash
cd ant-design-vue/agpay-ui-cashier
npm install
npm run serve
```

然后修改 API 地址指向本地运行的 Payment API。

### 方式 3：手动构建到 Payment API

```bash
# 1. 构建前端
cd ant-design-vue/agpay-ui-cashier
npm install
npm run build

# 2. 复制到 Payment API wwwroot
cp -r dist/* ../../aspnet-core/src/AGooday.AgPay.Payment.Api/wwwroot/cashier/

# 3. 运行 Payment API
cd ../../aspnet-core/src/AGooday.AgPay.Payment.Api
dotnet run
```

## 🐛 故障排查

### 问题 1：收银台页面 404

**原因**：静态文件未正确复制或路径配置错误

**解决方案**：
```bash
# 检查容器内文件
docker exec agpay-plus-payment-api ls -la /app/wwwroot/cashier

# 如果文件不存在，重新构建
docker compose build --no-cache payment-api
docker compose up -d payment-api
```

### 问题 2：收银台构建失败

**原因**：Node.js 版本不兼容或依赖安装失败

**解决方案**：
```bash
# 检查构建日志
docker compose logs payment-api

# 清理并重建
docker compose down
docker system prune -f
docker compose build --no-cache payment-api
```

### 问题 3：收银台白屏或资源加载失败

**原因**：前端路由配置或 API 地址配置错误

**解决方案**：
1. 检查浏览器控制台错误
2. 确认 `vue.config.js` 中的 `publicPath` 配置为 `/cashier/`
3. 检查前端 API 请求地址是否正确

## 📊 构建时间说明

由于 payment-api 需要同时构建前端和后端，构建时间会比其他 API 服务长：

- **其他 API 服务**：约 2-3 分钟
- **Payment API（含 cashier）**：约 5-7 分钟

首次构建时，Docker 需要下载 Node.js 镜像和安装 npm 依赖，会更长一些。

## 🎯 优势

将 cashier 集成到 Payment API 的优势：

1. ✅ **简化部署**：减少独立容器数量
2. ✅ **统一域名**：前后端使用相同域名，避免跨域问题
3. ✅ **资源优化**：减少容器资源占用
4. ✅ **版本同步**：前后端版本自动保持一致
5. ✅ **简化运维**：只需管理一个服务

## 📝 总结

- ✅ 收银台作为 Payment API 的静态资源
- ✅ 通过 `/cashier` 路径访问
- ✅ 更新 payment-api 会同时更新收银台
- ✅ 不需要在 docker-compose.yml 中配置独立的 ui-cashier 服务
- ✅ 端口 8819 已释放，可用于其他服务

---

**最后更新**: 2024
