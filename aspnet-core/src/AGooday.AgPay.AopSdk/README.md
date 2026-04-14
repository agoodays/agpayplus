# AGooday.AgPay.AopSdk

AgPay Plus SDK 是一个用于集成 AgPay Plus 支付系统的客户端库，提供了便捷的 API 调用方式。

## 接口文档

接口文档：[https://www.yuque.com/xiangyisheng/bhkges/cweewhugp7h7hvml](https://www.yuque.com/xiangyisheng/bhkges/cweewhugp7h7hvml "AgPay接口文档")

## 项目位置

```lua
AGooday.AgPay
├── src
    └── 5.Project.Common Layer
        ├── AGooday.AgPay.AopSdk
└── test
    ├── AGooday.AgPay.AopSdk.UnitTests
```

## 功能特性

- **支付接口**：支持微信支付、支付宝、云闪付等多种支付方式
- **退款接口**：支持订单退款操作
- **转账接口**：支持商户转账功能
- **分账接口**：支持订单分账功能
- **查询接口**：支持订单、退款、转账等查询操作
- **统一错误处理**：标准化的错误码和异常处理
- **签名验证**：自动处理签名和验签

## 快速开始

### 安装

```bash
dotnet add package AGooday.AgPay.AopSdk
```

### 初始化

```csharp
// 创建配置
var config = new AgPayConfig
{
    BaseUrl = "http://localhost:5819", // 支付网关地址
    AppId = "your_app_id",
    AppSecret = "your_app_secret"
};

// 初始化客户端
var client = new AgPayClient(config);
```

### 示例代码

#### 创建支付订单

```csharp
var request = new CreateOrderRequest
{
    MchNo = "your_merchant_no",
    AppId = "your_app_id",
    WayCode = "WX_JSAPI", // 支付方式
    Amount = 100, // 金额（分）
    OrderNo = "order_" + DateTime.Now.Ticks,
    Body = "测试订单",
    NotifyUrl = "https://yourdomain.com/notify",
    ReturnUrl = "https://yourdomain.com/return"
};

var response = await client.CreateOrderAsync(request);
if (response.Code == "000000")
{
    // 订单创建成功
    var orderId = response.Data.OrderId;
    var payInfo = response.Data.PayInfo;
    // 处理支付信息
}
```

#### 查询订单

```csharp
var request = new QueryOrderRequest
{
    MchNo = "your_merchant_no",
    AppId = "your_app_id",
    OrderNo = "your_order_no" // 商户订单号
};

var response = await client.QueryOrderAsync(request);
if (response.Code == "000000")
{
    // 订单查询成功
    var order = response.Data;
    // 处理订单信息
}
```

#### 申请退款

```csharp
var request = new RefundOrderRequest
{
    MchNo = "your_merchant_no",
    AppId = "your_app_id",
    OrderNo = "your_order_no",
    RefundNo = "refund_" + DateTime.Now.Ticks,
    RefundAmount = 100, // 退款金额（分）
    RefundReason = "退款原因"
};

var response = await client.RefundOrderAsync(request);
if (response.Code == "000000")
{
    // 退款申请成功
    var refundId = response.Data.RefundId;
    // 处理退款信息
}
```

## 错误处理

SDK 会自动处理 HTTP 错误和业务错误：

```csharp
try
{
    var response = await client.CreateOrderAsync(request);
    // 处理响应
}
catch (AgPayException ex)
{
    // 处理业务错误
    Console.WriteLine($"错误码: {ex.Code}, 错误信息: {ex.Message}");
}
catch (HttpRequestException ex)
{
    // 处理网络错误
    Console.WriteLine($"网络错误: {ex.Message}");
}
```

## 版本兼容性

- .NET 6.0+
- .NET Standard 2.1+

## 最佳实践

1. **配置管理**：将配置信息存储在安全的地方，避免硬编码
2. **错误处理**：始终捕获并处理可能的异常
3. **日志记录**：记录重要操作和错误信息
4. **重试机制**：对于网络错误，实现适当的重试机制
5. **签名验证**：确保使用安全的签名方式

## 测试

SDK 包含单元测试，位于 `AGooday.AgPay.AopSdk.UnitTests` 项目中。

## 许可证

MIT License
