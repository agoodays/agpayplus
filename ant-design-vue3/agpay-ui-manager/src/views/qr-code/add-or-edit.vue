<template>
  <ag-drawer
    v-model:open="localOpen"
    :mask-closable="false"
    :title="isAdd ? '新增码牌' : '修改码牌'"
    width="40%"
    @close="handleClose"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
  >
    <a-form
      ref="infoForm"
      :model="saveObject"
      layout="vertical"
      :rules="rules"
    >
      <a-row :gutter="16">
        <a-col v-if="isAdd" :span="14">
          <a-form-item label="批次号" name="batchId">
            <a-input-number v-model:value="saveObject.batchId" style="width: 70%; margin-right: 20px" />
            <a-button type="primary" size="small" @click="onToday">今天</a-button>
            <p class="agpay-tip-text">( 数字格式， 二维码编号的前缀， 建议采用： YYYYMMDD+次数表示 )</p>
          </a-form-item>
        </a-col>
        <a-col v-if="isAdd" :span="10">
          <a-form-item label="创建数量" name="addNum">
            <a-input-number v-model:value="saveObject.addNum" :min="1" :max="500" />
          </a-form-item>
        </a-col>
        <a-col v-if="isAdd" :span="24">
          <a-form-item label="选择模板" name="qrcShellId">
            <a-select v-model:value="saveObject.qrcShellId" placeholder="请选择模板">
              <a-select-option key="" value="">无</a-select-option>
              <a-select-option v-for="d in shellList" :key="d.id" :value="d.id">
                <a-tooltip placement="left">
                  <template #title>
                    <span
                      ><img
                        :style="{ width: '100%', height: '100%', cursor: 'pointer' }"
                        :src="d.shellImgViewUrl"
                        alt=""
                        @click="onPreview(d.shellImgViewUrl)"
                    /></span>
                  </template>
                  <span class="icon-style"><img class="icon" :src="d.shellImgViewUrl" alt="" /></span>
                </a-tooltip>
                {{ d.shellAlias }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="10">
          <a-form-item label="固定金额" name="fixedFlag">
            <a-radio-group v-model:value="saveObject.fixedFlag">
              <a-radio :value="0">任意金额</a-radio>
              <a-radio :value="1">固定金额</a-radio>
            </a-radio-group>
            <span v-if="saveObject.fixedFlag === 1">
              <a-input-number v-model:value="saveObject.fixedPayAmount" addon-after="元" />
              元
            </span>
          </a-form-item>
        </a-col>
        <a-col v-if="isAdd" :span="24">
          <a-form-item name="entryPage">
            <template #label>
              <span>
                <label title="选择页面类型" style="margin-right: 4px">扫码后页面类型</label>
                <a-popover placement="top">
                  <template #content>
                    <p>谨慎选择， 一经填写不可变更。</p>
                  </template>
                  <icons.QuestionCircleOutlined />
                </a-popover>
              </span>
            </template>
            <a-radio-group v-model:value="saveObject.entryPage">
              <a-radio :value="'default'">
                默认
                <a-popover placement="top">
                  <template #content>
                    <p>未指定，取决于二维码是否绑定到微信侧</p>
                  </template>
                  <icons.QuestionCircleOutlined />
                </a-popover>
              </a-radio>
              <a-radio :value="'h5'">固定H5页面</a-radio>
              <a-radio :value="'lite'">固定小程序页面</a-radio>
            </a-radio-group>
            <br/>
            <p class="agpay-tip-text">选择[默认/H5/小程序]任意一种后不可修改，请谨慎选择。</p>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item name="alipayWayCode">
            <template #label>
              <span>
                <label title="支付宝支付方式" style="margin-right: 4px">支付宝支付方式</label>
                <a-popover placement="top">
                  <template #content>
                    <p>仅H5呈现时生效</p>
                  </template>
                  <icons.QuestionCircleOutlined />
                </a-popover>
              </span>
            </template>
            <a-radio-group v-model:value="saveObject.alipayWayCode">
              <a-radio :value="'ALI_JSAPI'">ALI_JSAPI</a-radio>
              <a-radio :value="'ALI_WAP'">ALI_WAP</a-radio>
            </a-radio-group>
            <br/>
            <p class="agpay-tip-text">仅H5呈现时生效</p>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </ag-drawer>
</template>
<script setup>
/**
 * 二维码新增/编辑组件
 * 功能：支持二维码的新增和编辑操作，包含批次号、创建数量、状态、固定金额等配置
 */
import { AgDrawer } from '@/components'
import { QuestionCircleOutlined } from '@ant-design/icons-vue'
import { qrcApi } from '@/api/business/qr-code/qrc-api'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'
import { viewerApi } from '@/utils/viewer-api'
import { getStateOptions } from '@/constants/common-const'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

const icons = { QuestionCircleOutlined }

/**
 * 组件属性定义
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: [String, Number],
    default: null
  }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 本地打开状态
 */
const localOpen = ref(props.open)

/**
 * 监听外部打开状态变化
 */
watch(() => props.open, (val) => {
  localOpen.value = val
})

/**
 * 创建默认表单数据对象
 * @returns {Object} 默认表单数据
 */
const createDefaultSaveObject = () => ({
  batchId: null,
  addNum: 1,
  state: 1,
  fixedFlag: 0,
  entryPage: 'default',
  alipayWayCode: 'ALI_JSAPI'
})

/**
 * 表单引用
 */
const infoForm = ref(null)

/**
 * 是否为新增模式
 */
const isAdd = ref(true)

/**
 * 按钮加载状态
 */
const loading = ref(false)

/**
 * 模板列表
 */
const shellList = ref(null)

/**
 * 表单保存数据对象
 */
const saveObject = ref(createDefaultSaveObject())

/**
 * 表单验证规则
 */
const rules = {
  batchId: [{ required: true, message: '请输入批次号', trigger: 'blur' }],
  addNum: [
    { required: true, message: '请输入创建数量', trigger: 'blur' },
    {
      validator: (_rule, value, callback) => {
        if (value < 1 || value > 500) {
          callback('数量请介于1-500之间')
          return
        }
        callback()
      },
      trigger: 'blur'
    }
  ]
}

/**
 * 加载数据
 */
const loadData = async () => {
  isAdd.value = !props.recordId
  saveObject.value = createDefaultSaveObject()
  infoForm.value?.resetFields?.()

  if (isAdd.value) {
    const [shellRes, batchIdRes] = await Promise.all([
      qrcApi.listShells({ pageSize: -1, state: 1 }),
      qrcApi.getBatchIdDistinctCount()
    ])
    shellList.value = shellRes.records
    saveObject.value.batchId = +batchIdRes
    return
  }

  const res = await qrcApi.getById(props.recordId)
  saveObject.value = { ...res, fixedPayAmount: (res.fixedPayAmount / 100).toFixed(2) }
}

/**
 * 监听打开状态，加载数据
 */
watch(() => props.open, async (val) => {
  if (val) {
    await loadData()
  }
})

/**
 * 关闭抽屉
 */
const handleClose = () => {
  emit('update:open', false)
}

/**
 * 设置批次号为今天日期
 */
const onToday = () => {
  const today = new Date()
  const year = today.getFullYear().toString()
  const month = (today.getMonth() + 1).toString().padStart(2, '0')
  const day = today.getDate().toString().padStart(2, '0')
  saveObject.value.batchId = +`${year}${month}${day}00`
}

/**
 * 验证表单
 * @returns {boolean} 验证结果
 */
const validateForm = async () => {
  if (!infoForm.value?.validate) {
    return true
  }
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

/**
 * 确认提交
 */
const handleConfirm = async () => {
  const valid = await validateForm()
  if (!valid) return

  loading.value = true
  try {
    const params = { ...saveObject.value, fixedPayAmount: (saveObject.value.fixedPayAmount || 0) * 100 }
    if (isAdd.value) {
      await qrcApi.add(params)
      message.success('新增成功')
    } else {
      await qrcApi.updateById(props.recordId, params)
      message.success('修改成功')
    }
    emit('success')
    emit('update:open', false)
  } finally {
    loading.value = false
  }
}

/**
 * 预览图片
 * @param {string} url - 图片URL
 */
const onPreview = (url) => {
  viewerApi({
    images: [url],
    options: {
      initialViewIndex: 0
    }
  })
}
</script>

<style lang="less">
.icon-style {
  border-radius: 5px;
  padding-left: 2px;
  padding-right: 2px;
}

.icon {
  width: 18.37px;
  height: 26px;
  margin-bottom: 3px;
}
</style>