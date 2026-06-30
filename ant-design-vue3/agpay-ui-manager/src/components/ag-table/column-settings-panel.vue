<template>
  <div class="column-settings" tabindex="0" @click.stop @keydown.esc.prevent="emit('close')">
    <div class="column-settings-header">
      <div class="header-left">
        <setting-outlined style="margin-right: 8px" />
        <span>{{ t('agTable.columnSettings') }}</span>
        <a-divider type="vertical" />
        <a-checkbox :checked="isAllColumnsVisible" :indeterminate="isSomeColumnsVisible" @change="handleSelectAllColumns">
          {{ t('agTable.selectAll') }}
        </a-checkbox>
      </div>
      <div class="header-right">
        <a-space :size="8">
          <a-tag color="blue" size="small">
            {{
              t('agTable.visibleCount', {
                visible: visibleColumns.length,
                total: allColumns.length
              })
            }}
          </a-tag>
          <a-button type="text" size="small" @click="emit('reset')">
            <redo-outlined />
          </a-button>
          <a-button type="text" size="small" @click="emit('close')">
            <close-outlined />
          </a-button>
        </a-space>
      </div>
    </div>

    <div class="column-settings-content">
      <div
        v-for="(col, idx) in allColumns"
        :key="col.key"
        class="column-item"
        :class="{ 'is-dragging': dragKey === col.key }"
        tabindex="0"
        @keydown="(event) => handleColumnItemKeydown(event, col, idx)"
        draggable="true"
        @dragstart="emit('drag-start', { event: $event, key: col.key })"
        @dragover.prevent="emit('drag-over', { event: $event, key: col.key })"
        @drop="emit('drop', { event: $event, key: col.key })"
        @dragend="emit('drag-end')"
      >
        <div class="column-drag-handle">
          <holder-outlined />
        </div>

        <div class="column-checkbox-wrapper">
          <a-checkbox :checked="visibleColumns.includes(col.key)" @change="(e) => emit('toggle-column', { key: col.key, checked: e.target.checked })" @click.stop>
            <a-tooltip :title="col.title || col.key" placement="topLeft" :mouse-enter-delay="0.5">
              <span class="column-title">
                {{ col.title || col.key }}
              </span>
            </a-tooltip>
          </a-checkbox>
        </div>

        <div class="column-controls">
          <div class="column-fixed-group">
            <a-tooltip placement="top" :title="t('agTable.notFixed')">
              <a-button
                type="text"
                size="small"
                :class="{ active: !columnFixed[col.key] }"
                class="column-fixed-btn"
                @click.stop="emit('set-fixed', { key: col.key, value: undefined })"
              >
                <span>-</span>
              </a-button>
            </a-tooltip>
            <a-tooltip placement="top" :title="t('agTable.fixedLeft')">
              <a-button
                type="text"
                size="small"
                :class="{ active: columnFixed[col.key] === 'left' }"
                class="column-fixed-btn"
                @click.stop="emit('set-fixed', { key: col.key, value: 'left' })"
              >
                <vertical-left-outlined />
              </a-button>
            </a-tooltip>
            <a-tooltip placement="top" :title="t('agTable.fixedRight')">
              <a-button
                type="text"
                size="small"
                :class="{ active: columnFixed[col.key] === 'right' }"
                class="column-fixed-btn"
                @click.stop="emit('set-fixed', { key: col.key, value: 'right' })"
              >
                <vertical-right-outlined />
              </a-button>
            </a-tooltip>
          </div>

          <div class="column-width-group">
            <a-tooltip placement="top" :title="t('agTable.quickWidth')">
              <a-dropdown :trigger="['click']" placement="topRight" @click.stop>
                <template #overlay>
                  <a-menu @click="({ key }) => handleQuickWidthSelect(col.key, key)">
                    <a-menu-item key="100">100px</a-menu-item>
                    <a-menu-item key="150">150px</a-menu-item>
                    <a-menu-item key="200">200px</a-menu-item>
                    <a-menu-item key="300">300px</a-menu-item>
                    <a-menu-item key="auto">{{ t('agTable.autoWidth') }}</a-menu-item>
                  </a-menu>
                </template>
                <a-button type="text" size="small" class="column-width-preset">{{ t('agTable.preset') }}</a-button>
              </a-dropdown>
            </a-tooltip>
            <a-input-number
              size="small"
              :min="50"
              :max="1000"
              :value="columnWidths[col.key] || col.width || 150"
              :step="10"
              class="column-width-input"
              @change="(val) => emit('set-width', { key: col.key, value: val })"
              @click.stop
            />
          </div>

          <a-button type="text" size="small" :disabled="idx === 0" :title="t('agTable.moveUp')" @click.stop="emit('move', { key: col.key, dir: -1 })">
            <up-outlined />
          </a-button>
          <a-button
            type="text"
            size="small"
            :disabled="idx === allColumns.length - 1"
            :title="t('agTable.moveDown')"
            @click.stop="emit('move', { key: col.key, dir: 1 })"
          >
            <down-outlined />
          </a-button>
        </div>
      </div>
    </div>

    <div class="column-settings-footer">
      <a-button size="small" type="default" @click="emit('close')">
        <close-outlined />
        {{ t('common.cancel') }}
      </a-button>
      <a-button size="small" type="default" @click="emit('reset')">
        <redo-outlined />
        {{ t('common.reset') }}
      </a-button>
      <a-button size="small" type="primary" @click="emit('close')">
        <check-outlined />
        {{ t('common.done') }}
      </a-button>
    </div>
  </div>
</template>

<script setup>
import {
    CheckOutlined,
    CloseOutlined,
    DownOutlined,
    HolderOutlined,
    RedoOutlined,
    SettingOutlined,
    UpOutlined,
    VerticalLeftOutlined,
    VerticalRightOutlined
} from '@ant-design/icons-vue'
import { useI18n } from 'vue-i18n'

const props = defineProps({
  allColumns: { type: Array, required: true },
  visibleColumns: { type: Array, required: true },
  columnWidths: { type: Object, required: true },
  columnFixed: { type: Object, required: true },
  dragKey: { type: String, default: null },
  isAllColumnsVisible: { type: Boolean, default: false },
  isSomeColumnsVisible: { type: Boolean, default: false }
})

const emit = defineEmits([
  'select-all',
  'reset',
  'close',
  'toggle-column',
  'set-fixed',
  'set-width',
  'move',
  'drag-start',
  'drag-over',
  'drop',
  'drag-end'
])

const { t } = useI18n()

function handleSelectAllColumns(e) {
  emit('select-all', e)
}

function handleQuickWidthSelect(key, menuKey) {
  if (menuKey === 'auto') {
    emit('set-width', { key, value: undefined })
    return
  }
  emit('set-width', { key, value: Number.parseInt(menuKey, 10) })
}

function handleColumnItemKeydown(event, col, idx) {
  if (event.code === 'Enter' || event.code === 'Space') {
    event.preventDefault()
    emit('toggle-column', { key: col.key, checked: !props.visibleColumns.includes(col.key) })
    return
  }

  if (event.code === 'ArrowUp') {
    event.preventDefault()
    if (idx > 0) {
      emit('move', { key: col.key, dir: -1 })
    }
    return
  }

  if (event.code === 'ArrowDown') {
    event.preventDefault()
    if (idx < props.allColumns.length - 1) {
      emit('move', { key: col.key, dir: 1 })
    }
  }
}
</script>

<style scoped>
.column-settings {
  width: min(95vw, 560px);
  max-height: calc(100vh - 350px);
  background: var(--base-bg-color);
  border-radius: 4px;
  box-shadow: 0 2px 8px var(--shadow-color);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.column-settings-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.header-left,
.header-right {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 14px;
}

.column-settings-content {
  flex: 1;
  overflow-y: auto;
  padding: 4px 0;
}

.column-settings-content::-webkit-scrollbar {
  width: 6px;
}

.column-settings-content::-webkit-scrollbar-track {
  background: var(--hover-bg-color);
}

.column-settings-content::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 3px;
}

.column-item {
  display: flex;
  align-items: center;
  padding: 10px 12px;
  gap: 12px;
  border-bottom: 1px solid var(--border-color);
  transition: all 0.25s ease;
  background: var(--base-bg-color);
  justify-content: space-between;
}

.column-item:focus-visible {
  outline: 2px solid var(--primary-color);
  outline-offset: -2px;
}

.column-item:hover {
  background: var(--surface-subtle);
}

.column-item.is-dragging {
  opacity: 0.5;
  background: var(--primary-color-weak);
}

.column-drag-handle {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: move;
  color: var(--text-color-muted);
  border-radius: 4px;
  transition: all 0.3s;
}

.column-drag-handle:hover {
  color: var(--primary-color);
  background: var(--primary-color-weak);
}

.column-checkbox-wrapper {
  flex: 1;
  min-width: 0;
  max-width: 220px;
  overflow: hidden;
}

.column-title {
  display: inline-block;
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 13px;
}

.column-controls {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-shrink: 0;
  margin-left: auto;
}

.column-fixed-group {
  display: flex;
  gap: 2px;
  padding: 2px;
  background: var(--surface-light);
  border-radius: 4px;
  border: 1px solid var(--surface-lighter);
}

.column-fixed-btn {
  padding: 0 6px !important;
  min-width: 24px !important;
  height: 24px !important;
  font-size: 12px;
}

.column-fixed-btn.active {
  color: var(--primary-color) !important;
  background: var(--primary-color-weak) !important;
}

.column-width-group {
  display: flex;
  align-items: center;
  gap: 4px;
}

.column-width-preset {
  padding: 0 6px !important;
  min-width: 36px !important;
  height: 24px !important;
}

.column-width-input {
  width: 70px !important;
}

:deep(.column-settings .ant-input-number),
:deep(.column-settings .ant-input-number-input),
:deep(.column-settings .ant-input-number-handler-wrap),
:deep(.column-settings .ant-tag),
:deep(.column-settings .ant-btn-text) {
  background: var(--layout-surface);
  color: var(--text-color);
  border-color: var(--border-color);
}

:global([data-theme='dark']) .column-settings :deep(.ant-checkbox-wrapper),
:global([data-theme='dark']) .column-settings :deep(.ant-btn),
:global([data-theme='dark']) .column-settings :deep(.ant-dropdown-menu-item),
:global([data-theme='dark']) .column-settings :deep(.ant-space),
:global([data-theme='dark']) .column-settings :deep(.anticon) {
  color: var(--text-color) !important;
}

.column-settings-footer {
  padding: 12px 16px;
  border-top: 1px solid var(--border-color);
  flex-shrink: 0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
