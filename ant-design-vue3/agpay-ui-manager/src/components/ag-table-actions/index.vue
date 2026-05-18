<template>
  <div class="ag-table-action-columns">
    <template v-if="visibleActions.length <= maxShowNum">
      <!-- 直接显示所有操作 -->
      <slot></slot>
    </template>
    <template v-else>
      <!-- 显示前面的操作 + 更多菜单 -->
      <template v-for="(action, index) in visibleActions" :key="index">
        <component :is="action" v-if="index < maxShowNum - 1" />
      </template>
      <a-dropdown>
        <a-button type="link" size="small">
          {{ t('components.more') }}
          <down-outlined />
        </a-button>
        <template #overlay>
          <a-menu>
            <a-menu-item v-for="(action, index) in moreActions" :key="index">
              <component :is="action" />
            </a-menu-item>
          </a-menu>
        </template>
      </a-dropdown>
    </template>
  </div>
</template>

<script setup>
import { computed, useSlots } from 'vue'
import { DownOutlined } from '@ant-design/icons-vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  // 最多显示几个操作按钮，超过的放入"更多"菜单
  maxShowNum: {
    type: Number,
    default: 2
  }
})

const slots = useSlots()

// 获取所有有效的操作子节点
const visibleActions = computed(() => {
  const defaultSlot = slots.default?.() || []
  // 过滤出有效的 VNode
  return defaultSlot.filter((vnode) => {
    // 排除注释节点和纯文本节点
    return vnode.type && typeof vnode.type !== 'symbol'
  })
})

// 需要放入"更多"菜单的操作
const moreActions = computed(() => {
  if (visibleActions.value.length <= props.maxShowNum) {
    return []
  }
  return visibleActions.value.slice(props.maxShowNum - 1)
})
</script>

<style scoped>
.ag-table-action-columns {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 4px;
  flex-wrap: nowrap;
  overflow: visible;
}

.ag-table-action-columns :deep(.ant-btn-link) {
  padding: 2px 4px;
  border: none !important;
  box-shadow: none !important;
  font-size: 12px;
}
</style>
