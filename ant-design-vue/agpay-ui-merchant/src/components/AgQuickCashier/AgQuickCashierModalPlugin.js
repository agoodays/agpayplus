import AgQuickCashier from './AgQuickCashier.vue'

// 注册全局组件和方法
const AgQuickCashierModalPlugin = {
    install (Vue) {
        Vue.component('AgQuickCashier', AgQuickCashier)

        Vue.prototype.$openQuickCashierModal = () => {
            const QuickCashierConstructor = Vue.extend(AgQuickCashier)
            const quickCashierInstance = new QuickCashierConstructor()
            quickCashierInstance.$mount()
            quickCashierInstance.open()
        }
    }
}

export default AgQuickCashierModalPlugin
