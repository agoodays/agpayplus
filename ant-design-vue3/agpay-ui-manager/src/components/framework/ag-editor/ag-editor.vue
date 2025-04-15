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
      :style="{ height: height + 'px' }"
      :value="value"
      @input="$emit('input', $event)"
      :defaultConfig="editorConfig"
      :mode="mode"
      @onCreated="handleCreated"
    />
  </div>
</template>

<script setup>
import { shallowRef, onBeforeUnmount } from 'vue';
import { Editor, Toolbar } from '@wangeditor/editor-for-vue';
import { upload } from '@/api/manage';
import appConfig from '@/config/appConfig';
import storage from '@/utils/agpayStorageWrapper';
import '@wangeditor/editor/dist/css/style.css'; // 引入样式

// Props
defineProps({
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
          customUpload: (file, insertFn) => {
            upload.getFormParams(upload.form, file.name, file.size).then((res) => {
              const isLocalFile = res.formActionUrl === 'LOCAL_SINGLE_FILE_URL';
              const formParams = isLocalFile
                ? res.formParams
                : {
                    OSSAccessKeyId: res.formParams.ossAccessKeyId,
                    key: res.formParams.key,
                    Signature: res.formParams.signature,
                    policy: res.formParams.policy,
                    success_action_status: res.formParams.successActionStatus,
                  };
              const data = Object.assign(formParams, { file: file });
              const formActionUrl = isLocalFile ? upload.form : res.formActionUrl;
              upload.singleFile(formActionUrl, data).then((response) => {
                const ossFileUrl = isLocalFile ? response : res.ossFileUrl;
                insertFn(ossFileUrl, file.name, ossFileUrl);
              });
            });
          },
        },
        uploadVideo: {
          server: upload.form,
          headers: getHeaders(),
          fieldName: 'file',
          customUpload: (file, insertFn) => {
            upload.getFormParams(upload.form, file.name, file.size).then((res) => {
              const isLocalFile = res.formActionUrl === 'LOCAL_SINGLE_FILE_URL';
              const formParams = isLocalFile
                ? res.formParams
                : {
                    OSSAccessKeyId: res.formParams.ossAccessKeyId,
                    key: res.formParams.key,
                    Signature: res.formParams.signature,
                    policy: res.formParams.policy,
                    success_action_status: res.formParams.successActionStatus,
                  };
              const data = Object.assign(formParams, { file: file });
              const formActionUrl = isLocalFile ? upload.form : res.formActionUrl;
              upload.singleFile(formActionUrl, data).then((response) => {
                const ossFileUrl = isLocalFile ? response : res.ossFileUrl;
                insertFn(ossFileUrl, ossFileUrl);
              });
            });
          },
        },
      },
    }),
  },
  height: { type: Number, default: 500 },
  value: { type: String, default: '' },
});

// Emits
defineEmits(['input']);

// 编辑器实例
const editorRef = shallowRef();

// 获取请求头
function getHeaders() {
  const headers = {};
  headers[appConfig.ACCESS_TOKEN_NAME] = `Bearer ${storage.getToken()}`; // token
  return headers;
}

// 处理编辑器创建
const handleCreated = (editor) => {
  editorRef.value = editor; // 记录 editor 实例
};

// 组件销毁时销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value;
  if (editor) {
    editor.destroy();
  }
});

// 模式
const mode = 'default'; // 或 'simple'
</script>

<style scoped>
/* 样式可以根据需要调整 */
</style>