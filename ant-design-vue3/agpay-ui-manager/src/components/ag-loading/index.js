/**
 * AgLoading - 全局加载状态组件
 * 
 * 功能：
 * - 显示全局加载状态
 * - 隐藏加载状态
 * - 与 Pinia Store 集成
 * 
 * 使用示例：
 * ```javascript
 * import { AgLoading } from '@/components/ag-loading'
 * 
 * // 显示加载
 * AgLoading.show()
 * 
 * // 隐藏加载
 * AgLoading.hide()
 * 
 * // 异步操作示例
 * AgLoading.show()
 * try {
 *   await api.fetchData()
 * } finally {
 *   AgLoading.hide()
 * }
 * ```
 */
import { useSpinStore } from '@/store/modules/system/spin'

export const AgLoading = {
  /**
   * 显示全局加载状态
   */
  show: () => {
    useSpinStore().show()
  },

  /**
   * 隐藏全局加载状态
   */
  hide: () => {
    useSpinStore().hide()
  }
}

export default AgLoading
