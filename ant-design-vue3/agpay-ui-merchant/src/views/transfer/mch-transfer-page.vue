<template>
  <a-card :bordered="false" style="padding: 30px">
    <a-form>
      <div style="display: flex; flex-direction: row">
        <a-form-item>
          <a-select v-model:value="reqData.appId" @change="changeAppId" style="width: 300px">
            <a-select-option value="">请选择应用APPID</a-select-option>
            <a-select-option
              v-for="item in mchAppList"
              :key="item.appId"
              :value="item.appId"
            >{{ item.appName }} [{{ item.appId }}]</a-select-option>
          </a-select>
        </a-form-item>
      </div>
    </a-form>

    <a-divider v-if="!reqData.appId">请选择应用APPID</a-divider>
    <a-divider v-else-if="ifCodeList.length === 0">该应用尚未配置任何通道</a-divider>
    <div v-else class="paydemo">
      <div class="paydemo-type-content">
        <div class="paydemo-type-name article-title">选择通道</div>
        <div class="paydemo-type-body">
          <div
            v-for="item in ifCodeList"
            :key="item.ifCode"
            :class="{ 'paydemo-type': true, colorChange: true, 'this': reqData.ifCode === item.ifCode }"
            @click="changeCurrentIfCode(item.ifCode)"
          >
            <span class="colorChange">{{ item.ifName }}</span>
          </div>
        </div>
      </div>

      <div class="paydemo-form-item">
        <span>入账方式：</span>
        <a-radio-group v-model:value="reqData.entryType" style="display: flex">
          <div style="display: flex">
            <a-radio value="WX_CASH" :disabled="reqData.ifCode !== 'wxpay'">微信零钱</a-radio>
            <a-radio value="ALIPAY_CASH" :disabled="reqData.ifCode !== 'alipay'">支付宝余额</a-radio>
            <a-radio value="BANK_CARD" disabled>银行卡（暂未支持）</a-radio>
          </div>
        </a-radio-group>
      </div>

      <a-divider />
      <div class="paydemo-type-content">
        <div class="paydemo-type-name article-title">转账信息</div>
        <div class="paydemo-form-item">
          <label>订单编号：</label>
          <span id="payMchOrderNo">{{ reqData.mchOrderNo }}</span>
          <span @click="randomOrderNo" class="paydemo-btn" style="padding: 0 3px">刷新订单号</span>
        </div>
        <div class="paydemo-form-item">
          <span>转账金额(元)：</span>
          <a-input-number
            :max="100000"
            :min="0.01"
            :precision="2"
            v-model:value="reqData.amount"
          />
        </div>

        <div class="paydemo-form-item">
          <span>收款账号：</span>
          <a-input v-model:value="reqData.accountNo" style="width: 200px; margin-right: 10px" />
          <a-button
            v-show="reqData.entryType === 'WX_CASH'"
            size="small"
            danger
            @click="showChannelUserQR"
          >自动获取openID</a-button>
        </div>
        <div style="margin-left: 10px; color: red">提示：【微信官方】需要填入对应应用收款方的openID</div>
        <div style="margin-left: 10px; color: red">【支付宝官方】需要填入支付宝登录账号</div>

        <div class="paydemo-form-item" style="margin-top: 10px">
          <span>收款人姓名：</span>
          <a-input v-model:value="reqData.accountName" style="width: 200px" />
          <div style="margin-left: 10px; color: red">提示：填入则验证，否则不验证收款人姓名</div>
        </div>

        <div class="paydemo-form-item">
          <span>转账备注：</span>
          <a-input v-model:value="reqData.transferDesc" style="width: 200px" />
        </div>

        <div style="margin-top: 20px; text-align: left">
          <a-button
            type="primary"
            size="large"
            @click="immediatelyTransfer"
          >立即转账</a-button>
        </div>
      </div>
    </div>

    <ChannelUserModal ref="channelUserModalRef" @changeChannelUserId="changeChannelUserIdFunc" />
  </a-card>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { Modal, message } from 'ant-design-vue'
import ChannelUserModal from '@/components/channel-user/channel-user-modal.vue'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { transferApi } from '@/api/business/transfer/transfer-api'

const route = useRoute()

const ifCodeList = ref([])
const mchAppList = ref([])
const channelUserModalRef = ref(null)

const reqData = reactive({
  appId: '',
  mchOrderNo: '',
  ifCode: '',
  entryType: '',
  amount: 0.01,
  accountNo: '',
  accountName: '',
  transferDesc: '打款'
})

onMounted(async () => {
  const routeAppId = route.params.appId
  if (routeAppId) {
    reqData.appId = routeAppId
    await changeAppId(routeAppId)
  }

  try {
    const appRes = await mchAppApi.queryPage({ pageSize: -1 })
    mchAppList.value = appRes?.records || []
    if (mchAppList.value.length > 0 && !reqData.appId) {
      reqData.appId = mchAppList.value[0].appId
      await changeAppId(reqData.appId)
    }
  } catch (error) {
    console.error('加载应用列表失败:', error)
  }

  randomOrderNo()
})

const changeAppId = async (value) => {
  if (!value) {
    ifCodeList.value = []
    return
  }
  try {
    ifCodeList.value = await transferApi.queryIfCodes(value)
  } catch (error) {
    console.error('查询转账通道失败:', error)
    ifCodeList.value = []
  }
}

const randomOrderNo = () => {
  reqData.mchOrderNo = 'M' + Date.now() + Math.floor(Math.random() * 9000 + 1000)
}

const immediatelyTransfer = async () => {
  if (!reqData.amount || reqData.amount <= 0) {
    message.error('请输入转账金额')
    return
  }
  if (!reqData.ifCode) {
    message.error('请选择转账通道')
    return
  }
  if (!reqData.entryType) {
    message.error('请选择入账方式')
    return
  }
  if (!reqData.accountNo) {
    message.error('请输入收款账号')
    return
  }
  if (!reqData.transferDesc) {
    message.error('请输入转账备注')
    return
  }

  try {
    const apiRes = await transferApi.doTransfer({ ...reqData })
    randomOrderNo()

    if (apiRes.state === 2) {
      const succModal = Modal.success({
        title: '转账成功',
        content: '2s后自动关闭...'
      })
      setTimeout(() => succModal.destroy(), 2000)
    } else if (apiRes.state === 1) {
      Modal.warning({
        title: '转账处理中',
        content: '请前往转账订单列表查看最终状态'
      })
    } else if (apiRes.state === 3) {
      Modal.error({
        title: '转账处理失败',
        content: {
          props: {
            innerHTML: `<div>错误码：${apiRes.errCode}</div><div>错误信息：${apiRes.errMsg}</div>`
          }
        }
      })
    } else {
      message.error('转账异常')
    }
  } catch {
    randomOrderNo()
  }
}

const changeCurrentIfCode = (ifCode) => {
  reqData.ifCode = ifCode
  if (ifCode === 'wxpay') {
    reqData.entryType = 'WX_CASH'
  } else if (ifCode === 'alipay') {
    reqData.entryType = 'ALIPAY_CASH'
  } else {
    reqData.entryType = ''
  }
}

const showChannelUserQR = () => {
  channelUserModalRef.value?.showModal(reqData.appId, reqData.ifCode)
}

const changeChannelUserIdFunc = ({ channelUserId }) => {
  message.success('成功获取渠道用户ID')
  reqData.accountNo = channelUserId
}
</script>

<style scoped lang="less">
.paydemo-type-content {
  padding: 20px 0;
  margin-bottom: 20px;
  background-color: #fff;
  border-radius: 6px;
}
.paydemo-type-name {
  font-size: 16px;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  font-weight: 600;
}
.paydemo-type-body {
  display: flex;
  flex-direction: row;
  align-items: center;
  margin-bottom: 20px;
}
.paydemo-type {
  padding: 12px;
  border: solid 1px #e2e2e2;
  margin-right: 10px;
  cursor: pointer;
  border-radius: 4px;
  transition: all 0.2s;
}
.paydemo-type:hover {
  border-color: #1953ff;
}
.paydemo-type.this {
  color: #1953ff;
  border-color: #1953ff;
  font-weight: 500;
}
.paydemo-form-item {
  height: 38px;
  margin-bottom: 5px;
  display: flex;
  flex-direction: row;
  align-items: center;

  label {
    display: flex;
    align-items: center;
    margin-right: 15px;
    flex-wrap: wrap;
  }
}
.paydemo-btn {
  border: 1px solid #e2e2e2;
  background-color: #fff;
  color: #000;
  margin-left: 8px;
  display: inline-flex;
  flex-direction: row;
  align-items: center;
  cursor: pointer;
  border-radius: 4px;

  &:hover {
    color: #1953ff;
    border-color: #1953ff;
  }
}
</style>
