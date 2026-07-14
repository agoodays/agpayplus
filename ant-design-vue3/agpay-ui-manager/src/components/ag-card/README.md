# AgCard - 数据卡片列表组件 🎴

## 📌 当前推荐用法（2026）

- 推荐导入：`import { AgCard } from '@/components'`
- 若本文出现 `@/components/ag-xxx` 直引路径，属于历史写法，统一按上方推荐导入替换。
- 统一规范参考：[自定义组件使用指南](../../../CUSTOM_COMPONENTS_USAGE_GUIDE.md)

## 📝 组件说明

`AgCard` 是一个数据卡片列表组件，用于展示分页数据的卡片视图。支持响应式布局、新增卡片、分页等功能。

## ✨ 特性

- ✅ **响应式布局** - 支持不同屏幕尺寸的自适应列数
- ✅ **分页支持** - 内置分页器，支持自定义每页条数
- ✅ **新增卡片** - 可配置的新增入口卡片
- ✅ **数据加载** - 支持异步数据加载和搜索条件
- ✅ **插槽支持** - 自定义卡片内容和操作区域

## 📦 基础用法

```vue
<template>
  <AgCard
    :req-card-list-func="reqCardListFunc"
    :search-data="searchData"
    :span="span"
    :height="360"
    :name="'码牌模版'"
    :add-authority="hasPermission('ENT_DEVICE_QRC_SHELL_ADD')"
    :use-pagination="true"
    :page-size="11"
    @add="addFunc"
  >
    <template #cardContentSlot="{ record }">
      <div class="card-content">
        <img :src="record.imageUrl" />
        <div class="title">{{ record.name }}</div>
      </div>
    </template>
  </AgCard>
</template>

<script setup>
import { AgCard } from '@/components'
import { reactive } from 'vue'

const searchData = reactive({})

const span = reactive({
  xxl: 6,
  xl: 4,
  lg: 4,
  md: 3,
  sm: 2,
  xs: 1
})

async function reqCardListFunc(params) {
  return await api.queryList(params)
}

function addFunc() {
  // 新增逻辑
}
</script>
```

## 🔧 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| span | 响应式列数配置 | Object | `{ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }` |
| height | 卡片高度（像素） | Number | 200 |
| name | 卡片名称（用于新增卡片显示） | String | '' |
| addAuthority | 是否显示新增卡片 | Boolean | false |
| searchData | 搜索条件对象 | Object | `{}` |
| reqCardListFunc | 数据加载函数，必须返回 Promise | Function | required |
| usePagination | 是否使用分页 | Boolean | false |
| pageSize | 每页条数 | Number | 10 |

### span 对象结构

| 键 | 说明 | 类型 |
|-----|------|------|
| xxl | 超大型屏幕（≥1600px）每行列数 | Number |
| xl | 大型屏幕（≥1200px）每行列数 | Number |
| lg | 中型屏幕（≥992px）每行列数 | Number |
| md | 小型屏幕（≥768px）每行列数 | Number |
| sm | 超小屏幕（≥576px）每行列数 | Number |
| xs | 极小屏幕（<576px）每行列数 | Number |

## 📤 Events

| 事件名 | 说明 | 参数 |
|--------|------|------|
| add | 点击新增卡片时触发 | - |
| loadComplete | 数据加载完成时触发 | - |

## 📥 Slots

| 插槽名 | 说明 | 参数 |
|--------|------|------|
| cardContentSlot | 卡片内容插槽 | `{ record }` - 当前记录数据 |
| cardOpSlot | 卡片操作插槽 | `{ record }` - 当前记录数据 |

## 🎨 示例

### 1. 基础用法

```vue
<AgCard
  :req-card-list-func="reqCardListFunc"
  :search-data="searchData"
  :height="300"
  :name="'商品'"
>
  <template #cardContentSlot="{ record }">
    <div class="product-card">
      <img :src="record.coverUrl" />
      <div class="product-name">{{ record.productName }}</div>
      <div class="product-price">¥{{ record.price }}</div>
    </div>
  </template>
</AgCard>
```

### 2. 带分页的卡片列表

```vue
<AgCard
  :req-card-list-func="reqCardListFunc"
  :search-data="searchData"
  :height="360"
  :name="'用户'"
  :add-authority="hasPermission('USER_ADD')"
  :use-pagination="true"
  :page-size="8"
  @add="handleAdd"
>
  <template #cardContentSlot="{ record }">
    <div class="user-card">
      <a-avatar :src="record.avatar" />
      <div class="user-info">
        <div class="user-name">{{ record.username }}</div>
        <div class="user-email">{{ record.email }}</div>
      </div>
    </div>
  </template>
</AgCard>
```

### 3. 带操作的卡片

```vue
<AgCard
  :req-card-list-func="reqCardListFunc"
  :search-data="searchData"
  :height="280"
>
  <template #cardContentSlot="{ record }">
    <div class="content">
      <h3>{{ record.title }}</h3>
      <p>{{ record.description }}</p>
    </div>
  </template>
  <template #cardOpSlot="{ record }">
    <a-button size="small" @click="editFunc(record.id)">编辑</a-button>
    <a-button size="small" danger @click="deleteFunc(record.id)">删除</a-button>
  </template>
</AgCard>
```

### 4. 响应式布局配置

```vue
<AgCard
  :req-card-list-func="reqCardListFunc"
  :span="{ xxl: 8, xl: 6, lg: 4, md: 3, sm: 2, xs: 1 }"
  :height="240"
>
  <template #cardContentSlot="{ record }">
    <div>{{ record.name }}</div>
  </template>
</AgCard>
```

## 💡 使用场景

### 码牌模板管理

```vue
<AgCard
  :req-card-list-func="qrcShellApi.queryCardList"
  :search-data="searchData"
  :span="{ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }"
  :height="360"
  :name="'码牌模版'"
  :add-authority="hasPermission('ENT_DEVICE_QRC_SHELL_ADD')"
  :use-pagination="true"
  :page-size="11"
  @add="addFunc"
>
  <template #cardContentSlot="{ record }">
    <div class="qrc-shell-card">
      <img :src="record.shellImgViewUrl" />
      <div class="shell-name">{{ record.shellAlias }}</div>
      <div class="card-ops">
        <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_EDIT')" type="text" @click="editFunc(record.id)">
          <EditOutlined />
        </a-button>
        <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_DEL')" type="text" danger @click="delFunc(record.id)">
          <DeleteOutlined />
        </a-button>
      </div>
    </div>
  </template>
</AgCard>
```

## 🆚 与其他容器对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| **AgCard** | 数据卡片列表 | 支持分页、响应式布局、动态数据加载 |
| a-card | 静态卡片 | 轻量级，适合静态内容展示 |
| AgTable | 数据表格 | 适合复杂数据展示和操作 |

## 📚 相关组件

- [AgTable](../ag-table/README.md) - 数据表格
- [AgDrawer](../ag-drawer/README.md) - 抽屉容器
- [AgModal](../ag-modal/README.md) - 模态框

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgCard 组件已就绪，开始使用吧！
