<template>
  <div v-if="editorLoaded" class="ag-editor">
    <Toolbar class="ag-editor-toolbar" :editor="editorRef" :default-config="toolbarConfig" :mode="mode" />
    <Editor
      v-model="internalValue"
      class="ag-editor-content"
      :style="{ height: editorHeight + 'px' }"
      :default-config="mergedEditorConfig"
      :mode="mode"
      @on-created="handleCreated"
      @on-change="handleChange"
    />
  </div>
  <div v-else class="ag-editor-loading">
    <Skeleton active :paragraph="{ rows: 10 }" />
  </div>
</template>

<script setup>
import { ref, shallowRef, watch, onBeforeUnmount, computed, onMounted } from 'vue'
import { Skeleton } from 'ant-design-vue'

// 动态导入wangeditor
const [Editor, Toolbar, editorLoaded] = (() => {
  const loaded = ref(false)
  let EditorComponent = null
  let ToolbarComponent = null

  // 动态加载
  import('@wangeditor/editor-for-vue').then((module) => {
    EditorComponent = module.Editor
    ToolbarComponent = module.Toolbar
    loaded.value = true
  })

  // 动态加载样式
  import('@wangeditor/editor/dist/css/style.css')

  return [computed(() => EditorComponent), computed(() => ToolbarComponent), loaded]
})()
import { upload } from '@/api/manage'
import { appDefaultConfig } from '@/config/app-config'
import { useUserStore } from '@/store/modules/system/user'

// Props
const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  height: {
    type: Number,
    default: 500
  },
  toolbarConfig: {
    type: Object,
    default: () => ({})
  },
  editorConfig: {
    type: Object,
    default: () => ({
      placeholder: '请输入内容...'
    })
  },
  mode: {
    type: String,
    default: 'default',
    validator: (value) => ['default', 'simple'].includes(value)
  },
  uploadConfig: {
    type: Object,
    default: () => null
  }
})

// Emits
const emit = defineEmits(['update:modelValue', 'change'])

// 编辑器实例
const editorRef = shallowRef()

// 内部值
const internalValue = ref(props.modelValue)

// 编辑器高度（确保不小于 300px）
const editorHeight = computed(() => {
  return Math.max(props.height, 300)
})

// 用户 store
const userStore = useUserStore()

// 合并编辑器配置
const mergedEditorConfig = computed(() => {
  return {
    ...props.editorConfig,
    MENU_CONF: {
      ...props.editorConfig.MENU_CONF,
      // 自定义插入图片
      uploadImage: {
        server: upload.form,
        headers: getHeaders(),
        fieldName: 'file',
        customUpload: (file, insertFn) => {
          upload.getFormParams(upload.form, file.name, file.size).then((res) => {
            const isLocalFile = res.formActionUrl === 'LOCAL_SINGLE_FILE_URL'
            const formParams = isLocalFile
              ? res.formParams
              : {
                  OSSAccessKeyId: res.formParams.ossAccessKeyId,
                  key: res.formParams.key,
                  Signature: res.formParams.signature,
                  policy: res.formParams.policy,
                  success_action_status: res.formParams.successActionStatus
                }
            const data = Object.assign(formParams, { file: file })
            const formActionUrl = isLocalFile ? upload.form : res.formActionUrl
            upload.singleFile(formActionUrl, isLocalFile, data).then((response) => {
              const ossFileUrl = isLocalFile ? response : res.ossFileUrl
              insertFn(ossFileUrl, file.name, ossFileUrl)
            })
          })
        }
      },
      uploadVideo: {
        server: upload.form,
        headers: getHeaders(),
        fieldName: 'file',
        customUpload: (file, insertFn) => {
          upload.getFormParams(upload.form, file.name, file.size).then((res) => {
            const isLocalFile = res.formActionUrl === 'LOCAL_SINGLE_FILE_URL'
            const formParams = isLocalFile
              ? res.formParams
              : {
                  OSSAccessKeyId: res.formParams.ossAccessKeyId,
                  key: res.formParams.key,
                  Signature: res.formParams.signature,
                  policy: res.formParams.policy,
                  success_action_status: res.formParams.successActionStatus
                }
            const data = Object.assign(formParams, { file: file })
            const formActionUrl = isLocalFile ? upload.form : res.formActionUrl
            upload.singleFile(formActionUrl, isLocalFile, data).then((response) => {
              const ossFileUrl = isLocalFile ? response : res.ossFileUrl
              insertFn(ossFileUrl, ossFileUrl)
            })
          })
        }
      }
    }
  }
})

// 获取请求头
function getHeaders() {
  const headers = {}
  headers[appDefaultConfig.ACCESS_TOKEN_NAME] = `Bearer ${userStore.getToken}`
  return headers
}

// 监听外部值变化
watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal !== internalValue.value) {
      internalValue.value = newVal
    }
  }
)

// 处理编辑器创建
function handleCreated(editor) {
  editorRef.value = editor
}

// 处理内容变化
function handleChange(editor) {
  const html = editor.getHtml()
  emit('update:modelValue', html)
  emit('change', html)
}

// 组件销毁时销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor) {
    editor.destroy()
  }
})

// 暴露方法
defineExpose({
  getEditor: () => editorRef.value,
  getHtml: () => editorRef.value?.getHtml(),
  getText: () => editorRef.value?.getText(),
  isEmpty: () => editorRef.value?.isEmpty(),
  clear: () => editorRef.value?.clear(),
  focus: () => editorRef.value?.focus(),
  blur: () => editorRef.value?.blur()
})
</script>

<style scoped>
.ag-editor {
  border: 1px solid #ccc;
}

.ag-editor-toolbar {
  border-bottom: 1px solid #ccc;
}

.ag-editor-content {
  overflow-y: auto;
}
</style>
