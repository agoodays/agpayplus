<template>
  <div class="upload-demo-container">
    <a-space direction="vertical" style="width: 100%" :size="24">
      <!-- 基础上传 -->
      <AgCard title="1. 基础文件上传">
        <a-alert
          message="基础用法"
          description="点击上传按钮选择文件，支持拖拽上传"
          type="info"
          show-icon
          class="mb-4"
        />
        
        <AgUpload
          v-model="fileList1"
          :maxCount="5"
        >
          <a-button>
            <UploadOutlined />
            选择文件
          </a-button>
        </AgUpload>

        <div class="mt-3">
          <strong>已上传文件：</strong>
          <pre>{{ JSON.stringify(fileList1, null, 2) }}</pre>
        </div>
      </AgCard>

      <!-- 图片上传 - 卡片式 -->
      <AgCard title="2. 图片上传（卡片式）">
        <a-alert
          message="图片上传"
          description="支持预览、删除，最多上传3张图片"
          type="info"
          show-icon
          class="mb-4"
        />
        
        <AgUpload
          v-model="fileList2"
          :maxCount="3"
          accept="image/*"
          listType="picture-card"
        >
          <div v-if="fileList2.length < 3">
            <PlusOutlined />
            <div class="upload-text">上传图片</div>
          </div>
        </AgUpload>
      </AgCard>

      <!-- 图片上传 - 列表式 -->
      <AgCard title="3. 图片上传（列表式）">
        <a-alert
          message="列表式上传"
          description="以列表形式展示上传的图片"
          type="info"
          show-icon
          class="mb-4"
        />
        
        <AgUpload
          v-model="fileList3"
          :maxCount="5"
          accept="image/*"
          listType="picture"
        >
          <a-button>
            <UploadOutlined />
            上传图片
          </a-button>
        </AgUpload>
      </AgCard>

      <!-- 拖拽上传 -->
      <AgCard title="4. 拖拽上传">
        <a-alert
          message="拖拽上传"
          description="支持点击或拖拽文件到虚线框内上传"
          type="info"
          show-icon
          class="mb-4"
        />
        
        <AgUpload
          v-model="fileList4"
          :maxCount="10"
          listType="text"
          :showUploadList="true"
        >
          <p class="ant-upload-drag-icon">
            <InboxOutlined />
          </p>
          <p class="ant-upload-text">点击或拖拽文件到此区域上传</p>
          <p class="ant-upload-hint">
            支持单个或批量上传，最多10个文件
          </p>
        </AgUpload>
      </AgCard>

      <!-- 文件类型限制 -->
      <AgCard title="5. 文件类型限制">
        <a-alert
          message="文件类型限制"
          description="只允许上传 PDF、Word、Excel 文件"
          type="warning"
          show-icon
          class="mb-4"
        />
        
        <AgUpload
          v-model="fileList5"
          :maxCount="5"
          accept=".pdf,.doc,.docx,.xls,.xlsx"
          listType="text"
        >
          <a-button>
            <FileAddOutlined />
            上传文档
          </a-button>
        </AgUpload>

        <a-divider />
        <p>允许的文件类型：</p>
        <ul>
          <li>PDF 文档 (.pdf)</li>
          <li>Word 文档 (.doc, .docx)</li>
          <li>Excel 表格 (.xls, .xlsx)</li>
        </ul>
      </AgCard>

      <!-- 实际应用示例 -->
      <AgCard title="6. 实际应用示例 - 商户资质上传">
        <a-form
          :model="merchantForm"
          :label-col="{ span: 4 }"
          :wrapper-col="{ span: 16 }"
        >
          <a-form-item label="营业执照" required>
            <AgUpload
              v-model="merchantForm.businessLicense"
              :maxCount="1"
              accept="image/*"
              listType="picture-card"
            >
              <div v-if="merchantForm.businessLicense.length === 0">
                <PlusOutlined />
                <div class="upload-text">上传营业执照</div>
              </div>
            </AgUpload>
            <div class="form-tip">请上传清晰的营业执照照片</div>
          </a-form-item>

          <a-form-item label="法人身份证" required>
            <a-space direction="vertical" style="width: 100%">
              <div>
                <div class="mb-2">身份证正面：</div>
                <AgUpload
                  v-model="merchantForm.idCardFront"
                  :maxCount="1"
                  accept="image/*"
                  listType="picture-card"
                >
                  <div v-if="merchantForm.idCardFront.length === 0">
                    <PlusOutlined />
                    <div class="upload-text">正面</div>
                  </div>
                </AgUpload>
              </div>
              <div>
                <div class="mb-2">身份证反面：</div>
                <AgUpload
                  v-model="merchantForm.idCardBack"
                  :maxCount="1"
                  accept="image/*"
                  listType="picture-card"
                >
                  <div v-if="merchantForm.idCardBack.length === 0">
                    <PlusOutlined />
                    <div class="upload-text">反面</div>
                  </div>
                </AgUpload>
              </div>
            </a-space>
          </a-form-item>

          <a-form-item label="其他资质">
            <AgUpload
              v-model="merchantForm.otherQualifications"
              :maxCount="5"
              accept="image/*,.pdf"
              listType="picture"
            >
              <a-button>
                <UploadOutlined />
                上传其他资质
              </a-button>
            </AgUpload>
            <div class="form-tip">
              可上传行业许可证、资质证书等（最多5个文件）
            </div>
          </a-form-item>

          <a-form-item :wrapper-col="{ offset: 4, span: 16 }">
            <a-space>
              <a-button type="primary" @click="handleSubmit">
                提交审核
              </a-button>
              <a-button @click="handleReset">
                重置
              </a-button>
              <a-button @click="handlePreview">
                预览数据
              </a-button>
            </a-space>
          </a-form-item>
        </a-form>
      </AgCard>

      <!-- 数据预览 -->
      <AgCard title="表单数据预览">
        <pre>{{ JSON.stringify(merchantForm, null, 2) }}</pre>
      </AgCard>
    </a-space>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import {
  UploadOutlined,
  PlusOutlined,
  InboxOutlined,
  FileAddOutlined
} from '@ant-design/icons-vue'
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
.upload-demo-container {
  /* padding: 24px */
}

.upload-text {
  margin-top: 8px
}

.form-tip {
  color: #999;
  font-size: 12px;
  margin-top: 4px
}

.mb-2 {
  margin-bottom: 8px
}

.mb-4 {
  margin-bottom: 16px
}

.mt-3 {
  margin-top: 12px
}

pre {
  /* background: #f5f5f5; */
  padding: 16px;
  border-radius: 4px;
  overflow: auto;
  max-height: 400px
}
</style>
