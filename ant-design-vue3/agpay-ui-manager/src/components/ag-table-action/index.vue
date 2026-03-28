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
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  record: { type: Object, default: () => ({}) },
  more: { type: Array, default: () => [] }
})

const emit = defineEmits(['view', 'edit', 'delete', 'more'])

function emitView() {
  emit('view', props.record)
}
function emitEdit() {
  emit('edit', props.record)
}
function emitDelete() {
  emit('delete', props.record)
}
function onMoreClick(item) {
  emit('more', { item, record: props.record })
}
</script>

<style scoped></style>
