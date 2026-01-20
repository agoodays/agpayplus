// 运行时主题工具：将后端配置映射为 CSS 变量，供自定义组件和样式使用
// 默认运行时配置（保守回退）
// 优先级说明：运行时主题优先使用 CSS 变量（参见 src/theme/index.less），
// 这些 DEFAULTS 仅作为 JavaScript 层面的回退值，避免在没有后端配置时出现空值。
const DEFAULTS = {
  primaryColor: '#1677ff',
  borderRadius: '6px',
  sideMenuWidth: '200px',
  pageWidth: '99%',
  baseBgColor: '#fff',
  textColorWeak: 'rgba(0,0,0,0.35)',
  textColor: 'rgba(0,0,0,0.85)',
  tableRowAltBg: '#FCFCFC',
  antPrimaryColor: '#1677ff',
  overlayBg: '#000',
  textOnDark: 'rgba(255,255,255,0.87)',
  shadowColor: 'rgba(0,0,0,0.1)',
  borderColor: '#e7eaf3',
  surfaceVariant: 'rgba(0,0,0,0.03)',
  borderDashed: 'rgba(0,0,0,0.15)',
  textMuted: 'rgba(0,0,0,0.2)',
  layoutBg: '#f5f5f5',
  layoutSurface: '#f8f8f8',
  textOnPrimary: '#fff',
  textColorMuted: '#00000073',
  textColorSecondary: '#b3b3b3',
  hoverBgColor: '#dadada'
};

function toPx(val) {
  if (typeof val === 'number') return val + 'px';
  if (typeof val === 'string' && /^[0-9]+$/.test(val)) return val + 'px';
  return val;
}

export function applyThemeFromConfig(cfg = {}) {
  const root = document.documentElement;

  const primaryColor = cfg.primaryColor || cfg.theme || cfg.themeColor || cfg.themePrimary || DEFAULTS.primaryColor;
  const borderRadius = toPx(cfg.borderRadius || cfg.roundCorner || DEFAULTS.borderRadius);
  const sideMenuWidth = toPx(cfg.sideMenuWidth || DEFAULTS.sideMenuWidth);
  const pageWidth = cfg.pageWidth || DEFAULTS.pageWidth;

  const baseBgColor = cfg.baseBgColor || cfg.background || cfg.bodyBackground || DEFAULTS.baseBgColor;
  const textColorWeak = cfg.textColorWeak || cfg.textWeak || DEFAULTS.textColorWeak;
  const textColor = cfg.textColor || cfg.text || DEFAULTS.textColor;
  const tableRowAltBg = cfg.tableRowAltBg || cfg.tableAlternateBg || DEFAULTS.tableRowAltBg;
  const antPrimaryColor = cfg.antPrimaryColor || cfg.antPrimary || primaryColor || DEFAULTS.antPrimaryColor;
  const overlayBg = cfg.overlayBg || cfg.overlay || DEFAULTS.overlayBg;
  const textOnDark = cfg.textOnDark || DEFAULTS.textOnDark;
  const shadowColor = cfg.shadowColor || DEFAULTS.shadowColor;
  const borderColor = cfg.borderColor || DEFAULTS.borderColor;
  const surfaceVariant = cfg.surfaceVariant || cfg.surface || DEFAULTS.surfaceVariant;
  const borderDashed = cfg.borderDashed || DEFAULTS.borderDashed;
  const textMuted = cfg.textMuted || DEFAULTS.textMuted;
  const layoutBg = cfg.layoutBg || cfg.layoutBackground || DEFAULTS.layoutBg;
  const layoutSurface = cfg.layoutSurface || DEFAULTS.layoutSurface;
  const textOnPrimary = cfg.textOnPrimary || DEFAULTS.textOnPrimary;
  const textColorMuted = cfg.textColorMuted || DEFAULTS.textColorMuted;
  const textColorSecondary = cfg.textColorSecondary || DEFAULTS.textColorSecondary;
  const hoverBgColor = cfg.hoverBgColor || DEFAULTS.hoverBgColor;

  root.style.setProperty('--primary-color', primaryColor);
  root.style.setProperty('--border-radius', borderRadius);
  root.style.setProperty('--side-menu-width', sideMenuWidth);
  root.style.setProperty('--page-width', pageWidth);

  root.style.setProperty('--base-bg-color', baseBgColor);
  root.style.setProperty('--text-color-weak', textColorWeak);
  root.style.setProperty('--text-color', textColor);
  root.style.setProperty('--table-row-alt-bg', tableRowAltBg);
  root.style.setProperty('--ant-primary-color', antPrimaryColor);
  root.style.setProperty('--overlay-bg', overlayBg);
  root.style.setProperty('--text-on-dark', textOnDark);
  root.style.setProperty('--shadow-color', shadowColor);
  root.style.setProperty('--border-color', borderColor);
  root.style.setProperty('--surface-variant', surfaceVariant);
  root.style.setProperty('--border-dashed', borderDashed);
  root.style.setProperty('--text-muted', textMuted);
  root.style.setProperty('--layout-bg', layoutBg);
  root.style.setProperty('--layout-surface', layoutSurface);
  root.style.setProperty('--text-on-primary', textOnPrimary);
  root.style.setProperty('--text-color-muted', textColorMuted);
  root.style.setProperty('--text-color-secondary', textColorSecondary);
  root.style.setProperty('--hover-bg-color', hoverBgColor);

  // expose hover/transparent variants for components (compute from primaryColor if not explicitly provided)
  const primaryHover = cfg.primaryColorHover || cfg.primaryHover || hexToAlpha(primaryColor, 0.08);
  const primaryWeak = cfg.primaryColorWeak || cfg.primaryWeak || hexToAlpha(primaryColor, 0.06);
  root.style.setProperty('--primary-color-hover', primaryHover);
  root.style.setProperty('--primary-color-weak', primaryWeak);

  // return applied values for debugging/tests
  return {
    primaryColor,
    borderRadius,
    sideMenuWidth,
    pageWidth,
    baseBgColor,
  };
}

// small helper: convert hex to rgba with alpha
function hexToAlpha(hex, alpha) {
  if (!hex) return `rgba(0,0,0,${alpha})`;
  const h = hex.replace('#', '');
  const bigint = parseInt(h.length === 3 ? h.split('').map(c=>c+c).join('') : h, 16);
  const r = (bigint >> 16) & 255;
  const g = (bigint >> 8) & 255;
  const b = bigint & 255;
  return `rgba(${r}, ${g}, ${b}, ${alpha})`;
}

export default { applyThemeFromConfig };
