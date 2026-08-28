<template>
  <div class="ag-table-statistics">
    <div v-if="!statistics" class="statistics-empty">
      <a-empty :description="t('agTable.noStatisticsData')" :style="{ marginTop: '20px', marginBottom: '20px' }" />
    </div>

    <div v-else class="statistics-content">
      <div v-if="statisticsFormat.isObject" class="statistics-grid">
        <div v-for="(entry, idx) in statisticsFormat.entries" :key="`stat-${idx}`" class="statistics-card">
          <div class="statistics-label">{{ entry[0] }}</div>
          <div class="statistics-value">{{ entry[1] }}</div>
        </div>
      </div>

      <div v-else class="statistics-groups">
        <div v-for="group in statisticsFormat.groups" :key="`group-${group.id}`" class="statistics-group">
          <div v-if="group.name" class="statistics-group-title">{{ group.name }}</div>
          <div class="statistics-grid">
            <div v-for="(entry, idx) in group.entries" :key="`entry-${idx}`" class="statistics-card">
              <div class="statistics-label">{{ entry[0] }}</div>
              <div class="statistics-value">{{ entry[1] }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const props = defineProps({
  statistics: { type: [Object, Array], default: null },
})

const { t } = useI18n()

const statisticsFormat = computed(() => {
  const data = props.statistics
  if (!data) return { isObject: true, entries: [] }

  try {
    if (Array.isArray(data)) {
      const groups = data
        .filter((item) => item && typeof item === 'object')
        .map((item, idx) => {
          const entries = Object.entries(item).filter(([key]) => !key.startsWith('_'))

          return {
            id: `group-${idx}`,
            name: item._groupName || null,
            entries,
          }
        })
      return { isObject: false, groups }
    }

    if (typeof data === 'object') {
      const entries = Object.entries(data).filter(([key]) => !key.startsWith('_'))
      return { isObject: true, entries }
    }

    return { isObject: true, entries: [] }
  } catch (error) {
    console.warn('[ag-table] Parse statistics failed:', error)
    return { isObject: true, entries: [] }
  }
})
</script>

<style scoped>
.ag-table-statistics {
  padding: 16px;
  margin-bottom: 16px;
  background: var(--layout-surface);
  border-radius: 4px;
}

.statistics-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 120px;
}

.statistics-content {
  width: 100%;
}

.statistics-groups {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.statistics-group {
  padding-bottom: 12px;
  border-bottom: 1px solid var(--border-color);
}

.statistics-group:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.statistics-group-title {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-color);
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 2px solid var(--primary-color);
  display: inline-block;
}

.statistics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 12px;
}

.statistics-card {
  background: var(--base-bg-color);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  padding: 12px 16px;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.statistics-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 2px 8px var(--primary-color-hover);
  transform: translateY(-2px);
}

.statistics-label {
  font-size: 12px;
  color: var(--text-color-weak);
  margin-bottom: 8px;
  font-weight: 500;
}

.statistics-value {
  font-size: 20px;
  font-weight: 600;
  color: var(--primary-color);
}

@media (max-width: 768px) {
  .statistics-grid {
    grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
    gap: 8px;
  }
}
</style>
