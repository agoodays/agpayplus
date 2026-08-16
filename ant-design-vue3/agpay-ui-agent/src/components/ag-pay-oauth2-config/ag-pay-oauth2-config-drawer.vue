<template>
  <a-drawer
    v-model:open="localOpen"
    :title="'Oauth2配置'"
    width="80%"
    @close="handleClose"
  >
    <div v-show="configMode === 'mgrIsv'">
      <div style="margin-bottom: 20px">
        <label>选择配置的条目：</label>
        <a-select
          v-model:value="diyListSelectedInfoId"
          placeholder=""
          style="width: 380px; margin-right: 20px"
          @change="fetchSavedConfigs"
        >
          <a-select-option :value="infoId">默认</a-select-option>
          <a-select-option v-for="(item, key) in diyList" :key="key" :value="item.infoId">
            {{ item.remark + ' [ ID: ' + item.infoId + ' ]' }}
          </a-select-option>
        </a-select>
        <a-button v-show="diyAddMode === 'init'" type="primary" @click="diyAddMode = 'adding'">
          <template #icon><PlusOutlined /></template>
          创建
        </a-button>
      </div>
      <div v-show="diyAddMode === 'adding'">
        <label>输入名称：</label>
        <a-input v-model:value="addDiyListName" placeholder="" style="width: 160px" />
        <a-checkbox
          style="margin-left: 20px"
          v-model:checked="addDiyListIsCopyCurrentFlag"
        >
          复制当前参数
        </a-checkbox>
        <a-popover placement="top">
          <template #content>
            <p>勾选： 新创建的oauth2参数将来源自当前选择条目的记录值。且创建副本，互不干扰。</p>
            <p>不勾选： 创建全新的记录， 所有的参数需要重新填入。</p>
          </template>
          <template #title>
            <span>复制当前参数</span>
          </template>
          <QuestionCircleOutlined />
        </a-popover>
        <a-button type="danger" :style="{ marginLeft: '20px' }" @click="handleSaveDiy">
          <template #icon><CheckOutlined /></template>
          保存
        </a-button>
        <a-button type="primary" :style="{ marginLeft: '8px' }" @click="diyAddMode = 'init'">
          <template #icon><CloseOutlined /></template>
          取消
        </a-button>
      </div>
      <a-divider />
    </div>
    <a-tabs v-model:activeKey="currentIfCode" type="card" @change="fetchSavedConfigs">
      <a-tab-pane v-for="item in tabData" :key="item.code" :tab="item.name">
        <a-card style="padding: 30px">
          <component
            ref="currentComponentRef"
            :is="currentComponent"
            :config-mode="configMode"
            :form-data="ifParams"
            @update-if-params="handleUpdateIfParams"
          />
        </a-card>
      </a-tab-pane>
    </a-tabs>
    <template #footer>
      <div class="ag-drawer-footer">
        <slot name="footer">
          <a-space>
            <a-button @click="handleClose">
              <close-outlined />
              关闭
            </a-button>
            <a-button type="primary" :loading="loading" @click="handleSubmit">
              <check-outlined />
              保存
            </a-button>
          </a-space>
        </slot>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
/**
 * Oauth2 配置抽屉组件
 *
 * 用于配置微信 / 支付宝的 Oauth2 授权参数：
 * - 服务商模式（mgrIsv）下支持多配置条目管理（创建、选择、复制）
 * - 微信 / 支付宝 Tab 切换，动态加载对应渠道配置页面
 * - 参数保存前调用子组件的 validate 方法进行表单校验
 *
 * 通过 `update:open` 事件实现 v-model:open 双向绑定。
 */
import { payOauth2Api } from '@/api/business/pay-oauth2/pay-oauth2-api'
import { infoBox } from '@/utils/info-box'
import { CheckOutlined, CloseOutlined, PlusOutlined, QuestionCircleOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { markRaw, nextTick, ref, shallowRef, watch } from 'vue'

/** 渠道标签页定义（驱动 Tab 渲染） */
const TAB_DEFINITIONS = [
  { code: 'wxpay', name: '微信' },
  { code: 'alipay', name: '支付宝' }
]

/** 默认选中的渠道编码 */
const DEFAULT_IF_CODE = 'wxpay'

/** 创建模式状态 */
const DIY_ADD_MODE = {
  INIT: 'init',
  ADDING: 'adding'
}

const props = defineProps({
  /** 配置模式（如 mgrIsv、mgrMch） */
  configMode: { type: String, default: null },
  /** 抽屉打开状态（v-model:open 绑定） */
  open: { type: Boolean, default: false },
  /** 信息 ID */
  infoId: { type: String, default: '' },
  /** 是否服务商子商户 */
  isIsvSubMch: { type: Boolean, default: false }
})

const emit = defineEmits(['update:open'])

/** 抽屉本地打开状态（与 props.open 双向同步） */
const localOpen = ref(false)
/** 保存按钮加载状态 */
const loading = ref(false)
/** 当前选中的配置条目 ID */
const diyListSelectedInfoId = ref('')
/** 配置条目列表（服务商模式） */
const diyList = ref([])
/** 创建条目模式状态 */
const diyAddMode = ref(DIY_ADD_MODE.INIT)
/** 新建条目名称 */
const addDiyListName = ref('')
/** 新建条目时是否复制当前参数 */
const addDiyListIsCopyCurrentFlag = ref(true)
/** 当前选中的渠道编码 */
const currentIfCode = ref(DEFAULT_IF_CODE)
/** 渠道 Tab 数据 */
const tabData = ref([...TAB_DEFINITIONS])
/** 当前动态加载的配置组件（shallowRef 避免组件对象被深度响应式） */
const currentComponent = shallowRef(null)
/** 已加载组件缓存（ifCode -> component），避免重复 import */
const loadedComponents = shallowRef({})
/** 保存对象（包含 infoId、ifParams 等完整后端数据） */
const saveObject = ref({})
/** Oauth2 参数对象（传给子组件的 formData） */
const ifParams = ref({})
/** 子组件实例引用 */
const currentComponentRef = ref(null)

/** 监听 props.open 同步到本地状态，并在打开时初始化数据 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.infoId) {
      initDrawerData()
    }
  }
)

/** 监听本地 open 变化，同步 emit 给父组件 */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 初始化抽屉数据
 *
 * 重置选中条目，按需拉取配置条目列表，并加载当前渠道的已保存配置。
 */
const initDrawerData = () => {
  diyListSelectedInfoId.value = props.infoId
  if (props.configMode === 'mgrIsv') {
    fetchDiyList()
  }
  nextTick(() => {
    fetchSavedConfigs()
  })
}

/**
 * 关闭抽屉并重置全部状态
 */
const handleClose = () => {
  localOpen.value = false
  diyListSelectedInfoId.value = ''
  diyList.value = []
  diyAddMode.value = DIY_ADD_MODE.INIT
  addDiyListName.value = ''
  addDiyListIsCopyCurrentFlag.value = true
  currentIfCode.value = DEFAULT_IF_CODE
  saveObject.value = {}
  ifParams.value = {}
  currentComponent.value = null
}

/**
 * 动态加载当前渠道对应的配置组件（带缓存）
 *
 * 根据当前渠道编码（wxpay / alipay）和是否服务商子商户，
 * 动态 import 对应的配置页面组件，并缓存以避免重复加载。
 *
 * @returns {Promise<Object|null>} 加载成功的组件对象，失败返回 null
 */
const loadCurrentComponent = async () => {
  const suffix = props.isIsvSubMch ? 'isv-sub-mch-' : ''
  const componentPath = `./diy/${currentIfCode.value}/${suffix}oauth2-config-page.vue`

  const cached = loadedComponents.value[currentIfCode.value]
  if (cached) {
    return cached
  }

  try {
    const module = await import(componentPath)
    const component = markRaw(module.default || module)
    loadedComponents.value[currentIfCode.value] = component
    return component
  } catch (error) {
    console.error('加载 Oauth2 配置组件失败:', error)
    return null
  }
}

/**
 * 拉取服务商模式下的配置条目列表
 */
const fetchDiyList = async () => {
  try {
    const res = await payOauth2Api.queryDiyList({ configMode: props.configMode, infoId: props.infoId })
    diyList.value = res
  } catch (error) {
    console.error('获取配置条目列表失败:', error)
  }
}

/**
 * 拉取当前渠道的已保存配置并加载对应组件
 *
 * 切换渠道或切换条目时触发：
 * 1. 清空当前组件，拉取新配置
 * 2. 解析 ifParams，按渠道补全默认结构
 * 3. 动态加载对应渠道的配置组件
 */
const fetchSavedConfigs = async () => {
  currentComponent.value = null

  const params = {
    configMode: props.configMode,
    infoId: diyListSelectedInfoId.value,
    ifCode: currentIfCode.value
  }

  try {
    const res = await payOauth2Api.querySavedConfigs(params)
    if (res) {
      saveObject.value = res
      ifParams.value = JSON.parse(res.ifParams || '{}')

      if (currentIfCode.value === 'alipay') {
        ifParams.value.liteParams = ifParams.value.liteParams || {}
      }
      if (props.isIsvSubMch) {
        ifParams.value.isUseSubmchAccount = ifParams.value.isUseSubmchAccount || 0
      }
    }
  } catch (error) {
    console.error('获取 Oauth2 配置失败:', error)
  }

  await nextTick()

  const component = await loadCurrentComponent()
  if (component) {
    currentComponent.value = component
  } else {
    message.error('当前渠道不支持Oauth2配置！')
  }
}

/**
 * 子组件更新 ifParams 时同步到本地状态
 * @param {Object} params - 子组件 emit 的最新参数对象
 */
const handleUpdateIfParams = (params) => {
  ifParams.value = params
}

/**
 * 保存新建的配置条目
 *
 * 校验名称后通过二次确认提交创建请求，创建成功后刷新条目列表。
 */
const handleSaveDiy = async () => {
  if (!addDiyListName.value) {
    message.error('请输入名称')
    return
  }

  await new Promise((resolve, reject) => {
    infoBox.confirmPrimary(
      '确认新增该服务商的配置条目？',
      '新增后不支持修改/删除，请谨慎操作',
      async () => {
        try {
          const params = {
            infoId: props.infoId,
            configMode: props.configMode,
            remark: addDiyListName.value,
            copySourceInfoId: diyListSelectedInfoId.value
          }
          await payOauth2Api.createDiyList(params)
          message.success('保存成功')
          await fetchDiyList()
          resolve()
        } catch (error) {
          reject(error)
        }
      },
      () => {
        reject(new Error('用户取消'))
      }
    )
  }).catch(() => {
    // 用户取消或创建失败，静默处理
  })
}

/**
 * 提交保存 Oauth2 配置
 *
 * 流程：
 * 1. 调用子组件 validate 进行表单校验
 * 2. 校验通过后调用子组件 getSubmitParams 获取处理后参数
 * 3. 序列化 ifParams 并提交到后端
 */
const handleSubmit = async () => {
  try {
    await currentComponentRef.value.validate()
  } catch {
    return
  }

  if (Object.keys(ifParams.value).length === 0) {
    message.error('参数不能为空！')
    return
  }

  const params = currentComponentRef.value.getSubmitParams()
  saveObject.value.ifParams = JSON.stringify(params)

  loading.value = true
  try {
    await payOauth2Api.saveConfigParams(saveObject.value)
    message.success('保存成功')
  } catch (error) {
    console.error('保存 Oauth2 配置失败:', error)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped></style>
