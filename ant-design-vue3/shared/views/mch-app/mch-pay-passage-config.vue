<template>
  <ag-drawer
    v-model:open="localOpen"
    width="40%"
    title="配置支付通道"
    :mask-closable="false"
    :show-confirm="hasPermission('ENT_MCH_PAY_PASSAGE_ADD')"
    @confirm="handleConfirm"
    @close="handleClose">
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
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 商户支付通道配置组件
 * 功能：配置商户应用的支付通道和费率
 */
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { AgDrawer } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  appId: {
    type: String,
    default: ''
  },
  wayCode: {
    type: String,
    default: ''
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

/** 本地打开状态 */
const localOpen = ref(false)
const { hasPermission } = usePermission()

const cardList = ref([])

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.appId && props.wayCode) {
      cardList.value = []
      reloadCardList()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 加载支付通道卡片列表
 */
const reloadCardList = async () => {
  const resData = await mchAppApi.getAvailablePayInterfaceList(props.appId, props.wayCode)
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
}

/**
 * 保存支付通道配置
 */
const handleConfirm = async () => {
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
        appId: props.appId,
        wayCode: props.wayCode,
        ifCode: item.ifCode,
        rate: item.rate,
        state: item.state ? 1 : 0
      })
    }
  })

  if (hasError) return

  try {
    await mchAppApi.saveMchPayPassages({ reqParams: JSON.stringify(reqParams) })
    message.success('保存成功')
    localOpen.value = false
    emit('success')
  } catch (error) {
    console.error('保存失败:', error)
  }
}

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
}
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
  border-radius: 6px;
  overflow: hidden;
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: var(--base-bg-color);
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid var(--border-color);
  position: absolute;
  bottom: 0;
}
.ag-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}
.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  align-items: center;
}
.title {
  font-size: 16px;
  font-family:
    PingFang SC,
    PingFang SC-Bold;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}
</style>
