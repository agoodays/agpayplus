# Cashier 部署说明

> 简洁说明：收银台（Cashier）为前端静态站点，已集成到 `payment-api` 服务的 `wwwroot/cashier` 目录中。

---

## 1. 概述

Cashier 是嵌入在 `payment-api` 中的前端收银台页面（SPA）。部署有两种方式：

- 随 `payment-api` 镜像一起打包（推荐，在构建镜像时复制 `dist` 到 `wwwroot/cashier`）。
- 独立构建后将产物复制到宿主机的 `wwwroot/cashier`（用于调试或灰度）。

---

## 2. 构建与集成

### 2.1 使用部署脚本（推荐）

- 默认行为：`BUILD_CASHIER=false`，不会每次重建 cashier，节省时间。
- 强制构建：在部署或更新时使用 `--build-cashier` 或 `-BuildCashier` 参数。

示例：

```bash
# Linux - 首次部署并构建 cashier
./deploy.sh --env production --build-cashier

# Linux - 更新 payment-api 并重建 cashier
./update.sh --services payment-api --build-cashier
```

```powershell
# Windows
.\deploy.ps1 -Environment production -BuildCashier
.\update.ps1 -Services "payment-api" -BuildCashier
```

部署脚本会：
1. 在 ant-design-vue/agpay-ui-cashier 中执行前端构建（`npm ci && npm run build` 或项目指定命令）。
2. 将构建产物复制到 `aspnet-core/src/AGooday.AgPay.Payment.Api/wwwroot/cashier`。
3. 构建 `payment-api` 镜像并启动服务。

### 2.2 手动构建集成（调试）

```bash
# 进入 cashier 前端目录
cd ant-design-vue/agpay-ui-cashier
npm ci
npm run build

# 将 dist 复制到后端 wwwroot
cp -r dist/* ../aspnet-core/src/AGooday.AgPay.Payment.Api/wwwroot/cashier/

# 重新构建 payment-api 镜像（项目根目录）
docker compose build payment-api
docker compose up -d payment-api
```

---

## 3. 配置要点

- 收银台访问路径：`https://<HOST>:9819/cashier`（由 `payment-api` 提供静态资源）。
- 确保 `payment-api` 的静态文件路径正确挂载到容器中（`wwwroot/cashier`）。
- 如果使用 CDN 或独立托管，请修改前端构建时的 `BASE_URL` / `publicPath` 配置。

---

## 4. 测试与验证

1. 访问 `https://<HOST>:9819/cashier`，确认页面加载且静态资源（js/css）返回 200。
2. 在浏览器控制台确认没有跨域或路径错误。
3. 使用 `docker compose logs -f payment-api` 查看启动或静态文件错误日志。

---

## 5. 常见问题

- 页面 404：确认 `wwwroot/cashier/index.html` 存在且容器已重启。
- 静态资源 404：检查前端构建时的 `publicPath` 配置，或检查复制目录是否正确。
- 构建失败：检查前端依赖版本、Node 版本是否满足（建议 Node 16+）。

---

## 6. 参考

- 部署指南：`DEPLOYMENT_USAGE_GUIDE.md`
- 快速指南：`README_DOCKER.md`
- 故障排查：`TROUBLESHOOTING.md`

---

维护者: AgPay+ Team
