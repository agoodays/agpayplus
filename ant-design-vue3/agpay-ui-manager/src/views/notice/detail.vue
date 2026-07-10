<template>
  <ag-drawer
    v-model:open="localOpen"
    title="公告详情"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
    @close="handleClose"
  >
    <div class="article-container">
      <div class="title">{{ detailData.title }}</div>
      <div class="author">
        <span class="auther-text">作者：{{ detailData.publisher }}</span>
        <span>时间：{{ detailData.publishTime }}</span>
      </div>
      <div class="content" v-html="detailData.content"></div>
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 公告详情抽屉组件
 * 功能：展示公告的详细信息
 */
import { AgDrawer } from '@/components'
import { noticeApi } from '@/api/business/notice/notice-api'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open'])

/** 本地打开状态 */
const localOpen = ref(false)

/** 详情数据 */
const detailData = reactive({
  title: '',
  publisher: '',
  publishTime: '',
  content: ''
})

/** 重置详情数据 */
function resetDetailData() {
  Object.assign(detailData, {
    title: '',
    publisher: '',
    publishTime: '',
    content: ''
  })
}

/** 监听 open 属性变化 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val && props.recordId) {
      await loadDetail()
    } else if (!val) {
      resetDetailData()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 加载详情数据 */
const loadDetail = async () => {
  try {
    const res = await noticeApi.getById(props.recordId)
    Object.assign(detailData, res || {})
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error?.msg || error?.message || '加载详情失败')
  }
}

/** 处理关闭 */
const handleClose = () => {
  resetDetailData()
  localOpen.value = false
}
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
