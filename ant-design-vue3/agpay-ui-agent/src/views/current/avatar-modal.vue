<template>
  <a-modal
    title="修改头像"
    v-model:open="localOpen"
    :mask-closable="false"
    :confirm-loading="confirmLoading"
    :width="800"
    :footer="null"
    @cancel="handleClose"
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
        <a-button @click="triggerUpload">
          <template #icon><UploadOutlined /></template>
          选择图片
        </a-button>
      </a-col>
      <a-col :lg="{ span: 2, offset: 18 }" :md="2">
        <a-button type="primary" @click="finish">保存</a-button>
      </a-col>
    </a-row>
  </a-modal>
</template>

<script setup>
/**
 * 修改头像模态框组件
 * 功能：上传和裁剪用户头像
 */
import { ref, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import { PlusOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { upload } from '@/lib/ag-axios'
import { AgUpload } from '@/components'

const icons = { PlusOutlined, UploadOutlined }

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'ok'])

const localOpen = ref(false)
const confirmLoading = ref(false)
const options = reactive({
  img: '',
  autoCrop: true,
  autoCropWidth: 200,
  autoCropHeight: 200,
  fixedBox: true
})
const previews = reactive({})

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
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
    localOpen.value = false
  }, 500)
}
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
