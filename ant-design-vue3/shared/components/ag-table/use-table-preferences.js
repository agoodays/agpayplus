export function useTablePreferences({ props, state }) {
  function getDensityStorageKey() {
    return `agpay_table_${props.stateKey || location.pathname}_density`
  }

  function saveDensitySetting(density) {
    try {
      localStorage.setItem(getDensityStorageKey(), density)
    } catch {
      // ignored
    }
  }

  function loadDensitySetting() {
    let density = null
    try {
      density = localStorage.getItem(getDensityStorageKey())
    } catch {
      density = null
    }

    if (density && ['small', 'middle', 'large'].includes(density)) {
      state.density = density
    }
  }

  function handleDensityChange(payload) {
    const key = typeof payload === 'string' ? payload : payload?.key
    if (!key) return
    state.density = key
    saveDensitySetting(key)
  }

  return {
    saveDensitySetting,
    loadDensitySetting,
    handleDensityChange,
  }
}
