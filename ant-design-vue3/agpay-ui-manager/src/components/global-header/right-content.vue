<template>
  <div :class="wrpCls">
    <avatar-dropdown :menu="showMenu" :current-user="currentUser" :class="prefixCls" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import AvatarDropdown from './avatar-dropdown'

const props = defineProps({
  prefixCls: {
    type: String,
    default: 'ant-pro-global-header-index-action'
  },
  isMobile: {
    type: Boolean,
    default: false
  },
  topMenu: {
    type: Boolean,
    required: true
  },
  theme: {
    type: String,
    required: true
  }
})

const showMenu = ref(true)
const currentUser = ref({})

const wrpCls = computed(() => {
  return {
    'ant-pro-global-header-index-right': true,
    [`ant-pro-global-header-index-${props.isMobile || !props.topMenu ? 'light' : props.theme}`]: true
  }
})

onMounted(() => {
  currentUser.value = {
    name: 'dd'
  }
})
</script>
