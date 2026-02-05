# AgEditor - 富文本编辑器组件

## 📝 组件说明

基于 [wangEditor](https://www.wangeditor.com/) 封装的 Vue 3 富文本编辑器组件，支持图片、视频上传。

## ✨ 功能特性

- ✅ Vue 3 Composition API
- ✅ 支持 v-model 双向绑定
- ✅ 支持自定义工具栏
- ✅ 支持图片上传
- ✅ 支持视频上传
- ✅ 支持自定义高度
- ✅ 支持简洁模式/默认模式
- ✅ 暴露编辑器实例方法

## 📦 安装依赖

```bash
npm install @wangeditor/editor-for-vue @wangeditor/editor
```

## 🎯 Props

| 参数 | 说明 | 类型 | 默认值 |
|-----|------|------|-------|
| modelValue | 编辑器内容（v-model） | string | '' |
| height | 编辑器高度（px） | number | 500 |
| toolbarConfig | 工具栏配置 | object | {} |
| editorConfig | 编辑器配置 | object | { placeholder: '请输入内容...' } |
| mode | 编辑器模式 | 'default' \| 'simple' | 'default' |
| uploadConfig | 上传配置 | object | null |

## 📤 Events

| 事件名 | 说明 | 参数 |
|-------|------|------|
| update:modelValue | 内容变化时触发 | (html: string) |
| change | 内容变化时触发 | (html: string) |

## 🔧 暴露的方法

| 方法名 | 说明 | 返回值 |
|-------|------|-------|
| getEditor | 获取编辑器实例 | Editor |
| getHtml | 获取 HTML 内容 | string |
| getText | 获取纯文本内容 | string |
| isEmpty | 判断是否为空 | boolean |
| clear | 清空内容 | void |
| focus | 聚焦编辑器 | void |
| blur | 失焦编辑器 | void |

## 💡 使用示例

### 基础用法

```vue
<template>
  <div>
    <AgEditor v-model="content" />
    
    <div>
      内容：{{ content }}
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import AgEditor from '/@/components/ag-editor'

const content = ref('<p>初始内容</p>')
</script>
```

### 自定义高度

```vue
<template>
  <AgEditor 
    v-model="content" 
    :height="300"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgEditor from '/@/components/ag-editor'

const content = ref('')
</script>
```

### 简洁模式

```vue
<template>
  <AgEditor 
    v-model="content" 
    mode="simple"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgEditor from '/@/components/ag-editor'

const content = ref('')
</script>
```

### 自定义工具栏

```vue
<template>
  <AgEditor 
    v-model="content"
    :toolbarConfig="toolbarConfig"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgEditor from '/@/components/ag-editor'

const content = ref('')

const toolbarConfig = {
  toolbarKeys: [
    'headerSelect',
    'bold',
    'italic',
    'underline',
    '|',
    'bulletedList',
    'numberedList',
    '|',
    'uploadImage',
    'insertLink'
  ]
}
</script>
```

### 自定义编辑器配置

```vue
<template>
  <AgEditor 
    v-model="content"
    :editorConfig="editorConfig"
  />
</template>

<script setup>
import { ref } from 'vue'
import AgEditor from '/@/components/ag-editor'

const content = ref('')

const editorConfig = {
  placeholder: '请输入公告内容...',
  autoFocus: false,
  maxLength: 10000
}
</script>
```

### 图片上传配置

```vue
<template>
  <AgEditor 
    v-model="content"
    :uploadConfig="uploadConfig"
  />
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgEditor from '/@/components/ag-editor'
import { uploadApi } from '/@/api/upload'

const content = ref('')

const uploadConfig = {
  uploadImage: {
    server: '/api/upload/image',
    fieldName: 'file',
    maxFileSize: 2 * 1024 * 1024, // 2MB
    allowedFileTypes: ['image/*'],
    
    // 自定义上传
    customUpload: async (file, insertFn) => {
      try {
        const res = await uploadApi.uploadImage(file)
        // 插入图片到编辑器
        insertFn(res.url, res.name, res.url)
      } catch (error) {
        message.error('图片上传失败')
      }
    }
  }
}
</script>
```

### 使用编辑器方法

```vue
<template>
  <div>
    <AgEditor ref="editorRef" v-model="content" />
    
    <a-space class="mt-4">
      <a-button @click="getContent">获取内容</a-button>
      <a-button @click="clearContent">清空内容</a-button>
      <a-button @click="focusEditor">聚焦</a-button>
    </a-space>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import AgEditor from '/@/components/ag-editor'

const content = ref('')
const editorRef = ref()

function getContent() {
  const html = editorRef.value.getHtml()
  const text = editorRef.value.getText()
  const isEmpty = editorRef.value.isEmpty()
  
  console.log('HTML:', html)
  console.log('Text:', text)
  console.log('Is Empty:', isEmpty)
  
  message.info(`内容长度: ${text.length}`)
}

function clearContent() {
  editorRef.value.clear()
  message.success('已清空')
}

function focusEditor() {
  editorRef.value.focus()
}
</script>
```

### 在表单中使用

```vue
<template>
  <a-form
    :model="formState"
    @finish="handleSubmit"
  >
    <a-form-item
      label="标题"
      name="title"
      :rules="[{ required: true, message: '请输入标题' }]"
    >
      <a-input v-model:value="formState.title" />
    </a-form-item>
    
    <a-form-item
      label="内容"
      name="content"
      :rules="[{ required: true, message: '请输入内容' }]"
    >
      <AgEditor 
        v-model="formState.content"
        :height="400"
      />
    </a-form-item>
    
    <a-form-item>
      <a-button type="primary" html-type="submit">
        提交
      </a-button>
    </a-form-item>
  </a-form>
</template>

<script setup>
import { reactive } from 'vue'
import { message } from 'ant-design-vue'
import AgEditor from '/@/components/ag-editor'
import { articleApi } from '/@/api/article'

const formState = reactive({
  title: '',
  content: ''
})

async function handleSubmit() {
  try {
    await articleApi.create(formState)
    message.success('发布成功')
  } catch (error) {
    message.error('发布失败')
  }
}
</script>
```

## 📚 完整示例

### 文章编辑器

```vue
<template>
  <div class="article-editor">
    <a-card title="编辑文章">
      <a-form
        ref="formRef"
        :model="article"
        :rules="rules"
        layout="vertical"
      >
        <a-form-item label="标题" name="title">
          <a-input 
            v-model:value="article.title" 
            placeholder="请输入文章标题"
          />
        </a-form-item>
        
        <a-form-item label="摘要" name="summary">
          <a-textarea 
            v-model:value="article.summary"
            :rows="3"
            placeholder="请输入文章摘要"
          />
        </a-form-item>
        
        <a-form-item label="内容" name="content">
          <AgEditor 
            ref="editorRef"
            v-model="article.content"
            :height="500"
            :uploadConfig="uploadConfig"
            @change="handleContentChange"
          />
        </a-form-item>
        
        <a-form-item>
          <a-space>
            <a-button 
              type="primary" 
              :loading="submitting"
              @click="handleSubmit"
            >
              发布
            </a-button>
            
            <a-button @click="handlePreview">
              预览
            </a-button>
            
            <a-button @click="handleReset">
              重置
            </a-button>
          </a-space>
        </a-form-item>
      </a-form>
    </a-card>
    
    <!-- 预览模态框 -->
    <a-modal
      v-model:visible="previewVisible"
      title="预览"
      width="800px"
      :footer="null"
    >
      <div class="article-preview">
        <h2>{{ article.title }}</h2>
        <div class="summary">{{ article.summary }}</div>
        <div class="content" v-html="article.content"></div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import AgEditor from '/@/components/ag-editor'
import { articleApi } from '/@/api/article'
import { uploadApi } from '/@/api/upload'

const formRef = ref()
const editorRef = ref()
const submitting = ref(false)
const previewVisible = ref(false)

const article = reactive({
  title: '',
  summary: '',
  content: ''
})

const rules = {
  title: [{ required: true, message: '请输入标题' }],
  content: [{ required: true, message: '请输入内容' }]
}

const uploadConfig = {
  uploadImage: {
    maxFileSize: 5 * 1024 * 1024, // 5MB
    customUpload: async (file, insertFn) => {
      try {
        const res = await uploadApi.uploadImage(file)
        insertFn(res.url, res.name, res.url)
      } catch (error) {
        message.error('图片上传失败')
      }
    }
  }
}

function handleContentChange(html) {
  console.log('Content changed:', html.length)
}

async function handleSubmit() {
  try {
    await formRef.value.validate()
    
    submitting.value = true
    await articleApi.create(article)
    
    message.success('发布成功')
    handleReset()
  } catch (error) {
    if (error.errorFields) {
      message.error('请完善表单信息')
    } else {
      message.error('发布失败')
    }
  } finally {
    submitting.value = false
  }
}

function handlePreview() {
  if (!article.content) {
    message.warning('请先输入内容')
    return
  }
  previewVisible.value = true
}

function handleReset() {
  article.title = ''
  article.summary = ''
  article.content = ''
  editorRef.value?.clear()
}
</script>

<style scoped>
.article-preview {
  padding: 20px
}

.article-preview h2 {
  margin-bottom: 12px
}

.article-preview .summary {
  color: #666;
  margin-bottom: 20px;
  padding: 10px;
  background: #f5f5f5;
  border-left: 3px solid #1890ff
}

.article-preview .content {
  line-height: 1.8
}
</style>
```

## 🎨 工具栏配置

### 常用工具

```javascript
const toolbarConfig = {
  toolbarKeys: [
    // 标题
    'headerSelect',
    
    // 字体样式
    'bold',
    'italic',
    'underline',
    'through',
    
    // 颜色
    'color',
    'bgColor',
    
    // 对齐
    'justifyLeft',
    'justifyCenter',
    'justifyRight',
    
    // 列表
    'bulletedList',
    'numberedList',
    
    // 插入
    'uploadImage',
    'uploadVideo',
    'insertLink',
    'insertTable',
    'codeBlock',
    
    // 其他
    'undo',
    'redo',
    'fullScreen'
  ]
}
```

## ⚠️ 注意事项

1. **内容清理**
   - 编辑器会自动清理不安全的 HTML
   - 粘贴内容会自动过滤样式

2. **图片上传**
   - 需要配置上传服务器
   - 建议限制图片大小
   - 建议限制图片类型

3. **性能优化**
   - 大量内容时注意性能
   - 建议设置内容长度限制

4. **移动端支持**
   - 编辑器在移动端体验较差
   - 建议移动端使用简化版本

## 🔗 相关资源

- [wangEditor 官方文档](https://www.wangeditor.com/)
- [Vue 3 组件库](https://www.wangeditor.com/v5/for-frame.html#vue3)

---

**版本**: 1.0.0
**更新时间**: 2024-01-XX
**Vue 版本**: Vue 3 + Composition API
