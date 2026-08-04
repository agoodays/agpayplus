<template>
  <ag-drawer
    v-model:open="localOpen"
    title="绑定分账接收者账号"
    :closable="true"
    :mask-closable="false"
    width="80%"
    @close="handleClose"
  >
    <template #footer>
      <a-button type="primary" :style="{ marginRight: '8px' }" @click="reqBatchBindReceiver(0)">
        <template #icon><RocketOutlined /></template>
        发起绑定请求
      </a-button>
      <a-button @click="onClose">
        <template #icon><CloseOutlined /></template>
        关闭
      </a-button>
    </template>

    <a-form>
      <a-row justify="space-between" style="margin-left: -20px; margin-right: -20px; row-gap: 0px">
        <a-col :span="6" style="padding-left: 20px; padding-right: 20px">
          <a-form-item label="商户号">
            <div style="display: flex">
              <ag-select-infinite
                v-model="mchNo"
                placeholder="商户号（搜索商户名称）"
                search-field="mchName"
                :fetch-data="searchMch"
                :field-names="{ label: 'mchName', value: 'mchNo' }"
                @change="changeMchNo"
              />
            </div>
          </a-form-item>
        </a-col>
        <a-col :span="6" style="padding-left: 20px; padding-right: 20px">
          <a-form-item label="选择商户应用">
            <div style="display: flex">
              <a-select v-model:value="appId" placeholder="应用ID" @change="changeAppId">
                <a-select-option v-for="item in mchAppList" :key="item.appId">
                  {{ item.appName }} [{{ item.appId }}]
                </a-select-option>
              </a-select>
            </div>
          </a-form-item>
        </a-col>
        <a-col :span="8" style="padding-left: 20px; padding-right: 20px">
          <a-form-item label="选择要加入到的账号分组">
            <div style="display: flex">
              <a-select v-model:value="selectedReceiverGroupId" placeholder="账号分组">
                <a-select-option v-for="item in allReceiverGroup" :key="item.receiverGroupId">
                  {{ item.receiverGroupName }}
                </a-select-option>
              </a-select>
              <a-button
                v-if="hasPermission('ENT_DIVISION_RECEIVER_GROUP_ADD')"
                type="primary"
                class="mg-b-30"
                style="margin-bottom: 0px; margin-left: 20px"
                @click="addGroupFunc"
              >
                <plus-outlined /> 新增
              </a-button>
            </div>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>

    <a-divider style="margin-bottom: 10px; margin-top: 0px" />

    <a-form v-show="!!appId">
      <a-row justify="space-between" type="flex" style="margin-left: -20px; margin-right: -20px; row-gap: 0px">
        <a-col :span="6" style="padding-left: 20px; padding-right: 20px">
          <a-form-item label="选择接口">
            <a-select v-model:value="ifCode" placeholder="账号所属接口">
              <a-select-option v-for="item in appSupportIfCodes" :key="item.ifCode">
                <span class="icon-style" :style="{ backgroundColor: item.bgColor }">
                  <img class="icon" :src="item.icon" alt="" />
                </span>
                {{ item.ifName }}[{{ item.ifCode }}]
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>

    <!-- 微信账号表格 -->
    <a-card v-show="ifCode === 'wxpay'" title="微信账号">
      <template #extra>
        <a-button style="background: green; color: white" @click="addReceiverRow('wxpay')">
          <template #icon><WechatOutlined /></template>
          添加【微信官方】分账接收账号
        </a-button>
      </template>
      <ag-table
        row-key="rowKey"
        state-key="division_receiver_acc"
        :columns="accTableColumns"
        :data="receiverTableData.filter((item) => item.ifCode == 'wxpay')"
        :pagination="false"
        :show-toolbar="false"
        :scroll-x="1400"
      >
        <template #reqBindStateSlot="{ record }">
          <div v-show="record.reqBindState == 0" style="color: salmon">
            <info-circle-outlined /> 待绑定
          </div>
          <div v-show="record.reqBindState == 1" style="color: green">
            <check-circle-outlined /> 绑定成功
          </div>
          <div v-show="record.reqBindState == 2" style="color: red">
            <close-circle-outlined /> 绑定异常
          </div>
        </template>

        <template #receiverAliasSlot="{ record }">
          <a-input v-model:value="record.receiverAlias" style="width: 150px" placeholder="(选填)默认为账号" />
        </template>

        <template #accTypeSlot="{ record }">
          <a-select v-model:value="record.accType" style="width: 110px" placeholder="账号类型" default-value="0">
            <a-select-option value="0">个人</a-select-option>
            <a-select-option value="1">微信商户</a-select-option>
          </a-select>
        </template>

        <template #accNoSlot="{ record }">
          <a-input v-model:value="record.accNo" style="width: 150px" />
          <a-tooltip title="扫码获取">
            <qrcode-outlined
              v-if="record.accType == 0"
              class="icon-style"
              @click="showChannelUserModal('wxpay', record)"
            />
          </a-tooltip>
        </template>

        <template #accNameSlot="{ record }">
          <a-input v-model:value="record.accName" />
        </template>

        <template #relationTypeSlot="{ record }">
          <a-select
            style="width: 110px"
            label-in-value
            placeholder="分账关系类型"
            :default-value="{ key: 'PARTNER' }"
            @change="(value) => changeRelationType(record, value)"
          >
            <a-select-option v-for="option in relationOptions" :key="option.key" :value="option.key">
              {{ option.label }}
            </a-select-option>
          </a-select>
        </template>

        <template #relationTypeNameSlot="{ record }">
          <a-input v-model:value="record.relationTypeName" :disabled="record.relationType !== 'CUSTOM'" />
        </template>

        <template #divisionProfitSlot="{ record }">
          <a-input v-model:value="record.divisionProfit" style="width: 65px" /> %
        </template>

        <template #opSlot="{ record }">
          <a-button type="link" @click="delRow(record)">删除</a-button>
        </template>
      </ag-table>
    </a-card>

    <br />

    <!-- 支付宝账号表格 -->
    <a-card v-show="ifCode === 'alipay'" title="支付宝账号">
      <template #extra>
        <a-button style="background: dodgerblue; color: white" @click="addReceiverRow('alipay')">
          <template #icon><AlipayCircleOutlined /></template>
          添加【支付宝官方】分账接收账号
        </a-button>
      </template>
      <ag-table
        row-key="rowKey"
        :columns="accTableColumns"
        :data="receiverTableData.filter((item) => item.ifCode == 'alipay')"
        :pagination="false"
        :show-toolbar="false"
        :scroll-x="1400"
      >
        <template #reqBindStateSlot="{ record }">
          <div v-show="record.reqBindState == 0" style="color: salmon">
            <info-circle-outlined /> 待绑定
          </div>
          <div v-show="record.reqBindState == 1" style="color: green">
            <check-circle-outlined /> 绑定成功
          </div>
          <div v-show="record.reqBindState == 2" style="color: red">
            <close-circle-outlined /> 绑定异常
          </div>
        </template>

        <template #receiverAliasSlot="{ record }">
          <a-input v-model:value="record.receiverAlias" style="width: 150px" placeholder="(选填)默认为账号" />
        </template>

        <template #accTypeSlot="{ record }">
          <a-select v-model:value="record.accType" style="width: 110px" placeholder="账号类型" default-value="0">
            <a-select-option value="0">个人</a-select-option>
            <a-select-option value="1">支付宝商户</a-select-option>
          </a-select>
        </template>

        <template #accNoSlot="{ record }">
          <a-input v-model:value="record.accNo" style="width: 150px" />
          <a-tooltip title="扫码获取">
            <qrcode-outlined
              v-if="record.accType == 0"
              class="icon-style"
              @click="showChannelUserModal('alipay', record)"
            />
          </a-tooltip>
        </template>

        <template #accNameSlot="{ record }">
          <a-input v-model:value="record.accName" />
        </template>

        <template #relationTypeSlot="{ record }">
          <a-select
            style="width: 110px"
            label-in-value
            placeholder="分账关系类型"
            :default-value="{ key: 'PARTNER' }"
            @change="(value) => changeRelationType(record, value)"
          >
            <a-select-option v-for="option in relationOptions" :key="option.key" :value="option.key">
              {{ option.label }}
            </a-select-option>
          </a-select>
        </template>

        <template #relationTypeNameSlot="{ record }">
          <a-input v-model:value="record.relationTypeName" :disabled="record.relationType !== 'CUSTOM'" />
        </template>

        <template #divisionProfitSlot="{ record }">
          <a-input v-model:value="record.divisionProfit" style="width: 65px" /> %
        </template>

        <template #opSlot="{ record }">
          <a-button type="link" @click="delRow(record)">删除</a-button>
        </template>
      </ag-table>
    </a-card>

    <add-or-edit ref="infoAddOrEdit" @success="getReceiverGroup" />
    <channel-user-modal ref="channelUserModal" @change-channel-user-id="changeChannelUserIdFunc" />
  </ag-drawer>
</template>

<script setup>
/**
 * 分账接收者账号绑定组件
 * 功能：批量绑定微信/支付宝分账接收者账号
 */
import { divisionReceiverApi } from '@/api/business/division/division-receiver-api'
import { AgDrawer, AgSelectInfinite, AgTable } from '@/components'
import { ChannelUserModal } from '@/components/channel-user'
import { usePermission } from '@/composables/useCommon'
import { genRowKey } from '@/utils/util'
import {
  AlipayCircleOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  CloseOutlined,
  InfoCircleOutlined,
  PlusOutlined,
  QrcodeOutlined,
  RocketOutlined,
  WechatOutlined
} from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'
import AddOrEdit from '../group/add-or-edit.vue'

const { hasPermission } = usePermission()

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  }
})

/** 事件定义 */
const emit = defineEmits(['update:open', 'success'])

const accTableColumns = [
  { key: 'reqBindState', title: '状态', width: 120, customRender: 'reqBindStateSlot' },
  { key: 'receiverAlias', title: '账号别名', width: 120, customRender: 'receiverAliasSlot' },
  { key: 'accType', title: '账号类型', width: 120, customRender: 'accTypeSlot' },
  { key: 'accNo', title: '接收方账号', width: 200, customRender: 'accNoSlot' },
  { key: 'accName', title: '接收方姓名', width: 200, customRender: 'accNameSlot' },
  { key: 'relationType', title: '分账关系', width: 200, customRender: 'relationTypeSlot' },
  { key: 'relationTypeName', title: '关系名称', width: 200, customRender: 'relationTypeNameSlot' },
  { key: 'divisionProfit', title: '默认分账比例', width: 120, customRender: 'divisionProfitSlot' },
  { key: 'op', title: '操作', customRender: 'opSlot' }
]

const defaultReceiverTemplate = {
  reqBindState: 0,
  receiverAlias: '',
  receiverGroupId: '',
  appId: '',
  ifCode: '',
  accType: '0',
  accNo: '',
  accName: '',
  relationType: 'PARTNER',
  relationTypeName: '合作伙伴',
  divisionProfit: ''
}

const relationOptions = [
  { key: 'PARTNER', label: '合作伙伴' },
  { key: 'SERVICE_PROVIDER', label: '服务商' },
  { key: 'STORE', label: '门店' },
  { key: 'STAFF', label: '员工' },
  { key: 'STORE_OWNER', label: '店主' },
  { key: 'HEADQUARTER', label: '总部' },
  { key: 'BRAND', label: '品牌方' },
  { key: 'DISTRIBUTOR', label: '分销商' },
  { key: 'USER', label: '用户' },
  { key: 'SUPPLIER', label: '供应商' },
  { key: 'CUSTOM', label: '自定义' }
]

const infoAddOrEdit = ref(null)
const channelUserModal = ref(null)

const localOpen = ref(false)
const mchNo = ref(null)
const appId = ref(null)
const ifCode = ref(null)
const selectedReceiverGroupId = ref(null)
const mchAppList = ref([])
const allReceiverGroup = ref([])
const appSupportIfCodes = ref([])
const receiverTableData = ref([])

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val) {
      reset()
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

const reset = () => {
  mchNo.value = null
  appId.value = null
  ifCode.value = null
  selectedReceiverGroupId.value = null
  mchAppList.value = []
  allReceiverGroup.value = []
  appSupportIfCodes.value = []
  receiverTableData.value = []
}

const searchMch = (params) => {
  return divisionReceiverApi.listMch(params)
}

const changeMchNo = (value) => {
  if (!value) {
    reset()
    return
  }
  getMchApp(value)
  getReceiverGroup(value)
}

/**
 * 获取商户应用列表
 * @param {string} currentMchNo - 商户号
 */
const getMchApp = async (currentMchNo) => {
  const res = await divisionReceiverApi.listMchApp({ pageSize: -1, mchNo: currentMchNo })
  mchAppList.value = res.records || []
  if (mchAppList.value.length > 0) {
    appId.value = `${mchAppList.value[0].appId}`
    await changeAppId(appId.value)
  }
}

/**
 * 获取接收者分组列表
 * @param {string} currentMchNo - 商户号
 */
const getReceiverGroup = async (currentMchNo) => {
  const res = await divisionReceiverApi.listReceiverGroup({ pageSize: -1, mchNo: currentMchNo })
  allReceiverGroup.value = res.records || []
  if (allReceiverGroup.value.length > 0) {
    selectedReceiverGroupId.value = allReceiverGroup.value[0].receiverGroupId
  }
}

const addGroupFunc = () => {
  infoAddOrEdit.value?.show()
}

/**
 * 获取应用支持的接口列表
 * @param {string} value - 应用ID
 */
const changeAppId = async (value) => {
  const res = await divisionReceiverApi.listIfCodeByAppId(value)
  appSupportIfCodes.value = res || []
}

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false
  emit('success')
}

const delRow = (item) => {
  const index = receiverTableData.value.indexOf(item)
  if (index > -1) {
    receiverTableData.value.splice(index, 1)
  }
}

const changeRelationType = (record, value) => {
  record.relationType = value.key
  if (value.key !== 'CUSTOM') {
    record.relationTypeName = value.label
  } else {
    record.relationTypeName = ''
  }
}

const showChannelUserModal = (currentIfCode, record) => {
  channelUserModal.value?.showModal(appId.value, currentIfCode, record)
}

const changeChannelUserIdFunc = ({ channelUserId, extObject }) => {
  extObject.accNo = channelUserId
}

/**
 * 添加分账接收者行
 * @param {string} currentIfCode - 接口代码
 */
const addReceiverRow = (currentIfCode) => {
  if (!selectedReceiverGroupId.value) {
    return message.error('请选择要加入的分组')
  }
  receiverTableData.value.push(
    Object.assign({}, defaultReceiverTemplate, { rowKey: genRowKey(), ifCode: currentIfCode, appId: appId.value })
  )
}

/**
 * 批量绑定分账接收者
 */
const reqBatchBindReceiver = async () => {
  if (receiverTableData.value.length <= 0) {
    return message.error('请先添加账号')
  }

  for (let i = 0; i < receiverTableData.value.length; i++) {
    const currentReceiver = receiverTableData.value[i]
    currentReceiver.receiverGroupId = selectedReceiverGroupId.value

    if (currentReceiver.reqBindState === 1) {
      continue
    }

    if (!currentReceiver.accNo) {
      return message.error(`第${i + 1}条： 接收方账号不能为空`)
    }

    if (currentReceiver.relationType === 'CUSTOM' && !currentReceiver.relationTypeName) {
      return message.error(`第${i + 1}条： 自定义类型时接收方账号名称不能为空`)
    }

    if (!currentReceiver.divisionProfit || currentReceiver.divisionProfit <= 0 || currentReceiver.divisionProfit > 100) {
      return message.error(`第${i + 1}条： 默认分账比例请设置在[0.01% ~ 100% ] 之间`)
    }

    try {
      const apiRes = await divisionReceiverApi.add(currentReceiver)
      if (apiRes.bindState === 1) {
        currentReceiver.reqBindState = 1
      } else {
        currentReceiver.reqBindState = 2
        message.error(`第${i + 1}条： 绑定异常，错误码：${apiRes.errCode}，错误信息：${apiRes.errMsg}`)
      }
    } catch (error) {
      currentReceiver.reqBindState = 2
      console.error('绑定失败:', error)
    }
  }

  message.success('已完成所有账号的绑定操作')
}


</script>

<style scoped>
::v-deep(.ant-table-wrapper) {
  margin: 0;
}
.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
  margin-left: 4px;
  cursor: pointer;
}
.icon {
  width: 15px;
  height: 14px;
  margin-bottom: 3px;
}
</style>