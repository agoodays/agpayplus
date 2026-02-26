# AgModal - 模态框组件

## 📝 组件说明

基于 Ant Design Vue 的 Modal 组件封装，提供统一的模态框使用方式。

## ✨ 功能特性

- ✅ 支持 v-model 控制显示隐藏
- ✅ 自定义宽度
- ✅ 自定义标题
- ✅ 自定义按钮文字
- ✅ 支持确认加载状态
- ✅ 关闭时自动销毁
- ✅ 自定义底部按钮

## 🎯 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|-------|
| open | 是否显示（支持 v-model） | boolean | false |
| title | 标题 | string | '提示' |
| width | 宽度 | string \| number | 520 |
| closable | 是否显示关闭按钮 | boolean | true |
| maskClosable | 点击蒙层是否关闭 | boolean | true |
| destroyOnClose | 关闭时销毁子元素 | boolean | true |
| confirmLoading | 确认按钮加载状态 | boolean | false |
| okText | 确认按钮文字 | string | '确定' |
| cancelText | 取消按钮文字 | string | '取消' |

## 📤 Events

| 事件名 | 说明 | 参数 |
|-------|------|------|
| update:open | open 变化时触发 | (open: boolean) |
| ok | 点击确定按钮时触发 | - |
| cancel | 点击取消按钮时触发 | - |

## 🔧 Slots

| 插槽名 | 说明 |
|-------|------|
| default | 模态框内容 |
| footer | 自定义底部按钮 |

## 🔧 Methods

| 方法名 | 说明 | 参数 |
|-------|------|------|
| close | 关闭模态框 | - |

## 💡 使用示例

### 基础用法

```vue
<template>
  <div>
    <a-button @click="open = true">打开模态框</a-button>
    
    <AgModal
      v-model:open="open"
      title="提示"
      @ok="handleOk"
    >
      <p>这里是模态框内容</p>
    </AgModal>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgModal from '@/components/ag-modal'

const open = ref(false)

function handleOk() {
  message.success('点击了确定')
  open.value = false
}
</script>
```

### 表单提交

```vue
<template>
  <AgModal
    v-model:open="open"
    title="添加用户"
    :confirmLoading="loading"
    @ok="handleSubmit"
  >
    <a-form
      ref="formRef"
      :model="form"
      :rules="rules"
      :label-col="{ span: 6 }"
      :wrapper-col="{ span: 16 }"
    >
      <a-form-item label="用户名" name="username">
        <a-input v-model:value="form.username" />
      </a-form-item>
      
      <a-form-item label="邮箱" name="email">
        <a-input v-model:value="form.email" />
      </a-form-item>
      
      <a-form-item label="手机号" name="phone">
        <a-input v-model:value="form.phone" />
      </a-form-item>
    </a-form>
  </AgModal>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import AgModal from '@/components/ag-modal'

const open = ref(false)
const loading = ref(false)
const formRef = ref()

const form = reactive({
  username: '',
  email: '',
  phone: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名' }],
  email: [
    { required: true, message: '请输入邮箱' },
    { type: 'email', message: '邮箱格式不正确' }
  ],
  phone: [{ required: true, message: '请输入手机号' }]
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    
    loading.value = true
    await api.createUser(form)
    
    message.success('添加成功')
    open.value = false
  } catch (error) {
    if (!error.errorFields) {
      message.error('添加失败')
    }
  } finally {
    loading.value = false
  }
}
</script>
```

### 自定义宽度

```vue
<template>
  <AgModal
    v-model:open="open"
    title="详细信息"
    :width="800"
  >
    <a-descriptions :column="2" bordered>
      <a-descriptions-item label="订单号">
        ORD001
      </a-descriptions-item>
      <a-descriptions-item label="金额">
        ¥100.00
      </a-descriptions-item>
    </a-descriptions>
  </AgModal>
</template>
```

### 自定义按钮文字

```vue
<template>
  <AgModal
    v-model:open="open"
    title="确认删除"
    okText="删除"
    cancelText="取消"
    @ok="handleDelete"
  >
    <p>确定要删除这条记录吗？此操作不可恢复。</p>
  </AgModal>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgModal from '@/components/ag-modal'

const open = ref(false)

async function handleDelete() {
  try {
    await api.delete()
    message.success('删除成功')
    open.value = false
  } catch (error) {
    message.error('删除失败')
  }
}
</script>
```

### 自定义底部按钮

```vue
<template>
  <AgModal
    v-model:open="open"
    title="审核"
  >
    <a-form>
      <a-form-item label="审核意见">
        <a-textarea v-model:value="opinion" :rows="4" />
      </a-form-item>
    </a-form>
    
    <template #footer>
      <a-space>
        <a-button @click="open = false">取消</a-button>
        <a-button type="danger" @click="handleReject">驳回</a-button>
        <a-button type="primary" @click="handleApprove">通过</a-button>
      </a-space>
    </template>
  </AgModal>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgModal from '@/components/ag-modal'

const open = ref(false)
const opinion = ref('')

async function handleApprove() {
  if (!opinion.value) {
    message.warning('请输入审核意见')
    return
  }
  
  await api.approve({ opinion: opinion.value, status: 1 })
  message.success('审核通过')
  open.value = false
}

async function handleReject() {
  if (!opinion.value) {
    message.warning('请输入驳回原因')
    return
  }
  
  await api.approve({ opinion: opinion.value, status: 2 })
  message.success('已驳回')
  open.value = false
}
</script>
```

### 确认对话框

```vue
<template>
  <div>
    <a-button danger @click="showDeleteConfirm">删除</a-button>
    
    <AgModal
      v-model:open="deleteOpen"
      title="确认删除"
      :maskClosable="false"
      okText="确认删除"
      :confirmLoading="deleting"
      @ok="handleDelete"
    >
      <a-alert
        message="警告"
        description="删除后数据将无法恢复，请确认是否继续？"
        type="warning"
        show-icon
        class="mb-3"
      />
      
      <a-checkbox v-model:checked="confirmed">
        我已了解风险，确认删除
      </a-checkbox>
    </AgModal>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgModal from '@/components/ag-modal'

const deleteOpen = ref(false)
const deleting = ref(false)
const confirmed = ref(false)

function showDeleteConfirm() {
  confirmed.value = false
  deleteOpen.value = true
}

async function handleDelete() {
  if (!confirmed.value) {
    message.warning('请确认删除操作')
    return
  }
  
  deleting.value = true
  try {
    await api.delete()
    message.success('删除成功')
    deleteOpen.value = false
  } catch (error) {
    message.error('删除失败')
  } finally {
    deleting.value = false
  }
}
</script>
```

## 📚 完整示例

### 用户管理模态框

```vue
<template>
  <div class="user-page">
    <a-button type="primary" @click="showAddModal">
      添加用户
    </a-button>

    <!-- 添加/编辑模态框 -->
    <AgModal
      v-model:open="modalOpen"
      :title="modalTitle"
      :width="600"
      :confirmLoading="submitLoading"
      @ok="handleSubmit"
    >
      <a-form
        ref="formRef"
        :model="formData"
        :rules="rules"
        :label-col="{ span: 6 }"
        :wrapper-col="{ span: 16 }"
      >
        <a-form-item label="用户名" name="username">
          <a-input
            v-model:value="formData.username"
            placeholder="请输入用户名"
          />
        </a-form-item>

        <a-form-item label="真实姓名" name="realname">
          <a-input
            v-model:value="formData.realname"
            placeholder="请输入真实姓名"
          />
        </a-form-item>

        <a-form-item label="手机号" name="phone">
          <a-input
            v-model:value="formData.phone"
            placeholder="请输入手机号"
          />
        </a-form-item>

        <a-form-item label="邮箱" name="email">
          <a-input
            v-model:value="formData.email"
            placeholder="请输入邮箱"
          />
        </a-form-item>

        <a-form-item label="角色" name="roleId">
          <a-select
            v-model:value="formData.roleId"
            placeholder="请选择角色"
          >
            <a-select-option value="1">管理员</a-select-option>
            <a-select-option value="2">普通用户</a-select-option>
          </a-select>
        </a-form-item>

        <a-form-item label="状态" name="status">
          <a-radio-group v-model:value="formData.status">
            <a-radio :value="1">启用</a-radio>
            <a-radio :value="0">禁用</a-radio>
          </a-radio-group>
        </a-form-item>

        <a-form-item label="备注" name="remark">
          <a-textarea
            v-model:value="formData.remark"
            :rows="3"
            placeholder="请输入备注"
          />
        </a-form-item>
      </a-form>
    </AgModal>

    <!-- 删除确认框 -->
    <AgModal
      v-model:open="deleteOpen"
      title="确认删除"
      :maskClosable="false"
      okText="确认删除"
      :confirmLoading="deleteLoading"
      @ok="handleDelete"
    >
      <a-alert
        message="删除提示"
        :description="`确定要删除用户「${currentUser?.realname}」吗？`"
        type="warning"
        show-icon
      />
    </AgModal>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { message } from 'ant-design-vue'
import AgModal from '@/components/ag-modal'

const modalOpen = ref(false)
const deleteOpen = ref(false)
const submitLoading = ref(false)
const deleteLoading = ref(false)
const isEdit = ref(false)
const currentUser = ref(null)
const formRef = ref()

const formData = reactive({
  id: null,
  username: '',
  realname: '',
  phone: '',
  email: '',
  roleId: undefined,
  status: 1,
  remark: ''
})

const rules = {
  username: [{ required: true, message: '请输入用户名' }],
  realname: [{ required: true, message: '请输入真实姓名' }],
  phone: [
    { required: true, message: '请输入手机号' },
    { pattern: /^1\d{10}$/, message: '手机号格式不正确' }
  ],
  email: [
    { required: true, message: '请输入邮箱' },
    { type: 'email', message: '邮箱格式不正确' }
  ],
  roleId: [{ required: true, message: '请选择角色' }]
}

const modalTitle = computed(() => {
  return isEdit.value ? '编辑用户' : '添加用户'
})

function showAddModal() {
  isEdit.value = false
  resetForm()
  modalOpen.value = true
}

function showEditModal(record) {
  isEdit.value = true
  Object.assign(formData, record)
  modalOpen.value = true
}

function showDeleteModal(record) {
  currentUser.value = record
  deleteOpen.value = true
}

function resetForm() {
  Object.assign(formData, {
    id: null,
    username: '',
    realname: '',
    phone: '',
    email: '',
    roleId: undefined,
    status: 1,
    remark: ''
  })
  formRef.value?.resetFields()
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    
    submitLoading.value = true
    
    if (isEdit.value) {
      await api.updateUser(formData)
      message.success('更新成功')
    } else {
      await api.createUser(formData)
      message.success('添加成功')
    }
    
    modalOpen.value = false
    // 刷新列表
    
  } catch (error) {
    if (!error.errorFields) {
      message.error(isEdit.value ? '更新失败' : '添加失败')
    }
  } finally {
    submitLoading.value = false
  }
}

async function handleDelete() {
  deleteLoading.value = true
  try {
    await api.deleteUser(currentUser.value.id)
    message.success('删除成功')
    deleteOpen.value = false
    // 刷新列表
  } catch (error) {
    message.error('删除失败')
  } finally {
    deleteLoading.value = false
  }
}

defineExpose({
  showAddModal,
  showEditModal,
  showDeleteModal
})
</script>
```

## ⚠️ 注意事项

1. **v-model 绑定**
  - 使用 `v-model:open` 控制显示隐藏
  - 不要直接修改 props 的 open

2. **表单验证**
   - 在 @ok 事件中进行表单验证
   - 验证通过后再关闭模态框

3. **destroyOnClose**
   - 默认开启，关闭时销毁子元素
   - 可以避免数据残留问题

4. **加载状态**
   - 使用 `confirmLoading` 显示加载状态
   - 防止重复提交

5. **maskClosable**
   - 谨慎使用，避免用户误操作
   - 表单提交时建议设置为 false

## 🎯 最佳实践

1. **表单重置**
   ```javascript
   function resetForm() {
     formRef.value?.resetFields()
     // 或手动重置
     Object.assign(formData, initialData)
   }
   ```

2. **错误处理**
   ```javascript
   catch (error) {
     if (!error.errorFields) {
       // 非表单验证错误
       message.error('操作失败')
     }
   }
   ```

3. **组合使用**
   ```vue
   <AgModal
     v-model:open="open"
     :confirmLoading="loading"
     @ok="handleOk"
   >
     <!-- 内容 -->
   </AgModal>
   ```

---

**版本**: 1.0.0  
**更新时间**: 2024-01-XX  
**Vue 版本**: Vue 3 + Composition API
