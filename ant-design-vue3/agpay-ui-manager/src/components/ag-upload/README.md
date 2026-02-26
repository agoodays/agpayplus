# AgUpload - 文件上传组件 📤

## 📝 组件说明

`AgUpload` 是一个增强的文件上传组件，基于 Ant Design Upload 封装，支持拖拽上传、图片预览、进度显示等功能。

## ✨ 特性

- ✅ **多种模式** - 支持点击、拖拽上传
- ✅ **图片预览** - 内置图片预览功能
- ✅ **进度显示** - 实时显示上传进度
- ✅ **文件限制** - 支持文件类型、大小限制
- ✅ **批量上传** - 支持多文件上传
- ✅ **自定义上传** - 支持自定义上传逻辑
- ✅ **列表样式** - text/picture/picture-card

## 📦 基础用法

```vue
<template>
  <AgUpload
    v-model="fileList"
    action="/api/upload"
    :max-count="5"
  >
    <a-button>
      <UploadOutlined />
      点击上传
    </a-button>
  </AgUpload>
</template>

<script setup>
import { ref } from 'vue'
import { UploadOutlined } from '@ant-design/icons-vue'
import { AgUpload } from '@/components'

const fileList = ref([])
</script>
```

## 🔧 Props

继承 Ant Design Upload 的所有 Props，常用属性：

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|--------|
| modelValue(v-model) | 文件列表 | Array | [] |
| action | 上传地址 | String | - |
| accept | 接受的文件类型 | String | - |
| maxCount | 最大文件数 | Number | - |
| maxSize | 最大文件大小(MB) | Number | - |
| listType | 列表样式 | 'text' \| 'picture' \| 'picture-card' | 'text' |
| multiple | 是否多选 | Boolean | false |
| disabled | 是否禁用 | Boolean | false |
| showUploadList | 是否显示文件列表 | Boolean \| Object | true |
| beforeUpload | 上传前钩子 | Function | - |
| customRequest | 自定义上传 | Function | - |

## 📤 Events

| 事件名 | 说明 | 回调参数 |
|--------|------|---------|
| update:modelValue | 文件列表改变 | (fileList: Array) => void |
| change | 文件状态改变 | (info: Object) => void |
| preview | 点击预览 | (file: Object) => void |
| remove | 删除文件 | (file: Object) => void |
| download | 下载文件 | (file: Object) => void |

## 🎨 示例

### 1. 基础用法

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload"
>
  <a-button>
    <UploadOutlined />
    上传文件
  </a-button>
</AgUpload>
```

### 2. 拖拽上传

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload"
  :multiple="true"
>
  <div class="upload-dragger">
    <p class="upload-drag-icon">
      <InboxOutlined />
    </p>
    <p class="upload-text">点击或拖拽文件到此区域上传</p>
    <p class="upload-hint">支持单个或批量上传</p>
  </div>
</AgUpload>
```

### 3. 图片上传

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload/image"
  list-type="picture-card"
  :max-count="1"
  accept="image/*"
>
  <div v-if="fileList.length === 0">
    <PlusOutlined />
    <div class="upload-text">上传图片</div>
  </div>
</AgUpload>
```

### 4. 文件类型限制

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload"
  accept=".pdf,.doc,.docx"
  :before-upload="beforeUpload"
>
  <a-button>
    <UploadOutlined />
    上传文档
  </a-button>
</AgUpload>

<script setup>
import { message } from 'ant-design-vue'

function beforeUpload(file) {
  const isLt5M = file.size / 1024 / 1024 < 5
  if (!isLt5M) {
    message.error('文件大小不能超过 5MB!')
    return false
  }
  return true
}
</script>
```

### 5. 多文件上传

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload"
  :multiple="true"
  :max-count="5"
>
  <a-button>
    <UploadOutlined />
    批量上传（最多5个）
  </a-button>
</AgUpload>
```

### 6. 自定义上传

```vue
<template>
  <AgUpload
    v-model="fileList"
    :custom-request="customUpload"
    list-type="picture-card"
  >
    <div v-if="fileList.length < 3">
      <PlusOutlined />
      <div class="upload-text">上传</div>
    </div>
  </AgUpload>
</template>

<script setup>
import { ref } from 'vue'
import { uploadFile } from '@/api/upload'
import { message } from 'ant-design-vue'

const fileList = ref([])

async function customUpload({ file, onProgress, onSuccess, onError }) {
  try {
    const formData = new FormData()
    formData.append('file', file)
    
    const res = await uploadFile(formData, {
      onUploadProgress: (progressEvent) => {
        const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total)
        onProgress({ percent })
      }
    })
    
    onSuccess(res.data)
    message.success('上传成功')
  } catch (error) {
    onError(error)
    message.error('上传失败')
  }
}
</script>
```

### 7. 图片列表样式

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload/image"
  list-type="picture"
  accept="image/*"
  :max-count="5"
>
  <a-button>
    <UploadOutlined />
    上传图片
  </a-button>
</AgUpload>
```

## 💡 使用场景

### 头像上传

```vue
<template>
  <AgUpload
    v-model="avatar"
    action="/api/upload/avatar"
    list-type="picture-card"
    :max-count="1"
    accept="image/*"
    :before-upload="beforeAvatarUpload"
  >
    <div v-if="!avatar.length">
      <LoadingOutlined v-if="loading" />
      <PlusOutlined v-else />
      <div class="upload-text">上传头像</div>
    </div>
  </AgUpload>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'

const avatar = ref([])
const loading = ref(false)

function beforeAvatarUpload(file) {
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  if (!isJpgOrPng) {
    message.error('只能上传 JPG/PNG 格式的图片!')
    return false
  }
  
  const isLt2M = file.size / 1024 / 1024 < 2
  if (!isLt2M) {
    message.error('图片大小不能超过 2MB!')
    return false
  }
  
  return true
}
</script>
```

### 证件上传

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload/certificate"
  list-type="picture-card"
  :max-count="3"
  accept="image/*,.pdf"
>
  <div v-if="fileList.length < 3">
    <PlusOutlined />
    <div class="upload-text">上传证件</div>
  </div>
</AgUpload>
<div class="upload-tip">支持 jpg、png、pdf 格式，最多3个文件</div>
```

### 文档上传

```vue
<AgUpload
  v-model="fileList"
  action="/api/upload/document"
  accept=".doc,.docx,.pdf,.xls,.xlsx"
  :multiple="true"
  :max-count="10"
  :before-upload="beforeDocUpload"
>
  <a-button>
    <UploadOutlined />
    上传文档
  </a-button>
</AgUpload>

<script setup>
function beforeDocUpload(file) {
  const isLt10M = file.size / 1024 / 1024 < 10
  if (!isLt10M) {
    message.error('文件大小不能超过 10MB!')
    return false
  }
  return true
}
</script>
```

## 💡 最佳实践

### 1. 添加文件限制

```vue
<!-- ✅ 好的做法：限制文件类型和大小 -->
<AgUpload
  v-model="fileList"
  action="/api/upload"
  accept="image/*"
  :before-upload="beforeUpload"
>
  <a-button>上传图片</a-button>
</AgUpload>

<script setup>
function beforeUpload(file) {
  const isImage = file.type.startsWith('image/')
  const isLt5M = file.size / 1024 / 1024 < 5
  
  if (!isImage) {
    message.error('只能上传图片文件!')
    return false
  }
  if (!isLt5M) {
    message.error('图片大小不能超过 5MB!')
    return false
  }
  return true
}
</script>
```

### 2. 提供清晰提示

```vue
<!-- ✅ 好的做法：提供文件要求说明 -->
<AgUpload v-model="fileList" action="/api/upload">
  <a-button>上传文件</a-button>
</AgUpload>
<div class="upload-tip">
  支持 jpg、png 格式，单个文件不超过 5MB，最多上传 10 个文件
</div>
```

### 3. 处理上传错误

```vue
<template>
  <AgUpload
    v-model="fileList"
    :custom-request="customUpload"
  >
    <a-button>上传</a-button>
  </AgUpload>
</template>

<script setup>
async function customUpload({ file, onSuccess, onError }) {
  try {
    const res = await uploadFile(file)
    onSuccess(res.data)
    message.success('上传成功')
  } catch (error) {
    onError(error)
    message.error(error.message || '上传失败，请重试')
  }
}
</script>
```

### 4. 图片压缩

```vue
<script setup>
import imageCompression from 'browser-image-compression'

async function beforeUpload(file) {
  if (file.type.startsWith('image/')) {
    try {
      const options = {
        maxSizeMB: 1,
        maxWidthOrHeight: 1920
      }
      const compressedFile = await imageCompression(file, options)
      return compressedFile
    } catch (error) {
      message.error('图片压缩失败')
      return false
    }
  }
  return true
}
</script>
```

## 🆚 不同列表样式

| listType | 说明 | 适用场景 |
|----------|------|---------|
| text | 文本列表 | 文档、普通文件 |
| picture | 图片列表 | 多张图片上传 |
| picture-card | 卡片式 | 单张图片、头像 |

## 📚 相关组件

- [AgCard](../ag-card/README.md) - 卡片容器
- [AgModal](../ag-modal/README.md) - 模态框
- [AgEditor](../ag-editor/README.md) - 富文本编辑器

---

**创建时间**: 2024-01-XX  
**组件版本**: v1.0.0  
**状态**: ✅ 已完成

🎉 AgUpload 组件已就绪，开始使用吧！
