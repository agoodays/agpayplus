<template>
  <div class="editor-demo-container">
    <a-space direction="vertical" style="width: 100%" :size="24">
      <!-- 基础编辑器 -->
      <a-card title="1. 基础富文本编辑器">
        <a-alert
          message="基础用法"
          description="支持文本编辑、格式化、插入图片/链接等功能"
          type="info"
          show-icon
          class="mb-4"
        />

        <ag-editor v-model="content1" :height="300" placeholder="请输入内容..." />

        <a-divider />
        <div>
          <strong>HTML 内容：</strong>
          <pre>{{ content1 }}</pre>
        </div>
      </a-card>

      <!-- 简洁模式 -->
      <a-card title="2. 简洁模式编辑器">
        <a-alert
          message="简洁模式"
          description="只包含基础的文本编辑功能，适合简单的内容编辑"
          type="info"
          show-icon
          class="mb-4"
        />

        <ag-editor v-model="content2" :height="200" mode="simple" placeholder="请输入简单文本..." />
      </a-card>

      <!-- 自定义高度 -->
      <a-card title="3. 自定义高度">
        <a-alert
          message="自定义高度"
          description="可以通过 height 属性设置编辑器高度"
          type="info"
          show-icon
          class="mb-4"
        />

        <a-radio-group v-model:value="editorHeight" button-style="solid" class="mb-3">
          <a-radio-button :value="200">200px</a-radio-button>
          <a-radio-button :value="400">400px</a-radio-button>
          <a-radio-button :value="600">600px</a-radio-button>
        </a-radio-group>

        <ag-editor v-model="content3" :height="editorHeight" placeholder="请输入内容..." />
      </a-card>

      <!-- 实际应用 - 文章编辑 -->
      <a-card title="4. 实际应用 - 文章编辑">
        <a-form
          ref="articleFormRef"
          :model="articleForm"
          :rules="articleRules"
          :label-col="{ span: 2 }"
          :wrapper-col="{ span: 22 }"
        >
          <a-form-item label="标题" name="title">
            <a-input v-model:value="articleForm.title" placeholder="请输入文章标题" size="large" />
          </a-form-item>

          <a-form-item label="分类" name="category">
            <a-select v-model:value="articleForm.category" placeholder="请选择文章分类" style="width: 200px">
              <a-select-option value="news">新闻资讯</a-select-option>
              <a-select-option value="tech">技术文章</a-select-option>
              <a-select-option value="product">产品动态</a-select-option>
              <a-select-option value="announcement">公告通知</a-select-option>
            </a-select>
          </a-form-item>

          <a-form-item label="标签" name="tags">
            <a-select v-model:value="articleForm.tags" mode="tags" placeholder="请输入标签" style="width: 100%" />
          </a-form-item>

          <a-form-item label="摘要" name="summary">
            <a-textarea v-model:value="articleForm.summary" :rows="3" placeholder="请输入文章摘要" />
          </a-form-item>

          <a-form-item label="内容" name="content">
            <ag-editor v-model="articleForm.content" :height="500" placeholder="请输入文章内容..." />
          </a-form-item>

          <a-form-item :wrapper-col="{ offset: 2, span: 22 }">
            <a-space>
              <a-button type="primary" @click="handlePublish"> 发布文章 </a-button>
              <a-button @click="handleSaveDraft"> 保存草稿 </a-button>
              <a-button @click="handlePreviewArticle"> 预览 </a-button>
              <a-button @click="handleClearArticle"> 清空 </a-button>
            </a-space>
          </a-form-item>
        </a-form>
      </a-card>

      <!-- 实际应用 - 公告编辑 -->
      <a-card title="5. 实际应用 - 系统公告">
        <a-form :model="noticeForm" :label-col="{ span: 2 }" :wrapper-col="{ span: 22 }">
          <a-form-item label="标题">
            <a-input v-model:value="noticeForm.title" placeholder="请输入公告标题" />
          </a-form-item>

          <a-form-item label="类型">
            <a-radio-group v-model:value="noticeForm.type">
              <a-radio value="info">普通公告</a-radio>
              <a-radio value="warning">重要通知</a-radio>
              <a-radio value="urgent">紧急公告</a-radio>
            </a-radio-group>
          </a-form-item>

          <a-form-item label="内容">
            <ag-editor v-model="noticeForm.content" :height="300" placeholder="请输入公告内容..." />
          </a-form-item>

          <a-form-item label="发布范围">
            <a-checkbox-group v-model:value="noticeForm.targetUsers">
              <a-checkbox value="all">全部用户</a-checkbox>
              <a-checkbox value="merchant">商户</a-checkbox>
              <a-checkbox value="agent">代理商</a-checkbox>
              <a-checkbox value="admin">管理员</a-checkbox>
            </a-checkbox-group>
          </a-form-item>

          <a-form-item :wrapper-col="{ offset: 2, span: 22 }">
            <a-space>
              <a-button type="primary" @click="handlePublishNotice"> 发布公告 </a-button>
              <a-button @click="handlePreviewNotice"> 预览 </a-button>
            </a-space>
          </a-form-item>
        </a-form>
      </a-card>
    </a-space>

    <!-- 预览模态框 -->
    <a-modal v-model:open="previewOpen" :title="previewTitle" width="800px" :footer="null">
      <div class="preview-content">
        <div v-if="previewType === 'article'">
          <h2>{{ articleForm.title }}</h2>
          <div class="meta">
            <a-space>
              <span>分类：{{ getCategoryName(articleForm.category) }}</span>
              <a-divider type="vertical" />
              <span>标签：{{ articleForm.tags.join(', ') }}</span>
            </a-space>
          </div>
          <div class="summary">{{ articleForm.summary }}</div>
          <a-divider />
          <div class="content" v-html="articleForm.content"></div>
        </div>
        <div v-else-if="previewType === 'notice'">
          <h2>{{ noticeForm.title }}</h2>
          <div class="meta">
            <a-tag :color="getNoticeColor(noticeForm.type)">
              {{ getNoticeTypeName(noticeForm.type) }}
            </a-tag>
          </div>
          <a-divider />
          <div class="content" v-html="noticeForm.content"></div>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { AgCard, AgEditor } from '@/components'

// 基础内容
const content1 = ref('<p>这是初始内容</p>')
const content2 = ref('')
const content3 = ref('')
const editorHeight = ref(400)

// 文章表单
const articleFormRef = ref()
const articleForm = reactive({
  title: '',
  category: undefined,
  tags: [],
  summary: '',
  content: ''
})

const articleRules = {
  title: [{ required: true, message: '请输入标题' }],
  category: [{ required: true, message: '请选择分类' }],
  content: [{ required: true, message: '请输入内容' }]
}

// 公告表单
const noticeForm = reactive({
  title: '',
  type: 'info',
  content: '',
  targetUsers: ['all']
})

// 预览
const previewOpen = ref(false)
const previewTitle = ref('')
const previewType = ref('')

// 发布文章
async function handlePublish() {
  try {
    await articleFormRef.value.validate()
    console.log('发布文章:', articleForm)
    message.success('文章发布成功')
  } catch (error) {
    message.error('请完善文章信息')
  }
}

// 保存草稿
function handleSaveDraft() {
  console.log('保存草稿:', articleForm)
  message.success('草稿保存成功')
}

// 预览文章
function handlePreviewArticle() {
  if (!articleForm.title) {
    message.warning('请先输入标题')
    return
  }
  previewType.value = 'article'
  previewTitle.value = '文章预览'
  previewOpen.value = true
}

// 清空文章
function handleClearArticle() {
  Object.assign(articleForm, {
    title: '',
    category: undefined,
    tags: [],
    summary: '',
    content: ''
  })
  message.info('已清空')
}

// 发布公告
function handlePublishNotice() {
  if (!noticeForm.title || !noticeForm.content) {
    message.warning('请完善公告信息')
    return
  }
  console.log('发布公告:', noticeForm)
  message.success('公告发布成功')
}

// 预览公告
function handlePreviewNotice() {
  if (!noticeForm.title) {
    message.warning('请先输入标题')
    return
  }
  previewType.value = 'notice'
  previewTitle.value = '公告预览'
  previewOpen.value = true
}

// 获取分类名称
function getCategoryName(value) {
  const map = {
    news: '新闻资讯',
    tech: '技术文章',
    product: '产品动态',
    announcement: '公告通知'
  }
  return map[value] || value
}

// 获取公告类型名称
function getNoticeTypeName(value) {
  const map = {
    info: '普通公告',
    warning: '重要通知',
    urgent: '紧急公告'
  }
  return map[value] || value
}

// 获取公告颜色
function getNoticeColor(value) {
  const map = {
    info: 'blue',
    warning: 'orange',
    urgent: 'red'
  }
  return map[value] || 'blue'
}
</script>

<style scoped>
.editor-demo-container {
  /* padding: 24px */
}

.mb-3 {
  margin-bottom: 12px;
}

.mb-4 {
  margin-bottom: 16px;
}

pre {
  /* background: #f5f5f5; */
  padding: 16px;
  border-radius: 4px;
  overflow: auto;
  max-height: 300px;
}

.preview-content {
  padding: 20px;
}

.preview-content h2 {
  margin-bottom: 16px;
}

.preview-content .meta {
  color: var(--text-color-weak);
  margin-bottom: 16px;
}

.preview-content .summary {
  padding: 12px;
  background: var(--primary-color-weak);
  border-left: 3px solid var(--primary-color);
  margin-bottom: 16px;
}

.preview-content .content {
  line-height: 1.8;
}
</style>
