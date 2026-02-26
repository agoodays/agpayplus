# AgDateRangePicker - 高级日期范围选择器 📅⭐⭐⭐

## 📝 组件说明

`AgDateRangePicker` 是一个功能强大的日期范围选择器，支持快捷选择、多种 picker 类型、自动日期范围调整、浮动标签等功能。**推荐在新项目中使用。**

## ✨ 核心特性

### 🎯 智能功能
- **快捷选择** - 今天、昨天、近7天、近30天等快捷选项（仅 picker='date' 时可用）
- **自动识别** - 根据初始值自动识别返回字符串还是数组
- **智能格式化** - 根据 picker 类型自动调整日期范围到周期边界

### 📅 多种 Picker 类型
- **date（日报）** - 按日选择，返回 00:00:00 ~ 23:59:59
- **week（周报）** - 按周选择，返回周一 00:00:00 ~ 周日 23:59:59
- **month（月报）** - 按月选择，返回月初 00:00:00 ~ 月末 23:59:59
- **quarter（季报）** - 按季选择，返回季初 00:00:00 ~ 季末 23:59:59
- **year（年报）** - 按年选择，返回年初 00:00:00 ~ 年末 23:59:59

### 🎨 界面特性
- **浮动标签** - Material Design 风格的浮动标签
- **气泡提示** - 悬停显示完整的日期范围
- **自动关闭** - 选择完日期后自动关闭面板
- **多种尺寸** - small/middle/large

---

## 📦 安装使用

```vue
<script setup>
import { AgDateRangePicker } from '@/components'
</script>
```

---

## 🚀 快速开始

### 1. 基础用法 - 快捷选择（推荐）⭐⭐⭐

```vue
<template>
  <AgDateRangePicker
    v-model:value="dateRange"
    label="搜索时间"
  />
  
  <div>当前值: {{ dateRange }}</div>
</template>

<script setup>
import { ref } from 'vue'

const dateRange = ref('')
// 返回值示例：
// - '' (全部时间)
// - 'today' (今天)
// - 'near7' (近7天)
// - 'custom_2024-01-01 00:00:00_2024-01-31 23:59:59' (自定义)
</script>
```

**特点**：
- ✅ 自动识别返回字符串（因为初始值是 `''`）
- ✅ 显示快捷选择下拉框
- ✅ 支持"自定义时间"选项

**适用场景**：搜索条件、筛选功能

---

### 2. 数组格式返回值⭐⭐

```vue
<template>
  <AgDateRangePicker
    v-model:value="dateRange"
    label="统计时间"
  />
  
  <div>{{ dateRange[0] }} ~ {{ dateRange[1] }}</div>
</template>

<script setup>
const dateRange = ref([])
// 返回值示例：['2024-01-01 00:00:00', '2024-01-31 23:59:59']
</script>
```

**特点**：
- ✅ 自动识别返回数组（因为初始值是 `[]`）
- ✅ 选择快捷选项后自动转换为数组格式

**适用场景**：API 传参、数据处理

---

### 3. 直接选择模式（无快捷选择）⭐

```vue
<template>
  <AgDateRangePicker
    v-model:value="dateRange"
    label="活动时间"
    :show-quick-select="false"
  />
</template>

<script setup>
const dateRange = ref('')
</script>
```

**特点**：
- ✅ 界面更简洁，直接显示日期范围选择器
- ✅ 不显示"今天"、"近7天"等快捷选项

**适用场景**：表单填写、需要简洁界面的场景

---

### 4. 周报/月报/季报/年报选择器 ⭐⭐⭐

```vue
<template>
  <div>
    <!-- 报表类型选择 -->
    <AgSelect
      v-model:value="reportType"
      :options="reportTypeOptions"
      label="报表类型"
      @change="handleReportTypeChange"
    />

    <!-- 动态日期选择器 -->
    <AgDateRangePicker
      :key="`picker-${reportType}`"
      v-model:value="reportDate"
      :picker="pickerType"
      :format="dateFormat"
      label="选择时间"
    />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const reportType = ref('daily')
const reportDate = ref([])

const reportTypeOptions = [
  { label: '日报', value: 'daily' },
  { label: '周报', value: 'weekly' },
  { label: '月报', value: 'monthly' },
  { label: '季报', value: 'quarterly' },
  { label: '年报', value: 'yearly' }
]

// 根据报表类型动态设置 picker 类型
const pickerType = computed(() => {
  const map = {
    daily: 'date',
    weekly: 'week',
    monthly: 'month',
    quarterly: 'quarter',
    yearly: 'year'
  }
  return map[reportType.value]
})

// 根据报表类型动态设置显示格式
const dateFormat = computed(() => {
  const map = {
    daily: 'YYYY-MM-DD',
    weekly: 'YYYY-wo',      // 显示为：2024-10w
    monthly: 'YYYY-MM',     // 显示为：2024-03
    quarterly: 'YYYY-[Q]Q', // 显示为：2024-Q1
    yearly: 'YYYY'          // 显示为：2024
  }
  return map[reportType.value]
})

function handleReportTypeChange() {
  reportDate.value = []  // 清空日期
}
</script>
```

**输出说明**：
- **日报**：`['2024-03-15 00:00:00', '2024-03-15 23:59:59']`
- **周报**：`['2024-03-04 00:00:00', '2024-03-10 23:59:59']`（周一到周日）
- **月报**：`['2024-03-01 00:00:00', '2024-03-31 23:59:59']`（月初到月末）
- **季报**：`['2024-01-01 00:00:00', '2024-03-31 23:59:59']`（季初到季末）
- **年报**：`['2024-01-01 00:00:00', '2024-12-31 23:59:59']`（年初到年末）

**特点**：
- ✅ 自动调整日期范围到周期边界
- ✅ 不同 picker 类型自动禁用快捷选择
- ✅ 输出统一使用 `YYYY-MM-DD HH:mm:ss` 格式

---

## ⏰ 带时间选择

### 精确到秒

```vue
<template>
  <AgDateRangePicker
    v-model:value="dateRange"
    label="精确时间"
    :show-time="true"
    :show-quick-select="false"
  />
</template>

<script setup>
const dateRange = ref([])
// 返回值：['2024-01-01 09:30:45', '2024-01-31 18:20:30']
</script>
```

### 精确到分钟

```vue
<template>
  <AgDateRangePicker
    v-model:value="dateRange"
    label="时间范围"
    :show-time="true"
    format="YYYY-MM-DD HH:mm"
    output-format="YYYY-MM-DD HH:mm"
    :show-quick-select="false"
  />
</template>

<script setup>
const dateRange = ref([])
// 返回值：['2024-01-01 09:30', '2024-01-31 18:20']
</script>
```

---

## 🔧 Props 完整说明

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| value | 日期范围值（v-model:value） | String \| Array | `''` |
| format | 日期显示格式 | String | 根据 picker 自动设置 |
| showTime | 是否显示时间选择 | Boolean \| Object | `false` |
| showQuickSelect | 是否显示快捷选择（仅 picker='date' 有效） | Boolean | `true` |
| valueType | 返回值类型 | `'string'` \| `'array'` \| `'auto'` | `'auto'` |
| options | 快捷选择选项配置 | Array | 见下文 |
| label | 浮动标签文本 | String | `''` |
| required | 是否显示必填星号 | Boolean | `false` |
| size | 组件尺寸 | `'small'` \| `'middle'` \| `'large'` | `'middle'` |
| picker | 选择器类型 | `'date'` \| `'week'` \| `'month'` \| `'quarter'` \| `'year'` | `'date'` |
| outputFormat | 输出值的日期格式 | String | `'YYYY-MM-DD HH:mm:ss'` |

### picker 类型说明

| picker 值 | 说明 | 显示格式 | 输出示例 |
|----------|------|---------|---------|
| date | 日期选择 | YYYY-MM-DD | `2024-03-15 00:00:00 ~ 2024-03-15 23:59:59` |
| week | 周选择 | YYYY-wo | `2024-03-04 00:00:00 ~ 2024-03-10 23:59:59` |
| month | 月选择 | YYYY-MM | `2024-03-01 00:00:00 ~ 2024-03-31 23:59:59` |
| quarter | 季度选择 | YYYY-[Q]Q | `2024-01-01 00:00:00 ~ 2024-03-31 23:59:59` |
| year | 年选择 | YYYY | `2024-01-01 00:00:00 ~ 2024-12-31 23:59:59` |

### 默认快捷选项

```javascript
[
  { label: '全部时间', value: '' },
  { label: '今天', value: 'today' },
  { label: '昨天', value: 'yesterday' },
  { label: '近7天', value: 'near7' },
  { label: '近30天', value: 'near30' },
  { label: '本月', value: 'thisMonth' },
  { label: '本年', value: 'thisYear' },
  { label: '自定义时间', value: 'custom' }
]
```

---

## 📊 返回值格式说明

### 字符串格式（valueType='string' 或 auto 识别）

| 场景 | 返回值示例 |
|------|-----------|
| 快捷选项 - 今天 | `'today'` |
| 快捷选项 - 近7天 | `'near7'` |
| 快捷选项 - 全部时间 | `''` |
| 自定义时间（无 showTime） | `'custom_2024-01-01 00:00:00_2024-01-31 23:59:59'` |
| 自定义时间（有 showTime） | `'custom_2024-01-01 09:30:45_2024-01-31 18:20:30'` |

### 数组格式（valueType='array' 或 auto 识别）

| 场景 | 返回值示例 |
|------|-----------|
| 快捷选项 - 今天 | `['2024-03-15 00:00:00', '2024-03-15 23:59:59']` |
| 快捷选项 - 近7天 | `['2024-03-09 00:00:00', '2024-03-15 23:59:59']` |
| 自定义时间（date） | `['2024-01-01 00:00:00', '2024-01-31 23:59:59']` |
| 自定义时间（week） | `['2024-03-04 00:00:00', '2024-03-10 23:59:59']` |
| 自定义时间（month） | `['2024-03-01 00:00:00', '2024-03-31 23:59:59']` |

---

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:value | 值改变时触发 | `(value: String \| Array) => void` |
| change | 值改变时触发 | `(value: String \| Array) => void` |

---

## 💡 常见问题

### Q1: 为什么切换 picker 类型后日期被清空？

**A:** 不同 picker 类型的日期对象格式不兼容。例如：
- week picker 的日期对象传给 month picker 会报错
- **解决方案**：使用 `:key` 强制重新渲染

```vue
<AgDateRangePicker
  :key="`picker-${reportType}`"
  :picker="reportType"
/>
```

### Q2: 如何获取完整的时间范围？

**A:** 组件会自动根据 picker 类型调整时间范围：
- **date**：当天 00:00:00 ~ 23:59:59
- **week**：周一 00:00:00 ~ 周日 23:59:59
- **month**：月初 00:00:00 ~ 月末 23:59:59
- **quarter**：季初 00:00:00 ~ 季末 23:59:59
- **year**：年初 00:00:00 ~ 年末 23:59:59

### Q3: 为什么周报/月报不显示快捷选择？

**A:** 快捷选择（今天、近7天等）只对 `picker='date'` 有意义。对于周报、月报等，组件会**自动禁用**快捷选择。

### Q4: 如何自定义输出格式？

**A:** 使用 `output-format` 属性：

```vue
<AgDateRangePicker
  v-model:value="dateRange"
  output-format="YYYY-MM-DD"
/>
<!-- 输出：['2024-01-01', '2024-01-31'] -->

<AgDateRangePicker
  v-model:value="dateRange"
  output-format="YYYY-MM-DD HH:mm"
/>
<!-- 输出：['2024-01-01 00:00', '2024-01-31 23:59'] -->
```

### Q5: 选择完日期后面板不自动关闭？

**A:** 已修复！现在选择完日期后面板会自动关闭。

### Q6: 返回值类型如何自动识别？

**A:** 根据初始值自动判断：
```javascript
const date1 = ref('')    // → 返回字符串
const date2 = ref([])    // → 返回数组
```

---

## 🎯 最佳实践

### 1. 搜索筛选（推荐）

```vue
<template>
  <div class="search-bar">
    <AgDateRangePicker
      v-model:value="searchParams.dateRange"
      label="搜索时间"
      style="width: 300px"
    />
    <a-button type="primary" @click="handleSearch">搜索</a-button>
  </div>
</template>

<script setup>
const searchParams = reactive({
  dateRange: ''  // 字符串格式，直接传给后端
})

function handleSearch() {
  console.log('搜索参数:', searchParams.dateRange)
  // 输出：'today' 或 'near7' 或 'custom_...'
}
</script>
```

### 2. 表单使用

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-form-item name="activityDate" label="活动时间">
      <AgDateRangePicker
        v-model:value="form.activityDate"
        :show-quick-select="false"
        :required="true"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
const form = reactive({
  activityDate: []
})

const rules = {
  activityDate: [
    { required: true, message: '请选择活动时间', type: 'array' }
  ]
}
</script>
```

### 3. 动态报表生成

```vue
<template>
  <div>
    <AgSelect
      v-model:value="reportType"
      :options="reportTypeOptions"
      @change="handleTypeChange"
    />

    <AgDateRangePicker
      :key="reportType"
      v-model:value="reportDate"
      :picker="pickerType"
      :format="dateFormat"
    />

    <a-button type="primary" @click="generateReport">
      生成报表
    </a-button>
  </div>
</template>

<script setup>
// ... 见上文"周报/月报/季报/年报选择器"示例
</script>
```

---

## 🔗 相关组件

- [AgInput](../ag-input/README.md) - 浮动标签输入框
- [AgSelect](../ag-select/README.md) - 浮动标签下拉选择

---

## 📝 更新日志

### v2.0.0 (2024-03-16)
- ✨ **新增**：支持 week、month、quarter、year 等 picker 类型
- ✨ **新增**：`outputFormat` 属性，自定义输出格式
- ✨ **新增**：自动调整日期范围到周期边界
- 🐛 **修复**：周报/季报格式化错误导致 Invalid Date
- 🐛 **修复**：选择完日期后面板不自动关闭
- 🐛 **修复**：数组返回值时选择快捷选项导致模式切换
- ⚡ **优化**：只有 picker='date' 时才显示快捷选择
- 📝 **文档**：完善使用说明和最佳实践

### v1.0.0 (2024-01-15)
- 🎉 初始版本发布
  { label: '最近30天', value: [dayjs().subtract(30, 'day'), dayjs()] },
  { label: '本月', value: [dayjs().startOf('month'), dayjs().endOf('month')] }
]
</script>
```

### 4. 禁用日期

```vue
<template>
  <AgDateRangePicker
    v-model="dateRange"
    label="营业期限"
    :disabled-date="disabledDate"
  />
</template>

<script setup>
import dayjs from 'dayjs'

function disabledDate(current) {
  // 禁用今天之前的日期
  return current && current < dayjs().startOf('day')
}
</script>
```

### 5. 必填项

```vue
<AgDateRangePicker
  v-model="dateRange"
  label="活动时间"
  :required="true"
/>
```

### 6. 禁用状态

```vue
<AgDateRangePicker
  v-model="dateRange"
  label="禁用"
  :disabled="true"
/>
```

### 7. 不同尺寸

```vue
<!-- 小号 -->
<AgDateRangePicker
  v-model="dateRange1"
  label="小尺寸"
  size="small"
/>

<!-- 中号（默认）-->
<AgDateRangePicker
  v-model="dateRange2"
  label="中尺寸"
  size="middle"
/>

<!-- 大号 -->
<AgDateRangePicker
  v-model="dateRange3"
  label="大尺寸"
  size="large"
/>
```

### 8. 自定义格式

```vue
<AgDateRangePicker
  v-model="dateRange"
  label="日期"
  format="YYYY年MM月DD日"
  value-format="YYYY-MM-DD"
/>
```

## 💡 使用场景

### 订单查询

```vue
<AgDateRangePicker
  v-model="form.dateRange"
  label="下单时间"
  :presets="[
    { label: '今天', value: [dayjs(), dayjs()] },
    { label: '最近7天', value: [dayjs().subtract(7, 'day'), dayjs()] },
    { label: '最近30天', value: [dayjs().subtract(30, 'day'), dayjs()] }
  ]"
/>
```

### 活动时间

```vue
<AgDateRangePicker
  v-model="form.activityTime"
  label="活动时间"
  :show-time="true"
  :required="true"
  :disabled-date="disabledDate"
  format="YYYY-MM-DD HH:mm"
/>

<script setup>
import dayjs from 'dayjs'

function disabledDate(current) {
  return current && current < dayjs().startOf('day')
}
</script>
```

### 统计报表

```vue
<AgDateRangePicker
  v-model="query.dateRange"
  label="统计时间"
  :presets="[
    { label: '本周', value: [dayjs().startOf('week'), dayjs().endOf('week')] },
    { label: '本月', value: [dayjs().startOf('month'), dayjs().endOf('month')] },
    { label: '本季度', value: [dayjs().startOf('quarter'), dayjs().endOf('quarter')] }
  ]"
/>
```

## 📝 与表单验证结合

```vue
<template>
  <a-form :model="form" :rules="rules">
    <a-form-item name="dateRange">
      <AgDateRangePicker
        v-model="form.dateRange"
        label="活动时间"
        :required="true"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'

const form = reactive({
  dateRange: []
})

const rules = {
  dateRange: [
    { 
      required: true, 
      message: '请选择活动时间',
      validator: (rule, value) => {
        if (!value || value.length !== 2) {
          return Promise.reject('请选择完整的时间范围')
        }
        return Promise.resolve()
      }
    }
  ]
}
</script>
```

## 💡 最佳实践

### 1. 提供快捷选项

```vue
<!-- ✅ 好的做法：提供常用快捷选项 -->
<AgDateRangePicker
  v-model="dateRange"
  label="查询时间"
  :presets="[
    { label: '今天', value: [dayjs(), dayjs()] },
    { label: '最近7天', value: [dayjs().subtract(7, 'day'), dayjs()] },
    { label: '最近30天', value: [dayjs().subtract(30, 'day'), dayjs()] }
  ]"
/>
```

### 2. 限制选择范围

```vue
<!-- ✅ 好的做法：禁用不合理的日期 -->
<AgDateRangePicker
  v-model="dateRange"
  label="活动时间"
  :disabled-date="(current) => current && current < dayjs().startOf('day')"
/>
```

### 3. 明确时间精度

```vue
<!-- ✅ 好的做法：根据需求选择精度 -->
<!-- 日期级别 -->
<AgDateRangePicker label="日期" format="YYYY-MM-DD" />

<!-- 时间级别 -->
<AgDateRangePicker 
  label="时间" 
  :show-time="true"
  format="YYYY-MM-DD HH:mm:ss"
/>
```

### 4. 统一格式

```vue
<!-- ✅ 好的做法：显示格式和值格式分离 -->
<AgDateRangePicker
  v-model="dateRange"
  label="日期"
  format="YYYY年MM月DD日"
  value-format="YYYY-MM-DD"
/>
```

## 🆚 与其他组件对比

| 组件 | 用途 | 特点 |
|-----|------|------|
| a-date-picker | 单个日期 | 选择一个日期 |
| **AgDateRangePicker** | 日期范围 | 选择开始和结束日期 |

## 📚 相关组件

- [AgInput](../ag-input/README.md) - 文本输入框
- [AgSelect](../ag-select/README.md) - 下拉选择框

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgDateRangePicker 组件已就绪，开始使用吧！
