/**
 * 通用信息弹层
 *
 */
import { Modal } from 'ant-design-vue'
import { translateWithFallback } from '@/utils/i18n-util'

function getConfirmDefaults() {
  return {
    okText: translateWithFallback('common.confirm', '确定'),
    cancelText: translateWithFallback('common.cancel', '取消')
  }
}

function openMessageModal(method, title, content, okFunc, titleKey, titleFallback) {
  return method({
    title: title || translateWithFallback(titleKey, titleFallback),
    content,
    onOk: okFunc,
    okText: translateWithFallback('common.confirm', '确定')
  })
}

// 确认提示： 标题， 内容， 点击确定回调函数， 取消回调，  扩展参数
export const infoBox = {
  confirm: function (title, content, okFunc, cancelFunc = () => {}, extConfig = {}) {
    const defaultConfig = {
      ...getConfirmDefaults(),
      title: title || translateWithFallback('common.tip', '提示'),
      content,
      onOk: okFunc,
      onCancel: cancelFunc,
      confirmLoading: true
    }

    return Modal.confirm(Object.assign(defaultConfig, extConfig))
  },

  confirmPrimary: function (title, content, okFunc, cancelFunc = () => {}, extConfig = {}) {
    return this.confirm(title, content, okFunc, cancelFunc, Object.assign({ okType: 'primary' }, extConfig))
  },

  confirmDanger: function (title, content, okFunc, cancelFunc = () => {}, extConfig = {}) {
    return this.confirm(title, content, okFunc, cancelFunc, Object.assign({ okType: 'danger' }, extConfig))
  },

  modalError: function (title, content, okFunc = () => {}) {
    return openMessageModal(Modal.error, title, content, okFunc, 'common.error', '错误')
  },

  modalSuccess: function (title, content, okFunc = () => {}) {
    return openMessageModal(Modal.success, title, content, okFunc, 'common.success', '成功')
  },

  modalWarning: function (title, content, okFunc = () => {}) {
    return openMessageModal(Modal.warning, title, content, okFunc, 'common.warning', '警告')
  }
}
