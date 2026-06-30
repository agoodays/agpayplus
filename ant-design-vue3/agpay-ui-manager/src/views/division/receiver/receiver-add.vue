<template>
  <a-drawer
    :visible="visible"
    title="绑定分账接收者账号"
    class="drawer-width"
    :closable="true"
    :mask-closable="false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    @close="onClose"
  >
    <a-form-model>
      <a-row justify="space-between" style="margin-left: -20px; margin-right: -20px; row-gap: 0px">
        <a-col :span="6" style="padding-left: 20px; padding-right: 20px">
          <a-form-model-item label="商户号">
            <div style="display: flex">
              <ag-select
                v-model="mchNo"
                :api="searchMch"
                value-field="mchNo"
                label-field="mchName"
                placeholder="商户号（搜索商户名称）"
                @change="changeMchNo"
              />
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="6" style="padding-left: 20px; padding-right: 20px">
          <a-form-model-item label="选择商户应用">
            <div style="display: flex">
              <a-select v-model="appId" placeholder="应用ID" @change="changeAppId">
                <a-select-option v-for="item in mchAppList" :key="item.appId"
                  >{{ item.appName }} [{{ item.appId }}]</a-select-option
                >
              </a-select>
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="8" style="padding-left: 20px; padding-right: 20px">
          <a-form-model-item label="选择要加入到的账号分组">
            <div style="display: flex">
              <a-select v-model="selectedReceiverGroupId" placeholder="账号分组">
                <a-select-option v-for="item in allReceiverGroup" :key="item.receiverGroupId">{{
                  item.receiverGroupName
                }}</a-select-option>
              </a-select>
              <a-button
                v-if="$access('ENT_DIVISION_RECEIVER_GROUP_ADD')"
                type="primary"
                icon="plus"
                class="mg-b-30"
                style="margin-bottom: 0px; margin-left: 20px"
                @click="addGroupFunc"
                >新建</a-button
              >
            </div>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <a-divider style="margin-bottom: 10px; margin-top: 0px" />
    <a-form v-show="!!appId">
      <a-row justify="space-between" type="flex" style="margin-left: -20px; margin-right: -20px; row-gap: 0px">
        <a-col :span="6" style="padding-left: 20px; padding-right: 20px">
          <a-form-item label="选择接口">
            <a-select v-model="ifCode" placeholder="账号所属接口">
              <a-select-option v-for="item in appSupportIfCodes" :key="item.ifCode">
                <span class="icon-style" :style="{ backgroundColor: item.bgColor }"
                  ><img class="icon" :src="item.icon" alt=""
                /></span>
                {{ item.ifName }}[{{ item.ifCode }}]
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <a-card v-show="ifCode === 'wxpay'" title="微信账号">
      <template #extra>
        <a href="#">
          <a-button style="background: green; color: white" icon="wechat" @click="addReceiverRow('wxpay')"
            >添加【微信官方】分账接收账号</a-button
          >
        </a>
      </template>
      <a-table
        :columns="accTableColumns"
        :data-source="receiverTableData.filter((item) => item.ifCode == 'wxpay')"
        :pagination="false"
        row-key="rowKey"
      >
        <!-- 账号类型 -->
        <template #reqBindStateSlot="record">
          <div v-show="record.reqBindState == 0" style="color: salmon"><a-icon type="info-circle" /> 待绑定</div>
          <div v-show="record.reqBindState == 1" style="color: green"><a-icon type="check-circle" /> 绑定成功</div>
          <div v-show="record.reqBindState == 2" style="color: red"><a-icon type="close-circle" /> 绑定异常</div>
        </template>

        <!-- 账号别名 -->
        <template #receiverAliasSlot="record">
          <a-input v-model="record.receiverAlias" style="width: 150px" placeholder="(选填)默认为账号" />
        </template>

        <!-- 账号类型 -->
        <template #accTypeSlot="record">
          <a-select v-model="record.accType" style="width: 110px" placeholder="账号类型" default-value="0">
            <a-select-option value="0">个人</a-select-option>
            <a-select-option value="1">微信商户</a-select-option>
          </a-select>
        </template>

        <!-- 接收方账号 -->
        <template #accNoSlot="record">
          <a-input v-model="record.accNo" style="width: 150px" />
          <a-tooltip title="扫码获取">
            <a-icon
              v-if="record.accType == 0"
              type="qrcode"
              class="icon-style"
              @click="showChannelUserModal('wxpay', record)"
            />
          </a-tooltip>
        </template>

        <!-- 接收方姓名 -->
        <template #accNameSlot="record">
          <a-input v-model="record.accName" />
        </template>

        <!-- 分账关系 -->
        <template #relationTypeSlot="record">
          <a-select
            style="width: 110px"
            label-in-value
            placeholder="分账关系类型"
            :default-value="{ key: 'PARTNER' }"
            @change="changeRelationType(record, $event)"
          >
            <a-select-option v-for="option in relationOptions" :key="option.key" :value="option.key">{{
              option.label
            }}</a-select-option>
          </a-select>
        </template>

        <!-- 关系名称 -->
        <template #relationTypeNameSlot="record">
          <a-input v-model="record.relationTypeName" :disabled="record.relationType !== 'CUSTOM'" />
        </template>

        <!-- 默认分账比例 -->
        <template #divisionProfitSlot="record">
          <a-input v-model="record.divisionProfit" style="width: 65px" /> %
        </template>

        <template #opSlot="record"><a-button type="link" @click="delRow(record)">删除</a-button></template>
        >
      </a-table>
    </a-card>

    <br />
    <a-card v-show="ifCode === 'alipay'" title="支付宝账号">
      <template #extra>
        <a href="#">
          <a-button style="background: dodgerblue; color: white" icon="alipay-circle" @click="addReceiverRow('alipay')"
            >添加【支付宝官方】分账接收账号</a-button
          >
        </a>
      </template>
      <a-table
        :columns="accTableColumns"
        :data-source="receiverTableData.filter((item) => item.ifCode == 'alipay')"
        :pagination="false"
        row-key="rowKey"
      >
        <!-- 账号类型 -->
        <template #reqBindStateSlot="record">
          <div v-show="record.reqBindState == 0" style="color: salmon"><a-icon type="info-circle" /> 待绑定</div>
          <div v-show="record.reqBindState == 1" style="color: green"><a-icon type="check-circle" /> 绑定成功</div>
          <div v-show="record.reqBindState == 2" style="color: red"><a-icon type="close-circle" /> 绑定异常</div>
        </template>

        <!-- 账号别名 -->
        <template #receiverAliasSlot="record">
          <a-input v-model="record.receiverAlias" style="width: 150px" placeholder="(选填)默认为账号" />
        </template>

        <!-- 账号类型 -->
        <template #accTypeSlot="record">
          <a-select v-model="record.accType" style="width: 110px" placeholder="账号类型" default-value="0">
            <a-select-option value="0">个人</a-select-option>
            <a-select-option value="1">微信商户</a-select-option>
          </a-select>
        </template>

        <!-- 接收方账号 -->
        <template #accNoSlot="record">
          <a-input v-model="record.accNo" style="width: 150px" />
          <a-tooltip title="扫码获取">
            <a-icon
              v-if="record.accType == 0"
              type="qrcode"
              class="icon-style"
              @click="showChannelUserModal('alipay', record)"
            />
          </a-tooltip>
        </template>

        <!-- 接收方姓名 -->
        <template #accNameSlot="record">
          <a-input v-model="record.accName" />
        </template>

        <!-- 分账关系 -->
        <template #relationTypeSlot="record">
          <a-select
            style="width: 110px"
            label-in-value
            placeholder="分账关系类型"
            :default-value="{ key: 'PARTNER' }"
            @change="changeRelationType(record, $event)"
          >
            <a-select-option v-for="option in relationOptions" :key="option.key" :value="option.key">{{
              option.label
            }}</a-select-option>
          </a-select>
        </template>

        <!-- 关系名称 -->
        <template #relationTypeNameSlot="record">
          <a-input v-model="record.relationTypeName" :disabled="record.relationType !== 'CUSTOM'" />
        </template>

        <!-- 默认分账比例 -->
        <template #divisionProfitSlot="record">
          <a-input v-model="record.divisionProfit" style="width: 65px" /> %
        </template>

        <template #opSlot="record"><a-button type="link" @click="delRow(record)">删除</a-button></template>
        >
      </a-table>
    </a-card>

    <div class="drawer-btn-center">
      <a-button type="primary" icon="rocket" :style="{ marginRight: '8px' }" @click="reqBatchBindReceiver(0)"
        >发起绑定请求</a-button
      >
      <a-button icon="close" @click="onClose">关闭</a-button>
    </div>

    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="getReceiverGroup" />
    <ChannelUserModal ref="channelUserModal" @change-channel-user-id="changeChannelUserIdFunc($event)" />
  </a-drawer>
</template>

<script setup>
import { divisionReceiverApi } from '@/api/business/division/division-receiver-api'
import AgSelect from '@/components/ag-select'
import ChannelUserModal from '@/components/channel-user'
import { genRowKey } from '@/utils/util'
import { ref } from 'vue'
import InfoAddOrEdit from '../group/add-or-edit.vue'

// eslint-disable-next-line no-unused-vars
const accTableColumns = [
  { key: 'reqBindState', title: '状态', width: 120, scopedSlots: { customRender: 'reqBindStateSlot' } },
  { key: 'receiverAlias', title: '账号别名', width: 120, scopedSlots: { customRender: 'receiverAliasSlot' } },
  { key: 'accType', title: '账号类型', width: 120, scopedSlots: { customRender: 'accTypeSlot' } },
  { key: 'accNo', title: '接收方账号', width: 200, scopedSlots: { customRender: 'accNoSlot' } },
  { key: 'accName', title: '接收方姓名', width: 200, scopedSlots: { customRender: 'accNameSlot' } },
  { key: 'relationType', title: '分账关系', width: 200, scopedSlots: { customRender: 'relationTypeSlot' } },
  { key: 'relationTypeName', title: '关系名称', width: 200, scopedSlots: { customRender: 'relationTypeNameSlot' } },
  { key: 'divisionProfit', title: '默认分账比例', width: 120, scopedSlots: { customRender: 'divisionProfitSlot' } },
  { key: 'op', title: '操作', scopedSlots: { customRender: 'opSlot' } }
]

const defaultReceiverTemplate = {
  reqBindState: 0, // 默认待绑定
  receiverAlias: '',
  receiverGroupId: '',
  appId: '',
  ifCode: '',
  accType: '0',
  accNo: '',
  accName: '',
  relationType: 'PARTNER', // 默认合作伙伴, 需要同时更改select的defaultValue
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

const props = defineProps({
  callbackFunc: {
    type: Function,
    default: () => ({})
  }
})

const emit = defineEmits(['close'])

const infoAddOrEdit = ref()
const channelUserModal = ref()

const visible = ref(false)
const mchNo = ref(null)
const appId = ref(null)
const ifCode = ref(null)
const selectedReceiverGroupId = ref(null)
const mchAppList = ref([])
const allReceiverGroup = ref([])
const appSupportIfCodes = ref([])
const receiverTableData = ref([])

const show = () => {
  reset()
  visible.value = true
}

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

const getMchApp = (currentMchNo) => {
  divisionReceiverApi.listMchApp({ pageSize: -1, mchNo: currentMchNo }).then((res) => {
    mchAppList.value = res.records || []
    if (mchAppList.value.length > 0) {
      appId.value = `${mchAppList.value[0].appId}`
      changeAppId(appId.value)
    }
  })
}

const getReceiverGroup = (currentMchNo) => {
  divisionReceiverApi.listReceiverGroup({ pageSize: -1, mchNo: currentMchNo }).then((res) => {
    allReceiverGroup.value = res.records || []
    if (allReceiverGroup.value.length > 0) {
      selectedReceiverGroupId.value = allReceiverGroup.value[0].receiverGroupId
    }
  })
}

const addGroupFunc = () => {
  infoAddOrEdit.value?.show()
}

const changeAppId = (value) => {
  divisionReceiverApi.listIfCodeByAppId(value).then((res) => {
    appSupportIfCodes.value = res || []
  })
}

const onClose = () => {
  props.callbackFunc()
  visible.value = false
  emit('close')
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

const addReceiverRow = (currentIfCode) => {
  if (!selectedReceiverGroupId.value) {
    return window.$message.error('请选选择要加入的分组')
  }
  receiverTableData.value.push(
    Object.assign({}, defaultReceiverTemplate, { rowKey: genRowKey(), ifCode: currentIfCode, appId: appId.value })
  )
}

const reqBatchBindReceiver = (i) => {
  if (receiverTableData.value.length <= 0) {
    return window.$message.error('请先添加账号')
  }

  if (i >= receiverTableData.value.length) {
    return window.$message.success('已完成所有账号的绑定操作')
  }

  const currentReceiver = receiverTableData.value[i]
  currentReceiver.receiverGroupId = selectedReceiverGroupId.value

  if (currentReceiver.reqBindState === 1) {
    return reqBatchBindReceiver(i + 1)
  }

  if (!currentReceiver.accNo) {
    return window.$message.error(`第${i + 1}条： 接收方账号不能为空`)
  }

  if (currentReceiver.relationType === 'CUSTOM' && !currentReceiver.relationTypeName) {
    return window.$message.error(`第${i + 1}条： 自定义类型时接收方账号名称不能为空`)
  }

  if (!currentReceiver.divisionProfit || currentReceiver.divisionProfit <= 0 || currentReceiver.divisionProfit > 100) {
    return window.$message.error(`第${i + 1}条： 默认分账比例请设置在[0.01% ~ 100% ] 之间`)
  }

  divisionReceiverApi
    .add(currentReceiver)
    .then((apiRes) => {
      if (apiRes.bindState === 1) {
        currentReceiver.reqBindState = 1
        reqBatchBindReceiver(i + 1)
      } else {
        currentReceiver.reqBindState = 2
        window.$infoBox.modalError(`第${i + 1}条： 绑定异常`, `错误码：${apiRes.errCode}\n错误信息：${apiRes.errMsg}`)
      }
    })
    .catch(() => {
      currentReceiver.reqBindState = 2
    })
}

defineExpose({ show })
</script>
<style scoped>
::v-deep(.ant-table-wrapper) {
  margin: 0;
}
.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
}
.icon {
  width: 15px;
  height: 14px;
  margin-bottom: 3px;
}
</style>
