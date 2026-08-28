/*
 * localStorage key 常量
 *
 * KEY_PREFIX 从项目环境变量 VITE_APP_KEY_PREFIX 读取，
 * 各项目 .env 里必须定义：
 *   Agent   : VITE_APP_KEY_PREFIX=agpay_agt_
 *   Manager : VITE_APP_KEY_PREFIX=agpay_mgr_
 *   Merchant: VITE_APP_KEY_PREFIX=agpay_mch_
 */

const KEY_PREFIX = import.meta.env.VITE_APP_KEY_PREFIX || 'agpay_agt_'

export default {
  USER_TOKEN: `${KEY_PREFIX}user_token`,
  USER_INFO: `${KEY_PREFIX}user_info`,
  USER_POINTS: `${KEY_PREFIX}user_points`,
  USER_TAG_NAV: `${KEY_PREFIX}user_tag_nav`,
  APP_CONFIG: `${KEY_PREFIX}app_config`,
  THEME_CONFIG: `${KEY_PREFIX}theme_config`,
  LAYOUT_CONFIG: `${KEY_PREFIX}layout_config`,
  HOME_QUICK_ENTRY: `${KEY_PREFIX}home_quick_entry`,
  NOTICE_READ: `${KEY_PREFIX}notice_read`
}
