# AgPay
AgPay是一套适合互联网企业使用的支付系统，支持多渠道服务商和普通商户模式。已对接微信支付，支付宝，云闪付官方接口，支持聚合码支付。
AGPay基于.Net 8，使用 Web API开发，实现权限管理功能，是一套非常实用的web开发框架。

前端项目：https://github.com/agoodays/agpayplus/tree/main/ant-design-vue

> 解决方案目录结构

```lua
AGooday.AgPay
├── src
    ├── 1.Presentation Layer
        ├── AGooday.AgPay.Agent.Api
        ├── AGooday.AgPay.Manager.Api
        ├── AGooday.AgPay.Merchant.Api
        └── AGooday.AgPay.Payment.Api
    ├── 2.Application Layer(Service)
        └── AGooday.AgPay.Application
    ├── 3.Domain Layer(Domain)
        └── AGooday.AgPay.Domain
    ├── 4.Infrastructure Layer
        └── AGooday.AgPay.Domain.Core
    └── 5.Project.Common Layer
        ├── AGooday.AgPay.AopSdk
        ├── AGooday.AgPay.Common
        ├── AGooday.AgPay.Components.MQ
        ├── AGooday.AgPay.Components.OCR
        ├── AGooday.AgPay.Components.OSS
        ├── AGooday.AgPay.Components.SMS
        ├── AGooday.AgPay.Notice.Email
        └── AGooday.AgPay.Notice.Sms
└── test
    ├── AGooday.AgPay.AopSdk.UnitTests
    ├── AGooday.AgPay.Common.UnitTests
    ├── AGooday.AgPay.Infrastructure.UnitTests
    └── AGooday.AgPay.Notice.UnitTests
```