<template>
  <div :class="wrpCls">
    <div style="display: flex; align-items: center; height: 64px;">
      <a-button type="link" @click="openQuickCashier">快捷收银</a-button>
      <avatar-dropdown :menu="showMenu" :current-user="currentUser" :class="prefixCls" />
    </div>
  </div>
</template>

<script>
import AvatarDropdown from './AvatarDropdown'
// import store from '@/store'

export default {
  name: 'RightContent',
  components: {
    AvatarDropdown
  },
  props: {
    prefixCls: {
      type: String,
      default: 'ant-pro-global-header-index-action'
    },
    isMobile: {
      type: Boolean,
      default: () => false
    },
    topMenu: {
      type: Boolean,
      required: true
    },
    theme: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      showMenu: true,
      currentUser: {}
    }
  },
  computed: {
    wrpCls () {
      return {
        'ant-pro-global-header-index-right': true,
        [`ant-pro-global-header-index-${(this.isMobile || !this.topMenu) ? 'light' : this.theme}`]: true
      }
    }
  },
  mounted () {
    // console.log(store)
      this.currentUser = {
        name: 'dd'
      }
  },
  methods: {
    openQuickCashier () { // 快捷收银
      this.$openQuickCashierModal()
    }
  }
}
</script>
