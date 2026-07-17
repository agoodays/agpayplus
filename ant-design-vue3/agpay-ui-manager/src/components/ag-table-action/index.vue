<template>
  <a-space>
    <a-button type="link" @click="emitView">{{ t('components.view') }}</a-button>
    <a-button type="link" @click="emitEdit">{{ t('components.edit') }}</a-button>
    <a-popconfirm :title="t('components.confirmDelete')" @confirm="emitDelete">
      <a-button type="link">{{ t('components.delete') }}</a-button>
    </a-popconfirm>
    <a-dropdown v-if="more && more.length">
      <template #overlay>
        <a-menu>
          <a-menu-item v-for="(m, idx) in more" :key="idx" @click="onMoreClick(m)">{{ m.label }}</a-menu-item>
        </a-menu>
      </template>
      <a-button type="link">{{ t('components.more') }}</a-button>
    </a-dropdown>
  </a-space>
</template>

<script setup>
/**
 * 表格操作按钮组件
 * 功能：封装表格行内常用操作按钮（查看、编辑、删除、更多）
 */
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  record: { type: Object, default: () => ({}) },
  more: { type: Array, default: () => [] }
})

const emit = defineEmits(['view', 'edit', 'delete', 'more'])

/** 触发查看事件 */
function emitView() {
  emit('view', props.record)
}
/** 触发编辑事件 */
function emitEdit() {
  emit('edit', props.record)
}
/** 触发删除事件 */
function emitDelete() {
  emit('delete', props.record)
}
/** 触发更多操作事件 */
function onMoreClick(item) {
  emit('more', { item, record: props.record })
}
</script>

<style scoped></style>
