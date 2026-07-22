<template>
  <ag-pay-config-drawer
    v-bind="$props"
    @update:open="(val) => emit('update:open', val)"
    @success="() => emit('success')"
    @channel-change="(val) => emit('channel-change', val)"
    @tab-change="(val) => emit('tab-change', val)"
    @passage-state-update="(val) => emit('passage-state-update', val)"
  />
</template>

<script setup>
import AgPayConfigDrawer from './ag-pay-config-drawer.vue'

const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  permCode: {
    type: String,
    default: ''
  },
  configMode: {
    type: String,
    default: ''
  },
  infoId: {
    type: [String, Number],
    default: null
  },
  isIsvSubMch: {
    type: Boolean,
    default: false
  },
  channelListConfig: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['update:open', 'success', 'channel-change', 'tab-change', 'passage-state-update'])
</script>
