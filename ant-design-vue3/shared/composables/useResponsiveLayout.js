/**
 * useResponsiveLayout - 响应式布局 composable
 *
 * 负责：
 * - 监听窗口 resize，窄屏时自动折叠侧边栏（< responsiveBreakpoint）
 * - 窗口变宽时恢复折叠（仅对响应式触发的折叠生效，用户手动折叠不受影响）
 * - 使用 requestAnimationFrame 节流 resize，避免频繁布局计算
 *
 * 使用方式：
 *   const { collapsed, collapsedByResponsive, handleWindowResize } = useResponsiveLayout(1200)
 */
import { onBeforeUnmount, onMounted, ref } from 'vue'

export function useResponsiveLayout(responsiveBreakpoint = 1200) {
  const collapsed = ref(false)
  const collapsedByResponsive = ref(false)
  let resizeRafId = 0

  function handleWindowResize() {
    const shouldCollapse = typeof window !== 'undefined' && window.innerWidth < responsiveBreakpoint

    if (shouldCollapse && !collapsed.value) {
      collapsed.value = true
      collapsedByResponsive.value = true
      return
    }

    if (!shouldCollapse && collapsedByResponsive.value) {
      collapsed.value = false
      collapsedByResponsive.value = false
    }
  }

  function onWindowResize() {
    if (resizeRafId) cancelAnimationFrame(resizeRafId)
    resizeRafId = requestAnimationFrame(() => {
      handleWindowResize()
      resizeRafId = 0
    })
  }

  onMounted(() => {
    handleWindowResize()
    if (typeof window !== 'undefined') {
      window.addEventListener('resize', onWindowResize)
    }
  })

  onBeforeUnmount(() => {
    if (typeof window !== 'undefined') {
      window.removeEventListener('resize', onWindowResize)
    }
    if (resizeRafId) {
      cancelAnimationFrame(resizeRafId)
      resizeRafId = 0
    }
  })

  return {
    collapsed,
    collapsedByResponsive,
    handleWindowResize,
    onWindowResize
  }
}
