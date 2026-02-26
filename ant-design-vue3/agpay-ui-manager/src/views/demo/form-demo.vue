<template>
  <div class="form-demo-container">
    <a-card title="表单组件综合示例" class="mb-4">
      <a-alert
        message="这个示例展示了所有表单组件的使用方法"
        description="包括 AgSelect、AgUpload、AgDateRangePicker、AgEditor 等组件"
        type="info"
        show-icon
        class="mb-4"
      />

      <a-form
        ref="formRef"
        :model="formState"
        :rules="rules"
        :label-col="{ span: 4 }"
        :wrapper-col="{ span: 16 }"
      >
        <!-- 基础输入 -->
        <a-divider orientation="left">基础输入</a-divider>
        
        <a-form-item label="商户名称" name="merchantName">
          <a-input 
            v-model:value="formState.merchantName" 
            placeholder="请输入商户名称"
          />
        </a-form-item>

        <a-form-item label="商户编号" name="merchantNo">
          <a-input 
            v-model:value="formState.merchantNo"
            placeholder="请输入商户编号"
            style="text-transform: uppercase"
          />
        </a-form-item>

        <!-- 下拉选择 -->
        <a-divider orientation="left">下拉选择</a-divider>

        <a-form-item label="商户类型" name="merchantType">
          <AgSelect
            v-model="formState.merchantType"
            :options="merchantTypeOptions"
            placeholder="请选择商户类型"
          />
        </a-form-item>

        <a-form-item label="所属行业" name="industry">
          <AgSelect
            v-model="formState.industry"
            :options="industryOptions"
            placeholder="请选择所属行业"
            mode="multiple"
          />
        </a-form-item>

        <!-- 日期选择 -->
        <a-divider orientation="left">日期选择</a-divider>

        <a-form-item label="营业期限" name="businessPeriod">
          <AgDateRangePicker
            v-model="formState.businessPeriod"
            placeholder="请选择营业期限"
          />
        </a-form-item>

        <!-- 文件上传 -->
        <a-divider orientation="left">文件上传</a-divider>

        <a-form-item label="营业执照" name="businessLicense">
          <AgUpload
            v-model="formState.businessLicense"
            :maxCount="1"
            accept="image/*"
            listType="picture-card"
          >
            <div v-if="formState.businessLicense.length === 0">
              <PlusOutlined />
              <div class="upload-text">上传营业执照</div>
            </div>
          </AgUpload>
        </a-form-item>

        <a-form-item label="资质证明" name="qualifications">
          <AgUpload
            v-model="formState.qualifications"
            :maxCount="5"
            accept="image/*,.pdf"
            listType="picture"
          >
            <a-button>
              <UploadOutlined />
              上传资质证明
            </a-button>
          </AgUpload>
        </a-form-item>

        <!-- 富文本编辑 -->
        <a-divider orientation="left">富文本编辑</a-divider>

        <a-form-item label="商户介绍" name="description">
          <AgEditor
            v-model="formState.description"
            :height="300"
            placeholder="请输入商户介绍..."
          />
        </a-form-item>

        <!-- 其他 -->
        <a-divider orientation="left">其他</a-divider>

        <a-form-item label="备注" name="remark">
          <a-textarea
            v-model:value="formState.remark"
            :rows="4"
            placeholder="请输入备注信息"
          />
        </a-form-item>

        <!-- 操作按钮 -->
        <a-form-item :wrapper-col="{ offset: 4, span: 16 }">
          <a-space>
            <a-button type="primary" @click="handleSubmit" :loading="submitting">
              提交
            </a-button>
            <a-button @click="handleReset">
              重置
            </a-button>
            <a-button @click="handleFillDemo">
              填充示例数据
            </a-button>
          </a-space>
        </a-form-item>
      </a-form>
    </a-card>

    <!-- 表单数据预览 -->
    <a-card title="表单数据预览" class="mb-4">
      <pre>{{ JSON.stringify(formState, null, 2) }}</pre>
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { PlusOutlined, UploadOutlined } from '@ant-design/icons-vue'
import {
  AgSelect,
  AgUpload,
  AgDateRangePicker,
  AgEditor
} from '@/components'

const formRef = ref()
const submitting = ref(false)

// 表单数据
const formState = reactive({
  merchantName: '',
  merchantNo: '',
  merchantType: undefined,
  industry: [],
  businessPeriod: [],
  businessLicense: [],
  qualifications: [],
  description: '',
  remark: ''
})

// 商户类型选项
const merchantTypeOptions = [
  { label: '企业商户', value: 'enterprise' },
  { label: '个体工商户', value: 'individual' },
  { label: '个人商户', value: 'personal' }
]

// 行业选项
const industryOptions = [
  { label: '餐饮', value: 'catering' },
  { label: '零售', value: 'retail' },
  { label: '服务', value: 'service' },
  { label: '教育', value: 'education' },
  { label: '医疗', value: 'medical' },
  { label: '娱乐', value: 'entertainment' }
]

// 表单验证规则
const rules = {
  merchantName: [
    { required: true, message: '请输入商户名称' }
  ],
  merchantNo: [
    { required: true, message: '请输入商户编号' },
    { pattern: /^[A-Z0-9]+$/, message: '商户编号只能包含大写字母和数字' }
  ],
  merchantType: [
    { required: true, message: '请选择商户类型' }
  ],
  businessPeriod: [
    { required: true, message: '请选择营业期限' }
  ],
  businessLicense: [
    { required: true, message: '请上传营业执照' }
  ]
}

// 提交表单
async function handleSubmit() {
  try {
    await formRef.value.validate()
    
    submitting.value = true
    
    // 模拟 API 调用
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    console.log('提交数据:', formState)
    message.success('提交成功')
  } catch (error) {
    if (error.errorFields) {
      message.error('请完善表单信息')
    } else {
      message.error('提交失败')
    }
  } finally {
    submitting.value = false
  }
}

// 重置表单
function handleReset() {
  formRef.value.resetFields()
  message.info('表单已重置')
}

// 填充示例数据
function handleFillDemo() {
  Object.assign(formState, {
    merchantName: '示例商户有限公司',
    merchantNo: 'MCH202401001',
    merchantType: 'enterprise',
    industry: ['retail', 'service'],
    businessPeriod: [
      new Date('2024-01-01'),
      new Date('2034-12-31')
    ],
    businessLicense: [
      {
        uid: '-1',
        name: 'business-license.jpg',
        status: 'done',
        url: 'https://via.placeholder.com/300x200'
      }
    ],
    qualifications: [
      {
        uid: '-2',
        name: 'qualification-1.pdf',
        status: 'done',
        url: '#'
      }
    ],
    description: '<p>这是一家优秀的示例商户，提供优质的产品和服务。</p>',
    remark: '这是备注信息示例'
  })
  
  message.success('已填充示例数据')
}
</script>

<style scoped>
.form-demo-container {
  /* padding: 24px */
}

.upload-text {
  margin-top: 8px
}

pre {
  background: #f5f5f5;
  padding: 16px;
  border-radius: 4px;
  overflow: auto;
  max-height: 400px
}
</style>
