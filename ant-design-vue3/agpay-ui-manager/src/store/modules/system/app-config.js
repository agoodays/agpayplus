/*
 * 项目的配置信息
 *
 */
import { defineStore } from 'pinia'
import { appDefaultConfig } from '/@/config/app-config'
import localStorageKeyConst from '/@/constants/local-storage-key-const'
import { agSentry } from '/@/lib/ag-sentry'
import { localRead } from '/@/utils/local-util'

let state = { ...appDefaultConfig }

function pickKnownConfigFields(config = {}) {
  const normalizedConfig = {}
  for (const key in appDefaultConfig) {
    if (Object.prototype.hasOwnProperty.call(config, key)) {
      normalizedConfig[key] = config[key]
    }
  }
  return normalizedConfig
}

const appConfigData = localRead(localStorageKeyConst.APP_CONFIG)
let language = appDefaultConfig.language
if (appConfigData) {
  try {
    const parsedConfig = typeof appConfigData === 'string' ? JSON.parse(appConfigData) : appConfigData
    state = { ...state, ...pickKnownConfigFields(parsedConfig) }
    language = state.language
  } catch (e) {
    agSentry.captureError(e)
  }
}

/**
 * 获取初始化的语言
 */
export const getInitializedLanguage = function () {
  return language
}

export const useAppConfigStore = defineStore('appConfig', {
  state: () => ({
    // 读取config下的默认配置
    ...state,
  }),
  actions: {
    persistConfig() {
      const payload = {}
      for (const key in appDefaultConfig) {
        payload[key] = this[key]
      }
      localStorage.setItem(localStorageKeyConst.APP_CONFIG, JSON.stringify(payload))
    },
    reset() {
      for (const k in appDefaultConfig) {
        this[k] = appDefaultConfig[k]
      }
      this.persistConfig()
    },
    setLanguage(languageCode) {
      this.language = languageCode
      language = languageCode
      this.persistConfig()
    },
    showHelpDoc() {
      this.helpDocFlag = true
      this.persistConfig()
    },
    hideHelpDoc() {
      this.helpDocFlag = false
      this.persistConfig()
    },
  },
})
