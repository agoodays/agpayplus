<template>
  <a-drawer v-model:open="localOpen" title="代理商详情" width="40%" @close="handleClose">
    <a-spin :spinning="loading">
      <a-row justify="space-between" type="flex">
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="代理商号">
              {{ detailData.agentNo }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="代理商名称">
              {{ detailData.agentName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="登录名">
              {{ detailData.loginUsername }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="代理商简称">
              {{ detailData.agentShortName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="上级代理商号">
              {{ detailData.pid }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="服务商号">
              {{ detailData.isvNo }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="服务商名称">
              {{ detailData.isvName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="联系人姓名">
              {{ detailData.contactName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="联系人手机号">
              {{ detailData.contactTel }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="联系人邮箱">
              {{ detailData.contactEmail }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="是否允许发展下级">
              <a-tag :color="detailData.addAgentFlag === 1 ? 'green' : 'volcano'">
                {{ detailData.addAgentFlag === 0 ? '否' : detailData.addAgentFlag === 1 ? '是' : '未知' }}
              </a-tag>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="状态">
              <a-tag :color="detailData.state === 1 ? 'green' : 'volcano'">
                {{ detailData.state === 0 ? '禁用' : detailData.state === 1 ? '启用' : '未知' }}
              </a-tag>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="钱包余额">
              <a-tag :color="detailData.balanceAmount > 0 ? 'green' : 'volcano'">
                {{ detailData.balanceAmount }}
              </a-tag>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="不可用金额">
              <a-tag :color="detailData.unAmount > 0 ? 'green' : 'volcano'">
                {{ detailData.unAmount }}
              </a-tag>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="在途佣金">
              <a-tag :color="detailData.auditProfitAmount > 0 ? 'green' : 'volcano'">
                {{ detailData.auditProfitAmount }}
              </a-tag>
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
      <a-row justify="start" type="flex">
        <a-col :sm="24">
          <a-form-item label="备注">
            <a-textarea v-model:value="detailData.remark" :disabled="true" :rows="2" />
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 账户信息板块 -->      
      <a-divider orientation="left">
        <a-tag color="#FF4B33">账户信息 </a-tag>
      </a-divider>
      <a-row justify="space-between" type="flex">
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="代理商类型">
              {{ detailData.agentType === 1 ? '个人' : '企业' }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="收款账户类型">
              {{ detailData.settAccountTypeName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col v-if="detailData.settAccountType === 'BANK_PUBLIC'" :sm="12">
          <a-descriptions>
            <a-descriptions-item label="对公账户名称">
              {{ detailData.settAccountName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item :label="detailData.settAccountNoLabel">
              {{ detailData.settAccountNo }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col v-if="detailData.settAccountType === 'BANK_PUBLIC'" :sm="12">
          <a-descriptions>
            <a-descriptions-item label="开户银行名称">
              {{ detailData.settAccountBank }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col v-if="detailData.settAccountType === 'BANK_PUBLIC'" :sm="12">
          <a-descriptions>
            <a-descriptions-item label="开户行支行名称">
              {{ detailData.settAccountSubBank }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>

      <!-- 资料信息板块 -->
      <a-divider orientation="left">
        <a-tag color="#FF4B33"> 资料信息 </a-tag>
      </a-divider>
      <a-row justify="space-between" type="flex">
        <!-- 企业 -->
        <a-col v-if="detailData.agentType === 2" :span="10">
          <a-form-item label="营业执照照片" name="licenseImg">
            <ag-upload
              :urls="detailData.licenseImg ? [detailData.licenseImg] : []"
              :show-upload-list="{ showPreviewIcon: false, showRemoveIcon: false, showDownloadIcon: false }"
              list-type="picture"
              :read-only="true"
            />
          </a-form-item>
        </a-col>
        <!-- 企业对公 -->
        <a-col v-if="detailData.agentType === 2 && detailData.settAccountType === 'BANK_PUBLIC'" :span="10">
          <a-form-item label="开户许可证照片" name="permitImg">
            <ag-upload
              :urls="detailData.permitImg ? [detailData.permitImg] : []"
              :show-upload-list="{ showPreviewIcon: false, showRemoveIcon: false, showDownloadIcon: false }"
              list-type="picture"
              :read-only="true"
            />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item :label="'[' + imgLabel + ']身份证人像面照片'" name="idcard1Img">
            <ag-upload
              :urls="detailData.idcard1Img ? [detailData.idcard1Img] : []"
              :show-upload-list="{ showPreviewIcon: false, showRemoveIcon: false, showDownloadIcon: false }"
              list-type="picture"
              :read-only="true"
            />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item :label="'[' + imgLabel + ']身份证国徽面照片'" name="idcard2Img">
            <ag-upload
              :urls="detailData.idcard2Img ? [detailData.idcard2Img] : []"
              :show-upload-list="{ showPreviewIcon: false, showRemoveIcon: false, showDownloadIcon: false }"
              list-type="picture"
              :read-only="true"
            />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="[联系人]手持身份证照片" name="idcardInHandImg">
            <ag-upload
              :urls="detailData.idcardInHandImg ? [detailData.idcardInHandImg] : []"
              :show-upload-list="{ showPreviewIcon: false, showRemoveIcon: false, showDownloadIcon: false }"
              list-type="picture"
              :read-only="true"
            />
          </a-form-item>
        </a-col>
        <!-- 个人对私/企业对私 -->
        <a-col v-if="detailData.settAccountType === 'BANK_PRIVATE'" :span="10">
          <a-form-item :label="'[' + imgLabel + ']银行卡照片'" name="bankCardImg">
            <ag-upload
              :urls="detailData.bankCardImg ? [detailData.bankCardImg] : []"
              :show-upload-list="{ showPreviewIcon: false, showRemoveIcon: false, showDownloadIcon: false }"
              list-type="picture"
              :read-only="true"
            />
          </a-form-item>
        </a-col>
      </a-row>
    </a-spin>
  </a-drawer>
</template>

<script setup>
import { agentApi } from '@/api/business/agent/agent-api'
import AgUpload from '@/components/ag-upload'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'

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

const emit = defineEmits(['update:open'])

const loading = ref(false)
const localOpen = ref(false)
const imgLabel = ref('联系人')

const detailData = reactive({
  agentNo: '',
  agentName: '',
  loginUsername: '',
  agentShortName: '',
  pid: '',
  isvNo: '',
  isvName: '',
  contactName: '',
  contactTel: '',
  contactEmail: '',
  addAgentFlag: 1,
  state: 1,
  balanceAmount: 0,
  unAmount: 0,
  auditProfitAmount: 0,
  remark: '',
  agentType: 1,
  settAccountType: '',
  settAccountTypeName: '个人微信',
  settAccountNo: '',
  settAccountName: '',
  settAccountBank: '',
  settAccountSubBank: '',
  licenseImg: '',
  permitImg: '',
  idcard1Img: '',
  idcard2Img: '',
  idcardInHandImg: '',
  bankCardImg: ''
})

function resetDetailData() {
  Object.assign(detailData, {
    agentNo: '',
    agentName: '',
    loginUsername: '',
    agentShortName: '',
    pid: '',
    isvNo: '',
    isvName: '',
    contactName: '',
    contactTel: '',
    contactEmail: '',
    addAgentFlag: 1,
    state: 1,
    balanceAmount: 0,
    unAmount: 0,
    auditProfitAmount: 0,
    remark: '',
    agentType: 1,
    settAccountType: '',
    settAccountTypeName: '个人微信',
    settAccountNo: '',
    settAccountName: '',
    settAccountBank: '',
    settAccountSubBank: '',
    licenseImg: '',
    permitImg: '',
    idcard1Img: '',
    idcard2Img: '',
    idcardInHandImg: '',
    bankCardImg: ''
  })
  imgLabel.value = '联系人'
}

function normalizeSettleLabels(target) {
  switch (target.settAccountType) {
    case 'WX_CASH':
      target.settAccountTypeName = '个人微信'
      break
    case 'ALIPAY_CASH':
      target.settAccountTypeName = '个人支付宝'
      break
    case 'BANK_PRIVATE':
      target.settAccountTypeName = '对私账户'
      break
    case 'BANK_PUBLIC':
      target.settAccountTypeName = '对公账户'
      break
  }
}

watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.recordId) {
      loadDetail()
    }
  }
)

watch(localOpen, (val) => {
  emit('update:open', val)
})

const loadDetail = async () => {
  try {
    loading.value = true
    const res = await agentApi.getById(props.recordId)
    Object.assign(detailData, res)
    normalizeSettleLabels(detailData)
    imgLabel.value = res.agentType === 2 ? '法人' : '联系人'
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error?.msg || error?.message || '加载详情失败')
  } finally {
    loading.value = false
  }
}

const handleClose = () => {
  resetDetailData()
  emit('update:open', false)
}
</script>

<style lang="less" scoped>
:deep(.ant-descriptions-item-label) {
  font-weight: 500;
  color: var(--text-color);
  background-color: var(--layout-surface);
}
</style>
