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
      @change="handleChange"
      @preview="handlePreview"
    >
      <slot name="uploadSlot" :loading="loading" v-if="fileList.length < num"></slot>
    </a-upload>
  </div>
</template>

<script>
import { ref, watch } from 'vue'
import appConfig from '@/config/appConfig'
import storage from '@/utils/agpayStorageWrapper'
import 'viewerjs/dist/viewer.css'

function getHeaders () {
  const headers = {}
  headers[appConfig.ACCESS_TOKEN_NAME] = storage.getToken() // token
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
  // watch: {
  //   // 监听父组件传递过来的消息变化
  //   urls (newVal) {
  //     console.log('New Urls:', newVal)
  //     const fileItems = getDefaultFileList(newVal)
  //     this.fileList.value = [...fileItems]
  //     console.log(this.fileList.value)
  //   }
  // },
  setup (props, { emit }) {
    const fileList = ref([])
    const loading = ref(false)

    // 如果父组件传过来的数据是异步获取的，则需要进行监听
    watch(() => props.urls, () => {
      console.log('New Urls:', props.urls)
      const fileItems = getDefaultFileList(props.urls)
      fileList.value = [...fileItems]
      console.log(fileList.value)
    })

    const handleChange = (info) => {
      console.log(info.fileList)
      // 限制文件数量
      /* let fileList = [...info.fileList]
      fileList = fileList.length > this.num ? fileList.splice(0 - this.num) : fileList // 取最新加入的元素
      fileList = fileList.map(file => {
        if (file.response) {
          file.url = file.response.data
        }
        return file
      }) */
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

    return {
      loading, // 上传状态
      headers: getHeaders(), // 放入token
      fileList,
      handleChange
    }
  },
  created () {
  },
  methods: {
    // // 上传回调
    // handleChange (info) {
    //   console.log(this.props.bindName)
    //   console.log(info.fileList)
    //    // 限制文件数量
    //   /* let fileList = [...info.fileList]
    //   fileList = fileList.length > this.num ? fileList.splice(0 - this.num) : fileList // 取最新加入的元素
    //   fileList = fileList.map(file => {
    //     if (file.response) {
    //       file.url = file.response.data
    //     }
    //     return file
    //   }) */
    //   this.fileList = [...info.fileList]
    //   const res = info.file.response
    //   console.log(res)
    //   if (info.file.status === 'uploading') {
    //     this.loading = true
    //   }
    //   if (info.file.status === 'done') {
    //     if (res.code !== 0) {
    //       this.$message.error(res.msg)
    //     }
    //     this.loading = false
    //     console.log({ bindName: this.props.bindName, data: res.data })
    //     this.$emit('uploadSuccess', this.bindName, res.data)
    //   } else if (info.file.status === 'error') {
    //     console.log(info)
    //     this.$message.error(`上传失败`)
    //   }
    // },
    handlePreview (info) {
      console.log(info)
      this.$viewerApi({
        images: [info.url],
        options: {
          initialViewIndex: 0
        }
      })
    },
    // 上传图片前的校验
    beforeUpload (file) {
      console.log(file)
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
