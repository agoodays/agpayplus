<template>
  <div class="upload-demo">
    <a-space direction="vertical" style="width: 100%" :size="24">
      <!-- 组件概览 -->
      <a-card title="ag-upload 文件上传组件示例">
        <a-alert
          message="文件上传组件"
          description="支持基础上传、图片上传、拖拽上传、文件类型限制等多种功能"
          type="success"
          show-icon
          class="mb-4"
        />
      </a-card>

      <!-- 基础上传 -->
      <a-card title="1. 基础文件上传">
        <a-alert
          message="基础用法"
          description="点击上传按钮选择文件，支持拖拽上传"
          type="info"
          show-icon
          class="mb-4"
        />

        <ag-upload v-model="fileList1" :max-count="5">
          <a-button>
            <UploadOutlined />
            选择文件
          </a-button>
        </ag-upload>

        <div class="mt-3">
          <strong>已上传文件：</strong>
          <pre>{{ JSON.stringify(fileList1, null, 2) }}</pre>
        </div>
      </a-card>

      <!-- 图片上传 - 卡片式 -->
      <a-card title="2. 图片上传（卡片式）">
        <a-alert message="图片上传" description="支持预览、删除，最多上传3张图片" type="info" show-icon class="mb-4" />

        <ag-upload v-model="fileList2" :max-count="3" accept="image/*" list-type="picture-card">
          <div v-if="fileList2.length < 3">
            <PlusOutlined />
            <div class="upload-text">上传图片</div>
          </div>
        </ag-upload>
      </a-card>

      <!-- 图片上传 - 列表式 -->
      <a-card title="3. 图片上传（列表式）">
        <a-alert message="列表式上传" description="以列表形式展示上传的图片" type="info" show-icon class="mb-4" />

        <ag-upload v-model="fileList3" :max-count="5" accept="image/*" list-type="picture">
          <a-button>
            <UploadOutlined />
            上传图片
          </a-button>
        </ag-upload>
      </a-card>

      <!-- 拖拽上传 -->
      <a-card title="4. 拖拽上传">
        <a-alert message="拖拽上传" description="支持点击或拖拽文件到虚线框内上传" type="info" show-icon class="mb-4" />

        <ag-upload v-model="fileList4" :max-count="10" list-type="text" :show-upload-list="true">
          <p class="ant-upload-drag-icon">
            <InboxOutlined />
          </p>
          <p class="ant-upload-text">点击或拖拽文件到此区域上传</p>
          <p class="ant-upload-hint">支持单个或批量上传，最多10个文件</p>
        </ag-upload>
      </a-card>

      <!-- 文件类型限制 -->
      <a-card title="5. 文件类型限制">
        <a-alert
          message="文件类型限制"
          description="只允许上传 PDF、Word、Excel 文件"
          type="warning"
          show-icon
          class="mb-4"
        />

        <ag-upload v-model="fileList5" :max-count="5" accept=".pdf,.doc,.docx,.xls,.xlsx" list-type="text">
          <a-button>
            <FileAddOutlined />
            上传文档
          </a-button>
        </ag-upload>

        <a-divider />
        <p>允许的文件类型：</p>
        <ul>
          <li>PDF 文档 (.pdf)</li>
          <li>Word 文档 (.doc, .docx)</li>
          <li>Excel 表格 (.xls, .xlsx)</li>
        </ul>
      </a-card>

      <!-- 实际应用示例 -->
      <a-card title="6. 实际应用示例 - 商户资质上传">
        <a-form :model="merchantForm" :label-col="{ span: 4 }" :wrapper-col="{ span: 16 }">
          <a-form-item label="营业执照" required>
            <ag-upload v-model="merchantForm.businessLicense" :max-count="1" accept="image/*" list-type="picture-card">
              <div v-if="merchantForm.businessLicense.length === 0">
                <PlusOutlined />
                <div class="upload-text">上传营业执照</div>
              </div>
            </ag-upload>
            <div class="form-tip">请上传清晰的营业执照照片</div>
          </a-form-item>

          <a-form-item label="法人身份证" required>
            <a-space direction="vertical" style="width: 100%" :size="12">
              <div>
                <div class="mb-2">身份证正面：</div>
                <ag-upload v-model="merchantForm.idCardFront" :max-count="1" accept="image/*" list-type="picture-card">
                  <div v-if="merchantForm.idCardFront.length === 0">
                    <PlusOutlined />
                    <div class="upload-text">正面</div>
                  </div>
                </ag-upload>
              </div>
              <div>
                <div class="mb-2">身份证反面：</div>
                <ag-upload v-model="merchantForm.idCardBack" :max-count="1" accept="image/*" list-type="picture-card">
                  <div v-if="merchantForm.idCardBack.length === 0">
                    <PlusOutlined />
                    <div class="upload-text">反面</div>
                  </div>
                </ag-upload>
              </div>
            </a-space>
          </a-form-item>

          <a-form-item label="其他资质">
            <ag-upload
              v-model="merchantForm.otherQualifications"
              :max-count="5"
              accept="image/*,.pdf"
              list-type="picture"
            >
              <a-button>
                <UploadOutlined />
                上传其他资质
              </a-button>
            </ag-upload>
            <div class="form-tip">可上传行业许可证、资质证书等（最多5个文件）</div>
          </a-form-item>

          <a-form-item :wrapper-col="{ offset: 4, span: 16 }">
            <a-space>
              <a-button type="primary" @click="handleSubmit"> 提交审核 </a-button>
              <a-button @click="handleReset"> 重置 </a-button>
              <a-button @click="handlePreview"> 预览数据 </a-button>
            </a-space>
          </a-form-item>
        </a-form>
      </a-card>

      <!-- 数据预览 -->
      <a-card title="表单数据预览">
        <pre>{{ JSON.stringify(merchantForm, null, 2) }}</pre>
      </a-card>
    </a-space>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { UploadOutlined, PlusOutlined, InboxOutlined, FileAddOutlined } from '@ant-design/icons-vue'
import { AgCard, AgUpload } from '@/components'

// 基础上传
const fileList1 = ref([])

// 图片上传 - 卡片式
const fileList2 = ref([])

// 图片上传 - 列表式
const fileList3 = ref([])

// 拖拽上传
const fileList4 = ref([])

// 文件类型限制
const fileList5 = ref([])

// 商户表单
const merchantForm = reactive({
  businessLicense: [],
  idCardFront: [],
  idCardBack: [],
  otherQualifications: []
})

function handleSubmit() {
  // 验证必填项
  if (merchantForm.businessLicense.length === 0) {
    message.warning('请上传营业执照')
    return
  }
  if (merchantForm.idCardFront.length === 0 || merchantForm.idCardBack.length === 0) {
    message.warning('请上传完整的身份证照片')
    return
  }

  console.log('提交数据:', merchantForm)
  message.success('提交成功')
}

function handleReset() {
  merchantForm.businessLicense = []
  merchantForm.idCardFront = []
  merchantForm.idCardBack = []
  merchantForm.otherQualifications = []
  message.info('表单已重置')
}

function handlePreview() {
  console.log('表单数据:', merchantForm)
  message.info('数据已输出到控制台')
}
</script>

<style scoped>
.upload-demo {
  /* padding: 24px; */
}

.upload-text {
  margin-top: 8px;
}

.form-tip {
  color: var(--text-color-muted);
  font-size: 12px;
  margin-top: 4px;
}

.mb-2 {
  margin-bottom: 8px;
}

.mb-4 {
  margin-bottom: 16px;
}

.mt-3 {
  margin-top: 12px;
}

pre {
  /* background: #f5f5f5; */
  padding: 16px;
  border-radius: 4px;
  overflow: auto;
  max-height: 400px;
}
</style>
