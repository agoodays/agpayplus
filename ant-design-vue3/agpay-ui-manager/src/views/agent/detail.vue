<template>
  <ag-drawer
    v-model:open="localOpen"
    title="代理商详情"
    width="40%"
    :show-footer="false"
    @close="handleClose"
  >
    <a-spin :spinning="loading">
      <!-- 基本信息 -->
      <a-descriptions :column="2">
        <a-descriptions-item label="代理商号">{{ detailData.agentNo }}</a-descriptions-item>
        <a-descriptions-item label="代理商名称">{{ detailData.agentName }}</a-descriptions-item>
        <a-descriptions-item label="登录名">{{ detailData.loginUsername }}</a-descriptions-item>
        <a-descriptions-item label="代理商简称">{{ detailData.agentShortName }}</a-descriptions-item>
        <a-descriptions-item label="上级代理商号">{{ detailData.pid }}</a-descriptions-item>
        <a-descriptions-item label="服务商号">{{ detailData.isvNo }}</a-descriptions-item>
        <a-descriptions-item label="服务商名称">{{ detailData.isvName }}</a-descriptions-item>
        <a-descriptions-item label="联系人姓名">{{ detailData.contactName }}</a-descriptions-item>
        <a-descriptions-item label="联系人手机号">{{ detailData.contactTel }}</a-descriptions-item>
        <a-descriptions-item label="联系人邮箱">{{ detailData.contactEmail }}</a-descriptions-item>
        <a-descriptions-item label="是否允许发展下级">
          <a-tag v-bind="getFlagInfo(detailData.addAgentFlag, t)">
            {{ getFlagInfo(detailData.addAgentFlag, t).text }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="状态">
          <a-tag v-bind="getStateInfo(detailData.state, t)">
            {{ getStateInfo(detailData.state, t).text }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="钱包余额">
          <a-tag :color="detailData.balanceAmount > 0 ? 'green' : 'volcano'">
            {{ detailData.balanceAmount }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="不可用金额">
          <a-tag :color="detailData.unAmount > 0 ? 'green' : 'volcano'">
            {{ detailData.unAmount }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="在途佣金">
          <a-tag :color="detailData.auditProfitAmount > 0 ? 'green' : 'volcano'">
            {{ detailData.auditProfitAmount }}
          </a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="备注" :span="2">{{ detailData.remark || '-' }}</a-descriptions-item>
      </a-descriptions>

      <!-- 账户信息 -->
      <a-divider orientation="left">
        <a-tag color="#FF4B33">账户信息</a-tag>
      </a-divider>
      <a-descriptions :column="2" :bordered="false">
        <a-descriptions-item label="代理商类型">{{ getAgentTypeInfo(detailData.agentType, t).text }}</a-descriptions-item>
        <a-descriptions-item label="收款账户类型">{{ detailData.settAccountTypeName }}</a-descriptions-item>
        <a-descriptions-item label="对公账户名称" v-if="detailData.settAccountType === SETT_ACCOUNT_TYPE_ENUM.BANK_PUBLIC.value">
          {{ detailData.settAccountName }}
        </a-descriptions-item>
        <a-descriptions-item :label="detailData.settAccountNoLabel">{{ detailData.settAccountNo }}</a-descriptions-item>
        <a-descriptions-item label="开户银行名称" v-if="detailData.settAccountType === SETT_ACCOUNT_TYPE_ENUM.BANK_PUBLIC.value">
          {{ detailData.settAccountBank }}
        </a-descriptions-item>
        <a-descriptions-item label="开户行支行名称" v-if="detailData.settAccountType === SETT_ACCOUNT_TYPE_ENUM.BANK_PUBLIC.value">
          {{ detailData.settAccountSubBank }}
        </a-descriptions-item>
      </a-descriptions>

      <!-- 资料信息 -->
      <a-divider orientation="left">
        <a-tag color="#FF4B33">资料信息</a-tag>
      </a-divider>
      <a-row :gutter="16">
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
  </ag-drawer>
</template>

<script setup>
/**
 * 代理商详情抽屉组件
 * 功能：展示代理商的详细信息，包含基本信息、账户信息、资料信息
 */
import { agentApi } from '@/api/business/agent/agent-api'
import { AgDrawer, AgUpload } from '@/components'
import {
  SETT_ACCOUNT_TYPE_ENUM,
  getAgentTypeInfo,
  getFlagInfo,
  getSettAccountTypeInfo,
  getStateInfo
} from '@/constants/common-const'
import { message } from 'ant-design-vue'
import { reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

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

/** 加载状态 */
const loading = ref(false)

/** 本地打开状态 */
const localOpen = ref(false)

/** 图片标签（联系人/法人） */
const imgLabel = ref('')

/** 详情数据 */
const detailData = reactive({})

/** 重置详情数据 */
function resetDetailData() {
  Object.assign(detailData, {})
  imgLabel.value = ''
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
    loading.value = true
    const res = await agentApi.getById(props.recordId)
    Object.assign(detailData, res)
    detailData.settAccountTypeName = getSettAccountTypeInfo(detailData.settAccountType, t).text
    detailData.settAccountNoLabel = getSettAccountTypeInfo(detailData.settAccountType, t).label
    imgLabel.value = getAgentTypeInfo(res.agentType, t).label
  } catch (error) {
    console.error('加载详情失败:', error)
    message.error(error?.msg || error?.message || '加载详情失败')
  } finally {
    loading.value = false
  }
}

/** 处理关闭 */
const handleClose = () => {
  resetDetailData()
  localOpen.value = false
}
</script>

<style lang="less" scoped>
:deep(.ant-descriptions-item-label) {
  font-weight: 500;
  color: var(--text-color);
  background-color: var(--layout-surface);
}
</style>
