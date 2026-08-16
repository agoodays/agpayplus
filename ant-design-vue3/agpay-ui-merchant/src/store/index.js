import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

export { pinia as store }

// 导出所有 stores
export { useUserStore } from './modules/system/user'
export { useAppStore } from './modules/system/app'
