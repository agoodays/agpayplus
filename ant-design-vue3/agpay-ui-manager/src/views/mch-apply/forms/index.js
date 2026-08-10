const formMap = {
  wxpay: () => import('./wxpay-apply-form.vue'),
  alipay: () => import('./alipay-apply-form.vue')
}

const fallbackForm = () => import('./fallback-apply-form.vue')

export function getApplyFormComponent(ifCode) {
  const key = (ifCode || '').toLowerCase()
  return formMap[key] || fallbackForm
}

export function isSupportedIfCode(ifCode) {
  return !!(ifCode && formMap[(ifCode || '').toLowerCase()])
}
