import { basicApi } from '/@/api/system/basic-api';
import { localSave } from '/@/utils/local-util';
import localStorageKeyConst from '/@/constants/local-storage-key-const';
import { applyThemeFromConfig } from '/@/utils/theme';

/**
 * ThemeService
 *
 * 责任：
 * - 从后端拉取站点配置（siteInfos）并将其映射为运行时 CSS 变量（覆盖 `src/theme/index.less` 中的默认值）。
 * - 将原始 siteInfos 保存到 localStorage（键为 `localStorageKeyConst.APP_CONFIG`）。
 * - 提供导出函数用于启动时加载主题：`loadAndApplyTheme()`。
 *
 * 支持字段（后端可按需返回，支持多种命名习惯）：
 * - primaryColor / themeColor / theme / color 或 antdToken['primary-color']
 * - borderRadius / roundCorner / radius
 * - sideMenuWidth / sideWidth / menuWidth
 * - pageWidth / page_w
 * - baseBgColor / background / bodyBackground / layoutBackground
 * - textColor / textColorWeak / tableRowAltBg / overlayBg / borderColor / shadowColor
 * - surfaceVariant / borderDashed / textMuted / layoutBg / layoutSurface / hoverBgColor
 * - antdToken: 可直接传入 AntD token 对象，函数 `mapAntdTokenToRuntimeCfg` 会尝试将常见 token 映射为运行时字段
 *
 * 返回值：
 * - `loadAndApplyTheme()` 返回被应用的运行时配置对象（以便测试/调试），若后端不可用则返回 null。
 *
 * 示例后端 `siteInfos` JSON：
 * {
 *   "primaryColor": "#1890ff",
 *   "borderRadius": "8",
 *   "sideMenuWidth": "220",
 *   "pageWidth": "1200px",
 *   "baseBgColor": "#ffffff",
 *   "textColor": "rgba(0,0,0,0.85)",
 *   "tableRowAltBg": "#F7F9FC",
 *   "overlayBg": "rgba(0,0,0,0.5)",
 *   "hoverBgColor": "#f0f2f5",
 *   "antdToken": {
 *     "primary-color": "#1890ff",
 *     "border-radius-base": "8px",
 *     "body-background": "#ffffff",
 *     "text-color": "rgba(0,0,0,0.85)",
 *     "border-color-base": "#e6eef7"
 *   }
 * }
 *
 * 后端只需返回上面示例中任意子集字段，ThemeService 会合并 antdToken 与通用字段，后端显式字段优先。
 */

// 内置最小的默认 antd token（保守回退）
// 注意：运行时优先使用 CSS 变量（例如 `--primary-color`、`--border-radius`），
// 后端提供的 `antdToken` 会被直接应用到运行时映射中。
const defaultAntdToken = {
  // 仅作为构建/调用回退值，建议通过 CSS 变量覆盖：
  // CSS var: --primary-color
  'primary-color': '#1677ff',
  // CSS var: --border-radius
  'border-radius-base': '6px',
};

// 导出默认 token 以便工具/测试或文档引用
export const DEFAULT_ANTD_TOKEN = defaultAntdToken;

/**
 * 将 antd token 映射为运行时主题配置（供 applyThemeFromConfig 使用）
 * @param {Object} antdToken 后端或本地提供的 antd token
 */
export function mapAntdTokenToRuntimeCfg(antdToken = defaultAntdToken) {
  return {
    primaryColor: antdToken['primary-color'] || antdToken.primaryColor || defaultAntdToken['primary-color'],
    borderRadius: antdToken['border-radius-base'] || antdToken.borderRadiusBase || defaultAntdToken['border-radius-base'],
    sideMenuWidth: antdToken.sideMenuWidth || antdToken.side_width || undefined,
    pageWidth: antdToken.pageWidth || antdToken.page_width || undefined,
    // extended mappings: map common antd token keys to our runtime CSS vars
    baseBgColor: antdToken['body-background'] || antdToken['background-color'] || antdToken['layout-background'],
    textColor: antdToken['text-color'] || antdToken['color-text'] || undefined,
    textColorWeak: antdToken['text-color-weak'] || antdToken['color-text-weak'] || undefined,
    tableRowAltBg: antdToken['table-row-bg'] || antdToken['table-bg'],
    overlayBg: antdToken['overlay-color'] || undefined,
    borderColor: antdToken['border-color-base'] || antdToken['color-border'] || undefined,
    shadowColor: antdToken['shadow-color'] || undefined,
  };
}

/*
 ThemeService
 - 从后端获取站点配置（siteInfos），映射并注入运行时 CSS 变量
 - 保存到 localStorage（localStorageKeyConst.APP_CONFIG）以便 store/其他模块使用
 - 返回应用后的主题对象
*/

async function fetchSiteInfos() {
  try {
    const res = await basicApi.getSiteInfos();
    return res || null;
  } catch (e) {
    console.warn('themeService: fetchSiteInfos failed', e);
    return null;
  }
}

function normalizeConfig(siteInfo) {
  // 期望后端返回类似结构： { primaryColor, borderRadius, sideMenuWidth, pageWidth, antdToken: {...} }
  if (!siteInfo) return {};
  const cfg = {};
  // 支持多种命名习惯
  cfg.primaryColor = siteInfo.primaryColor || siteInfo.themeColor || siteInfo.theme || siteInfo.color || (siteInfo.antdToken && (siteInfo.antdToken['primary-color'] || siteInfo.antdToken['primaryColor'])) || (defaultAntdToken && (defaultAntdToken['primary-color'] || defaultAntdToken.colorPrimary));
  cfg.borderRadius = siteInfo.borderRadius || siteInfo.roundCorner || siteInfo.radius;
  cfg.sideMenuWidth = siteInfo.sideMenuWidth || siteInfo.sideWidth || siteInfo.menuWidth;
  cfg.pageWidth = siteInfo.pageWidth || siteInfo.page_w || siteInfo.pageWidth;
  cfg.antdToken = siteInfo.antdToken || null;
  // 额外可配置的全局样式字段（后端可返回任意组合）
  cfg.baseBgColor = siteInfo.baseBgColor || siteInfo.background || siteInfo.bodyBackground || siteInfo.layoutBackground;
  cfg.textColor = siteInfo.textColor || siteInfo.colorText || siteInfo.color || undefined;
  cfg.textColorWeak = siteInfo.textColorWeak || siteInfo.colorTextWeak || siteInfo.textWeak;
  cfg.tableRowAltBg = siteInfo.tableRowAltBg || siteInfo.tableAlternateBg;
  cfg.overlayBg = siteInfo.overlayBg || siteInfo.overlayColor;
  cfg.borderColor = siteInfo.borderColor || siteInfo.colorBorder;
  cfg.shadowColor = siteInfo.shadowColor || siteInfo.boxShadowColor;
  cfg.surfaceVariant = siteInfo.surfaceVariant || siteInfo.surface;
  cfg.borderDashed = siteInfo.borderDashed;
  cfg.textMuted = siteInfo.textMuted;
  cfg.layoutBg = siteInfo.layoutBg || siteInfo.layoutBackground;
  cfg.layoutSurface = siteInfo.layoutSurface;
  cfg.textOnPrimary = siteInfo.textOnPrimary;
  cfg.textColorMuted = siteInfo.textColorMuted;
  cfg.textColorSecondary = siteInfo.textColorSecondary;
  cfg.hoverBgColor = siteInfo.hoverBgColor;
  return cfg;
}

async function loadAndApplyTheme() {
  const siteInfo = await fetchSiteInfos();
  if (siteInfo) {
    // 保存原始配置，供其他模块读取
    try {
      localSave(localStorageKeyConst.APP_CONFIG, JSON.stringify(siteInfo));
    } catch (e) {
      console.warn('themeService: save app config failed', e);
    }

    const cfg = normalizeConfig(siteInfo);

    // 优先使用后端提供的 antdToken（如果存在），否则使用通用字段映射。
    // 将后端 antdToken（若存在）映射为运行时配置，并与通用字段合并，后端显式字段优先
    const antdMapped = cfg.antdToken ? mapAntdTokenToRuntimeCfg(cfg.antdToken) : {};
    const runtimeCfg = Object.assign({}, antdMapped, {
      primaryColor: cfg.primaryColor || antdMapped.primaryColor,
      borderRadius: cfg.borderRadius || antdMapped.borderRadius,
      sideMenuWidth: cfg.sideMenuWidth || antdMapped.sideMenuWidth,
      pageWidth: cfg.pageWidth || antdMapped.pageWidth,
      baseBgColor: cfg.baseBgColor,
      textColor: cfg.textColor,
      textColorWeak: cfg.textColorWeak,
      tableRowAltBg: cfg.tableRowAltBg,
      overlayBg: cfg.overlayBg,
      borderColor: cfg.borderColor,
      shadowColor: cfg.shadowColor,
      surfaceVariant: cfg.surfaceVariant,
      layoutBg: cfg.layoutBg,
      layoutSurface: cfg.layoutSurface,
      textOnPrimary: cfg.textOnPrimary,
      textColorMuted: cfg.textColorMuted,
      textColorSecondary: cfg.textColorSecondary,
      hoverBgColor: cfg.hoverBgColor,
    });

    const applied = applyThemeFromConfig(runtimeCfg);
    return applied;
  }

  // 如果后端不可用，应用 JS 端 DEFAULTS（applyThemeFromConfig 无参会使用内部 DEFAULTS）
  applyThemeFromConfig();
  return null;
}

export default {
  loadAndApplyTheme,
  fetchSiteInfos,
  normalizeConfig,
};
