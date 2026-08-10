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
      <template v-if="!readOnly && (replaceMode || fileList.length < num)">
        <slot name="uploadSlot" :loading="loading">
          <a-button class="ag-upload-btn">
            <loading-outlined v-if="loading" /><upload-outlined v-else /> {{ t('components.upload') }}
          </a-button>
        </slot>
      </template>
      <slot v-else />
    </a-upload>
  </div>
</template>

<script setup>
import { ACCESS_TOKEN_NAME } from '@/constants/system/token-const'
import { uploadFile } from '@/lib/ag-axios'
import { useUserStore } from '@/store/modules/system/user'
import { viewerApi } from '@/utils/viewer-api'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { useInjectFormItemContext } from 'ant-design-vue/es/form/FormItemContext'
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

/** 用户 store（在 setup 顶层初始化，避免在 computed 中重复调用） */
const userStore = useUserStore()

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
  const token = userStore.getToken
  return token ? { [ACCESS_TOKEN_NAME]: `Bearer ${token}` } : {}
})

/**
 * 从上传完成后的 fileList 中提取文件项
 * @param {Array} fileList - a-upload 的 fileList
 * @returns {Array} 处理后的文件项数组
 */
function getFileItems(fileList) {
  const fileItems = []
  for (const item of fileList) {
    const url = item?.response?.data
    if (!url) continue
    item.name = url.split('/').pop()
    item.url = url
    item.thumbUrl = url
    fileItems.push(item)
  }
  return fileItems
}

/**
 * 根据 url 数组构建默认的 fileList
 * @param {Array} urls - 文件 url 数组
 * @returns {Array} 初始 fileList
 */
function getDefaultFileList(urls) {
  const fileItems = []
  urls.forEach((url, index) => {
    if (!url || url.length <= 0) return
    fileItems.push({
      uid: String(index),
      name: url.split('/').pop(),
      status: 'done',
      url: url,
      thumbUrl: url
    })
  })
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