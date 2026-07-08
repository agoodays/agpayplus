<template>
  <div class="ag-table-toolbar">
    <div class="toolbar-left">
      <slot name="left"></slot>
      
      <!-- 批量选择 -->
      <div v-if="rowSelectionEnabled" class="batch-selection-group">
        <a-checkbox 
          :checked="isAllSelected" 
          :indeterminate="isIndeterminate"
          @change="handleSelectAllChange"
        >
          全选
        </a-checkbox>
        <a-button type="link" size="small" @click="emit('clear-selection')">
          清空选择
        </a-button>
      </div>
    </div>

    <div class="toolbar-right">
      <slot name="right"></slot>

      <a-tooltip v-if="showAutoRefresh" placement="top" :title="t('agTable.autoRefresh')">
        <div class="auto-refresh-group">
          <sync-outlined :spin="autoRefreshEnabled" class="refresh-icon" />
          <span class="refresh-countdown">{{ autoRefreshCountdown }}s</span>
          <a-switch :checked="autoRefreshEnabled" size="small" @update:checked="handleAutoRefreshChange" />
        </div>
      </a-tooltip>

      <a-tooltip
        v-if="enableStatistics"
        placement="top"
        :title="showStatistics ? t('agTable.closeStatistics') : t('agTable.statistics')"
      >
        <button
          type="button"
          class="toolbar-icon-btn"
          :aria-label="showStatistics ? t('agTable.closeStatistics') : t('agTable.statistics')"
          @click="toggleStatistics"
        >
          <bar-chart-outlined v-if="!showStatistics" class="toolbar-icon" />
          <close-circle-outlined v-else class="toolbar-icon" />
        </button>
      </a-tooltip>

      <a-dropdown>
        <template #overlay>
          <a-menu @click="handleDensityChange">
            <a-menu-item key="small">
              <check-outlined v-if="density === 'small'" style="margin-right: 8px" />
              <span :style="{ marginLeft: density !== 'small' ? '20px' : '0' }">{{ t('agTable.densityCompact') }}</span>
            </a-menu-item>
            <a-menu-item key="middle">
              <check-outlined v-if="density === 'middle'" style="margin-right: 8px" />
              <span :style="{ marginLeft: density !== 'middle' ? '20px' : '0' }">{{ t('agTable.densityDefault') }}</span>
            </a-menu-item>
            <a-menu-item key="large">
              <check-outlined v-if="density === 'large'" style="margin-right: 8px" />
              <span :style="{ marginLeft: density !== 'large' ? '20px' : '0' }">{{ t('agTable.densityLoose') }}</span>
            </a-menu-item>
          </a-menu>
        </template>
        <a-tooltip placement="top" :title="t('agTable.tableDensity')">
          <column-height-outlined class="toolbar-icon" />
        </a-tooltip>
      </a-dropdown>

      <a-tooltip v-if="showDownload" placement="top" :title="t('agTable.dataExport')">
        <download-outlined class="toolbar-icon" @click="emit('download')" />
      </a-tooltip>

      <a-dropdown
        :open="columnSettingsOpen"
        :trigger="['click']"
        placement="bottomRight"
        :get-popup-container="(trigger) => trigger.parentElement"
        @update:open="handleColumnSettingsOpenChange"
      >
        <template #overlay>
          <ag-table-column-settings-panel
            :all-columns="allColumns"
            :visible-columns="visibleColumns"
            :column-widths="columnWidths"
            :column-fixed="columnFixed"
            :drag-key="dragKey"
            :is-all-columns-visible="isAllColumnsVisible"
            :is-some-columns-visible="isSomeColumnsVisible"
            @select-all="(event) => emit('select-all', event)"
            @reset="emit('reset-columns')"
            @close="emit('update:columnSettingsOpen', false)"
            @toggle-column="(payload) => emit('toggle-column', payload)"
            @set-fixed="(payload) => emit('set-fixed', payload)"
            @set-width="(payload) => emit('set-width', payload)"
            @move="(payload) => emit('move', payload)"
            @drag-start="(payload) => emit('drag-start', payload)"
            @drag-over="(payload) => emit('drag-over', payload)"
            @drop="(payload) => emit('drop', payload)"
            @drag-end="emit('drag-end')"
          />
        </template>
        <a-tooltip placement="top" :title="t('agTable.columnSettings')">
          <setting-outlined class="toolbar-icon" />
        </a-tooltip>
      </a-dropdown>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import {
    BarChartOutlined,
    CheckOutlined,
    CloseCircleOutlined,
    ColumnHeightOutlined,
    DownloadOutlined,
    SettingOutlined,
    SyncOutlined,
} from '@ant-design/icons-vue'
import { useI18n } from 'vue-i18n'
import AgTableColumnSettingsPanel from './column-settings-panel.vue'

const props = defineProps({
  showAutoRefresh: { type: Boolean, default: false },
  showDownload: { type: Boolean, default: false },
  enableStatistics: { type: Boolean, default: false },
  autoRefreshEnabled: { type: Boolean, default: false },
  autoRefreshCountdown: { type: Number, default: 0 },
  showStatistics: { type: Boolean, default: false },
  density: { type: String, default: 'middle' },
  columnSettingsOpen: { type: Boolean, default: false },
  allColumns: { type: Array, default: () => [] },
  visibleColumns: { type: Array, default: () => [] },
  columnWidths: { type: Object, default: () => ({}) },
  columnFixed: { type: Object, default: () => ({}) },
  dragKey: { type: String, default: null },
  isAllColumnsVisible: { type: Boolean, default: false },
  isSomeColumnsVisible: { type: Boolean, default: false },
  
  // 批量选择相关
  selectedRowKeys: { type: Array, default: () => [] },
  rowSelectionEnabled: { type: Boolean, default: false }
})

const emit = defineEmits([
  'update:autoRefreshEnabled',
  'update:showStatistics',
  'update:columnSettingsOpen',
  'density-change',
  'download',
  'select-all',
  'reset-columns',
  'toggle-column',
  'set-fixed',
  'set-width',
  'move',
  'drag-start',
  'drag-over',
  'drop',
  'drag-end',
  'select-all-rows',
  'clear-selection'
])

const { t } = useI18n()

// 是否全选
const isAllSelected = computed(() => {
  return props.selectedRowKeys.length > 0
})

// 是否部分选中
const isIndeterminate = computed(() => {
  return props.selectedRowKeys.length > 0
})

function handleAutoRefreshChange(checked) {
  emit('update:autoRefreshEnabled', checked)
}

function toggleStatistics() {
  emit('update:showStatistics', !props.showStatistics)
}

function handleColumnSettingsOpenChange(open) {
  emit('update:columnSettingsOpen', open)
}

function handleDensityChange({ key }) {
  emit('density-change', key)
}

function handleSelectAllChange(e) {
  emit('select-all-rows', e.target.checked)
}
</script>

<style scoped>
.ag-table-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0 16px 0;
  gap: 16px;
}

.toolbar-left {
  display: flex;
  gap: 12px;
  align-items: center;
  flex-wrap: wrap;
}

.toolbar-right {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
}

.batch-selection-group {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 4px 8px;
  background: var(--layout-surface);
  border-radius: 4px;
}

.auto-refresh-group {
  display: flex;
  align-items: center;
  padding: 0 8px;
}

.refresh-icon {
  font-size: 16px;
  color: var(--primary-color);
}

.refresh-countdown {
  min-width: 32px;
  padding: 2px 6px;
  font-size: 12px;
  color: var(--primary-color);
  font-weight: 600;
  text-align: center;
}

.toolbar-icon {
  font-size: 16px;
  font-weight: bold;
  color: var(--text-color);
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.3s;
}

.toolbar-icon:hover {
  color: var(--primary-color);
  background: var(--primary-color-weak);
}

.toolbar-icon-btn {
  border: none;
  background: transparent;
  padding: 0;
  margin: 0;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.toolbar-icon-btn:focus-visible {
  outline: 2px solid var(--primary-color);
  outline-offset: 2px;
  border-radius: 6px;
}

@media (max-width: 768px) {
  .ag-table-toolbar {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }

  .toolbar-right {
    width: 100%;
    flex-wrap: wrap;
  }
}
</style>