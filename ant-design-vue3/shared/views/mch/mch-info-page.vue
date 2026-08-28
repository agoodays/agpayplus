<template>
  <div class="mch-info-page">
    <a-card :bordered="false" class="mch-info-card">
      <template #title>
        <span><b>商户中心</b></span>
      </template>
      <template #extra>
        <a-button type="link" @click="copyFunc">复制商户信息</a-button>
      </template>
      <div class="info-item">
        <span class="label">商户名称</span>
        <span class="desc">{{ mchInfo.mchName }}</span>
      </div>
      <div class="info-item">
        <span class="label">商户简称</span>
        <span class="desc">{{ mchInfo.mchShortName }}</span>
      </div>
      <div class="info-item">
        <span class="label">登录名</span>
        <span class="desc">{{ mchInfo.loginUsername }}</span>
      </div>
      <div class="info-item">
        <span class="label">商户号</span>
        <span class="desc">{{ mchInfo.mchNo }}</span>
      </div>
      <div class="info-item">
        <span class="label">商户类型</span>
        <span class="desc">{{ mchInfo.type === 1 ? '普通商户' : '特约商户' }}</span>
      </div>
      <div class="info-item" v-if="mchInfo.isvNo">
        <span class="label">服务商号</span>
        <span class="desc">{{ mchInfo.isvNo }}</span>
      </div>
      <div class="info-item">
        <span class="label">注册时间</span>
        <span class="desc">{{ mchInfo.createdAt }}</span>
      </div>
    </a-card>

    <a-card :bordered="false" class="ali-auth-card">
      <template #title>
        <span><b>支付宝代运营授权请求</b></span>
      </template>
      <a-alert message="注意！！！仅当使用支付宝如意lite产品时使用" type="info" show-icon style="margin-bottom: 16px" />
      <a-form ref="infoFormRef" :model="alipayAuthData" :rules="rules" layout="vertical">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="授权方式" prop="authType">
              <a-radio-group v-model:value="alipayAuthData.authType">
                <a-radio :value="'qrcode'">使用支付宝扫授权码</a-radio>
                <a-radio :value="'apply'">发送支付宝授权消息</a-radio>
              </a-radio-group>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="支付宝账号" prop="alipayAccount">
              <a-input placeholder="请输入支付宝账号(一般为手机或邮箱)" v-model:value="alipayAuthData.alipayAccount" />
            </a-form-item>
          </a-col>
        </a-row>
        <a-form-item>
          <a-button type="primary" @click="alipayAuthFunc">发起授权</a-button>
        </a-form-item>
      </a-form>
    </a-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { mainApi } from '@/api/business/main/main-api'
import { infoBox } from '@/utils/info-box'

const mchInfo = ref({})
const infoFormRef = ref(null)
const alipayAuthData = reactive({
  authType: '',
  alipayAccount: ''
})
const rules = {
  authType: [{ required: true, trigger: 'change', message: '请选择授权方式' }],
  alipayAccount: [{ required: true, message: '请输入支付宝账号', trigger: 'blur' }]
}

const detail = async () => {
  try {
    const res = await mainApi.getCurrentUserInfo()
    mchInfo.value = res || {}
  } catch (err) {
    console.error('加载商户信息失败:', err)
  }
}

const copyFunc = () => {
  const data = [
    { title: '商户名称', value: 'mchName' },
    { title: '商户简称', value: 'mchShortName' },
    { title: '登录名', value: 'loginUsername' },
    { title: '商户号', value: 'mchNo' },
    { title: '商户类型', value: 'type' },
    { title: '服务商号', value: 'isvNo' },
    { title: '注册时间', value: 'createdAt' }
  ]
  const getText = (c) => {
    if (c.value === 'type') {
      return `${c.title}: ${mchInfo.value[c.value] === 1 ? '普通商户' : '特约商户'}`
    } else {
      return `${c.title}: ${mchInfo.value[c.value]}`
    }
  }
  const text = data.map((c) => getText(c)).join('\n')
  if (navigator.clipboard) {
    navigator.clipboard.writeText(text).then(() => {
      message.success('复制成功')
    }).catch(() => {
      console.log('复制失败')
    })
  }
}

const alipayAuthFunc = () => {
  infoFormRef.value?.validate().then(() => {
    infoBox.modalSuccess('发送成功', `授权消息已发送至支付宝：${alipayAuthData.alipayAccount}，请前往支付宝App处理`)
  }).catch(() => {})
}

onMounted(() => {
  detail()
})
</script>

<style lang="less" scoped>
.mch-info-page {
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.mch-info-card {
  :deep(.ant-card-body) {
    padding: 20px 24px;
  }
}

.ali-auth-card {
  :deep(.ant-card-body) {
    padding: 20px 24px;
  }
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;

  &:last-child {
    border-bottom: none;
  }

  .label {
    font-weight: 500;
    font-size: 14px;
    color: #999;
    margin-right: 10px;
    flex-shrink: 0;
  }

  .desc {
    font-weight: 500;
    font-size: 14px;
    letter-spacing: 0.05em;
    color: #262626;
    text-align: right;
    word-break: break-all;
  }
}
</style>
