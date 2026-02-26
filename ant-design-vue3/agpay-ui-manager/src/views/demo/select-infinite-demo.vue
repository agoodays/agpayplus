<template>
  <div class="select-infinite-demo-container">
    <a-card title="AgSelectInfinite 分页下拉选择器">
      <a-alert
        message="支持分页加载和搜索的增强下拉选择器"
        description="滚动到底部自动加载下一页数据，支持关键字搜索"
        type="info"
        show-icon
        class="mb-4"
      />

      <a-row :gutter="[16, 16]">
        <!-- 基础用法 -->
        <a-col :span="12">
          <AgCard title="基础用法" size="small">
            <AgSelectInfinite
              v-model="value1"
              label="选择商户"
              placeholder="请选择商户"
              :fetch-data="fetchMerchants"
              :field-names="{ label: 'name', value: 'id' }"
            />
            <div class="result">选中值: {{ value1 }}</div>
          </AgCard>
        </a-col>

        <!-- 支持搜索 -->
        <a-col :span="12">
          <AgCard title="支持搜索" size="small">
            <AgSelectInfinite
              v-model="value2"
              label="搜索用户"
              placeholder="输入关键字搜索"
              :fetch-data="fetchUsers"
              :field-names="{ label: 'realname', value: 'userId' }"
            />
            <div class="result">选中值: {{ value2 }}</div>
          </AgCard>
        </a-col>

        <!-- 多选模式 -->
        <a-col :span="12">
          <AgCard title="多选模式" size="small">
            <AgSelectInfinite
              v-model="value3"
              label="选择标签"
              placeholder="可选择多个"
              mode="multiple"
              :fetch-data="fetchTags"
              :allow-clear="true"
            />
            <div class="result">选中值: {{ value3 }}</div>
          </AgCard>
        </a-col>

        <!-- 自定义选项 -->
        <a-col :span="12">
          <AgCard title="自定义选项渲染" size="small">
            <AgSelectInfinite
              v-model="value4"
              label="选择商品"
              placeholder="自定义显示"
              :fetch-data="fetchProducts"
              :field-names="{ label: 'name', value: 'id' }"
            >
              <template #option="{ option }">
                <div class="custom-option">
                  <div class="option-name">{{ option.name }}</div>
                  <div class="option-info">
                    <span class="option-code">编号: {{ option.code }}</span>
                    <span class="option-price">¥{{ option.price }}</span>
                  </div>
                </div>
              </template>
            </AgSelectInfinite>
            <div class="result">选中值: {{ value4 }}</div>
          </AgCard>
        </a-col>

        <!-- 必填项 -->
        <a-col :span="12">
          <AgCard title="必填项" size="small">
            <AgSelectInfinite
              v-model="value5"
              label="商户类型"
              placeholder="请选择"
              :fetch-data="fetchTypes"
              :required="true"
            />
            <div class="result">选中值: {{ value5 }}</div>
          </AgCard>
        </a-col>

        <!-- 禁用状态 -->
        <a-col :span="12">
          <AgCard title="禁用状态" size="small">
            <AgSelectInfinite
              v-model="value6"
              label="禁用选择"
              :fetch-data="fetchMerchants"
              :disabled="true"
            />
          </AgCard>
        </a-col>

        <!-- 不同尺寸 -->
        <a-col :span="24">
          <AgCard title="不同尺寸" size="small">
            <a-space direction="vertical" style="width: 100%" :size="16">
              <div>
                <div class="label">小尺寸</div>
                <AgSelectInfinite
                  v-model="value7"
                  label="小尺寸"
                  size="small"
                  :fetch-data="fetchMerchants"
                  style="width: 300px"
                />
              </div>
              <div>
                <div class="label">中尺寸（默认）</div>
                <AgSelectInfinite
                  v-model="value8"
                  label="中尺寸"
                  size="middle"
                  :fetch-data="fetchMerchants"
                  style="width: 300px"
                />
              </div>
              <div>
                <div class="label">大尺寸</div>
                <AgSelectInfinite
                  v-model="value9"
                  label="大尺寸"
                  size="large"
                  :fetch-data="fetchMerchants"
                  style="width: 300px"
                />
              </div>
            </a-space>
          </AgCard>
        </a-col>

        <!-- 综合示例：表单 -->
        <a-col :span="24">
          <AgCard title="表单中使用" size="small">
            <a-form
              :model="form"
              :label-col="{ span: 4 }"
              :wrapper-col="{ span: 16 }"
            >
              <a-form-item label="所属商户" name="merchantId">
                <AgSelectInfinite
                  v-model="form.merchantId"
                  label="选择商户"
                  placeholder="请选择商户"
                  :fetch-data="fetchMerchants"
                  :field-names="{ label: 'name', value: 'id' }"
                  :required="true"
                />
              </a-form-item>

              <a-form-item label="负责人" name="userIds">
                <AgSelectInfinite
                  v-model="form.userIds"
                  label="选择负责人"
                  placeholder="可选择多人"
                  mode="multiple"
                  :fetch-data="fetchUsers"
                  :field-names="{ label: 'realname', value: 'userId' }"
                  :allow-clear="true"
                />
              </a-form-item>

              <a-form-item label="商品" name="productId">
                <AgSelectInfinite
                  v-model="form.productId"
                  label="选择商品"
                  placeholder="输入商品名搜索"
                  :fetch-data="fetchProducts"
                  :field-names="{ label: 'name', value: 'id' }"
                >
                  <template #option="{ option }">
                    <div class="custom-option">
                      <span>{{ option.name }}</span>
                      <span style="color: #ff4d4f; margin-left: 8px">
                        ¥{{ option.price }}
                      </span>
                    </div>
                  </template>
                </AgSelectInfinite>
              </a-form-item>

              <a-form-item :wrapper-col="{ offset: 4, span: 16 }">
                <a-space>
                  <a-button type="primary" @click="handleSubmit">
                    提交
                  </a-button>
                  <a-button @click="handleReset">
                    重置
                  </a-button>
                </a-space>
              </a-form-item>
            </a-form>

            <a-divider>表单数据</a-divider>
            <pre>{{ JSON.stringify(form, null, 2) }}</pre>
          </AgCard>
        </a-col>
      </a-row>
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { message } from 'ant-design-vue'
import { AgCard, AgSelectInfinite } from '@/components'

// 单选值
const value1 = ref('')
const value2 = ref('')
const value3 = ref([])
const value4 = ref('')
const value5 = ref('')
const value6 = ref('001')
const value7 = ref('')
const value8 = ref('')
const value9 = ref('')

// 表单数据
const form = reactive({
  merchantId: '',
  userIds: [],
  productId: ''
})

// 模拟商户数据
function generateMerchants(page, pageSize, keyword = '') {
  const allData = Array.from({ length: 100 }, (_, i) => ({
    id: `M${String(i + 1).padStart(3, '0')}`,
    name: `商户${i + 1}`,
    code: `MCH${String(i + 1).padStart(4, '0')}`
  }))

  // 搜索过滤
  const filtered = keyword
    ? allData.filter(item => 
        item.name.includes(keyword) || item.code.includes(keyword)
      )
    : allData

  // 分页
  const start = (page - 1) * pageSize
  const end = start + pageSize
  const data = filtered.slice(start, end)

  return {
    data,
    total: filtered.length
  }
}

// 模拟用户数据
function generateUsers(page, pageSize, keyword = '') {
  const allData = Array.from({ length: 50 }, (_, i) => ({
    userId: `U${String(i + 1).padStart(3, '0')}`,
    realname: `用户${i + 1}`,
    username: `user${i + 1}`
  }))

  const filtered = keyword
    ? allData.filter(item => 
        item.realname.includes(keyword) || item.username.includes(keyword)
      )
    : allData

  const start = (page - 1) * pageSize
  const end = start + pageSize
  const data = filtered.slice(start, end)

  return {
    data,
    total: filtered.length
  }
}

// 模拟标签数据
function generateTags(page, pageSize, keyword = '') {
  const allData = Array.from({ length: 30 }, (_, i) => ({
    value: `tag${i + 1}`,
    label: `标签${i + 1}`
  }))

  const filtered = keyword
    ? allData.filter(item => item.label.includes(keyword))
    : allData

  const start = (page - 1) * pageSize
  const end = start + pageSize
  const data = filtered.slice(start, end)

  return {
    data,
    total: filtered.length
  }
}

// 模拟商品数据
function generateProducts(page, pageSize, keyword = '') {
  const allData = Array.from({ length: 80 }, (_, i) => ({
    id: `P${String(i + 1).padStart(3, '0')}`,
    name: `商品${i + 1}`,
    code: `PRD${String(i + 1).padStart(4, '0')}`,
    price: (Math.random() * 1000).toFixed(2)
  }))

  const filtered = keyword
    ? allData.filter(item => 
        item.name.includes(keyword) || item.code.includes(keyword)
      )
    : allData

  const start = (page - 1) * pageSize
  const end = start + pageSize
  const data = filtered.slice(start, end)

  return {
    data,
    total: filtered.length
  }
}

// 模拟类型数据
function generateTypes(page, pageSize) {
  const allData = [
    { value: '1', label: '企业商户' },
    { value: '2', label: '个人商户' },
    { value: '3', label: '个体工商户' }
  ]

  return {
    data: allData,
    total: allData.length
  }
}

// 数据加载函数（模拟异步请求）
async function fetchMerchants({ page, pageSize, keyword }) {
  // 模拟网络延迟
  await new Promise(resolve => setTimeout(resolve, 300))
  return generateMerchants(page, pageSize, keyword)
}

async function fetchUsers({ page, pageSize, keyword }) {
  await new Promise(resolve => setTimeout(resolve, 300))
  return generateUsers(page, pageSize, keyword)
}

async function fetchTags({ page, pageSize, keyword }) {
  await new Promise(resolve => setTimeout(resolve, 200))
  return generateTags(page, pageSize, keyword)
}

async function fetchProducts({ page, pageSize, keyword }) {
  await new Promise(resolve => setTimeout(resolve, 300))
  return generateProducts(page, pageSize, keyword)
}

async function fetchTypes({ page, pageSize }) {
  await new Promise(resolve => setTimeout(resolve, 100))
  return generateTypes(page, pageSize)
}

// 表单操作
function handleSubmit() {
  console.log('表单数据:', form)
  message.success('提交成功')
}

function handleReset() {
  form.merchantId = ''
  form.userIds = []
  form.productId = ''
  message.info('已重置')
}
</script>

<style scoped>
.select-infinite-demo-container {
  /* padding: 24px; */
}

.result {
  margin-top: 12px;
  padding: 8px;
  background-color: #f5f5f5;
  border-radius: 4px;
  font-size: 12px;
  color: #666;
}

.label {
  margin-bottom: 8px;
  font-size: 14px;
  color: #666;
}

.custom-option {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.option-name {
  font-weight: 500;
  color: #333;
}

.option-info {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
}

.option-code {
  color: #999;
}

.option-price {
  color: #ff4d4f;
  font-weight: 500;
}

.mb-4 {
  margin-bottom: 16px;
}
</style>
