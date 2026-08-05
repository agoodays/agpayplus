# AgPayPlus Developer Guide

> 文档版本：v1.1  
> 项目版本：.NET 9 + Vue 3

---

## 目录

1. [项目概览](#1-项目概览)
2. [整体架构设计](#2-整体架构设计)
3. [后端架构详解](#3-后端架构详解)
4. [前端架构详解](#4-前端架构详解)
5. [核心模块职责](#5-核心模块职责)
6. [关键类与函数说明](#6-关键类与函数说明)
7. [依赖关系图](#7-依赖关系图)
8. [项目运行方式](#8-项目运行方式)
9. [部署架构](#9-部署架构)
10. [技术栈清单](#10-技术栈清单)
11. [开发规范](#11-开发规范)
12. [常见问题与解决方案](#12-常见问题与解决方案)

---

## 1. 项目概览

AgPayPlus 是一套适合互联网企业使用的聚合支付平台，基于 .NET 9 + Vue 3 技术栈，支持多渠道服务商和普通商户模式。完整的项目简介、技术栈概览、功能列表与产品截图请参阅 [README.md](README.md)。

---

## 2. 整体架构设计

### 2.1 架构分层

AgPayPlus 采用 **领域驱动设计 (DDD)** 的分层架构：

```
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                     │
│              (API Controllers, Middleware)               │
│    Manager.Api  │  Agent.Api  │  Merchant.Api  │  Payment.Api  │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                  Application Layer                      │
│              (Application Services)                      │
│         业务逻辑协调、事务管理、权限控制                     │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                    Domain Layer                         │
│               (Domain Models, Entities)                  │
│         核心业务逻辑、领域对象、业务规则                     │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│               Infrastructure Layer                      │
│           (Repositories, External Services)             │
│         数据持久化、缓存、消息队列、第三方服务                │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│               Common & Components Layer                 │
│            (Utils, Cache, MQ, OSS, SMS, OCR)           │
│         通用工具类、组件库、跨层基础设施                      │
└─────────────────────────────────────────────────────────┘
```

### 2.2 系统架构图

```
┌──────────────────────────────────────────────────────────────────┐
│                          用户层                                   │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐        │
│  │运营平台   │  │代理商系统│  │商户系统   │  │收银台    │        │
│  │(Manager) │  │(Agent)   │  │(Merchant)│  │(Cashier) │        │
│  └─────┬────┘  └─────┬────┘  └─────┬────┘  └─────┬────┘        │
└────────┼─────────────┼─────────────┼─────────────┼──────────────┘
         │             │             │             │
         ↓             ↓             ↓             ↓
┌──────────────────────────────────────────────────────────────────┐
│                        前端层 (Vue 3)                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  路由层 (Vue Router 4)                                    │  │
│  │  状态管理 (Pinia)                                         │  │
│  │  UI组件 (Ant Design Vue 3)                                │  │
│  │  通用组件库 (ag-table, ag-form, ag-modal...)              │  │
│  └──────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
         │
         ↓ HTTPS
┌──────────────────────────────────────────────────────────────────┐
│                        网关层 (可选)                               │
│              API Gateway (Nginx / Ocelot)                        │
└──────────────────────────────────────────────────────────────────┘
         │
         ↓
┌──────────────────────────────────────────────────────────────────┐
│                        API 层 (.NET 9)                            │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐        │
│  │Manager   │  │Agent     │  │Merchant  │  │Payment   │        │
│  │API:9817  │  │API:9816  │  │API:9818  │  │API:9819  │        │
│  └─────┬────┘  └─────┬────┘  └─────┬────┘  └─────┬────┘        │
└────────┼─────────────┼─────────────┼─────────────┼──────────────┘
         │             │             │             │
         └─────────────┴─────────────┴─────────────┘
                              │
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                    应用服务层 (Application)                       │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  IAccountBillService, IMchInfoService, IOrderService...   │  │
│  └──────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                     领域层 (Domain)                               │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  实体: AccountBill, MchInfo, PayOrder, RefundOrder...     │  │
│  │  值对象: Money, Address...                                │  │
│  │  领域服务: 支付逻辑、分账逻辑、退款逻辑                       │  │
│  └──────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                   基础设施层 (Infrastructure)                     │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐   │
│  │  Repository    │  │  DbContext     │  │  Unit of Work  │   │
│  │  (EF Core)     │  │  (MySQL)       │  │                │   │
│  └────────────────┘  └────────────────┘  └────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│                   数据存储层                                       │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐        │
│  │ MySQL    │  │ Redis    │  │ RabbitMQ │  │ OSS      │        │
│  │ 数据库    │  │ 缓存     │  │ 消息队列  │  │ 对象存储  │        │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘        │
└──────────────────────────────────────────────────────────────────┘
```

---

## 3. 后端架构详解

### 3.1 分层职责

#### 3.1.1 表示层 (Presentation Layer)

**位置**: `aspnet-core/src/AGooday.AgPay.*.Api`

**职责**:
- 处理 HTTP 请求和响应
- API 路由和控制器定义
- 请求参数验证
- 身份认证和授权
- Swagger API 文档生成

**关键项目**:
- `AGooday.AgPay.Manager.Api` - 运营平台 API (端口: 9817)
- `AGooday.AgPay.Agent.Api` - 代理商系统 API (端口: 9816)
- `AGooday.AgPay.Merchant.Api` - 商户系统 API (端口: 9818)
- `AGooday.AgPay.Payment.Api` - 支付网关 API (端口: 9819)

**入口文件**: [Program.cs](aspnet-core/src/AGooday.AgPay.Agent.Api/Program.cs)

```csharp
// 程序入口，配置依赖注入、中间件、CORS、JWT、Swagger等
var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddDbContext<AgPayDbContext>(...);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)...;
builder.Services.AddSwaggerGen(...);

var app = builder.Build();

// 配置中间件
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

#### 3.1.2 应用层 (Application Layer)

**位置**: `aspnet-core/src/AGooday.AgPay.Application`

**职责**:
- 实现业务逻辑协调
- 事务管理
- 权限控制
- 数据转换 (DTO ↔ Entity)
- 调用领域服务和基础设施

**关键接口**:
- [IAccountBillService.cs](aspnet-core/src/AGooday.AgPay.Application/Interfaces/IAccountBillService.cs)
- `IMchInfoService`
- `IPayOrderService`
- `IRefundOrderService`

**服务实现**: [AccountBillService.cs](aspnet-core/src/AGooday.AgPay.Application/Services/AccountBillService.cs)

```csharp
public class AccountBillService : IAccountBillService
{
    private readonly IAccountBillRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountBillService(IAccountBillRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountBill> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }
    
    // 其他业务方法...
}
```

#### 3.1.3 领域层 (Domain Layer)

**位置**: `aspnet-core/src/AGooday.AgPay.Domain` 和 `AGooday.AgPay.Domain.Core`

**职责**:
- 核心业务逻辑
- 领域模型定义
- 业务规则实现
- 领域事件

**关键实体**: [AccountBill.cs](aspnet-core/src/AGooday.AgPay.Domain/Models/AccountBill.cs)

```csharp
public class AccountBill : Entity<string>
{
    public string MchNo { get; set; }
    public string AgentNo { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    // 其他属性...
}
```

#### 3.1.4 基础设施层 (Infrastructure Layer)

**位置**: `aspnet-core/src/AGooday.AgPay.Infrastructure`

**职责**:
- 数据持久化
- 仓储实现
- 数据库上下文
- 外部服务集成

**数据库上下文**: [AgPayDbContext.cs](aspnet-core/src/AGooday.AgPay.Infrastructure/Context/AgPayDbContext.cs)

```csharp
public class AgPayDbContext : DbContext
{
    public DbSet<AccountBill> AccountBills { get; set; }
    public DbSet<MchInfo> MchInfos { get; set; }
    public DbSet<PayOrder> PayOrders { get; set; }
    // 其他 DbSet...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 实体映射配置
        modelBuilder.ApplyConfiguration(new AccountBillConfiguration());
        // 其他配置...
    }
}
```

**仓储实现**: [AccountBillRepository.cs](aspnet-core/src/AGooday.AgPay.Infrastructure/Repositories/AccountBillRepository.cs)

```csharp
public class AccountBillRepository : Repository<AccountBill>, IAccountBillRepository
{
    public AccountBillRepository(AgPayDbContext context) : base(context) { }

    public async Task<AccountBill> GetByMchNoAsync(string mchNo)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.MchNo == mchNo);
    }
}
```

#### 3.1.5 公共层和组件库

**位置**: `aspnet-core/src/AGooday.AgPay.Common` 和 `AGooday.AgPay.Components.*`

**组件列表**:
- `AGooday.AgPay.Common` - 通用工具类 (字符串处理、日期处理、加密解密等)
- `AGooday.AgPay.Components.Cache` - 缓存组件 (Redis 封装)
- `AGooday.AgPay.Components.MQ` - 消息队列组件 (RabbitMQ 封装)
- `AGooday.AgPay.Components.OCR` - OCR 识别组件
- `AGooday.AgPay.Components.OSS` - 对象存储组件 (阿里云 OSS、MinIO 等)
- `AGooday.AgPay.Components.SMS` - 短信组件
- `AGooday.AgPay.Components.Third` - 第三方支付组件

**依赖注入**: [NativeInjectorBootStrapper.cs](aspnet-core/src/AGooday.AgPay.Base.Api/Extensions/NativeInjectorBootStrapper.cs)

```csharp
public static class NativeInjectorBootStrapper
{
    public static void RegisterServices(IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IAccountBillService, AccountBillService>();
        services.AddScoped<IMchInfoService, MchInfoService>();
        
        // Repositories
        services.AddScoped<IAccountBillRepository, AccountBillRepository>();
        services.AddScoped<IMchInfoRepository, MchInfoRepository>();
        
        // Infrastructure Services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IMessageQueueService, RabbitMQService>();
    }
}
```

### 3.2 配置管理

**配置文件**: [appsettings.json](aspnet-core/src/AGooday.AgPay.Agent.Api/appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=agpayplusdb;User=root;Password=123456;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "agpay",
    "Audience": "agpay",
    "ExpirationMinutes": 120
  },
  "Redis": {
    "Host": "localhost",
    "Port": 6379,
    "Password": "",
    "Database": 0
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "admin",
    "Password": "admin"
  },
  "OSS": {
    "Provider": "Aliyun",
    "Endpoint": "oss-cn-hangzhou.aliyuncs.com",
    "AccessKeyId": "your-key",
    "AccessKeySecret": "your-secret"
  }
}
```

---

## 4. 前端架构详解

前端（Vue 3 目标项目 `ant-design-vue3/agpay-ui-manager`，技术栈：Vite + Vue 3 + Pinia + Vue Router + Ant Design Vue）的目录结构、Pinia 状态管理、路由动态生成、API 层封装、通用组件库、主题样式、localStorage key 管理等详细说明，请参阅 [AGENTS.md](AGENTS.md) 第 4 章「Vue3 前端架构」。

Vue2 → Vue3 迁移的关键技术差异与命名规范请参阅 [AGENTS.md](AGENTS.md) 第 9 章「常见坑位」。

---

## 5. 核心模块职责

### 5.1 业务模块

| 模块 | 后端 API | 前端页面 | 核心功能 |
|------|----------|----------|----------|
| **服务商管理** | `/api/isvInfo` | `views/isv/` | 服务商 CRUD、支付配置、费率配置 |
| **代理商管理** | `/api/agentInfo` | `views/agent/` | 代理商 CRUD、分润配置、层级管理 |
| **商户管理** | `/api/mchInfo` | `views/mch/` | 商户 CRUD、应用管理、支付配置 |
| **商户应用** | `/api/mchApp` | `views/mch-app/` | 应用 CRUD、支付接口配置、授权配置 |
| **商户门店** | `/api/mchStore` | `views/mch-store/` | 门店 CRUD、位置管理 |
| **支付配置** | `/api/payInterfaceDefine` | `views/pay-config/` | 支付接口定义、费率配置、通道配置 |
| **订单管理** | `/api/payOrder` | `views/order/` | 支付订单、退款订单、转账订单查询 |
| **分账管理** | `/api/division` | `views/division/` | 分账接收者、分账记录管理 |
| **码牌管理** | `/api/qrc` | `views/qrc/` | 码牌生成、绑定、管理 |
| **数据统计** | `/api/statistics` | `views/statistics/` | 交易统计、渠道统计、商户统计 |
| **账户账单** | `/api/accountBill` | `views/account-bill/` | 账户余额、账单明细 |
| **系统配置** | `/api/sysConfig` | `views/sys/config/` | 系统参数配置、短信配置、OSS 配置 |
| **权限管理** | `/api/sysRole` | `views/sys/role/` | 角色 CRUD、权限分配 |
| **用户管理** | `/api/sysUser` | `views/sys/user/` | 用户 CRUD、团队管理 |

### 5.2 公共模块

| 模块 | 位置 | 功能 |
|------|------|------|
| **缓存模块** | `AGooday.AgPay.Components.Cache` | Redis 缓存封装、分布式锁 |
| **消息队列模块** | `AGooday.AgPay.Components.MQ` | RabbitMQ 封装、延迟消息 |
| **OSS 模块** | `AGooday.AgPay.Components.OSS` | 阿里云 OSS、MinIO 文件存储 |
| **SMS 模块** | `AGooday.AgPay.Components.SMS` | 阿里云短信、腾讯云短信 |
| **OCR 模块** | `AGooday.AgPay.Components.OCR` | 身份证识别、银行卡识别 |
| **第三方支付** | `AGooday.AgPay.Components.Third` | 微信支付、支付宝、云闪付 SDK |

---

## 6. 关键类与函数说明

### 6.1 后端关键类

#### 6.1.1 数据库上下文

**类**: `AgPayDbContext`  
**文件**: [aspnet-core/src/AGooday.AgPay.Infrastructure/Context/AgPayDbContext.cs](aspnet-core/src/AGooday.AgPay.Infrastructure/Context/AgPayDbContext.cs)

**职责**: 管理数据库连接和实体映射

```csharp
public class AgPayDbContext : DbContext
{
    // 实体集
    public DbSet<AccountBill> AccountBills { get; set; }
    public DbSet<AgentInfo> AgentInfos { get; set; }
    public DbSet<MchInfo> MchInfos { get; set; }
    public DbSet<PayOrder> PayOrders { get; set; }
    public DbSet<RefundOrder> RefundOrders { get; set; }
    // 其他实体...
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置实体映射
        modelBuilder.ApplyConfiguration(new AccountBillConfiguration());
        modelBuilder.ApplyConfiguration(new MchInfoConfiguration());
        // 其他配置...
    }
}
```

#### 6.1.2 基础实体类

**类**: `Entity<TKey>`  
**文件**: `aspnet-core/src/AGooday.AgPay.Domain.Core/Models/Entity.cs`

```csharp
public abstract class Entity<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
```

#### 6.1.3 泛型仓储

**接口**: `IRepository<TEntity>`  
**实现**: `Repository<TEntity>`  
**文件**: `aspnet-core/src/AGooday.AgPay.Infrastructure/Repositories/`

```csharp
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(object id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(object id);
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AgPayDbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    
    public Repository(AgPayDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }
    
    public async Task<TEntity> GetByIdAsync(object id)
    {
        return await DbSet.FindAsync(id);
    }
    
    // 其他方法实现...
}
```

#### 6.1.4 应用服务基类

**类**: `ServiceBase`  
**职责**: 提供通用的服务层功能

```csharp
public abstract class ServiceBase
{
    protected readonly IUnitOfWork UnitOfWork;
    
    protected ServiceBase(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }
    
    protected async Task<bool> CommitAsync()
    {
        return await UnitOfWork.CommitAsync();
    }
}
```

### 6.2 前端关键函数

#### 6.2.1 HTTP 请求函数

**对象**: `req` (AgAxios 单例)  
**文件**: [ant-design-vue3/agpay-ui-manager/src/lib/ag-axios.js](ant-design-vue3/agpay-ui-manager/src/lib/ag-axios.js)

**方法**:

| 方法 | 说明 | 示例 |
|------|------|------|
| `req.list(url, params)` | GET 分页列表 | `req.list('/api/mchInfo', { current: 1, size: 10 })` |
| `req.getById(url, id)` | GET 单条记录 | `req.getById('/api/mchInfo', 'M123456')` |
| `req.add(url, data)` | POST 新增 | `req.add('/api/mchInfo', { mchName: 'Test' })` |
| `req.updateById(url, id, data)` | PUT 修改 | `req.updateById('/api/mchInfo', 'M123456', { mchName: 'Updated' })` |
| `req.delById(url, id)` | DELETE 删除 | `req.delById('/api/mchInfo', 'M123456')` |
| `req.export(url, bizType, params)` | 导出数据 | `req.export('/api/export', 'mch', params)` |

#### 6.2.2 组合式函数

**文件**: [ant-design-vue3/agpay-ui-manager/src/composables/useCommon.js](ant-design-vue3/agpay-ui-manager/src/composables/useCommon.js)

**导出函数**:

| 函数 | 说明 |
|------|------|
| `useTable(api, params)` | 表格数据管理 (加载、分页、排序) |
| `useForm(initialValues)` | 表单数据管理 (验证、重置) |
| `useModal()` | 弹窗状态管理 (显示、隐藏) |
| `useDrawer()` | 抽屉状态管理 |
| `useSearch()` | 搜索条件管理 |

**useTable 使用示例**:

```javascript
import { useTable } from '@/composables/useCommon'
import { mchApi } from '@/api/business/mch/mch-api'

export default {
  setup() {
    const { 
      dataSource, 
      loading, 
      pagination, 
      loadData, 
      handleTableChange 
    } = useTable(mchApi.queryPage, {
      current: 1,
      pageSize: 10
    })
    
    onMounted(() => {
      loadData()
    })
    
    return {
      dataSource,
      loading,
      pagination,
      handleTableChange
    }
  }
}
```

#### 6.2.3 工具函数

**文件**: `ant-design-vue3/agpay-ui-manager/src/utils/`

| 文件 | 功能 |
|------|------|
| `format-util.js` | 数据格式化 (金额、日期、手机号) |
| `time-util.js` | 时间处理 (基于 dayjs) |
| `dom-util.js` | DOM 操作工具 |
| `filter.js` | 数据过滤和转换 |
| `theme.js` | 主题切换工具 |
| `local-util.js` | localStorage 工具 |

---

## 7. 依赖关系图

### 7.1 后端依赖关系

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                           │
│  Manager.Api  Agent.Api  Merchant.Api  Payment.Api     │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
                        ↓
┌─────────────────────────────────────────────────────────┐
│                 Application Layer                       │
│              (Application Services)                     │
│  IAccountBillService  IMchInfoService  IOrderService    │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
                        ↓
┌─────────────────────────────────────────────────────────┐
│              Domain Layer + Infrastructure              │
│  Domain.Models  Domain.Core  Infrastructure            │
│  (Entities)      (Events)     (Repositories)           │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
                        ↓
┌─────────────────────────────────────────────────────────┐
│            Common & Components Layer                    │
│  Common  Components.Cache  Components.MQ  Components.* │
└─────────────────────────────────────────────────────────┘
```

### 7.2 前端依赖关系

```
┌─────────────────────────────────────────────────────────┐
│                      Views                              │
│  agent/  mch/  order/  sys/  statistics/               │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
                        ↓
┌─────────────────────────────────────────────────────────┐
│               Components + Composables                  │
│  ag-table  ag-form  ag-modal  useTable  useForm        │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
                        ↓
┌─────────────────────────────────────────────────────────┐
│               API Layer + Store                         │
│  api/business/*  api/system/*  store/modules/*         │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on
                        ↓
┌─────────────────────────────────────────────────────────┐
│           Infrastructure (lib/utils)                    │
│  ag-axios.js  encrypt.js  format-util.js  theme.js     │
└─────────────────────────────────────────────────────────┘
```

### 7.3 第三方依赖

#### 后端 NuGet 包

| 包名 | 版本 | 用途 |
|------|------|------|
| `Microsoft.EntityFrameworkCore` | 9.0 | ORM 框架 |
| `Pomelo.EntityFrameworkCore.MySql` | 9.0 | MySQL 数据库提供程序 |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0 | JWT 认证 |
| `StackExchange.Redis` | 2.7 | Redis 客户端 |
| `RabbitMQ.Client` | 6.8 | RabbitMQ 客户端 |
| `AutoMapper` | 12.0 | 对象映射 |
| `FluentValidation` | 11.0 | 模型验证 |
| `MediatR` | 12.0 | 中介者模式 |
| `Quartz` | 3.8 | 任务调度 |
| `Serilog` | 3.1 | 日志框架 |
| `Swashbuckle.AspNetCore` | 6.5 | Swagger API 文档 |

#### 前端 npm 包

| 包名 | 版本 | 用途 |
|------|------|------|
| `vue` | 3.4 | Vue 3 框架 |
| `vue-router` | 4.2 | 路由管理 |
| `pinia` | 2.1 | 状态管理 |
| `ant-design-vue` | 3.9 | UI 组件库 |
| `axios` | 1.6 | HTTP 客户端 |
| `dayjs` | 1.11 | 日期处理 |
| `@ant-design/icons-vue` | 7.0 | 图标库 |
| `pinia-plugin-persistedstate` | 3.2 | Pinia 持久化插件 |

---

## 8. 项目运行方式

本地开发与部署运行的完整命令请参阅：
- [AGENTS.md](AGENTS.md) 第 3 章「高频命令」— 本地前端 / 后端开发命令
- [DEPLOYMENT.md](DEPLOYMENT.md) — Docker Compose 部署、更新、回滚
- [CHEATSHEET.md](CHEATSHEET.md) — 常用命令速查表

### 8.1 环境要求

| 组件 | 最低版本 | 推荐版本 |
|------|----------|----------|
| .NET SDK | 9.0 | 9.0 |
| Node.js | 16.x | 18.x+ |
| MySQL | 8.0 | 8.0+ |
| Redis | 6.0 | 7.0+ |
| RabbitMQ | 3.13 | 3.13+ |
| Docker | 20.10 | 24.0+ |
| Docker Compose | 2.0 | 2.20+ |

### 8.2 服务访问地址

| 服务 | 地址 | 默认账号 |
|------|------|----------|
| **运营平台** | https://localhost:8817 | agpayadmin / agpay123 |
| **代理商系统** | https://localhost:8816 | - / agpay666 |
| **商户系统** | https://localhost:8818 | - / agpay666 |
| **支付网关 API** | https://localhost:9819 | - |
| **收银台** | https://localhost:9819/cashier | - |
| **Swagger API 文档** | https://localhost:9817/swagger | - |
| **RabbitMQ 管理** | http://localhost:15672 | admin / admin |

---

## 9. 部署架构

Docker Compose 部署架构、服务组成、服务通信、环境配置、SSL 证书生成、更新与回滚等完整内容请参阅 [DEPLOYMENT.md](DEPLOYMENT.md)。环境变量配置说明请参阅 [ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md)。

---

## 10. 技术栈清单

### 10.1 后端技术栈

| 类别 | 技术 | 版本 | 说明 |
|------|------|------|------|
| **框架** | .NET | 9.0 | 核心运行时和 SDK |
| **Web API** | ASP.NET Core | 9.0 | RESTful API 框架 |
| **ORM** | Entity Framework Core | 9.0 | 对象关系映射 |
| **数据库** | MySQL | 8.0+ | 关系型数据库 |
| **缓存** | Redis | 6.0+ | 分布式缓存 |
| **消息队列** | RabbitMQ | 3.13+ | 消息中间件 |
| **认证** | JWT | - | 身份认证 |
| **API 文档** | Swagger/OpenAPI | 6.5 | API 文档生成 |
| **日志** | Serilog | 3.1 | 结构化日志 |
| **任务调度** | Quartz.NET | 3.8 | 定时任务 |
| **对象映射** | AutoMapper | 12.0 | DTO-Entity 映射 |
| **验证** | FluentValidation | 11.0 | 模型验证 |
| **中介者** | MediatR | 12.0 | 解耦组件通信 |
| **监控** | Prometheus | - | 指标收集 |

### 10.2 前端技术栈

#### Vue 3 (目标项目)

| 类别 | 技术 | 版本 | 说明 |
|------|------|------|------|
| **框架** | Vue.js | 3.4 | 渐进式 JavaScript 框架 |
| **构建工具** | Vite | 5.0 | 下一代前端构建工具 |
| **UI 组件库** | Ant Design Vue | 3.9 | 企业级 UI 组件库 |
| **状态管理** | Pinia | 2.1 | Vue 3 官方状态管理 |
| **路由** | Vue Router | 4.2 | Vue.js 官方路由 |
| **HTTP 客户端** | Axios | 1.6 | Promise based HTTP client |
| **图标** | @ant-design/icons-vue | 7.0 | Ant Design 图标库 |
| **日期处理** | dayjs | 1.11 | 轻量级日期库 |
| **国际化** | Vue I18n | 9.8 | 国际化插件 |
| **代码规范** | ESLint + Prettier | - | 代码检查和格式化 |
| **样式** | Less | 4.2 | CSS 预处理器 |

#### Vue 2 (遗留项目)

| 类别 | 技术 | 版本 | 说明 |
|------|------|------|------|
| **框架** | Vue.js | 2.7 | 渐进式 JavaScript 框架 |
| **构建工具** | Webpack | 5.x | 模块打包工具 |
| **UI 组件库** | Ant Design Vue | 1.7 | 企业级 UI 组件库 |
| **状态管理** | Vuex | 3.6 | Vue.js 状态管理模式 |
| **路由** | Vue Router | 3.6 | Vue.js 官方路由 |

### 10.3 开发工具

| 工具 | 说明 |
|------|------|
| **Visual Studio 2022** | .NET 开发 IDE |
| **Visual Studio Code** | 前端开发编辑器 |
| **WebStorm** | JavaScript IDE |
| **SQLyog** | MySQL 客户端 |
| **Docker Desktop** | Docker 容器管理 |
| **Postman** | API 测试工具 |

---

## 11. 开发规范

### 11.1 后端开发规范

#### 11.1.1 命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| **类名** | PascalCase | `MchInfoService` |
| **接口名** | I + PascalCase | `IMchInfoService` |
| **方法名** | PascalCase | `GetByIdAsync` |
| **参数名** | camelCase | `mchNo` |
| **属性名** | PascalCase | `MchName` |
| **常量** | 全大写+下划线 | `MAX_RETRY_COUNT` |

#### 11.1.2 API 路由规范

```
GET    /api/{resource}              # 查询列表
GET    /api/{resource}/{id}         # 查询单条
POST   /api/{resource}              # 新增
PUT    /api/{resource}/{id}         # 修改
DELETE /api/{resource}/{id}         # 删除
```

#### 11.1.3 分层职责

| 层级 | 职责 | 禁止事项 |
|------|------|----------|
| **API 层** | 处理 HTTP 请求/响应、参数验证 | 直接访问数据库、包含业务逻辑 |
| **应用层** | 业务逻辑协调、事务管理 | 直接 SQL 操作 |
| **领域层** | 核心业务逻辑、业务规则 | 依赖基础设施层 |
| **基础设施层** | 数据持久化、外部服务 | 包含业务逻辑 |

### 11.2 前端开发规范

#### 11.2.1 命名规范

| 类型 | Vue 2 (遗留) | Vue 3 (目标) |
|------|-------------|-------------|
| **页面文件** | PascalCase: `AgentList.vue` | kebab-case: `agent-list.vue` |
| **组件文件** | PascalCase: `AgTable.vue` | kebab-case: `ag-table.vue` |
| **组件目录** | PascalCase: `AgTable/` | kebab-case: `ag-table/` |
| **API 文件** | 统一: `manage.js` | kebab-case: `agent-api.js` |
| **composables** | - | camelCase: `useTable.js` |
| **工具文件** | camelCase: `util.js` | kebab-case: `util.js` |

#### 11.2.2 组件规范

**强制使用自定义组件**:
- `ag-table` 替代 `a-table`
- `ag-form` 替代 `a-form`
- `ag-modal` 替代 `a-modal`
- `ag-drawer` 替代 `a-drawer`
- `ag-upload` 替代 `a-upload`

#### 11.2.3 代码规范

- 所有 Vue 3 组件必须使用 `<script setup>` 语法
- CSS 类名必须使用组件特定前缀 (如 `shell-card-wrapper`)
- 空值必须使用 `|| '-'` 处理
- localStorage key 必须从 `constants/local-storage-key-const.js` 导入
- 枚举常量必须集中到 `constants/common-const.js`

### 11.3 Git 提交规范

```
feat: 新功能
fix: 修复 Bug
docs: 文档更新
style: 代码格式调整（不影响功能）
refactor: 代码重构
test: 测试相关
chore: 构建/工具变动
```

**示例**:
```
feat: 新增商户导出功能
fix: 修复订单查询时间范围过滤失效问题
docs: 更新部署文档
refactor: 重构支付服务层代码
```

---

## 12. 常见问题与解决方案

常见问题（后端数据库 / JWT / Redis、前端构建 / 组件 / 表单 / 图标、部署服务 / 端口 / 镜像）的解决方案分散在各专项文档中，请参阅：

- [DEPLOYMENT.md](DEPLOYMENT.md) — 部署与运维故障排查
- [AGENTS.md](AGENTS.md) 第 9 章「常见坑位」— 前端开发常见坑位与已废弃项

---

## 参考文档

### 项目文档

| 文档 | 说明 |
|------|------|
| [README.md](README.md) | 项目总体说明 |
| [DEPLOYMENT.md](DEPLOYMENT.md) | 完整部署与更新指南 |
| [CHEATSHEET.md](CHEATSHEET.md) | 常用命令速查 |
| [ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md) | 环境变量说明 |
| [AGENTS.md](AGENTS.md) | AI 编码智能体指南 |

### 外部文档

- [.NET 9 官方文档](https://docs.microsoft.com/dotnet/)
- [Vue 3 官方文档](https://vuejs.org/)
- [Ant Design Vue 文档](https://www.antdv.com/)
- [Pinia 官方文档](https://pinia.vuejs.org/)
- [Entity Framework Core 文档](https://docs.microsoft.com/ef/core/)
- [Docker 官方文档](https://docs.docker.com/)

---

**文档版本**: v1.1  
**最后更新**: 2026-08-05  
**维护团队**: AgPayPlus Development Team
