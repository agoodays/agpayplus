# AgStateSwitch 通用状态切换组件使用指南

## 组件简介

`AgStateSwitch` 是一个通用的状态切换按钮组件，支持两种显示模式：
- **Switch 模式** - 可交互的开关按钮
- **Badge 模式** - 只读的状态徽章

## 核心特性

- ✅ **双模式** - Switch（开关）/ Badge（徽章）
- ✅ **状态映射** - 0=停用，1=启用，其他=未知
- ✅ **异步支持** - onChange 支持 Promise
- ✅ **错误处理** - 切换失败自动回滚
- ✅ **权限控制** - 支持禁用状态
- ✅ **加载状态** - 异步操作时显示 loading
- ✅ **自定义文本** - 所有文本可自定义

## 基本用法

### 1. Badge 模式（只读显示）

```vue
<template>
  <ag-state-switch :state="record.state" />
</template>

<script setup>
import { AgStateSwitch } from '@/components'

// record.state 的值:
// 0 = 停用（红色徽章）
// 1 = 启用（绿色徽章）
// 其他 = 未知（黄色徽章）
</script>
```

**效果：**
- 启用：🟢 启用
- 停用：🔴 停用
- 未知：🟡 未知

### 2. Switch 模式（可交互）

```vue
<template>
  <ag-state-switch
    :state="record.state"
    show-switch
    :on-change="handleStateChange"
  />
</template>

<script setup>
import { AgStateSwitch } from '@/components'
import { updateUserState } from '@/api/user'

async function handleStateChange(newState) {
  // newState: 1 表示启用，0 表示停用
  await updateUserState(record.id, newState)
  // 成功后会自动更新组件状态
  // 失败会自动回滚并抛出错误
}
</script>
```

## Props 属性

| 参数 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| state | 状态值（0=停用，1=启用） | `Number` | `-1` |
| showSwitch | 是否显示为 Switch 开关 | `Boolean` | `false` |
| disabled | 是否禁用（仅 Switch 模式） | `Boolean` | `false` |
| activeText | 激活状态文本 | `String` | `'启用'` |
| inactiveText | 停用状态文本 | `String` | `'停用'` |
| unknownText | 未知状态文本 | `String` | `'未知'` |
| checkedText | Switch 选中时的文本 | `String` | `''` |
| uncheckedText | Switch 未选中时的文本 | `String` | `''` |
| onChange | 切换回调函数 | `Function` | `() => Promise.resolve()` |

## Events 事件

| 事件名 | 说明 | 回调参数 |
| --- | --- | --- |
| update:state | 状态更新（v-model） | `(newState: Number)` - 0 或 1 |
| change | 状态改变 | `(newState: Number)` - 0 或 1 |

## 使用示例

### 示例 1：表格中显示状态（Badge 模式）

```vue
<template>
  <ag-table :table-columns="columns" :req-table-data-func="loadData">
    <template #state="{ record }">
      <ag-state-switch :state="record.state" />
    </template>
  </ag-table>
</template>

<script setup>
import { ref } from 'vue'
import { AgTable, AgStateSwitch } from '@/components'

const columns = ref([
  { title: 'ID', key: 'id', dataIndex: 'id' },
  { title: '名称', key: 'name', dataIndex: 'name' },
  { title: '状态', key: 'state', customRender: 'state' }
])
</script>
```

### 示例 2：表格中切换状态（Switch 模式）

```vue
<template>
  <ag-table :table-columns="columns" :req-table-data-func="loadData">
    <template #state="{ record }">
      <ag-state-switch
        :state="record.state"
        show-switch
        :on-change="(newState) => handleStateChange(record, newState)"
      />
    </template>
  </ag-table>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { AgTable, AgStateSwitch } from '@/components'
import { updateUserState } from '@/api/user'

const columns = ref([
  { title: 'ID', key: 'id', dataIndex: 'id' },
  { title: '用户名', key: 'username', dataIndex: 'username' },
  { title: '状态', key: 'state', customRender: 'state', width: 100 }
])

async function handleStateChange(record, newState) {
  try {
    await updateUserState(record.id, newState)
    message.success('状态更新成功')
    record.state = newState  // 更新本地数据
  } catch (error) {
    message.error('状态更新失败')
    throw error  // 抛出错误以触发回滚
  }
}
</script>
```

### 示例 3：自定义文本

```vue
<template>
  <!-- Badge 模式 - 自定义状态文本 -->
  <ag-state-switch
    :state="record.status"
    active-text="在线"
    inactive-text="离线"
    unknown-text="维护中"
  />

  <!-- Switch 模式 - 自定义开关文本 -->
  <ag-state-switch
    :state="record.enabled"
    show-switch
    checked-text="开"
    unchecked-text="关"
    :on-change="handleChange"
  />
</template>
```

### 示例 4：带权限控制的 Switch

```vue
<template>
  <ag-state-switch
    :state="record.state"
    show-switch
    :disabled="!hasPermission"
    :on-change="handleStateChange"
  />
</template>

<script setup>
import { computed } from 'vue'
import { useStore } from '@/store'

const store = useStore()

// 检查是否有权限
const hasPermission = computed(() => {
  return store.state.user.permissions.includes('user:state:update')
})

async function handleStateChange(newState) {
  // 处理状态切换
  await updateState(newState)
}
</script>
```

### 示例 5：配合表单使用

```vue
<template>
  <a-form :model="formData">
    <a-form-item label="启用状态">
      <ag-state-switch
        v-model:state="formData.enabled"
        show-switch
        :on-change="handleFormStateChange"
      />
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'
import { AgStateSwitch } from '@/components'

const formData = reactive({
  name: '',
  enabled: 1
})

function handleFormStateChange(newState) {
  console.log('表单状态更新:', newState)
  return Promise.resolve()
}
</script>
```

### 示例 6：完整的 CRUD 示例

```vue
<template>
  <a-card title="用户管理">
    <ag-table
      :table-columns="columns"
      :req-table-data-func="loadUsers"
      column-state-key="user_list"
    >
      <template #state="{ record }">
        <ag-state-switch
          :state="record.state"
          show-switch
          :disabled="record.id === currentUserId"
          :on-change="(newState) => updateUserState(record, newState)"
        />
      </template>
    </ag-table>
  </a-card>
</template>

<script setup>
import { ref, computed } from 'vue'
import { message } from 'ant-design-vue'
import { AgTable, AgStateSwitch } from '@/components'
import { getUserList, updateUser } from '@/api/user'
import { useStore } from '@/store'

const store = useStore()

const columns = ref([
  { title: 'ID', key: 'id', dataIndex: 'id', width: 80 },
  { title: '用户名', key: 'username', dataIndex: 'username', width: 150 },
  { title: '邮箱', key: 'email', dataIndex: 'email', width: 200 },
  { title: '状态', key: 'state', customRender: 'state', width: 100 }
])

// 当前登录用户ID
const currentUserId = computed(() => store.state.user.id)

// 加载用户列表
async function loadUsers(params) {
  const res = await getUserList(params)
  return {
    total: res.total,
    records: res.list
  }
}

// 更新用户状态
async function updateUserState(record, newState) {
  try {
    // 调用 API
    await updateUser(record.id, { state: newState })
    
    // 更新成功
    message.success(`已${newState === 1 ? '启用' : '停用'}用户：${record.username}`)
    
    // 更新本地数据
    record.state = newState
  } catch (error) {
    // 更新失败
    message.error(error.message || '状态更新失败')
    throw error  // 抛出错误触发组件回滚
  }
}
</script>
```

## API 参考

### onChange 回调函数

```typescript
type OnChange = (newState: number) => Promise<void>

// 示例
const onChange = async (newState) => {
  // newState: 1 = 启用, 0 = 停用
  
  // 1. 调用后端 API
  await apiUpdateState(id, newState)
  
  // 2. 成功后不需要做任何事，组件会自动更新
  
  // 3. 如果失败，抛出错误，组件会自动回滚
  // throw new Error('更新失败')
}
```

### 状态值映射

| 数值 | 状态 | Badge 颜色 | Switch 状态 |
| --- | --- | --- | --- |
| 1 | 启用/激活 | 🟢 绿色 success | checked |
| 0 | 停用/禁用 | 🔴 红色 error | unchecked |
| 其他 | 未知 | 🟡 黄色 warning | - |

## 最佳实践

### 1. 何时使用 Badge 模式？

- 没有编辑权限时
- 只需要展示状态时
- 状态不可更改时
- 列表页的只读展示

### 2. 何时使用 Switch 模式？

- 有编辑权限时
- 需要快速切换状态时
- 状态可以实时更改时
- 管理后台的状态切换

### 3. 错误处理

```vue
<script setup>
async function handleStateChange(newState) {
  try {
    await updateState(newState)
    // 成功 - 组件自动更新
  } catch (error) {
    // 失败 - 组件自动回滚
    console.error('更新失败:', error)
    throw error  // 必须抛出错误才能触发回滚
  }
}
</script>
```

### 4. 权限控制

```vue
<template>
  <ag-state-switch
    :state="record.state"
    :show-switch="hasEditPermission"
    :disabled="!hasEditPermission"
    :on-change="handleChange"
  />
</template>

<script setup>
import { computed } from 'vue'

const hasEditPermission = computed(() => {
  // 检查用户权限
  return checkPermission('user:edit')
})
</script>
```

### 5. 防止自己禁用自己

```vue
<template>
  <ag-state-switch
    :state="record.state"
    show-switch
    :disabled="record.id === currentUserId"
    :on-change="handleChange"
  />
</template>

<script setup>
// 当前用户不能禁用自己
const currentUserId = store.state.user.id
</script>
```

## 常见问题

### Q1: 为什么状态更新后没有刷新？

确保 `onChange` 回调成功完成，并且更新了本地数据：

```javascript
async function handleChange(record, newState) {
  await updateApi(record.id, newState)
  record.state = newState  // 更新本地数据
}
```

### Q2: 如何阻止状态切换？

在 `onChange` 中抛出错误：

```javascript
async function handleChange(newState) {
  if (someCondition) {
    throw new Error('不允许切换')
  }
  await updateApi(newState)
}
```

### Q3: 如何显示自定义的加载提示？

组件已内置 loading 状态，Switch 会显示转圈动画。

### Q4: 可以用在非表格场景吗？

可以，这是一个通用组件，可用于任何需要状态显示或切换的场景。

## 样式定制

### 自定义徽章颜色

```vue
<style scoped>
:deep(.ant-badge-status-success) {
  background-color: #52c41a;
}

:deep(.ant-badge-status-error) {
  background-color: #ff4d4f;
}

:deep(.ant-badge-status-warning) {
  background-color: #faad14;
}
</style>
```

### 自定义 Switch 样式

```vue
<style scoped>
:deep(.ant-switch-checked) {
  background-color: #52c41a;
}
</style>
```

## 相关组件

- [AgTable](../ag-table/README.md) - 通用表格组件
- [AgTableAction](../ag-table-action/README.md) - 表格操作列组件

## 更新日志

- `2024-01-XX` - 组件创建
  - 支持 Badge 和 Switch 双模式
  - 支持异步状态切换
  - 支持权限控制和禁用
  - 支持自定义文本
  - 错误自动回滚
