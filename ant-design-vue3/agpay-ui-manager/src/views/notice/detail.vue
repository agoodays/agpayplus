<template>
  <a-drawer
    :visible="visible"
    :title="true ? '公告详情' : ''"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
    @close="onClose"
  >
    <div class="article-container">
      <div class="title">{{ detailData.title }}</div>
      <div class="author">
        <span class="auther-text">作者：{{ detailData.publisher }}</span>
        <span>时间：{{ detailData.publishTime }}</span>
      </div>
      <div class="content" v-html="detailData.content"></div>
    </div>
  </a-drawer>
</template>

<script setup>
import { noticeApi } from '@/api/business/notice/notice-api'
import { ref } from 'vue'

defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const detailData = ref({})
const recordId = ref(null)
const visible = ref(false)

function show(currentRecordId) {
  recordId.value = currentRecordId
  visible.value = true
  noticeApi.getById(currentRecordId).then((res) => {
    detailData.value = res || {}
  })
}

function onClose() {
  visible.value = false
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less">
.article-container {
  max-width: 100%;
  width: 800px;
  margin: 0 auto;
  padding: 26px 6px 6px;

  .title {
    height: 50px;
    line-height: 1.75;
    font-size: 30px;
    font-weight: 700;
    letter-spacing: 3px;
    margin-bottom: 15px;
  }

  .subtitle {
    box-sizing: border-box;
    width: 400px;
    font-size: 15px;
  }

  .author {
    width: 400px;
    color: #969696;
    margin-bottom: 15px;
  }

  .author .auther-text {
    margin-right: 20px;
  }

  .content {
    margin: 45px auto 0;
    line-height: 1.5;
  }
}
</style>
