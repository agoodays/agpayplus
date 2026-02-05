<template>
  <div class="ag-editor">
    <Toolbar
      class="ag-editor-toolbar"
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      :mode="mode"
    />
    <Editor
      class="ag-editor-content"
      :style="{ height: height + 'px' }"
      v-model="internalValue"
      :defaultConfig="editorConfig"
      :mode="mode"
      @onCreated="handleCreated"
      @onChange="handleChange"
    />
  </div>
</template>

<script setup>
import { ref, shallowRef, watch, onBeforeUnmount, computed } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import '@wangeditor/editor/dist/css/style.css'

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

// 监听外部值变化
watch(() => props.modelValue, (newVal) => {
  if (newVal !== internalValue.value) {
    internalValue.value = newVal
  }
})

// 合并编辑器配置
const mergedEditorConfig = computed(() => {
  const config = { ...props.editorConfig }
  
  // 如果提供了上传配置，则合并
  if (props.uploadConfig) {
    config.MENU_CONF = config.MENU_CONF || {}
    
    if (props.uploadConfig.uploadImage) {
      config.MENU_CONF.uploadImage = props.uploadConfig.uploadImage
    }
    
    if (props.uploadConfig.uploadVideo) {
      config.MENU_CONF.uploadVideo = props.uploadConfig.uploadVideo
    }
  }
  
  return config
})

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
  border: 1px solid #ccc
}

.ag-editor-toolbar {
  border-bottom: 1px solid #ccc
}

.ag-editor-content {
  overflow-y: auto
}
</style>
