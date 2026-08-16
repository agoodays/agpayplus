<template>
  <a-space class="ag-table-action" :size="0">
    <a-button type="link" size="small" @click="emitView">{{ t('components.view') }}</a-button>
    <a-button type="link" size="small" @click="emitEdit">{{ t('components.edit') }}</a-button>
    <a-popconfirm :title="t('components.confirmDelete')" @confirm="emitDelete">
      <a-button type="link" size="small">{{ t('components.delete') }}</a-button>
    </a-popconfirm>
    <template v-if="hasMore">
      <a-dropdown>
        <template #overlay>
          <a-menu>
            <a-menu-item
              v-for="(m, idx) in more"
              :key="idx"
              @click="onMoreClick(m)"
            >
              {{ m.label }}
            </a-menu-item>
          </a-menu>
        </template>
        <a-button type="link" size="small">{{ t('components.more') }}</a-button>
      </a-dropdown>
    </template>
  </a-space>
</template>

<script setup>
/**
 * AgTableAction - 表格行内操作按钮组件
 *
 * 封装表格行内常用的"查看 / 编辑 / 删除"操作按钮，支持扩展"更多"下拉菜单。
 * 删除操作内置 a-popconfirm 二次确认。
 *
 * @example
 * <AgTableAction
 *   :record="record"
 *   @view="handleView"
 *   @edit="handleEdit"
 *   @delete="handleDelete"
 * />
 *
 * @example
 * // 扩展更多操作
 * <AgTableAction
 *   :record="record"
 *   :more="[{ label: '详情', value: 'detail' }]"
 *   @view="handleView"
 *   @edit="handleEdit"
 *   @delete="handleDelete"
 *   @more="handleMore"
 * />
 */
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps({
  /** 当前行数据，会作为事件回调参数透传 */
  record: {
    type: Object,
    default: () => ({})
  },
  /** 更多操作菜单配置，格式为 { label: string, value: string }[] */
  more: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['view', 'edit', 'delete', 'more'])

/** 是否存在"更多"操作（避免渲染空下拉菜单） */
const hasMore = computed(() => Array.isArray(props.more) && props.more.length > 0)

/** 触发查看事件 */
function emitView() {
  emit('view', props.record)
}

/** 触发编辑事件 */
function emitEdit() {
  emit('edit', props.record)
}

/** 触发删除事件（已通过 popconfirm 确认） */
function emitDelete() {
  emit('delete', props.record)
}

/**
 * 点击更多菜单项
 * @param {{ label: string, value: string }} item - 菜单项配置
 */
function onMoreClick(item) {
  emit('more', { item, record: props.record })
}
</script>

<style scoped>
.ag-table-action :deep(.ant-btn-link) {
  padding: 0 4px;
  height: auto;
}
</style>
