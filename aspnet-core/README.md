# AgPay Plus

AgPay Plus 是一套适合互联网企业使用的支付系统，支持多渠道服务商和普通商户模式。已对接微信支付，支付宝，云闪付官方接口，支持聚合码支付。

AgPay Plus 基于 .NET 9，使用 Web API 开发，实现权限管理功能，是一套非常实用的Web开发框架。

## 前端项目
- 前端项目地址：https://github.com/agoodays/agpayplus/tree/main/ant-design-vue

## 主要功能

- **多渠道支付**：支持微信支付、支付宝、云闪付等多种支付渠道
- **聚合码支付**：生成聚合二维码，支持多种支付方式扫码
- **商户管理**：支持普通商户和服务商模式
- **订单管理**：完整的订单生命周期管理
- **退款管理**：支持订单退款功能
- **转账管理**：支持商户转账功能
- **分账管理**：支持订单分账功能
- **权限管理**：基于角色的权限控制系统
- **数据统计**：交易数据统计和分析
- **监控告警**：集成 Prometheus 监控
- **容器化部署**：支持 Docker 容器化部署

## 技术栈

- **后端**：.NET 9, C#, ASP.NET Core Web API
- **数据库**：MySQL
- **缓存**：Redis
- **消息队列**：RabbitMQ
- **认证**：JWT
- **监控**：Prometheus
- **容器化**：Docker, Docker Compose
- **前端**：Vue 3, Ant Design Vue

## 解决方案目录结构

```lua
AGooday.AgPay
├── src
    ├── 1.Presentation Layer
        ├── AGooday.AgPay.Agent.Api        # 代理端API
        ├── AGooday.AgPay.Manager.Api      # 管理端API
        ├── AGooday.AgPay.Merchant.Api     # 商户端API
        └── AGooday.AgPay.Payment.Api      # 支付网关API
    ├── 2.Application Layer(Service)
        └── AGooday.AgPay.Application      # 应用服务层
    ├── 3.Domain Layer(Domain)
        ├── AGooday.AgPay.Domain           # 领域层
        └── AGooday.AgPay.Domain.Core      # 领域核心
    ├── 4.Infrastructure Layer
        └── AGooday.AgPay.Infrastructure   # 基础设施层
    └── 5.Project.Common Layer
        ├── AGooday.AgPay.AopSdk           # 支付SDK
        ├── AGooday.AgPay.Common           # 通用工具
        ├── AGooday.AgPay.Components.Cache # 缓存组件
        ├── AGooday.AgPay.Components.MQ    # 消息队列组件
        ├── AGooday.AgPay.Components.OCR   # OCR组件
        ├── AGooday.AgPay.Components.OSS   # 对象存储组件
        ├── AGooday.AgPay.Components.SMS   # 短信组件
        ├── AGooday.AgPay.Components.Third # 第三方支付组件
        ├── AGooday.AgPay.Notice.Core      # 通知核心
        ├── AGooday.AgPay.Notice.Email     # 邮件通知
        ├── AGooday.AgPay.Notice.Sms       # 短信通知
        └── AGooday.AgPay.Logging.Serilog  # 日志组件
├── test
    ├── AGooday.AgPay.AopSdk.UnitTests     # SDK单元测试
    ├── AGooday.AgPay.Common.UnitTests     # 通用工具单元测试
    ├── AGooday.AgPay.Infrastructure.UnitTests # 基础设施单元测试
    └── AGooday.AgPay.Notice.UnitTests     # 通知单元测试
├── docs
    ├── rabbitmq_plugin                    # RabbitMQ插件
    └── sql                                # SQL脚本
└── README.md                              # 项目说明文档
```

## 快速开始

### 环境要求

- .NET 9 SDK
- MySQL 8.0+
- Redis 6.0+
- RabbitMQ 3.8+
- Docker (可选，用于容器化部署)

### 安装步骤

1. **克隆项目**

```bash
git clone https://github.com/agoodays/agpayplus.git
cd agpayplus/aspnet-core
```

2. **创建数据库**

执行 `docs/sql/agpayplusinit.sql` 脚本创建数据库和初始化数据。

3. **配置环境变量**

修改各API项目的 `appsettings.json` 文件，配置数据库连接、Redis、RabbitMQ等信息。

4. **构建项目**

```bash
# 使用PowerShell执行
./build.ps1

# 或直接使用dotnet命令
dotnet build
```

5. **启动服务**

```bash
# 使用PowerShell执行
./start-services.ps1

# 或直接使用dotnet命令启动各个服务
dotnet run --project src/AGooday.AgPay.Manager.Api
dotnet run --project src/AGooday.AgPay.Merchant.Api
dotnet run --project src/AGooday.AgPay.Payment.Api
dotnet run --project src/AGooday.AgPay.Agent.Api
```

6. **访问服务**

- 管理端API: http://localhost:5817
- 商户端API: http://localhost:5818
- 支付网关API: http://localhost:5819
- 代理端API: http://localhost:5816

### 容器化部署

1. **构建Docker镜像**

```bash
docker build -t agpayplus:latest .
```

2. **使用Docker Compose启动**

```bash
docker-compose up -d
```

3. **启动监控服务**

```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

## 监控系统

AgPay Plus 集成了 Prometheus 监控系统，通过 `/metrics` 端点暴露指标：

- 管理端: http://localhost:5817/metrics
- 商户端: http://localhost:5818/metrics
- 支付网关: http://localhost:5819/metrics
- 代理端: http://localhost:5816/metrics

同时提供了 Grafana 仪表板，位于 `grafana/dashboards/agpay-dashboard.json`。

## API文档

各API服务启动后，可以通过 Swagger 查看API文档：

- 管理端API: http://localhost:5817/swagger
- 商户端API: http://localhost:5818/swagger
- 支付网关API: http://localhost:5819/swagger
- 代理端API: http://localhost:5816/swagger

## 贡献指南

1. **Fork 项目**
2. **创建功能分支**
3. **提交代码**
4. **创建 Pull Request**

## 系统架构

AgPay Plus 采用分层架构设计，遵循领域驱动设计 (DDD) 原则：

- **表示层**：处理HTTP请求和响应，包含API控制器和中间件
- **应用层**：实现业务逻辑，协调领域对象完成业务操作
- **领域层**：核心业务逻辑和领域模型
- **基础设施层**：提供技术支持，如数据库访问、缓存、消息队列等
- **公共层**：通用组件和工具类

## 安全特性

- **JWT认证**：基于JSON Web Token的身份验证
- **刷新令牌**：支持令牌刷新机制
- **令牌黑名单**：实现令牌撤销功能
- **HTTPS支持**：默认启用HTTPS
- **输入验证**：请求参数验证
- **防SQL注入**：使用参数化查询
- **防XSS攻击**：输出编码
- **CORS配置**：跨域资源共享设置

## 性能优化

- **Redis缓存**：缓存热点数据和会话信息
- **数据库索引**：优化查询性能
- **异步处理**：使用异步方法提高并发能力
- **消息队列**：解耦异步任务
- **连接池**：数据库连接池管理
- **内存优化**：减少内存使用

## API版本控制

AgPay Plus 使用URL路径进行API版本控制，例如：

```
/api/v1/merchants
/api/v2/transactions
```

## 错误处理

系统实现了统一的错误处理机制：

- **全局异常处理**：捕获并处理未处理的异常
- **错误码系统**：定义标准化的错误码
- **错误响应格式**：统一的错误响应结构
- **日志记录**：详细的错误日志

## 最佳实践

- **代码风格**：遵循C#编码规范
- **命名约定**：统一的命名规范
- **注释规范**：详细的代码注释
- **单元测试**：关键功能的单元测试
- **集成测试**：系统集成测试
- **代码审查**：代码审查流程

## 常见问题排查

### 服务启动失败
- 检查数据库连接配置
- 检查Redis和RabbitMQ连接
- 查看应用日志

### 支付失败
- 检查支付渠道配置
- 检查商户信息和权限
- 查看支付日志

### 性能问题
- 检查数据库索引
- 监控Redis缓存命中率
- 分析请求响应时间

### 安全问题
- 定期更新依赖包
- 检查JWT配置
- 审查权限设置

## 许可证

MIT License

## 联系我们

- 项目地址: https://github.com/agoodays/agpayplus
- 问题反馈: https://github.com/agoodays/agpayplus/issues
