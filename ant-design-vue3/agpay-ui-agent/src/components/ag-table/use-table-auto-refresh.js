import { onBeforeUnmount, ref, watch } from 'vue'

export function useTableAutoRefresh({ props, state, reload }) {
  const autoRefreshEnabled = ref(props.enableAutoRefresh)

  function startAutoRefresh() {
    if (state.autoRefreshTimerId) clearInterval(state.autoRefreshTimerId)

    state.autoRefreshTimerId = setInterval(() => {
      if (autoRefreshEnabled.value) {
        state.autoRefreshCountdown -= 1
        if (state.autoRefreshCountdown <= 0) {
          state.autoRefreshCountdown = props.autoRefreshInterval
          reload()
        }
      }
    }, 1000)
  }

  function stopAutoRefresh() {
    if (state.autoRefreshTimerId) {
      clearInterval(state.autoRefreshTimerId)
      state.autoRefreshTimerId = null
    }
  }

  function handleAutoRefreshEnabledChange(val) {
    autoRefreshEnabled.value = !!val
  }

  function initAutoRefresh() {
    if (props.showAutoRefresh && props.enableAutoRefresh) {
      autoRefreshEnabled.value = true
    }
  }

  watch(
    autoRefreshEnabled,
    (val) => {
      if (val && props.showAutoRefresh) {
        state.autoRefreshCountdown = props.autoRefreshInterval
        startAutoRefresh()
      } else {
        stopAutoRefresh()
      }
    },
    { flush: 'post' }
  )

  onBeforeUnmount(() => {
    stopAutoRefresh()
  })

  return {
    autoRefreshEnabled,
    startAutoRefresh,
    stopAutoRefresh,
    handleAutoRefreshEnabledChange,
    initAutoRefresh,
  }
}
