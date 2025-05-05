/*
 * 项目的配置信息
 *
 */
import { defineStore } from 'pinia';
import { appDefaultConfig } from '/@/config/app-config';
import localStorageKeyConst from '/@/constants/local-storage-key-const';
import { agSentry } from '/@/lib/ag-sentry';
import { localRead } from '/@/utils/local-util';

let state = { ...appDefaultConfig };

let appConfigStr = localRead(localStorageKeyConst.APP_CONFIG);
let language = appDefaultConfig.language;
if (appConfigStr) {
  try {
    state = JSON.parse(appConfigStr);
    language = state.language;
  } catch (e) {
    agSentry.captureError(e);
  }
}

/**
 * 获取初始化的语言
 */
export const getInitializedLanguage = function () {
  return language;
};

export const useAppConfigStore = defineStore('appConfig', {
  state: () => ({
    // 读取config下的默认配置
    ...state,
  }),
  actions: {
    reset() {
      for (const k in appDefaultConfig) {
        this[k] = appDefaultConfig[k];
      }
    },
    showHelpDoc() {
      this.helpDocFlag = true;
    },
    hideHelpDoc() {
      this.helpDocFlag = false;
    },
  },
});
