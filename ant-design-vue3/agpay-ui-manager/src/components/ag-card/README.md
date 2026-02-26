# AgCard - 卡片容器组件 🎴

## 📝 组件说明

`AgCard` 是一个增强的卡片容器组件，基于 Ant Design Card 封装，提供统一的样式和更便捷的使用方式。

## ✨ 特性

- ✅ **统一样式** - 统一的卡片样式和间距
- ✅ **标题操作** - 支持标题和操作区域
- ✅ **插槽支持** - 支持自定义标题和操作
- ✅ **无缝集成** - 完全兼容 Ant Design Card
- ✅ **悬停效果** - 可选的悬停阴影效果
- ✅ **不同尺寸** - default/small

## 📦 基础用法

```vue
<template>
  <AgCard title="基础卡片">
    <p>这是卡片内容</p>
  </AgCard>
</template>

<script setup>
import { AgCard } from '@/components'
</script>
```

## 🔧 Props

继承 Ant Design Card 的所有 Props，常用属性：

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| title | 卡片标题 | String | - |
| bordered | 是否有边框 | Boolean | true |
| hoverable | 鼠标悬停时阴影效果 | Boolean | false |
| size | 卡片尺寸 | 'default' \| 'small' | 'default' |
| loading | 加载状态 | Boolean | false |
| extra | 标题右侧操作 | String \| VNode | - |

## 📤 Slots

| 插槽名 | 说明 |
|--------|------|
| default | 卡片内容 |
| title | 自定义标题 |
| extra | 自定义操作区 |
| cover | 封面图片 |
| actions | 底部操作 |

## 🎨 示例

### 1. 基础用法

```vue
<AgCard title="基础卡片">
  <p>这是一个基础的卡片组件示例。</p>
  <p>卡片可以包含任意内容。</p>
</AgCard>
```

### 2. 带操作的卡片

```vue
<AgCard title="带操作的卡片">
  <template #extra>
    <a-space>
      <a-button size="small">编辑</a-button>
      <a-button size="small" danger>删除</a-button>
    </a-space>
  </template>
  
  <p>这个卡片右上角有操作按钮。</p>
</AgCard>
```

### 3. 无标题卡片

```vue
<AgCard>
  <p><strong>无标题卡片</strong></p>
  <p>这是一个没有标题的卡片。</p>
</AgCard>
```

### 4. 悬停效果

```vue
<AgCard title="悬停卡片" hoverable>
  <p>鼠标悬停时会有阴影效果。</p>
</AgCard>
```

### 5. 小尺寸卡片

```vue
<AgCard title="小卡片" size="small">
  <p>这是一个小尺寸的卡片。</p>
</AgCard>
```

### 6. 无边框卡片

```vue
<AgCard title="无边框卡片" :bordered="false">
  <p>这个卡片没有边框。</p>
</AgCard>
```

### 7. 加载状态

```vue
<AgCard title="加载中" :loading="loading">
  <p>卡片内容...</p>
</AgCard>
```

### 8. 封面图片

```vue
<AgCard hoverable>
  <template #cover>
    <img
      alt="example"
      src="https://example.com/image.jpg"
    />
  </template>
  <a-card-meta
    title="商品名称"
    description="商品描述"
  />
</AgCard>
```

### 9. 底部操作

```vue
<AgCard title="操作卡片">
  <template #actions>
    <EditOutlined key="edit" />
    <DeleteOutlined key="delete" />
    <SettingOutlined key="setting" />
  </template>
  <p>卡片内容...</p>
</AgCard>
```

## 💡 使用场景

### 信息展示

```vue
<AgCard title="商户信息">
  <template #extra>
    <a-tag color="green">已认证</a-tag>
  </template>
  
  <a-descriptions :column="2" bordered>
    <a-descriptions-item label="商户名称">
      示例商户有限公司
    </a-descriptions-item>
    <a-descriptions-item label="商户编号">
      MCH202401001
    </a-descriptions-item>
    <a-descriptions-item label="联系电话">
      138-0000-0000
    </a-descriptions-item>
    <a-descriptions-item label="邮箱">
      merchant@example.com
    </a-descriptions-item>
  </a-descriptions>
</AgCard>
```

### 统计卡片

```vue
<AgCard>
  <a-statistic
    title="今日交易"
    :value="1234"
    suffix="笔"
    :value-style="{ color: '#3f8600' }"
  >
    <template #prefix>
      <ArrowUpOutlined />
    </template>
  </a-statistic>
</AgCard>
```

### 表单容器

```vue
<AgCard title="基本信息">
  <a-form :model="form" :label-col="{ span: 6 }">
    <a-form-item label="商户名称">
      <a-input v-model:value="form.name" />
    </a-form-item>
    <a-form-item label="商户类型">
      <a-select v-model:value="form.type" />
    </a-form-item>
  </a-form>
</AgCard>
```

### 嵌套卡片

```vue
<AgCard title="嵌套卡片示例">
  <a-row :gutter="16">
    <a-col :span="8">
      <AgCard title="子卡片 1" size="small">
        <p>这是嵌套在父卡片内的子卡片。</p>
      </AgCard>
    </a-col>
    <a-col :span="8">
      <AgCard title="子卡片 2" size="small">
        <p>可以在卡片内嵌套多个子卡片。</p>
      </AgCard>
    </a-col>
    <a-col :span="8">
      <AgCard title="子卡片 3" size="small">
        <p>适合展示分组信息。</p>
      </AgCard>
    </a-col>
  </a-row>
</AgCard>
```

## 💡 最佳实践

### 1. 合理使用标题和操作区

```vue
<!-- ✅ 好的做法 -->
<AgCard title="用户列表">
  <template #extra>
    <a-button type="primary">添加用户</a-button>
  </template>
  <!-- 内容 -->
</AgCard>
```

### 2. 嵌套卡片使用小尺寸

```vue
<!-- ✅ 好的做法：子卡片使用 size="small" -->
<AgCard title="父卡片">
  <a-row :gutter="16">
    <a-col :span="12">
      <AgCard title="子卡片" size="small">
        内容
      </AgCard>
    </a-col>
  </a-row>
</AgCard>
```

### 3. 统计卡片不需要标题

```vue
<!-- ✅ 好的做法：统计卡片直接放内容 -->
<AgCard>
  <a-statistic title="总销售额" :value="112893" prefix="¥" />
</AgCard>
```

### 4. 列表卡片添加悬停效果

```vue
<!-- ✅ 好的做法：列表项卡片添加 hoverable -->
<a-row :gutter="16">
  <a-col :span="6" v-for="item in list" :key="item.id">
    <AgCard hoverable>
      <a-card-meta :title="item.title" :description="item.desc" />
    </AgCard>
  </a-col>
</a-row>
```

## 🆚 与其他容器对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| **AgCard** | 内容展示 | 轻量级，适合静态内容 |
| AgDrawer | 侧边展示 | 从侧边滑出，不占页面空间 |
| AgModal | 弹窗展示 | 居中弹出，有遮罩层 |

## 🎨 布局示例

### 响应式布局

```vue
<a-row :gutter="[16, 16]">
  <a-col :xs="24" :sm="12" :md="8" :lg="6">
    <AgCard title="卡片1" hoverable>内容</AgCard>
  </a-col>
  <a-col :xs="24" :sm="12" :md="8" :lg="6">
    <AgCard title="卡片2" hoverable>内容</AgCard>
  </a-col>
  <a-col :xs="24" :sm="12" :md="8" :lg="6">
    <AgCard title="卡片3" hoverable>内容</AgCard>
  </a-col>
  <a-col :xs="24" :sm="12" :md="8" :lg="6">
    <AgCard title="卡片4" hoverable>内容</AgCard>
  </a-col>
</a-row>
```

## 📚 相关组件

- [AgDrawer](../ag-drawer/README.md) - 抽屉容器
- [AgModal](../ag-modal/README.md) - 模态框
- [AgTable](../ag-table/README.md) - 数据表格

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgCard 组件已就绪，开始使用吧！
