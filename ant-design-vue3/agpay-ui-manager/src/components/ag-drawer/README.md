# AgDrawer - 抽屉组件

## 📝 组件说明

基于 Ant Design Vue 的 Drawer 组件封装，提供统一的抽屉使用方式。

## ✨ 功能特性

- ✅ 支持 v-model 控制显示隐藏
- ✅ **按比例自动设置大小** ⭐ 新功能
- ✅ 预设尺寸（small/medium/large/xlarge）
- ✅ 自定义比例（0-1 之间的小数）
- ✅ 固定宽度（px 或百分比）
- ✅ 自定义标题和底部
- ✅ 支持确认加载状态
- ✅ 关闭时自动销毁

## 📏 宽度设置方式对比

| 方式 | 属性 | 示例 | 说明 | 推荐场景 |
|-----|------|------|------|----------|
| **预设尺寸** ⭐ | size | `size="large"` | 快速、响应式、易用 | 大多数场景（推荐） |
| **自定义比例** | widthRatio | `:widthRatio="0.6"` | 灵活、响应式 | 需要精确控制比例 |
| **固定宽度** | width | `:width="900"` 或 `width="60%"` | 精确控制 | 特定宽度需求 |

### 预设尺寸说明

| 尺寸 | 值 | 屏幕占比 | 适用场景 |
|-----|---|----------|----------|
| `small` | 30% | 30% | 简单信息展示、快速查看 |
| `medium` | 50% | 50% | 一般详情展示、标准表单 |
| `large` | 70% | 70% | 复杂内容、多字段表单 |
| `xlarge` | 90% | 90% | 大量信息、宽表格展示 |

## 🎯 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|-------|
| open | 是否显示（支持 v-model） | boolean | false |
| title | 标题 | string | '详情' |
| width | 宽度（px 或百分比字符串） | string \| number | 720 |
| widthRatio | 按屏幕比例设置宽度（0-1之间）| number | 0 |
| size | 预设尺寸（small/medium/large/xlarge） | string | '' |
| closable | 是否显示关闭按钮 | boolean | true |
| destroyOnClose | 关闭时销毁子元素 | boolean | true |
| showFooter | 是否显示底部 | boolean | true |
| showConfirm | 是否显示确认按钮 | boolean | false |
| confirmText | 确认按钮文字 | string | '确定' |
| cancelText | 取消按钮文字 | string | '关闭' |
| confirmLoading | 确认按钮加载状态 | boolean | false |

### 宽度设置优先级

1. **size** - 预设尺寸（优先级最高）
   - `small`: 30% 屏幕宽度
   - `medium`: 50% 屏幕宽度
   - `large`: 70% 屏幕宽度
   - `xlarge`: 90% 屏幕宽度

2. **widthRatio** - 自定义比例（0-1之间的小数）
   - 例如：0.6 表示 60% 屏幕宽度

3. **width** - 固定宽度（默认使用）
   - 数字：像素值，例如 720 表示 720px
   - 字符串：支持 px 和百分比，例如 '800px' 或 '60%'

## 📤 Events

| 事件名 | 说明 | 参数 |
|-------|------|------|
| update:open | open 变化时触发 | (open: boolean) |
| close | 关闭时触发 | - |
| confirm | 点击确认按钮时触发 | - |

## 🔧 Slots

| 插槽名 | 说明 |
|-------|------|
| default | 抽屉内容 |
| footer | 自定义底部内容 |

## 🔧 Methods

| 方法名 | 说明 | 参数 |
|-------|------|------|
| close | 关闭抽屉 | - |

## 💡 使用示例

### 基础用法

```vue
<template>
  <div>
    <a-button @click="open = true">打开抽屉</a-button>
    
    <AgDrawer
      v-model:open="open"
      title="用户详情"
    >
      <p>这里是抽屉内容</p>
    </AgDrawer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AgDrawer from '@/components/ag-drawer'

const open = ref(false)
</script>
```

### 自定义宽度

```vue
<template>
  <AgDrawer
### 自定义宽度

```vue
<template>
  <!-- 固定宽度 900px -->
  <AgDrawer
    v-model:open="open1"
    title="固定宽度 900px"
    :width="900"
  >
    <p>使用固定像素宽度</p>
  </AgDrawer>

  <!-- 百分比宽度 -->
  <AgDrawer
    v-model:open="open2"
    title="百分比宽度 60%"
    width="60%"
  >
    <p>使用百分比宽度，适应不同屏幕</p>
  </AgDrawer>
</template>
```

### 使用预设尺寸（推荐）⭐

```vue
<template>
  <!-- 小尺寸：30% 屏幕宽度 -->
  <AgDrawer
    v-model:open="open1"
    title="小尺寸抽屉"
    size="small"
  >
    <p>适合简单信息展示</p>
  </AgDrawer>

  <!-- 中等尺寸：50% 屏幕宽度 -->
  <AgDrawer
    v-model:open="open2"
    title="中等尺寸抽屉"
    size="medium"
  >
    <p>适合一般详情展示</p>
  </AgDrawer>

  <!-- 大尺寸：70% 屏幕宽度 -->
  <AgDrawer
    v-model:open="open3"
    title="大尺寸抽屉"
    size="large"
  >
    <p>适合复杂内容展示</p>
  </AgDrawer>

  <!-- 超大尺寸：90% 屏幕宽度 -->
  <AgDrawer
    v-model:open="open4"
    title="超大尺寸抽屉"
    size="xlarge"
  >
    <p>适合大量信息或表格展示</p>
  </AgDrawer>
</template>

<script setup>
import { ref } from 'vue'
import AgDrawer from '@/components/ag-drawer'

const open1 = ref(false)
const open2 = ref(false)
const open3 = ref(false)
const open4 = ref(false)
</script>
```

### 使用自定义比例

```vue
<template>
  <!-- 40% 屏幕宽度 -->
  <AgDrawer
    v-model:open="open1"
    title="40% 宽度"
    :widthRatio="0.4"
  >
    <p>占据 40% 的屏幕宽度</p>
  </AgDrawer>

  <!-- 75% 屏幕宽度 -->
  <AgDrawer
    v-model:open="open2"
    title="75% 宽度"
    :widthRatio="0.75"
  >
    <p>占据 75% 的屏幕宽度</p>
  </AgDrawer>
</template>

<script setup>
import { ref } from 'vue'
import AgDrawer from '@/components/ag-drawer'

const open1 = ref(false)
const open2 = ref(false)
</script>
```

### 响应式宽度示例

```vue
<template>
  <div>
    <a-button @click="showDrawer('small')">小尺寸</a-button>
    <a-button @click="showDrawer('medium')">中等</a-button>
    <a-button @click="showDrawer('large')">大尺寸</a-button>
    
    <AgDrawer
      v-model:open="open"
      :title="`${currentSize} 尺寸抽屉`"
      :size="currentSize"
    >
      <a-descriptions :column="1">
        <a-descriptions-item label="当前尺寸">
          {{ currentSize }}
        </a-descriptions-item>
        <a-descriptions-item label="屏幕占比">
          {{ getSizePercent(currentSize) }}
        </a-descriptions-item>
      </a-descriptions>
    </AgDrawer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AgDrawer from '@/components/ag-drawer'

const open = ref(false)
const currentSize = ref('medium')

function showDrawer(size) {
  currentSize.value = size
  open.value = true
}

function getSizePercent(size) {
  const map = {
    small: '30%',
    medium: '50%',
    large: '70%',
    xlarge: '90%'
  }
  return map[size] || '50%'
}
</script>
```

### 自定义宽度（旧方式）

```vue
<template>
  <AgDrawer
    v-model:open="open"
    title="详细信息"
    :width="900"
  >
    <p>更宽的抽屉</p>
  </AgDrawer>
</template>
```

### 显示确认按钮

```vue
<template>
  <AgDrawer
    v-model:open="open"
    title="编辑信息"
    :showConfirm="true"
    :confirmLoading="loading"
    @confirm="handleConfirm"
  >
    <a-form>
      <!-- 表单内容 -->
    </a-form>
  </AgDrawer>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgDrawer from '@/components/ag-drawer'

const open = ref(false)
const loading = ref(false)

async function handleConfirm() {
  loading.value = true
  try {
    // 提交数据
    await api.submit()
    message.success('提交成功')
    open.value = false
  } finally {
    loading.value = false
  }
}
</script>
```

### 自定义底部

```vue
<template>
  <AgDrawer
    v-model:open="open"
    title="操作"
  >
    <p>内容</p>
    
    <template #footer>
      <a-space>
        <a-button @click="open = false">取消</a-button>
        <a-button type="default" @click="handleSave">保存草稿</a-button>
        <a-button type="primary" @click="handleSubmit">提交</a-button>
      </a-space>
    </template>
  </AgDrawer>
</template>
```

### 无底部

```vue
<template>
  <AgDrawer
    v-model:open="open"
    title="查看详情"
    :showFooter="false"
  >
    <a-descriptions :column="2">
      <a-descriptions-item label="名称">示例</a-descriptions-item>
      <a-descriptions-item label="状态">正常</a-descriptions-item>
    </a-descriptions>
  </AgDrawer>
</template>
```

### 订单详情示例

```vue
<template>
  <div>
    <a-table :dataSource="orders" :columns="columns">
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'action'">
          <a @click="showDetail(record)">查看详情</a>
        </template>
      </template>
    </a-table>

    <AgDrawer
      v-model:open="detailOpen"
      title="订单详情"
      :width="800"
    >
      <a-descriptions
        :column="2"
        bordered
        :labelStyle="{ width: '120px' }"
      >
        <a-descriptions-item label="订单号">
          {{ currentOrder?.orderNo }}
        </a-descriptions-item>
        <a-descriptions-item label="金额">
          ¥{{ currentOrder?.amount }}
        </a-descriptions-item>
        <a-descriptions-item label="状态">
          <a-tag :color="getStatusColor(currentOrder?.status)">
            {{ getStatusText(currentOrder?.status) }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="创建时间">
          {{ currentOrder?.createTime }}
        </a-descriptions-item>
        <a-descriptions-item label="备注" :span="2">
          {{ currentOrder?.remark || '-' }}
        </a-descriptions-item>
      </a-descriptions>
    </AgDrawer>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AgDrawer from '@/components/ag-drawer'

const detailOpen = ref(false)
const currentOrder = ref(null)

const orders = ref([
  { orderNo: 'ORD001', amount: 100, status: 1, createTime: '2024-01-01 10:00:00' }
])

const columns = [
  { title: '订单号', dataIndex: 'orderNo' },
  { title: '金额', dataIndex: 'amount' },
  { title: '操作', key: 'action' }
]

function showDetail(record) {
  currentOrder.value = record
  detailOpen.value = true
}

function getStatusColor(status) {
  const map = { 0: 'default', 1: 'success', 2: 'error' }
  return map[status] || 'default'
}

function getStatusText(status) {
  const map = { 0: '待支付', 1: '已支付', 2: '已取消' }
  return map[status] || '未知'
}
</script>
```

### 使用 ref 调用方法

```vue
<template>
  <AgDrawer
    ref="drawerRef"
    v-model:open="open"
    title="信息"
  >
    <p>内容</p>
  </AgDrawer>
</template>

<script setup>
import { ref } from 'vue'
import AgDrawer from '@/components/ag-drawer'

const drawerRef = ref()
const open = ref(false)

function closeDrawer() {
  drawerRef.value.close()
}
</script>
```

## 📚 完整示例

### 商户详情抽屉

```vue
<template>
  <div class="merchant-page">
    <a-table :dataSource="merchants" :columns="columns">
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'action'">
          <a-space>
            <a @click="showDetail(record)">详情</a>
            <a @click="showEdit(record)">编辑</a>
          </a-space>
        </template>
      </template>
    </a-table>

    <!-- 详情抽屉 -->
    <AgDrawer
      v-model:open="detailOpen"
      title="商户详情"
      :width="800"
    >
      <a-spin :spinning="detailLoading">
        <a-descriptions
          title="基本信息"
          :column="2"
          bordered
        >
          <a-descriptions-item label="商户编号">
            {{ merchantDetail?.merchantNo }}
          </a-descriptions-item>
          <a-descriptions-item label="商户名称">
            {{ merchantDetail?.merchantName }}
          </a-descriptions-item>
          <a-descriptions-item label="商户类型">
            {{ getMerchantTypeText(merchantDetail?.type) }}
          </a-descriptions-item>
          <a-descriptions-item label="状态">
            <a-tag :color="getStatusColor(merchantDetail?.status)">
              {{ getStatusText(merchantDetail?.status) }}
            </a-tag>
          </a-descriptions-item>
          <a-descriptions-item label="联系人">
            {{ merchantDetail?.contactName }}
          </a-descriptions-item>
          <a-descriptions-item label="联系电话">
            {{ merchantDetail?.contactPhone }}
          </a-descriptions-item>
          <a-descriptions-item label="地址" :span="2">
            {{ merchantDetail?.address }}
          </a-descriptions-item>
        </a-descriptions>

        <a-divider />

        <a-descriptions
          title="统计信息"
          :column="3"
          bordered
        >
          <a-descriptions-item label="交易笔数">
            {{ merchantDetail?.orderCount || 0 }}
          </a-descriptions-item>
          <a-descriptions-item label="交易金额">
            ¥{{ merchantDetail?.orderAmount || 0 }}
          </a-descriptions-item>
          <a-descriptions-item label="余额">
            ¥{{ merchantDetail?.balance || 0 }}
          </a-descriptions-item>
        </a-descriptions>
      </a-spin>
    </AgDrawer>

    <!-- 编辑抽屉 -->
    <AgDrawer
      v-model:open="editOpen"
      title="编辑商户"
      :width="720"
      :showConfirm="true"
      :confirmLoading="submitLoading"
      @confirm="handleSubmit"
    >
      <a-form
        ref="formRef"
        :model="formData"
        :rules="rules"
        :label-col="{ span: 6 }"
        :wrapper-col="{ span: 16 }"
      >
        <a-form-item label="商户名称" name="merchantName">
          <a-input v-model:value="formData.merchantName" />
        </a-form-item>

        <a-form-item label="商户类型" name="type">
          <a-select v-model:value="formData.type">
            <a-select-option value="1">企业</a-select-option>
            <a-select-option value="2">个体</a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item label="联系人" name="contactName">
          <a-input v-model:value="formData.contactName" />
        </a-form-item>

        <a-form-item label="联系电话" name="contactPhone">
          <a-input v-model:value="formData.contactPhone" />
        </a-form-item>

        <a-form-item label="地址" name="address">
          <a-textarea
            v-model:value="formData.address"
            :rows="3"
          />
        </a-form-item>
      </a-form>
    </AgDrawer>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import AgDrawer from '@/components/ag-drawer'

const detailOpen = ref(false)
const editOpen = ref(false)
const detailLoading = ref(false)
const submitLoading = ref(false)
const merchantDetail = ref(null)
const formRef = ref()

const merchants = ref([])
const columns = [
  { title: '商户编号', dataIndex: 'merchantNo' },
  { title: '商户名称', dataIndex: 'merchantName' },
  { title: '操作', key: 'action' }
]

const formData = reactive({
  merchantName: '',
  type: undefined,
  contactName: '',
  contactPhone: '',
  address: ''
})

const rules = {
  merchantName: [{ required: true, message: '请输入商户名称' }],
  type: [{ required: true, message: '请选择商户类型' }]
}

async function showDetail(record) {
  detailOpen.value = true
  detailLoading.value = true
  
  try {
    // 加载详情
    const res = await api.getDetail(record.id)
    merchantDetail.value = res.data
  } finally {
    detailLoading.value = false
  }
}

function showEdit(record) {
  Object.assign(formData, record)
  editOpen.value = true
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    
    submitLoading.value = true
    await api.update(formData)
    
    message.success('保存成功')
    editOpen.value = false
  } catch (error) {
    if (!error.errorFields) {
      message.error('保存失败')
    }
  } finally {
    submitLoading.value = false
  }
}

function getMerchantTypeText(type) {
  const map = { 1: '企业', 2: '个体' }
  return map[type] || '-'
}

function getStatusColor(status) {
  const map = { 0: 'default', 1: 'success', 2: 'error' }
  return map[status] || 'default'
}

function getStatusText(status) {
  const map = { 0: '禁用', 1: '正常', 2: '已删除' }
  return map[status] || '未知'
}
</script>
```

## ⚠️ 注意事项

1. **v-model 绑定**
  - 使用 `v-model:open` 控制显示隐藏
  - 不要直接修改 props 的 open

2. **宽度设置优先级** ⭐
   - `size` > `widthRatio` > `width`
   - 建议优先使用 `size` 预设尺寸（响应式，易用）
   - 需要精确比例时使用 `widthRatio`
   - 特殊需求时使用固定 `width`

3. **响应式设计**
   - 使用 `size` 或 `widthRatio` 可以自动适应不同屏幕
   - 推荐使用百分比而非固定像素，提升跨设备体验

4. **destroyOnClose**
   - 默认开启，关闭时销毁子元素
   - 可以避免数据残留问题

5. **确认按钮**
   - 需要时设置 `showConfirm="true"`
   - 监听 `@confirm` 事件处理确认逻辑

6. **加载状态**
   - 使用 `confirmLoading` 显示加载状态
   - 防止重复提交

## 🎯 最佳实践

### 推荐做法 ✅

```vue
<!-- 推荐：使用预设尺寸 -->
<AgDrawer size="large" />

<!-- 推荐：使用自定义比例 -->
<AgDrawer :widthRatio="0.6" />

<!-- 推荐：使用百分比 -->
<AgDrawer width="60%" />
```

### 不推荐做法 ❌

```vue
<!-- 不推荐：固定像素在不同屏幕上体验不一致 -->
<AgDrawer :width="1200" />
```

### 场景选择建议

| 场景 | 推荐方式 | 示例 |
|-----|---------|------|
| 快速开发 | `size` | `size="large"` |
| 精确比例 | `widthRatio` | `:widthRatio="0.65"` |
| 特殊需求 | `width` | `width="60%"` |
| 简单信息 | `size="small"` | 30% 屏幕宽度 |
| 一般详情 | `size="medium"` | 50% 屏幕宽度 |
| 复杂内容 | `size="large"` | 70% 屏幕宽度 |
| 表格数据 | `size="xlarge"` | 90% 屏幕宽度 |

---

**版本**: 1.1.0  
**更新时间**: 2024-01-XX  
**Vue 版本**: Vue 3 + Composition API  
**新增功能**: 按比例自动设置大小（size、widthRatio）⭐
