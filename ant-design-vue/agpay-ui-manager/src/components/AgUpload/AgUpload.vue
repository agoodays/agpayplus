<template>
  <div>
    <a-upload
      :name="name"
      :action="action"
      :headers="headers"
      :accept="accept"
      :multiple="multiple"
      :showUploadList="showUploadList"
      :before-upload="beforeUpload"
      :file-list="fileList"
      :list-type="listType"
      :custom-request="customRequest"
      @change="handleChange"
      @preview="handlePreview"
    >
      <slot name="uploadSlot" :loading="loading" v-if="fileList.length < num"></slot>
    </a-upload>
  </div>
</template>

<script>
import { ref, watch } from 'vue'
import { upload } from '@/api/manage'
import appConfig from '@/config/appConfig'
import storage from '@/utils/agpayStorageWrapper'
import 'viewerjs/dist/viewer.css'

function getHeaders () {
  const headers = {}
  headers[appConfig.ACCESS_TOKEN_NAME] = `Bearer ${storage.getToken()}` // token
  return headers
}

function getFileItems (fileList) {
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

function getDefaultFileList (urls) {
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

export default {
  name: 'AgUpload',
  props: {
    name: { type: String, default: 'file' },
    bindName: { type: String, default: '' },
    action: { type: String, default: '' },
    accept: { type: String, default: '' },
    multiple: { type: Boolean, default: false },
    urls: { type: Array, default () { return [] } },
    listType: { type: String, default: 'picture' }, // or text
    showUploadList: { type: [Boolean, Object], default: true },
    size: { type: Number, default: 10 }, // 文件大小限制
    num: { type: Number, default: 1 } // 文件数量限制
  },
  setup (props, { emit }) {
    const fileList = ref([])
    const loading = ref(false)

    // 如果父组件传过来的数据是异步获取的，则需要进行监听
    watch(() => props.urls, () => {
      const fileItems = getDefaultFileList(props.urls)
      fileList.value = [...fileItems]
    })

    const handleChange = (info) => {
      console.log(info)
      // // 限制文件数量
      // let fileList = [...info.fileList]
      // fileList = fileList.length > this.num ? fileList.splice(0 - this.num) : fileList // 取最新加入的元素
      // fileList = fileList.map(file => {
      //   if (file.response) {
      //     file.url = file.response.data
      //   }
      //   return file
      // })
      const res = info.file.response
      if (info.file.status === 'uploading') {
        loading.value = true
        fileList.value = [...info.fileList]
      }
      if (info.file.status === 'done') {
        if (res.code !== 0) {
          this.$message.error(res.msg)
        }
        loading.value = false
        fileList.value = getFileItems(info.fileList)
        emit('uploadSuccess', props.bindName, fileList.value)
      } else if (info.file.status === 'removed') {
        fileList.value = getFileItems(info.fileList)
        emit('uploadSuccess', props.bindName, fileList.value)
      } else if (info.file.status === 'error') {
        console.log(info)
        this.$message.error(`上传失败`)
      }
    }

    const customRequest = ({ file, onSuccess, onError, onProgress }) => {
      loading.value = true
      upload.getFormParams(upload.avatar, file.name, file.size).then(res => {
        const isLocalFile = res.formActionUrl === 'LOCAL_SINGLE_FILE_URL'
        const formParams = isLocalFile ? res.formParams : {
          OSSAccessKeyId: res.formParams.ossAccessKeyId,
          key: res.formParams.key,
          Signature: res.formParams.signature,
          policy: res.formParams.policy,
          success_action_status: res.formParams.successActionStatus
        }
        const data = Object.assign(formParams, { file: file })
        const formActionUrl = isLocalFile ? upload.avatar : res.formActionUrl
        upload.singleFile(formActionUrl, data).then((response) => {
          // 上传成功回调
          // onSuccess(response)
          loading.value = false
          const ossFileUrl = isLocalFile ? response : res.ossFileUrl
          fileList.value = getDefaultFileList([ossFileUrl])
        }).catch((error) => {
          // 上传失败回调
          // onError(error)
          loading.value = false
          this.$message.error(error.msg)
        })
      }).catch(() => {
        loading.value = false
      })
    }

    return {
      loading, // 上传状态
      headers: getHeaders(), // 放入token
      fileList,
      handleChange,
      customRequest
    }
  },
  created () {
  },
  methods: {
    isAssetTypeAnImage (fileName, fileType) {
      if (fileType) {
        return fileType.startsWith('image')
      }
      // 后缀获取
      let suffix = ''
      // 获取类型结果
      let result = ''
      const fileArr = fileName.split('.')
      suffix = fileArr[fileArr.length - 1]
      if (suffix !== '') {
        suffix = suffix.toLocaleLowerCase()
        // 图片格式
        const imglist = ['png', 'jpg', 'jpeg', 'bmp', 'gif', 'svg', 'ico']
        // 进行图片匹配
        result = imglist.find(item => item === suffix)
      }
      return result
    },
    handlePreview (info) {
      // console.log(info)
      if (this.isAssetTypeAnImage(info.url, info.type)) {
        this.$viewerApi({
          images: [info.url],
          options: {
            initialViewIndex: 0
          }
        })
      }
    },
    // 上传图片前的校验
    beforeUpload (file) {
      // console.log(file)
      const validate = file.size / 1024 / 1024 < this.size
      if (!validate && this.size > 0) {
        this.$message.error('文件应小于' + this.size + 'M!')
        return false
      }
      return true
    }
  }
}
</script>
