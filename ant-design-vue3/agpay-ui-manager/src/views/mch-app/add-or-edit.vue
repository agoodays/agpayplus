<template>
  <a-drawer
    v-model:open="visible"
    :title="isAdd ? '新增应用' : '修改应用'"
    :width="720"
    :mask-closable="false"
    :body-style="{ paddingBottom: '80px' }"
    @close="handleClose"
  >
    <a-form
      ref="formRef"
      :model="formState"
      :rules="rules"
      layout="vertical"
    >
      <!-- 基本信息 -->
      <a-row :gutter="16">
        <a-col v-if="!isAdd" :span="12">
          <a-form-item label="应用AppId" name="appId">
            <a-input
              v-model:value="formState.appId"
              placeholder="应用AppId"
              disabled
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="商户号" name="mchNo">
            <a-select
              v-model:value="formState.mchNo"
              placeholder="请选择商户"
              show-search
              :filter-option="false"
              :disabled="!isAdd"
              @search="handleSearchMch"
            >
              <a-select-option
                v-for="item in mchList"
                :key="item.mchNo"
                :value="item.mchNo"
              >
                {{ item.mchName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="应用名称" name="appName">
            <a-input
              v-model:value="formState.appName"
              placeholder="请输入应用名称"
            />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="备注" name="remark">
            <a-input
              v-model:value="formState.remark"
              placeholder="请输入备注"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="formState.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">停用</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="是否设置为默认应用" name="defaultFlag">
            <a-radio-group v-model:value="formState.defaultFlag">
              <a-radio :value="0">否</a-radio>
              <a-radio :value="1">是</a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 签名配置 -->
      <a-divider orientation="left" style="color: #1a66ff">
        签名配置
      </a-divider>

      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item name="appSignType">
            <template #label>
              <span>支持的签名方式</span>
              <a-tooltip>
                <template #title>
                  若需要使用系统测试或者商户通APP则必须支持MD5，若仅通过API调用则根据需求进行选择。
                </template>
                <question-circle-outlined style="margin-left: 4px" />
              </a-tooltip>
            </template>
            <a-checkbox-group v-model:value="formState.appSignType">
              <a-checkbox value="MD5">MD5</a-checkbox>
              <a-checkbox value="RSA2">RSA2</a-checkbox>
            </a-checkbox-group>
          </a-form-item>
        </a-col>
      </a-row>

      <!-- MD5秘钥 -->
      <template v-if="formState.appSignType?.includes('MD5')">
        <a-row :gutter="16">
          <a-col :span="24">
            <a-form-item label="设置MD5秘钥" name="appSecret">
              <a-textarea
                v-model:value="formState.appSecret"
                :placeholder="appSecretPlaceholder"
                :rows="3"
              />
              <a-button
                type="primary"
                ghost
                style="margin-top: 8px"
                @click="handleGenerateSecret"
              >
                <sync-outlined />
                随机生成私钥
              </a-button>
            </a-form-item>
          </a-col>
        </a-row>
      </template>

      <!-- RSA2配置 -->
      <template v-if="formState.appSignType?.includes('RSA2')">
        <a-row :gutter="16">
          <a-col :span="24">
            <a-form-item label="设置RSA2应用公钥" name="appRsa2PublicKey">
              <a-textarea
                v-model:value="formState.appRsa2PublicKey"
                placeholder="请输入RSA2应用公钥"
                :rows="4"
              />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row :gutter="16">
          <a-col :span="24">
            <a-form-item label="支付网关系统公钥（回调验签使用）">
              <a-textarea
                :value="sysRSA2PublicKey"
                disabled
                :rows="6"
              />
            </a-form-item>
          </a-col>
        </a-row>
      </template>
    </a-form>

    <!-- 底部按钮 -->
    <template #footer>
      <div style="text-align: center">
        <a-space>
          <a-button @click="handleClose">
            <close-outlined />
            取消
          </a-button>
          <a-button type="primary" :loading="loading" @click="handleSubmit">
            <check-outlined />
            保存
          </a-button>
        </a-space>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
import { ref, reactive, watch, computed, nextTick } from 'vue'
import { message } from 'ant-design-vue'
import {
  QuestionCircleOutlined,
  SyncOutlined,
  CloseOutlined,
  CheckOutlined
} from '@ant-design/icons-vue'
import { API_URL_MCH_APP, API_URL_MCH_LIST, req, getSysRSA2PublicKey } from '/@/api/manage'

// Props & Emits
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  },
  mchNo: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:visible', 'success'])

// State
const formRef = ref()
const loading = ref(false)
const isAdd = ref(true)
const visible = ref(false)
const mchList = ref([])
const sysRSA2PublicKey = ref('')
const originalAppSecret = ref('')

// 表单数据
const formState = reactive({
  appId: '',
  mchNo: '',
  appName: '',
  remark: '',
  state: 1,
  defaultFlag: 0,
  appSignType: ['MD5'],
  appSecret: '',
  appRsa2PublicKey: ''
})

// MD5秘钥占位符
const appSecretPlaceholder = computed(() => {
  return isAdd.value ? '请输入MD5秘钥' : (originalAppSecret.value || '请输入MD5秘钥')
})

// 表单验证规则
const rules = computed(() => ({
  mchNo: [
    { required: true, message: '请选择商户', trigger: 'change' }
  ],
  appName: [
    { required: true, message: '请输入应用名称', trigger: 'blur' }
  ],
  appSignType: [
    { required: true, message: '请选择签名方式', trigger: 'change', type: 'array' }
  ],
  appSecret: [
    {
      validator: (rule, value) => {
        if (formState.appSignType?.includes('MD5')) {
          if (isAdd.value && !value) {
            return Promise.reject('请输入MD5秘钥')
          }
          // 编辑时，如果没有输入新值且没有原始值，则提示
          if (!isAdd.value && !value && !originalAppSecret.value) {
            return Promise.reject('请输入MD5秘钥')
          }
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ],
  appRsa2PublicKey: [
    {
      validator: (rule, value) => {
        if (formState.appSignType?.includes('RSA2') && !value) {
          return Promise.reject('请输入RSA2应用公钥')
        }
        return Promise.resolve()
      },
      trigger: 'blur'
    }
  ]
}))

// 监听 props.visible 变化
watch(() => props.visible, (val) => {
  visible.value = val
  if (val) {
    initForm()
  }
})

// 监听 visible 变化
watch(visible, (val) => {
  emit('update:visible', val)
})

/**
 * 初始化表单
 */
const initForm = async () => {
  isAdd.value = !props.recordId

  // 加载系统 RSA2 公钥
  await loadSysRSA2PublicKey()

  if (isAdd.value) {
    // 新增模式，重置表单
    resetForm()
    // 如果传入了商户号，则自动填充
    if (props.mchNo) {
      formState.mchNo = props.mchNo
      // 加载商户信息
      await handleSearchMch('')
    }
  } else {
    // 编辑模式，加载数据
    await loadDetail()
  }
}

/**
 * 加载系统RSA2公钥
 */
const loadSysRSA2PublicKey = async () => {
  try {
    const key = await getSysRSA2PublicKey()
    sysRSA2PublicKey.value = key
  } catch (error) {
    console.error('加载系统RSA2公钥失败:', error)
  }
}

/**
 * 加载详情数据
 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await req.getById(API_URL_MCH_APP, props.recordId)
    
    Object.assign(formState, res)
    
    // 保存原始密钥，用于占位符显示
    originalAppSecret.value = res.appSecret || ''
    
    // 清空密钥输入框（编辑时不显示原密钥）
    formState.appSecret = ''
    
    // 处理签名方式（字符串转数组）
    if (typeof res.appSignType === 'string') {
      formState.appSignType = res.appSignType.split(',')
    }
  } catch (error) {
    message.error(error.msg || '加载数据失败')
  } finally {
    loading.value = false
  }
}

/**
 * 重置表单
 */
const resetForm = () => {
  Object.assign(formState, {
    appId: '',
    mchNo: props.mchNo || '',
    appName: '',
    remark: '',
    state: 1,
    defaultFlag: 0,
    appSignType: ['MD5'],
    appSecret: '',
    appRsa2PublicKey: ''
  })
  
  originalAppSecret.value = ''
  
  nextTick(() => {
    formRef.value?.clearValidate()
  })
}

/**
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  try {
    const res = await req.list(API_URL_MCH_LIST, {
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 生成随机密钥
 */
const handleGenerateSecret = () => {
  const length = 128
  const chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ'
  let secret = ''
  for (let i = 0; i < length; i++) {
    secret += chars.charAt(Math.floor(Math.random() * chars.length))
  }
  formState.appSecret = secret
  message.success('密钥已生成')
}

/**
 * 提交表单
 */
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    
    loading.value = true
    
    // 构建提交数据
    const data = { ...formState }
    
    // 处理签名方式（数组转字符串）
    if (Array.isArray(data.appSignType)) {
      data.appSignType = data.appSignType.join(',')
    }
    
    // 编辑模式下，如果没有输入新密钥，则删除该字段
    if (!isAdd.value && !data.appSecret) {
      delete data.appSecret
    }
    
    // 提交数据
    if (isAdd.value) {
      await req.add(API_URL_MCH_APP, data)
      message.success('新增成功')
    } else {
      await req.updateById(API_URL_MCH_APP, props.recordId, data)
      message.success('修改成功')
    }
    
    handleClose()
    emit('success')
  } catch (error) {
    if (error.errorFields) {
      // 表单验证失败
      return
    }
    console.error('提交失败:', error)
    message.error(error.msg || '操作失败')
  } finally {
    loading.value = false
  }
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  visible.value = false
}
</script>

<style lang="less" scoped>
:deep(.ant-divider-inner-text) {
  color: #1a66ff;
}
</style>
