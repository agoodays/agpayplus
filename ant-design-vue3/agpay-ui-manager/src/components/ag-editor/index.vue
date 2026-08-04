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
/**
 * AgEditor - 富文本编辑器
 *
 * 基于 wangeditor 封装，支持图片/视频上传至 OSS。
 * 编辑器组件和样式按需动态加载，避免首屏体积过大。
 *
 * @example
 * <AgEditor v-model="form.content" :height="600" />
 */
import { ref, shallowRef, watch, onBeforeUnmount, computed, onMounted } from 'vue'
import { Skeleton } from 'ant-design-vue'
import { upload, uploadFile } from '@/lib/ag-axios'
import { appDefaultConfig } from '@/config/app-config'
import { useUserStore } from '@/store/modules/system/user'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  /** 编辑器高度（px），不小于 300 */
  height: {
    type: Number,
    default: 500
  },
  /** 工具栏配置 */
  toolbarConfig: {
    type: Object,
    default: () => ({})
  },
  /** 编辑器配置 */
  editorConfig: {
    type: Object,
    default: () => ({
      placeholder: '请输入内容...'
    })
  },
  /** 编辑器模式：default | simple */
  mode: {
    type: String,
    default: 'default',
    validator: (value) => ['default', 'simple'].includes(value)
  },
  /** 上传配置（预留） */
  uploadConfig: {
    type: Object,
    default: () => null
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

/** 编辑器实例（使用 shallowRef 避免深层响应式开销） */
const editorRef = shallowRef()

/** 内部 HTML 值 */
const internalValue = ref(props.modelValue)

/** 编辑器组件（动态加载） */
const Editor = shallowRef(null)
/** 工具栏组件（动态加载） */
const Toolbar = shallowRef(null)
/** 编辑器是否已加载完成 */
const editorLoaded = ref(false)

/** 编辑器高度（确保不小于 300px） */
const editorHeight = computed(() => Math.max(props.height, 300))

// 用户 store（在 setup 顶层初始化，避免 computed 内重复调用）
const userStore = useUserStore()

/** 请求头（带 token），使用 computed 缓存 */
const uploadHeaders = computed(() => {
  const headers = {}
  headers[appDefaultConfig.ACCESS_TOKEN_NAME] = `Bearer ${userStore.getToken}`
  return headers
})

/** 合并后的编辑器配置（含图片/视频上传） */
const mergedEditorConfig = computed(() => {
  return {
    ...props.editorConfig,
    MENU_CONF: {
      ...props.editorConfig.MENU_CONF,
      // 自定义插入图片
      uploadImage: {
        server: upload.form,
        headers: uploadHeaders.value,
        fieldName: 'file',
        customUpload: async (file, insertFn) => {
          const ossFileUrl = await uploadFile(upload.form, file)
          insertFn(ossFileUrl, file.name, ossFileUrl)
        }
      },
      // 自定义插入视频
      uploadVideo: {
        server: upload.form,
        headers: uploadHeaders.value,
        fieldName: 'file',
        customUpload: async (file, insertFn) => {
          const ossFileUrl = await uploadFile(upload.form, file)
          insertFn(ossFileUrl, ossFileUrl)
        }
      }
    }
  }
})

// 按需动态加载编辑器组件和样式
onMounted(async () => {
  const module = await import('@wangeditor/editor-for-vue')
  Editor.value = module.Editor
  Toolbar.value = module.Toolbar
  editorLoaded.value = true
  // 动态加载样式
  await import('@wangeditor/editor/dist/css/style.css')
})

// 监听外部值变化，同步到内部
watch(
  () => props.modelValue,
  (newVal) => {
    if (newVal !== internalValue.value) {
      internalValue.value = newVal
    }
  }
)

/** 编辑器创建回调 */
function handleCreated(editor) {
  editorRef.value = editor
}

/** 编辑器内容变化回调 */
function handleChange(editor) {
  const html = editor.getHtml()
  emit('update:modelValue', html)
  emit('change', html)
}

// 组件销毁时销毁编辑器实例，避免内存泄漏
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor) {
    editor.destroy()
  }
})

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
