<template>
  <a-drawer
    :visible="visible"
    :title="true ? '代理商详情' : ''"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    @close="onClose"
  >
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
          <a-input v-model:value="detailData.remark" type="textarea" disabled="disabled" style="height: 50px" />
        </a-form-item>
      </a-col>
    </a-row>

    <!-- 账户信息板块 -->
    <a-row justify="space-between" type="flex">
      <a-col :sm="24">
        <a-divider orientation="left">
          <a-tag color="#FF4B33">账户信息 </a-tag>
        </a-divider>
      </a-col>
    </a-row>
    <div>
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
    </div>

    <!-- 资料信息板块 -->
    <a-row justify="space-between" type="flex">
      <a-col :span="24">
        <a-divider orientation="left">
          <a-tag color="#FF4B33"> 资料信息 </a-tag>
        </a-divider>
      </a-col>
    </a-row>
    <div>
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
    </div>
  </a-drawer>
</template>

<script setup>
import { agentApi } from '@/api/business/agent/agent-api'
import 'viewerjs/dist/viewer.css'
import { ref } from 'vue'
import AgUpload from '@/components/ag-upload'

defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const visible = ref(false)
const detailData = ref({})
const recordId = ref(null)
const imgLabel = ref('联系人')

function buildDefaultDetailData() {
  return {
    state: 1,
    addAgentFlag: 1,
    type: 1,
    settAccountTypeName: '个人微信',
    settAccountNoLabel: '个人微信号'
  }
}

function normalizeSettleLabels(target) {
  switch (target.settAccountType) {
    case 'WX_CASH':
      target.settAccountTypeName = '个人微信'
      target.settAccountNoLabel = '个人微信号'
      break
    case 'ALIPAY_CASH':
      target.settAccountTypeName = '个人支付宝'
      target.settAccountNoLabel = '支付宝账号'
      break
    case 'BANK_PRIVATE':
      target.settAccountTypeName = '对私账户'
      target.settAccountNoLabel = '收款银行卡号'
      break
    case 'BANK_PUBLIC':
      target.settAccountTypeName = '对公账户'
      target.settAccountNoLabel = '对公账号'
      break
  }
}

function show(currentRecordId) {
  detailData.value = buildDefaultDetailData()
  recordId.value = currentRecordId
  visible.value = true

  agentApi.getById(currentRecordId).then((res) => {
    const next = { ...res }
    normalizeSettleLabels(next)
    detailData.value = next
    imgLabel.value = next.agentType === 2 ? '法人' : '联系人'
  })
}

function onClose() {
  visible.value = false
}

function getDefaultFileList(url) {
  if (!url) {
    return []
  }
  return [
    {
      uid: '-1',
      name: url.split('/').pop(),
      status: 'done',
      url,
      thumbUrl: url
    }
  ]
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less">
//.detail-upload-list-inline .ant-upload-list-item-card-actions.picture {
//  display: none;
//}
</style>
