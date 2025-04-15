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

<script setup>
import { ref, watch } from 'vue';
import { upload } from '@/api/manage';
import appConfig from '@/config/appConfig';
import storage from '@/utils/agpayStorageWrapper';
import 'viewerjs/dist/viewer.css';

// Props
defineProps({
  name: { type: String, default: 'file' },
  bindName: { type: String, default: '' },
  action: { type: String, default: '' },
  accept: { type: String, default: '' },
  multiple: { type: Boolean, default: false },
  urls: { type: Array, default: () => [] },
  listType: { type: String, default: 'picture' }, // or text
  showUploadList: { type: [Boolean, Object], default: true },
  size: { type: Number, default: 10 }, // 文件大小限制
  num: { type: Number, default: 1 }, // 文件数量限制
});

// Emits
defineEmits(['uploadSuccess']);

// Local state
const fileList = ref([]);
const loading = ref(false);

// 获取请求头
const getHeaders = () => {
  const headers = {};
  headers[appConfig.ACCESS_TOKEN_NAME] = `Bearer ${storage.getToken()}`; // token
  return headers;
};

// 初始化文件列表
const getDefaultFileList = (urls) => {
  return urls
    .filter((url) => url && url.length > 0)
    .map((url, index) => ({
      uid: index,
      name: url.split('/').pop(),
      status: 'done',
      url,
      thumbUrl: url,
    }));
};

// 初始化文件列表
fileList.value = getDefaultFileList(props.urls);

// 如果父组件传过来的数据是异步获取的，则需要进行监听
watch(
  () => props.urls,
  (newUrls) => {
    fileList.value = getDefaultFileList(newUrls);
  }
);

// 上传文件时的回调
const handleChange = (info) => {
  const res = info.file.response;

  if (info.file.status === 'uploading') {
    loading.value = true;
    fileList.value = [...info.fileList];
  }

  if (info.file.status === 'done') {
    if (res.code !== 0) {
      window.$message.error(res.msg);
    }
    loading.value = false;
    fileList.value = getDefaultFileList(info.fileList.map((file) => file.response?.data || ''));
    emit('uploadSuccess', props.bindName, fileList.value);
  } else if (info.file.status === 'removed') {
    fileList.value = getDefaultFileList(info.fileList.map((file) => file.response?.data || ''));
    emit('uploadSuccess', props.bindName, fileList.value);
  } else if (info.file.status === 'error') {
    window.$message.error('上传失败');
  }
};

// 自定义上传逻辑
const customRequest = ({ file, onSuccess, onError }) => {
  loading.value = true;
  upload
    .getFormParams(props.action, file.name, file.size)
    .then((res) => {
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
      const data = Object.assign(formParams, { file });
      const formActionUrl = isLocalFile ? props.action : res.formActionUrl;

      upload
        .singleFile(formActionUrl, data)
        .then((response) => {
          loading.value = false;
          const ossFileUrl = isLocalFile ? response : res.ossFileUrl;
          fileList.value = getDefaultFileList([ossFileUrl]);
          onSuccess({ code: 0, msg: 'SUCCESS', data: ossFileUrl });
        })
        .catch((error) => {
          loading.value = false;
          onError(error);
        });
    })
    .catch(() => {
      loading.value = false;
    });
};

// 上传图片前的校验
const beforeUpload = (file) => {
  const validate = file.size / 1024 / 1024 < props.size;
  if (!validate && props.size > 0) {
    window.$message.error(`文件应小于 ${props.size}M!`);
    return false;
  }
  return true;
};

// 预览文件
const handlePreview = (file) => {
  if (isAssetTypeAnImage(file.url, file.type)) {
    window.$viewerApi({
      images: [file.url],
      options: {
        initialViewIndex: 0,
      },
    });
  }
};

// 判断是否为图片
const isAssetTypeAnImage = (fileName, fileType) => {
  if (fileType) {
    return fileType.startsWith('image');
  }
  const suffix = fileName.split('.').pop().toLowerCase();
  return ['png', 'jpg', 'jpeg', 'bmp', 'gif', 'svg', 'ico'].includes(suffix);
};

const headers = getHeaders();
</script>

<style scoped>
/* 样式可以根据需要调整 */
</style>