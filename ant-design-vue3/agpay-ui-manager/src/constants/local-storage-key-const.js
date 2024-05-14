/*
 * key  常量
 *
 */

/**
 * key前缀
 */
const KEY_PREFIX = 'agpay_mgr_';

/**
 * localStorageKey集合
 */
export default {
  // 用户token
  USER_TOKEN: `${KEY_PREFIX}user_token`,
  // 用户信息
  USER_INFO: `${KEY_PREFIX}user_info`,
  // 用户权限点
  USER_POINTS: `${KEY_PREFIX}user_points`,
  // 用户的tag列表
  USER_TAG_NAV: `${KEY_PREFIX}user_tag_nav`,
  // app config 配置信息
  APP_CONFIG: `${KEY_PREFIX}app_config`,
  // 首页快捷入口
  HOME_QUICK_ENTRY: `${KEY_PREFIX}home_quick_entry`,
  // 通知信息已读
  NOTICE_READ: `${KEY_PREFIX}notice_read`,
};
