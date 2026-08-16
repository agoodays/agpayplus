<template>
  <div>
    <a-card style="box-sizing: border-box; padding: 30px">
      <a-row :gutter="24">
        <a-col :md="24" :lg="24">
          <a-space :size="16" wrap>
            <a-form-item label="应用" style="margin-bottom: 0">
              <a-select v-model:value="appId" @change="changeAppId" style="width: 300px" placeholder="选择应用">
                <a-select-option v-for="item in mchAppList" :key="item.appId" :value="item.appId">
                  {{ item.appName }} [{{ item.appId }}]
                </a-select-option>
              </a-select>
            </a-form-item>
            <a-form-item label="门店" style="margin-bottom: 0">
              <a-select v-model:value="storeId" style="width: 300px" placeholder="选择门店">
                <a-select-option v-for="item in mchStoreList" :key="item.storeId" :value="item.storeId">
                  {{ item.storeName }} [{{ item.storeId }}]
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-space>
        </a-col>
      </a-row>

      <a-divider v-if="!appId">请选择应用</a-divider>
      <a-divider v-else-if="noConfigText">您尚未配置任何支付方式</a-divider>
      <a-divider v-else />

      <div style="width: 100%" class="paydemo" v-if="payTestShow">
        <div class="paydemo-type-content">
          <div class="paydemo-type-name article-title" v-show="showTitle('WX')">微信支付</div>
          <div class="paydemo-type-body">
            <div class="paydemo-type" v-show="appPaywayList.includes('WX_NATIVE')" @click="changeCurrentWayCode('WX_NATIVE', 'codeImgUrl')" :class="{ this: currentWayCode === 'WX_NATIVE' }">
              <img :src="payImg('wx_native')" class="paydemo-type-img" /><span>微信二维码</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('WX_BAR')" @click="changeCurrentWayCode('WX_BAR', '')" :class="{ this: currentWayCode === 'WX_BAR' }">
              <img :src="payImg('wx_bar')" class="paydemo-type-img" /><span>微信条码</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('WX_JSAPI')" @click="changeCurrentWayCode('WX_JSAPI', 'codeImgUrl')" :class="{ this: currentWayCode === 'WX_JSAPI' }">
              <img :src="payImg('wx_jsapi')" class="paydemo-type-img" /><span>公众号/小程序</span>
            </div>
            <div class="paydemo-type-h5" v-show="appPaywayList.includes('WX_H5')" @click="changeCurrentWayCode('WX_H5', 'payurl')" :class="{ this: currentWayCode === 'WX_H5' }">
              <img :src="payImg('wx_h5')" class="paydemo-type-img" /><span>微信H5</span>
            </div>
          </div>

          <div class="paydemo-type-name article-title" v-show="showTitle('ALI')">支付宝支付</div>
          <div class="paydemo-type-body">
            <div class="paydemo-type" v-show="appPaywayList.includes('ALI_QR')" @click="changeCurrentWayCode('ALI_QR', 'codeImgUrl')" :class="{ this: currentWayCode === 'ALI_QR' }">
              <img :src="payImg('ali_qr')" class="paydemo-type-img" /><span>支付宝二维码</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('ALI_BAR')" @click="changeCurrentWayCode('ALI_BAR', '')" :class="{ this: currentWayCode === 'ALI_BAR' }">
              <img :src="payImg('ali_bar')" class="paydemo-type-img" /><span>支付宝条码</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('ALI_JSAPI')" @click="changeCurrentWayCode('ALI_JSAPI', 'codeImgUrl')" :class="{ this: currentWayCode === 'ALI_JSAPI' }">
              <img :src="payImg('ali_jsapi')" class="paydemo-type-img" /><span>支付宝生活号</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('ALI_PC')" @click="changeCurrentWayCode('ALI_PC', 'payurl')" :class="{ this: currentWayCode === 'ALI_PC' }">
              <img :src="payImg('ali_pc')" class="paydemo-type-img" /><span>支付宝PC网站</span>
            </div>
            <div class="paydemo-type-h5" v-show="appPaywayList.includes('ALI_WAP')" @click="changeCurrentWayCode('ALI_WAP', 'payurl')" :class="{ this: currentWayCode === 'ALI_WAP' }">
              <img :src="payImg('ali_wap')" class="paydemo-type-img" /><span>支付宝WAP</span>
            </div>
          </div>

          <div class="paydemo-type-name article-title" v-show="showQtTitle()">其它支付</div>
          <div class="paydemo-type-body">
            <div class="paydemo-type" v-show="appPaywayList.includes('WX_JSAPI') || appPaywayList.includes('ALI_JSAPI')" @click="changeCurrentWayCode('QR_CASHIER', 'codeImgUrl')" :class="{ this: currentWayCode === 'QR_CASHIER' }">
              <img :src="payImg('qr_cashier')" class="paydemo-type-img" /><span>聚合主扫</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('WX_BAR') || appPaywayList.includes('ALI_BAR')" @click="changeCurrentWayCode('AUTO_BAR', 'codeImgUrl')" :class="{ this: currentWayCode === 'AUTO_BAR' }">
              <img :src="payImg('auto_bar')" class="paydemo-type-img" /><span>聚合被扫</span>
            </div>
            <div class="paydemo-type" v-show="appPaywayList.includes('PP_PC')" @click="changeCurrentWayCode('PP_PC', 'payurl')" :class="{ this: currentWayCode === 'PP_PC' }">
              <img :src="payImg('pp_pc')" class="paydemo-type-img" /><span>PayPal支付</span>
            </div>
          </div>
        </div>

        <a-divider />

        <div class="paydemo-type-content">
          <div class="paydemo-type-name article-title">支付信息</div>
          <div>
            <div class="paydemo-form-item">
              <label>订单编号：</label>
              <span>{{ mchOrderNo }}</span>
              <span @click="randomOrderNo" class="paydemo-btn" style="padding: 0 3px">刷新订单号</span>
            </div>
            <div class="paydemo-form-item">
              <label>订单标题：</label>
              <a-input v-model:value="orderTitle" style="width: 200px" />
            </div>
            <div class="paydemo-form-item">
              <label>分账方式：</label>
              <a-radio-group v-model:value="divisionMode">
                <a-radio :value="0">订单不分账</a-radio>
                <a-radio :value="1">支付完成自动分账</a-radio>
                <a-radio :value="2">手动分账（冻结商户资金）</a-radio>
              </a-radio-group>
            </div>

            <a-divider />

            <div class="paydemo-form-item">
              <span style="margin-right: 15px">支付金额(元)：</span>
              <a-radio-group v-model:value="paytestAmount">
                <a-radio :value="0.01">￥0.01</a-radio>
                <a-radio :value="0.15">￥0.15</a-radio>
                <a-radio :value="0.21">￥0.21</a-radio>
                <a-radio :value="0.29">￥0.29</a-radio>
                <a-radio :value="0.64">￥0.64</a-radio>
              </a-radio-group>
              <a-radio style="margin-left: 15px" @click="amountInputShow">
                自定义金额
                <a-input-number
                  v-show="amountInput"
                  ref="amountInputFocus"
                  :max="100000"
                  :min="0.01"
                  v-model:value="customAmount"
                  :precision="2"
                  :step="0.01"
                  style="margin-left: 6px"
                />
              </a-radio>
            </div>

            <div style="margin-top: 20px; text-align: left">
              <a-button type="primary" @click="immediatelyPay" style="padding: 5px 20px">立即支付</a-button>
            </div>
          </div>
        </div>
      </div>
    </a-card>

    <pay-test-modal ref="payTestModal" @closeBarCode="$refs.payTestBarCodeRef?.processCatch?.()" />
    <pay-test-bar-code ref="payTestBarCodeRef" @barCodeValue="barCodeChange" @CodeAgainChange="testCodeChange" />
  </div>
</template>

<script setup>
import { message } from 'ant-design-vue'
import { onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { mchStoreApi } from '@/api/business/mch-store/mch-store-api'
import { payTestApi } from '@/api/business/pay-test/pay-test-api'
import PayTestModal from './pay-test-modal.vue'
import PayTestBarCode from './pay-test-bar-code.vue'
import './payTest.css'

const appId = ref('')
const mchAppList = ref([])
const storeId = ref('')
const mchStoreList = ref([])
const appPaywayList = ref([])
const currentWayCode = ref('')
const currentPayDataType = ref('')
const mchOrderNo = ref('')
const authCode = ref('')
const paytestAmount = ref(0.01)
const customAmount = ref(0.01)
const amountInput = ref(false)
const noConfigText = ref(false)
const divisionMode = ref(0)
const orderTitle = ref('接口调试')

const payTestModal = ref(null)
const payTestBarCodeRef = ref(null)
const amountInputFocus = ref(null)
const route = useRoute()

// 图片导入映射
const payImgCache = {}
function payImg(name) {
  if (!payImgCache[name]) {
    payImgCache[name] = new URL(`@/assets/payTestImg/${name}.svg`, import.meta.url).href
  }
  return payImgCache[name]
}

const payTestShow = () => appId.value !== '' && appPaywayList.value.length > 0

const showTitle = (prefix) => {
  const str = appPaywayList.value.join(',')
  return str.includes(prefix)
}

const showQtTitle = () => {
  const str = appPaywayList.value.join(',')
  return str.includes('WX') || str.includes('ALI') || str.includes('PP_PC')
}

const changeCurrentWayCode = (wayCode, dataType) => {
  currentWayCode.value = wayCode
  currentPayDataType.value = dataType
}

const changeAppId = (value) => {
  appPaywayListHandle(value)
}

const randomOrderNo = () => {
  mchOrderNo.value = 'M' + Date.now() + Math.floor(Math.random() * 9000 + 1000)
}

const amountInputShow = () => {
  amountInput.value = true
  customAmount.value = 0.01
  setTimeout(() => {
    amountInputFocus.value?.focus()
  }, 50)
}

const appPaywayListHandle = async (value) => {
  if (!value) {
    appPaywayList.value = []
    noConfigText.value = false
    return
  }
  try {
    const res = await payTestApi.getPayways(value)
    appPaywayList.value = res || []
    noConfigText.value = appPaywayList.value.length === 0
    currentWayCode.value = ''
    currentPayDataType.value = ''
  } catch (err) {
    console.error('获取支付方式失败:', err)
    appPaywayList.value = []
    noConfigText.value = true
  }
}

const barCodeChange = (value) => {
  authCode.value = value
  immediatelyPay()
}

const testCodeChange = () => {
  randomOrderNo()
}

const immediatelyPay = async () => {
  const amount = amountInput.value ? customAmount.value : paytestAmount.value
  if (!amount || amount === 0) {
    return message.error('请输入支付金额')
  }
  if (!currentWayCode.value) {
    return message.error('请选择支付方式')
  }
  if (!orderTitle.value || orderTitle.value.length > 20) {
    return message.error('请输入正确的订单标题[20字以内]')
  }

  const barCodeWays = ['WX_BAR', 'ALI_BAR', 'AUTO_BAR']
  if (barCodeWays.includes(currentWayCode.value) && !payTestBarCodeRef.value.getVisible()) {
    payTestBarCodeRef.value.showModal()
    return
  }

  try {
    const res = await payTestApi.createOrder({
      wayCode: (currentWayCode.value === 'WX_JSAPI' || currentWayCode.value === 'ALI_JSAPI') ? 'QR_CASHIER' : currentWayCode.value,
      amount,
      appId: appId.value,
      storeId: storeId.value,
      mchOrderNo: mchOrderNo.value,
      payDataType: currentPayDataType.value,
      authCode: authCode.value,
      divisionMode: divisionMode.value,
      orderTitle: orderTitle.value
    })
    payTestModal.value.showModal(currentWayCode.value, res)
    randomOrderNo()
  } catch (err) {
    console.error('支付订单创建失败:', err)
    payTestBarCodeRef.value.processCatch()
    randomOrderNo()
    message.error(err?.message || '支付订单创建失败')
  }
}

onMounted(async () => {
  // 优先使用路由携带的 appId 参数
  const routeAppId = route.params.appId
  if (routeAppId) {
    appId.value = routeAppId
    await appPaywayListHandle(routeAppId)
  }

  try {
    const [appRes, storeRes] = await Promise.all([
      mchAppApi.queryPage({ pageSize: -1 }),
      mchStoreApi.queryPage({ pageSize: -1 })
    ])
    mchAppList.value = appRes?.records || []
    mchStoreList.value = storeRes?.records || []
    if (!appId.value && mchAppList.value.length > 0) {
      appId.value = mchAppList.value[0].appId
      await appPaywayListHandle(appId.value)
    }
    if (!storeId.value && mchStoreList.value.length > 0) {
      storeId.value = mchStoreList.value[0].storeId
    }
  } catch (err) {
    console.error('加载应用/门店列表失败:', err)
  }
  randomOrderNo()
})
</script>
