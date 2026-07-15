<template>
  <div class="ag-upload">
    <a-upload
      :name="name"
      :action="action"
      :headers="headers"
      :accept="accept"
      :multiple="multiple"
      :show-upload-list="showUploadList"
      :file-list="fileList"
      :list-type="listType"
      :custom-request="readOnly ? undefined : customRequest"
      :disabled="readOnly"
      :before-upload="beforeUpload"
      @change="handleChange"
      @preview="handlePreview"
    >
      <slot name="uploadSlot" :loading="loading" v-if="!readOnly && (replaceMode || fileList.length < num)">
        <a-button>
          <upload-outlined /> {{ t('components.upload') }}
        </a-button>
      </slot>
      <slot v-else />
    </a-upload>
  </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/store/modules/system/user'
import { upload, uploadFile } from '@/lib/ag-axios'
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { viewerApi } from '@/utils/viewer-api'
import { useInjectFormItemContext } from 'ant-design-vue/es/form/FormItemContext'

const { t } = useI18n()

const props = defineProps({
  name: { type: String, default: 'file' },
  bindName: { type: String, default: '' },
  action: { type: String, default: '' },
  accept: { type: String, default: '' },
  multiple: { type: Boolean, default: false },
  urls: { type: Array, default: () => [] },
  listType: { type: String, default: 'picture' },
  showUploadList: { type: [Boolean, Object], default: true },
  size: { type: Number, default: 10 },
  num: { type: Number, default: 1 },
  replaceMode: { type: Boolean, default: false },
  readOnly: { type: Boolean, default: false },
  beforeUpload: { type: Function, default: undefined }
})

const emit = defineEmits(['change', 'success', 'error', 'uploadSuccess'])

/**
 * 表单上下文（自动检测是否在 a-form-item 内部）
 * 如果组件在 a-form-item 内，则自动获得表单验证能力
 */
const formItemContext = useInjectFormItemContext()

const fileList = ref([])
const loading = ref(false)

const headers = computed(() => {
  const token = useUserStore().getToken
  return token ? { [ACCESS_TOKEN_NAME]: `Bearer ${token}` } : {}
})

function getFileItems(fileList) {
  const fileItems = []
  for (const item of fileList) {
    const url = item.response.data
    item.name = url.split('/').pop()
    item.url = url
    item.thumbUrl = url
    fileItems.push(item)
  }
  return fileItems
}

function getDefaultFileList(urls) {
  const fileItems = []
  for (const i in urls) {
    const url = urls[i]
    if (!url || url?.length <= 0) {
      continue
    }
    fileItems.push({
      uid: i,
      name: url.split('/').pop(),
      status: 'done',
      url: url,
      thumbUrl: url
    })
  }
  return fileItems
}

const fileItems = getDefaultFileList(props.urls)
fileList.value = [...fileItems]

watch(
  () => props.urls,
  () => {
    const fileItems = getDefaultFileList(props.urls)
    fileList.value = [...fileItems]
  }
)

function handleChange(info) {
  emit('change', info)
  const res = info.file.response
  if (info.file.status === 'uploading') {
    loading.value = true
    fileList.value = [...info.fileList]
  }
  if (info.file.status === 'done') {
    if (res && res.code !== 0) {
      message.error(res.msg)
    }
    loading.value = false
    fileList.value = getFileItems(info.fileList)
    emit('uploadSuccess', props.bindName, fileList.value)
    emit('success', info.file.response)
    // 如果组件在 a-form-item 内，上传成功后自动触发表单验证状态更新
    // 确保 formItemContext 和 onFieldChange 方法存在
    if (formItemContext && typeof formItemContext.onFieldChange === 'function') {
      formItemContext.onFieldChange()
    }
  } else if (info.file.status === 'removed') {
    fileList.value = getFileItems(info.fileList)
    emit('uploadSuccess', props.bindName, fileList.value)
    // 如果组件在 a-form-item 内，删除文件后自动触发表单验证状态更新
    // 确保 formItemContext 和 onFieldChange 方法存在
    if (formItemContext && typeof formItemContext.onFieldChange === 'function') {
      formItemContext.onFieldChange()
    }
  } else if (info.file.status === 'error') {
    message.error('上传失败')
    emit('error', info.file.error)
  }
}

/**
 * 自定义上传请求处理
 * @param {Object} options - 上传选项
 * @param {File} options.file - 文件对象
 * @param {Function} options.onSuccess - 成功回调
 * @param {Function} options.onError - 失败回调
 */
async function customRequest({ file, onSuccess, onError }) {
  loading.value = true
  try {
    const ossFileUrl = await uploadFile(props.action, file)
    loading.value = false
    fileList.value = getDefaultFileList([ossFileUrl])
    onSuccess({ code: 0, msg: 'SUCCESS', data: ossFileUrl })
  } catch (error) {
    loading.value = false
    onError(error)
  }
}

function isAssetTypeAnImage(fileName, fileType) {
  if (fileType) {
    return fileType.startsWith('image')
  }
  let suffix = ''
  const fileArr = fileName.split('.')
  suffix = fileArr[fileArr.length - 1]
  if (suffix !== '') {
    suffix = suffix.toLocaleLowerCase()
    const imglist = ['png', 'jpg', 'jpeg', 'bmp', 'gif', 'svg', 'ico']
    return imglist.find((item) => item === suffix)
  }
  return false
}

function handlePreview(info) {
  if (isAssetTypeAnImage(info.url, info.type)) {
    viewerApi({
      images: [info.url],
      options: {
        initialViewIndex: 0
      }
    })
  }
}

defineExpose({
  loading,
  fileList,
  handleChange,
  customRequest
})
</script>

<style scoped></style>