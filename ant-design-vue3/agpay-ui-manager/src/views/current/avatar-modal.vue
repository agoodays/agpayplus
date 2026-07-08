<template>
  <a-modal
    title="修改头像"
    :visible="visible"
    :mask-closable="false"
    :confirm-loading="confirmLoading"
    :width="800"
    :footer="null"
    @cancel="cancelHandel"
  >
    <a-row>
      <a-col :xs="24" :md="12" style="height: '350px'">
        <div class="avatar-upload-area">
          <ag-upload
            name="file"
            :before-upload="beforeUpload"
            :show-upload-list="false"
            :action="upload.icon"
          >
            <div class="upload-placeholder" v-if="!options.img">
              <component :is="icons.PlusOutlined" style="font-size: 48px; color: #ccc" />
              <p>点击上传图片</p>
            </div>
            <img v-else :src="options.img" style="max-width: 100%; max-height: 300px" />
          </ag-upload>
        </div>
      </a-col>
      <a-col :xs="24" :md="12" style="height: '350px'">
        <div class="avatar-upload-preview">
          <img :src="previews.url" :style="previews.img" />
        </div>
      </a-col>
    </a-row>
    <br />
    <a-row>
      <a-col :lg="2" :md="2">
        <a-button icon="upload" @click="triggerUpload">选择图片</a-button>
      </a-col>
      <a-col :lg="{ span: 2, offset: 18 }" :md="2">
        <a-button type="primary" @click="finish">保存</a-button>
      </a-col>
    </a-row>
  </a-modal>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { PlusOutlined } from '@ant-design/icons-vue'
import { upload } from '@/lib/ag-axios'
import AgUpload from '@/components/ag-upload'

const icons = { PlusOutlined }

const emit = defineEmits(['ok'])

const visible = ref(false)
const confirmLoading = ref(false)
const options = reactive({
  img: '',
  autoCrop: true,
  autoCropWidth: 200,
  autoCropHeight: 200,
  fixedBox: true
})
const previews = reactive({})

const show = (id) => {
  visible.value = true
}

const close = () => {
  visible.value = false
}

const cancelHandel = () => {
  close()
}

const triggerUpload = () => {
  document.querySelector('.ant-upload').click()
}

const beforeUpload = (file) => {
  const reader = new FileReader()
  reader.readAsDataURL(file)
  reader.onload = () => {
    options.img = reader.result
    previews.url = reader.result
    previews.img = { width: '100%', height: '100%' }
  }
  return false
}

const finish = () => {
  confirmLoading.value = true
  setTimeout(() => {
    confirmLoading.value = false
    message.success('上传成功')
    emit('ok', options.img)
    visible.value = false
  }, 500)
}

defineExpose({ show })
</script>

<style lang="less" scoped>
.avatar-upload-area {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 300px;
  border: 1px dashed #d9d9d9;
  border-radius: 4px;
  cursor: pointer;
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  color: #999;
}

.avatar-upload-preview {
  position: absolute;
  top: 50%;
  transform: translate(50%, -50%);
  width: 180px;
  height: 180px;
  border-radius: 50%;
  box-shadow: 0 0 4px #ccc;
  overflow: hidden;

  img {
    width: 100%;
    height: 100%;
  }
}
</style>
