// custom-variables.js 已被降级为兼容占位符
// 项目已切换为运行时 CSS 变量主题方案；该文件仅保留向后兼容的最小导出，
// 以防止历史引用在构建时抛错。

export const lessModifyVars = {};

// 兼容性注记：项目使用运行时 CSS 变量（src/theme/index.less + themeService）作为优先方案。
// 此处保留最小的默认 token 仅用于避免历史构建引用报错。
// 建议：运行时主题由 `--primary-color` 等 CSS 变量控制，后端配置会覆盖这些变量。
// 若需程序化获取默认 token，请参考 `src/utils/themeService.js` 中导出的 `DEFAULT_ANTD_TOKEN` 与 `mapAntdTokenToRuntimeCfg`。
export const antdToken = { 'primary-color': '#1677ff' };
export default lessModifyVars;

