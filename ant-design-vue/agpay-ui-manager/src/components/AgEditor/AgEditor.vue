<template>
  <div class="ag-editor">
    <Toolbar
      class="ag-editor-toolbar"
      :editor="editorRef"
      :defaultConfig="toolbarConfig"
      :mode="mode"
    />
    <Editor
      class="ag-editor-box"
      :style="{ 'height': height + 'px' }"
      :value="value"
      @input="$emit('input', $event)"
      :defaultConfig="editorConfig"
      :mode="mode"
      @onCreated="handleCreated"
    />
<!--    <Editor
        class="ag-editor-box"
        :style="{ 'height': height + 'px' }"
        v-model="valueHtml"
        :defaultConfig="editorConfig"
        :mode="mode"
        @onCreated="handleCreated"
    />-->
<!--    <Editor
        class="ag-editor-box"
        :style="{ 'height': height + 'px' }"
        v-model="valueHtml"
        :defaultConfig="editorConfig"
        :mode="mode"
        @onChange="valChange"
        @onCreated="handleCreated"
    />-->
  </div>
</template>

<script>
import { upload } from '@/api/manage'
import appConfig from '@/config/appConfig'
import storage from '@/utils/agpayStorageWrapper'
import '@wangeditor/editor/dist/css/style.css' // 引入 css
// import { onBeforeUnmount, ref, shallowRef, onMounted } from 'vue'
import { onBeforeUnmount, shallowRef } from 'vue'
// import { onBeforeUnmount, computed, shallowRef } from 'vue'
// import { onBeforeUnmount, ref, watch, shallowRef } from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'

function getHeaders () {
  const headers = {}
  headers[appConfig.ACCESS_TOKEN_NAME] = `Bearer ${storage.getToken()}` // token
  return headers
}

export default {
  name: 'AgEditor',
  props: {
    toolbarConfig: { type: Object, default: () => ({}) },
    editorConfig: {
      type: Object,
      default: () => ({
        placeholder: '请输入内容...',
        MENU_CONF: {
          // 自定义插入图片
          uploadImage: {
            server: upload.form,
            headers: getHeaders(),
            fieldName: 'file',
            customInsert: (res, insertFn) => {
              // res 即服务端的返回结果
              // 从 res 中找到 url alt href ，然后插入图片
              insertFn(
                  res.data, // 图片 src ，必须
                  res.data, // 图片描述文字，非必须
                  res.data // 图片的链接，非必须
              )
            }
          },
          uploadVideo: {
            server: upload.form,
            headers: getHeaders(),
            fieldName: 'file',
            // 自定义插入视频
            customInsert: (res, insertFn) => {
              // res 即服务端的返回结果
              // 从 res 中找到 url poster ，然后插入视频
              insertFn(
                  res.data, // 视频 src ，必须
                  res.data // 视频封面图片 url ，可选
              )
            }
          }
        }
      })
    },
    height: { type: Number, default: 500 },
    value: { type: String, default: '' }
    // modelValue: { type: String, default: '' }
  },
  // emits: ['update:modelValue'],// 定义自定义事件，可省略
  components: { Editor, Toolbar },
  setup (props, { emit }) {
    // console.log(props)

    // 编辑器实例，必须用 shallowRef
    const editorRef = shallowRef()

    // // 内容 HTML
    // const valueHtml = ref('<p>hello</p>')
    //
    // // 模拟 ajax 异步获取内容
    // onMounted(() => {
    //   setTimeout(() => {
    //     valueHtml.value = '<p>模拟 Ajax 异步设置内容</p>'
    //   }, 1500)
    // })

    // const valueHtml = computed({
    //   get: () => props.modelValue,
    //   set: (nv) => {
    //     // console.log(nv)
    //     emit('update:modelValue', nv)
    //   }
    // })

    // // 初始化
    // const valueHtml = ref(props.modelValue)
    //
    // // 如果父组件传过来的数据是异步获取的，则需要进行监听
    // watch(() => props.modelValue, () => { valueHtml.value = props.modelValue })
    //
    // // 数据双向绑定
    // function valChange (editor) {
    //   console.log(editor.getHtml())
    //   emit('update:modelValue', editor.getHtml())
    // }

    // const toolbarConfig = {}
    // const editorConfig = { placeholder: '请输入内容...' }

    // 组件销毁时，也及时销毁编辑器
    onBeforeUnmount(() => {
      const editor = editorRef.value
      // console.log(editor)
      if (editor == null) return
      editor.destroy()
    })

    const handleCreated = (editor) => {
      editorRef.value = editor // 记录 editor 实例，重要！
    }

    return {
      editorRef,
      // valueHtml,
      // valChange,
      mode: 'default', // 或 'simple'
      // toolbarConfig,
      // editorConfig,
      handleCreated
    }
  }
}
</script>

<style scoped>

</style>
