<template>
  <a-drawer
    :visible="visible"
    title="配置支付通道"
    @close="onClose"
    :closable="true"
    :mask-closable="false"
    :drawer-style="{ overflow: 'hidden', backgroundColor: '#f0f2f5' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-list :data-source="[]" v-if="cardList.length === 0" />
    <div v-else>
      <a-row :gutter="[24, 24]" style="width:100%">
        <a-col
          v-for="(record, key) in cardList"
          :key="key"
          :xxl="8"
          :xl="12"
          :lg="12"
          :md="24"
          :sm="24"
          :xs="24"
        >
          <div class="ag-card-content">
            <div class="ag-card-content-header" :style="{ backgroundColor: record.bgColor }">
              <img v-if="record.icon" :src="record.icon" style="height: 50px">
            </div>
            <div class="ag-card-content-body">
              <div class="title">{{ record.ifName }}</div>
              <a-form layout="inline">
                <a-form-item label="费率：" :validate-status="record.error" :help="record.help">
                  <a-input v-model:value="record.rate" :disabled="!record.state && record.passageId !== ''" suffix="%" />
                </a-form-item>
              </a-form>
            </div>
            <div class="ag-card-ops">
              <a-switch checked-children="启用" un-checked-children="停用" v-model:checked="record.state" />
            </div>
          </div>
        </a-col>
      </a-row>
      <div
        :style="{
          position: 'absolute',
          right: 0,
          bottom: 0,
          width: '100%',
          borderTop: '1px solid #e9e9e9',
          padding: '10px 16px',
          background: '#fff',
          textAlign: 'center',
          zIndex: 1
        }"
      >
        <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose">取消</a-button>
        <a-button type="primary" icon="check" @click="handleOkFunc">保存</a-button>
      </div>
    </div>
  </a-drawer>
</template>

<script setup>
import { ref } from 'vue'
import { message } from 'ant-design-vue'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'

const props = defineProps({
  callbackFunc: { type: Function, default: () => ({}) }
})

const cardList = ref([])
const appId = ref(null)
const wayCode = ref(null)
const visible = ref(false)

const show = (appIdVal, wayCodeVal) => {
  appId.value = appIdVal
  wayCode.value = wayCodeVal
  visible.value = true
  cardList.value = []
  refCardList()
}

const refCardList = () => {
  mchAppApi.getAvailablePayInterfaceList(appId.value, wayCode.value).then(resData => {
    if (!resData.records || resData.records.length === 0) {
      cardList.value = []
      return
    }
    const newItems = []
    resData.records.forEach(item => {
      newItems.push({
        passageId: item.passageId ? item.passageId : '',
        ifCode: item.ifCode,
        ifName: item.ifName,
        icon: item.icon,
        bgColor: item.bgColor,
        rate: item.rate,
        state: item.state === 1,
        error: '',
        help: ''
      })
    })
    cardList.value = newItems
  })
}

const handleOkFunc = () => {
  const reqParams = []
  let hasError = false

  cardList.value.forEach(item => {
    item.error = ''
    item.help = ''
    const reg = /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/
    if (item.state) {
      if (!item.rate) {
        item.error = 'error'
        item.help = '请输入费率'
        hasError = true
      } else if (!reg.test(item.rate) || item.rate > 100) {
        item.error = 'error'
        item.help = '最多四位小数'
        hasError = true
      }
    }
    if (!hasError) {
      reqParams.push({
        id: item.passageId,
        appId: appId.value,
        wayCode: wayCode.value,
        ifCode: item.ifCode,
        rate: item.rate,
        state: item.state ? 1 : 0
      })
    }
  })

  if (hasError) return

  mchAppApi.queryMchPayPassagePage({ reqParams: JSON.stringify(reqParams) }).then(() => {
    message.success('保存成功')
    visible.value = false
    props.callbackFunc()
  })
}

const onClose = () => {
  visible.value = false
}

defineExpose({ show })
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: #fff;
  border-radius: 6px;
  overflow:hidden;
  height: 300px;
  border: 1px solid #e8e8e8;
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: #fff;
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid #e8e8e8;
  position: absolute;
  bottom: 0;
}
.ag-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  height: 125px;
}
.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: start;
  align-items: center;
  height: 125px;
}
.title {
  font-size: 16px;
  font-family: PingFang SC, PingFang SC-Bold;
  font-weight: 700;
  color: #1a1a1a;
  letter-spacing: 1px;
  height: 62px;
  line-height: 62px;
}
</style>
