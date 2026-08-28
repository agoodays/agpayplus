import { reactive, ref } from 'vue'

/**
 * 清除对象中指定 key 的空值与对应占位符
 *
 * 用于提交前清理表单中的占位字段（_ph 后缀），
 * 并将空值字段置为 undefined 以避免序列化输出。
 *
 * @param {Object} obj - 待清理的对象
 * @param {string} key - 待清理的字段名
 */
const clearEmptyKey = (obj, key) => {
  if (!obj[key]) {
    obj[key] = undefined
  }
  obj[key + '_ph'] = undefined
}

/**
 * 初始化占位符字段
 *
 * 将敏感字段（如 appSecret、privateKey）的已保存值转移至 `_ph` 占位符，
 * 并清空原字段，以便在 UI 中以 placeholder 形式提示已配置过。
 *
 * @param {Object} target - 目标对象（会被原地修改）
 * @param {string} key - 字段名
 * @param {string} placeholder - 占位符文本
 */
const initPlaceholder = (target, key, placeholder) => {
  const value = target[key]
  target[key + '_ph'] = value || placeholder
  if (value) {
    target[key] = ''
  }
}

/**
 * Oauth2 配置表单 Composable
 *
 * 封装 Oauth2 配置页面（微信/支付宝，普通商户/服务商子商户）的通用逻辑：
 * - 表单数据初始化（含敏感字段占位符处理）
 * - ifParams 双向同步（emit update-if-params）
 * - 表单校验与重置
 * - 提交参数清理（移除占位符与空值）
 * - 文件上传成功回调
 *
 * @param {Object} props - 组件 props（需包含 formData）
 * @param {Object} props.formData - 后端返回的初始表单数据
 * @param {Function} emit - 组件 emit 函数
 * @param {Object} options - 配置项
 * @param {Array<{key: string, placeholder: string}>} [options.placeholders] - 顶层字段占位符配置
 * @param {Array<{key: string, placeholder: string}>} [options.liteParamsPlaceholders] - liteParams 字段占位符配置
 * @param {boolean} [options.hasLiteParams=false] - 是否包含 liteParams 嵌套对象
 * @param {Array<string>} [options.clearKeys=[]] - 提交时需清理的顶层字段名
 * @param {Array<string>} [options.clearLiteParamsKeys=[]] - 提交时需清理的 liteParams 字段名
 * @returns {Object} 表单相关状态与方法
 */
export function useOauth2Form(props, emit, options = {}) {
  const {
    placeholders = [],
    liteParamsPlaceholders = [],
    hasLiteParams = false,
    clearKeys = [],
    clearLiteParamsKeys = []
  } = options

  /** 表单实例引用 */
  const infoForm = ref(null)

  /** 表单数据副本（用于初始化占位符，不直接渲染） */
  const formDataRef = reactive({ ...props.formData })

  if (hasLiteParams) {
    formDataRef.liteParams = formDataRef.liteParams || {}
  }

  // 顶层字段占位符初始化
  placeholders.forEach(({ key, placeholder }) => {
    initPlaceholder(formDataRef, key, placeholder)
  })

  // liteParams 字段占位符初始化
  if (hasLiteParams) {
    liteParamsPlaceholders.forEach(({ key, placeholder }) => {
      initPlaceholder(formDataRef.liteParams, key, placeholder)
    })
  }

  /** 实际渲染用的表单数据 */
  const ifParams = reactive({ ...formDataRef })

  // 初始化时同步一次参数到父组件
  emit('update-if-params', { ...ifParams })

  /**
   * 更新顶层字段并同步到父组件
   * @param {string} key - 字段名
   * @param {*} value - 字段值
   */
  const updateIfParams = (key, value) => {
    emit('update-if-params', {
      ...ifParams,
      [key]: value
    })
  }

  /**
   * 更新 liteParams 字段并同步到父组件
   * @param {string} key - 字段名
   * @param {*} value - 字段值
   */
  const updateIfParamsLiteParams = (key, value) => {
    emit('update-if-params', {
      ...ifParams,
      liteParams: {
        ...ifParams.liteParams,
        [key]: value
      }
    })
  }

  /**
   * 顶层字段上传成功回调
   * @param {string} name - 字段名（bindName）
   * @param {Array} fileList - 文件列表
   */
  const uploadSuccess = (name, fileList) => {
    const [firstItem] = fileList
    ifParams[name] = firstItem?.url
    updateIfParams(name, firstItem?.url)
  }

  /**
   * liteParams 字段上传成功回调
   * @param {string} name - 字段名（bindName）
   * @param {Array} fileList - 文件列表
   */
  const uploadSuccessLiteParams = (name, fileList) => {
    const [firstItem] = fileList
    ifParams.liteParams[name] = firstItem?.url
    updateIfParamsLiteParams(name, firstItem?.url)
  }

  /**
   * 获取提交参数
   *
   * 深拷贝当前表单数据，清理占位符与空值字段后返回。
   * @returns {Object} 处理后的提交参数
   */
  const getSubmitParams = () => {
    const params = JSON.parse(JSON.stringify(ifParams) || '{}')
    clearKeys.forEach(key => clearEmptyKey(params, key))
    if (hasLiteParams && params.liteParams) {
      clearLiteParamsKeys.forEach(key => clearEmptyKey(params.liteParams, key))
    }
    return params
  }

  /**
   * 表单校验
   * @returns {Promise<void>} 校验失败时 reject
   */
  const validate = async () => {
    await infoForm.value.validate()
  }

  /**
   * 重置表单
   */
  const resetFields = () => {
    infoForm.value?.resetFields?.()
  }

  return {
    infoForm,
    ifParams,
    updateIfParams,
    updateIfParamsLiteParams,
    uploadSuccess,
    uploadSuccessLiteParams,
    getSubmitParams,
    validate,
    resetFields
  }
}
