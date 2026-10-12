<template>
  <div class="quick-cashier">
    <a-button type="link" @click="open">快捷收银</a-button>
    <a-modal v-model:open="visible" title="快捷收银" :footer="null" :mask-closable="false" :width="560">
      <div class="quick-cashier-selectors">
        <a-form-item class="quick-cashier-form-item" label="应用">
          <a-select v-model:value="saveObject.appId" placeholder="选择应用">
            <a-select-option v-for="item in appList" :key="item.appId" :value="item.appId">
              {{ item.appName }} [{{ item.appId }}]
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item class="quick-cashier-form-item" label="门店">
          <a-select v-model:value="saveObject.storeId" allow-clear placeholder="选择门店">
            <a-select-option v-for="item in storeList" :key="item.storeId" :value="item.storeId">
              {{ item.storeName }} [{{ item.storeId }}]
            </a-select-option>
          </a-select>
        </a-form-item>
      </div>

      <div class="quick-cashier-label">选择收款方式</div>
      <div class="quick-cashier-methods">
        <button
          v-for="method in methods"
          :key="method.id"
          type="button"
          class="quick-cashier-method"
          :class="{ active: payMethod === method.id }"
          @click="selectMethod(method.id, method.wayCode)"
        >
          <component :is="method.icon" />
          <span>{{ method.label }}</span>
        </button>
      </div>

      <a-form-item class="quick-cashier-form-item" label="订单金额">
        <a-input ref="amountInput" v-model:value="amount" inputmode="decimal" @change="syncAmount" />
      </a-form-item>
      <a-form-item class="quick-cashier-form-item" label="备注">
        <a-input v-model:value="saveObject.sellerRemark" :maxlength="100" />
      </a-form-item>

      <div class="quick-cashier-keyboard">
        <div class="quick-cashier-numbers">
          <button v-for="(key, index) in numberList" :key="index" type="button" @click="enterAmount(key)">
            {{ key }}
          </button>
        </div>
        <div class="quick-cashier-operations">
          <a-button class="quick-cashier-delete" aria-label="删除末位金额" @click="deleteAmount">
            <DeleteOutlined />
            <span>删除</span>
          </a-button>
          <a-button class="quick-cashier-confirm" type="primary" :loading="submitting" @click="immediatelyPay">
            确认支付
          </a-button>
        </div>
      </div>
    </a-modal>

    <PayTestModal ref="payTestModal" @close-bar-code="closeBarCode" />
    <PayTestBarCode ref="payTestBarCode" @bar-code-value="handleBarCode" @code-again-change="randomOrderNo" />
  </div>
</template>

<script setup>
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { mchStoreApi } from '@/api/business/mch-store/mch-store-api'
import { payTestApi } from '@/api/business/pay-test/pay-test-api'
import PayTestBarCode from '@/components/pay-test/pay-test-bar-code.vue'
import PayTestModal from '@/components/pay-test/pay-test-modal.vue'
import { DeleteOutlined, QrcodeOutlined, ScanOutlined, SmileOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { nextTick, onMounted, reactive, ref } from 'vue'

const methods = [
  { id: 'sm', label: '付款主扫', wayCode: 'QR_CASHIER', icon: QrcodeOutlined },
  { id: 'fk', label: '付款被扫', wayCode: 'AUTO_BAR', icon: ScanOutlined },
  { id: 'sl', label: '刷脸支付', wayCode: 'AUTO_BAR', icon: SmileOutlined }
]
const numberList = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '0', '00']
const visible = ref(false)
const submitting = ref(false)
const payMethod = ref('sm')
const amount = ref('')
const appList = ref([])
const storeList = ref([])
const amountInput = ref(null)
const payTestModal = ref(null)
const payTestBarCode = ref(null)
const saveObject = reactive({
  appId: undefined,
  storeId: undefined,
  authCode: null,
  amount: 0,
  sellerRemark: '',
  divisionMode: 0,
  mchOrderNo: '',
  orderTitle: '快捷收款',
  payDataType: 'codeImgUrl',
  wayCode: 'QR_CASHIER'
})

const open = () => {
  payMethod.value = 'sm'
  amount.value = ''
  saveObject.amount = 0
  saveObject.authCode = null
  saveObject.sellerRemark = ''
  saveObject.wayCode = 'QR_CASHIER'
  saveObject.appId = appList.value[0]?.appId
  saveObject.storeId = storeList.value[0]?.storeId
  visible.value = true
  randomOrderNo()
  nextTick(() => amountInput.value?.focus())
}

const selectMethod = (method, wayCode) => {
  payMethod.value = method
  saveObject.wayCode = wayCode
}

const syncAmount = () => {
  const numericAmount = Number(amount.value)
  saveObject.amount = Number.isFinite(numericAmount) ? numericAmount : 0
}

const enterAmount = (key) => {
  if (
    Number(amount.value) >= 9999999 ||
    (key === '.' && amount.value.includes('.')) ||
    (amount.value.includes('.') && amount.value.split('.').pop().length >= 2)
  ) {
    return
  }
  if ((amount.value === '' || amount.value.length >= 6) && key === '00') return

  amount.value = amount.value === '0' ? key : `${amount.value}${key}`
  if (amount.value === '.') amount.value = '0.'
  syncAmount()
}

const deleteAmount = () => {
  amount.value = amount.value.slice(0, -1)
  syncAmount()
}

const randomOrderNo = () => {
  saveObject.mchOrderNo = `M${Date.now()}${Math.floor(Math.random() * 9000 + 1000)}`
}

const handleBarCode = (value) => {
  saveObject.authCode = value
  immediatelyPay()
}

const closeBarCode = () => {
  payTestBarCode.value?.closeModal?.()
}

const immediatelyPay = async () => {
  const amountPattern = /^(0|[1-9]\d*)(\.\d{1,2})?$/
  if (!amountPattern.test(amount.value) || Number(amount.value) < 0) {
    message.error('请输入合法金额')
    return
  }
  if (saveObject.amount === 0) {
    message.error('请输入支付金额')
    return
  }
  if (!saveObject.appId) {
    message.error('请选择应用')
    return
  }
  if (!saveObject.orderTitle || saveObject.orderTitle.length > 20) {
    message.error('请输入正确的订单标题[20字以内]')
    return
  }

  if (saveObject.wayCode === 'AUTO_BAR' && !payTestBarCode.value?.getVisible()) {
    payTestBarCode.value?.showModal()
    return
  }

  submitting.value = true
  try {
    const result = await payTestApi.createOrder(saveObject)
    payTestModal.value?.showModal(saveObject.wayCode, result)
    randomOrderNo()
    saveObject.authCode = null
  } catch (error) {
    payTestBarCode.value?.processCatch()
    message.error(error?.message || '支付订单创建失败')
    randomOrderNo()
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  try {
    const [appResult, storeResult] = await Promise.all([
      mchAppApi.queryPage({ pageSize: -1 }),
      mchStoreApi.queryPage({ pageSize: -1 })
    ])
    appList.value = appResult?.records || []
    storeList.value = storeResult?.records || []
    saveObject.appId = appList.value[0]?.appId
    saveObject.storeId = storeList.value[0]?.storeId
  } catch (error) {
    message.error(error?.message || '应用或门店列表加载失败')
  }
  randomOrderNo()
})
</script>

<style lang="less" scoped>
.quick-cashier-selectors {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;

  :deep(.ant-form-item) {
    margin-bottom: 12px;
  }
}

.quick-cashier-form-item :deep(.ant-form-item-label) {
  flex: 0 0 76px;
  max-width: 76px;
  text-align: right;
}

.quick-cashier-form-item :deep(.ant-form-item-label > label) {
  justify-content: flex-end;
  color: var(--text-color-weak);
}

.quick-cashier-form-item :deep(.ant-form-item-control) {
  flex: 1 1 0;
  min-width: 0;
}

.quick-cashier-label {
  margin: 12px 0;
  text-align: center;
  font-weight: 600;
}

.quick-cashier-methods {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 12px;
  margin-bottom: 18px;
}

.quick-cashier-method {
  display: flex;
  min-height: 76px;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  background: var(--surface-light);
  color: var(--text-color);
  cursor: pointer;
  transition:
    border-color 0.2s ease,
    color 0.2s ease,
    background-color 0.2s ease;

  .anticon {
    font-size: 22px;
  }

  &:hover {
    border-color: var(--primary-color);
    background: var(--surface-lighter);
  }

  &.active {
    border-color: var(--primary-color, #1677ff);
    background: var(--primary-color-weak);
    color: var(--primary-color, #1677ff);
  }
}

.quick-cashier-keyboard {
  display: grid;
  grid-template-columns: 3fr 1fr;
  align-items: stretch;
  gap: 8px;
  border-top: 1px solid var(--border-color);
  margin-top: 18px;
  padding-top: 18px;
}

.quick-cashier-numbers {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 6px;

  button {
    min-height: 48px;
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background: var(--surface-light);
    color: var(--text-color);
    font-size: 18px;
    cursor: pointer;
    transition:
      border-color 0.2s ease,
      background-color 0.2s ease;

    &:hover {
      border-color: var(--primary-color);
      background: var(--surface-lighter);
    }

    &:active {
      transform: translateY(1px);
    }
  }
}

.quick-cashier-operations {
  display: grid;
  grid-template-rows: repeat(4, minmax(0, 1fr));
  gap: 6px;

  :deep(.ant-btn) {
    width: 100%;
    min-height: 48px;
  }

  :deep(.quick-cashier-delete) {
    grid-row: span 1;
    gap: 8px;
    border-color: var(--border-color);
    background: var(--surface-light) !important;
    color: var(--text-color);
    transition:
      border-color 0.2s ease,
      background-color 0.2s ease;
  }

  :deep(.quick-cashier-delete:hover) {
    border-color: var(--primary-color);
    background: var(--surface-lighter) !important;
  }

  :deep(.quick-cashier-confirm) {
    grid-row: span 3;
    height: 100%;
    white-space: normal;
    font-size: 16px;
    font-weight: 600;
    transition:
      filter 0.2s ease,
      transform 0.1s ease;
  }

  :deep(.quick-cashier-confirm:hover) {
    filter: brightness(1.06);
  }

  :deep(.quick-cashier-confirm:active) {
    transform: translateY(1px);
  }
}

:global(:root[data-theme='dark'] .ant-btn:not(.ant-btn-link):not(.ant-btn-primary).quick-cashier-delete) {
  background: var(--surface-light) !important;
}

:global(:root[data-theme='dark'] .ant-btn:not(.ant-btn-link):not(.ant-btn-primary).quick-cashier-delete:hover) {
  background: var(--surface-lighter) !important;
}

@media (max-width: 520px) {
  .quick-cashier-selectors {
    grid-template-columns: 1fr;
    gap: 0;
  }
}
</style>
