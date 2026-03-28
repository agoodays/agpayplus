/**
 * 通用组件索引文件
 * 用于统一导出所有通用组件
 */

// 基础组件
export { default as AgTable } from './ag-table/index.vue'
export { default as AgSearch } from './ag-search/index.vue'
export { default as AgUpload } from './ag-upload/index.vue'
export { default as AgCard } from './ag-card/index.vue'
export { default as AgStateSwitch } from './ag-state-switch/index.vue'
export { default as AgEditor } from './ag-editor/index.vue'

// 表格相关组件
export { default as AgTableAction } from './ag-table-action/index.vue'
export { default as AgTableActions } from './ag-table-actions/index.vue'

// 表单组件 - 浮动标签系列
export { default as AgInput } from './ag-input/index.vue'
export { default as AgInputNumber } from './ag-input-number/index.vue'
export { default as AgInputNumberRange } from './ag-input-number-range/index.vue'
export { default as AgDateRangePicker } from './ag-date-range-picker/index.vue'
export { default as AgTextarea } from './ag-textarea/index.vue'
export { default as AgSelect } from './ag-select/index.vue'
export { default as AgSelectInfinite } from './ag-select-infinite/index.vue'

// 容器组件
export { default as AgDrawer } from './ag-drawer/index.vue'
export { default as AgModal } from './ag-modal/index.vue'

// 工具组件
export { default as AgLoading } from './ag-loading/index.js'
export { default as GlobalLoad } from './global-load/index.vue'

// 工具提示：所有组件已完成 Vue 3 迁移
// - 使用 Composition API
// - 支持 v-model
// - 完整的 TypeScript 类型支持
// - 详细的文档和示例
