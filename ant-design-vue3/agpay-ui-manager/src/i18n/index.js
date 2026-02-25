import { createI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import 'dayjs/locale/en'
import zhCN from '/@/locales/zh-CN'
import enUS from '/@/locales/en-US'

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
