import enUSLocal from '@/locales/en-US'
import zhCNLocal from '@/locales/zh-CN'
import dayjs from 'dayjs'
import 'dayjs/locale/en'
import 'dayjs/locale/zh-cn'
import { createI18n } from 'vue-i18n'
import enUSShared from './locales/en-US'
import zhCNShared from './locales/zh-CN'

function deepMerge(target, ...sources) {
  const out = Array.isArray(target) ? [...target] : { ...target }
  for (const src of sources) {
    if (!src) continue
    for (const [k, v] of Object.entries(src)) {
      if (v && typeof v === 'object' && !Array.isArray(v)
        && out[k] && typeof out[k] === 'object' && !Array.isArray(out[k])) {
        out[k] = deepMerge(out[k], v)
      } else {
        out[k] = v
      }
    }
  }
  return out
}

const zhCN = deepMerge({}, zhCNShared, zhCNLocal)
const enUS = deepMerge({}, enUSShared, enUSLocal)

export const LOCALE_MAP = {
  zh_CN: 'zh-CN',
  en_US: 'en-US'
}

const DAYJS_LOCALE_MAP = {
  'zh-CN': 'zh-cn',
  'en-US': 'en'
}

export function normalizeLocale(language) {
  return LOCALE_MAP[language] || 'zh-CN'
}

export function setAppLocale(i18n, language) {
  const locale = normalizeLocale(language)
  i18n.global.locale.value = locale
  dayjs.locale(DAYJS_LOCALE_MAP[locale] || 'zh-cn')
}

export const i18n = createI18n({
  legacy: false,
  locale: 'zh-CN',
  fallbackLocale: 'zh-CN',
  messages: {
    'zh-CN': zhCN,
    'en-US': enUS
  }
})
