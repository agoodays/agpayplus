<template>
  <div class="ag-card-list">
    <a-row :gutter="[24, 24]">
      <!-- 新增卡片 -->
      <a-col
        v-if="addAuthority"
        :xxl="24 / span.xxl"
        :xl="24 / span.xl"
        :lg="24 / span.lg"
        :md="24 / span.md"
        :sm="24 / span.sm"
        :xs="24 / span.xs"
        @click="emit('add')"
      >
        <div class="ag-card-add" :style="{ height: height + 'px' }">
          <div class="ag-card-add-top">
            <img src="@/assets/svg/add-icon.svg" alt="add-icon" class="ag-card-add-icon" />
            <img src="@/assets/svg/add-icon-hover.svg" alt="add-icon" class="ag-card-add-icon-hover" />
          </div>
          <div class="ag-card-add-text">新增{{ name }}</div>
        </div>
      </a-col>

      <!-- 数据卡片 -->
      <a-col
        v-for="item in cardDataList"
        :key="item.id"
        :xxl="24 / span.xxl"
        :xl="24 / span.xl"
        :lg="24 / span.lg"
        :md="24 / span.md"
        :sm="24 / span.sm"
        :xs="24 / span.xs"
      >
        <slot name="cardContentSlot" :record="item"></slot>
        <slot name="cardOpSlot" :record="item"></slot>
      </a-col>
    </a-row>

    <!-- 分页器（仅在 usePagination 为 true 时显示） -->
    <div v-if="usePagination" style="text-align: center; margin-top: 20px">
      <a-pagination
        v-model:current="paginationInfo.current"
        :total="paginationInfo.total"
        :page-size="paginationInfo.pageSize"
        show-less-items
        @change="handlePageChange"
      />
    </div>
  </div>
</template>

<script setup>
/**
 * AgCard - 卡片列表组件
 *
 * 功能：
 * - 以卡片形式展示数据，支持新增卡片入口、分页、响应式布局
 * - 通过 loadData 异步加载数据
 * - 支持搜索条件透传和分页
 *
 * @example
 * <AgCard
 *   :search-data="searchData"
 *   :load-data="loadData"
 *   :use-pagination="true"
 *   :page-size="12"
 *   :span="{ xxl: 6, xl: 4, lg: 3, md: 2, sm: 1, xs: 1 }"
 *   add-authority
 *   name="商户"
 *   @add="handleAdd"
 * />
 */
import { ref, onMounted } from 'vue'

const props = defineProps({
  /** 响应式栅格配置，每个值表示一行显示的卡片数 */
  span: {
    type: Object,
    default: () => ({ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 })
  },
  /** 卡片高度（px） */
  height: {
    type: Number,
    default: 200
  },
  /** 新增卡片的业务名称（用于"新增xxx"文案） */
  name: {
    type: String,
    default: ''
  },
  /** 是否显示新增卡片入口 */
  addAuthority: {
    type: Boolean,
    default: false
  },
  /** 搜索条件，变化后会透传给 loadData */
  searchData: {
    type: Object,
    default: () => ({})
  },
  /** 数据加载函数，接收 params，返回 { records, total } 或数组 */
  loadData: {
    type: Function,
    required: true
  },
  /** 是否启用分页 */
  usePagination: {
    type: Boolean,
    default: false
  },
  /** 每页条数 */
  pageSize: {
    type: Number,
    default: 10
  }
})

const emit = defineEmits(['add', 'loadComplete'])

/** 卡片数据列表 */
const cardDataList = ref([])
/** 分页信息 */
const paginationInfo = ref({ current: 1, pageSize: props.pageSize, total: 0 })

/**
 * 刷新卡片列表数据
 * @param {boolean} [isToFirst=false] - 是否回到第一页
 * @returns {Promise<void>}
 */
async function reload(isToFirst = false) {
  if (props.usePagination && isToFirst) {
    paginationInfo.value.current = 1
  }

  let params = { ...props.searchData }

  if (props.usePagination) {
    params = {
      ...params,
      pageNumber: paginationInfo.value.current,
      pageSize: paginationInfo.value.pageSize
    }
  }

  try {
    const res = await props.loadData(params)
    if (props.usePagination) {
      cardDataList.value = res?.records || []
      paginationInfo.value.total = res?.total || 0
    } else {
      cardDataList.value = Array.isArray(res) ? res : res?.records || []
      paginationInfo.value.total = cardDataList.value.length
    }
  } catch (err) {
    console.error('AgCard 加载失败:', err)
    cardDataList.value = []
    paginationInfo.value.total = 0
  } finally {
    emit('loadComplete')
  }
}

/**
 * 分页变化处理
 * @param {number} page - 新页码
 */
function handlePageChange(page) {
  paginationInfo.value.current = page
  reload()
}

defineExpose({
  reload
})

onMounted(() => {
  reload()
})
</script>

<style lang="less" scoped>
.ag-card-add {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border: 2px dashed var(--border-dashed);
  background: var(--surface-variant);
  border-radius: var(--border-radius);
  box-sizing: border-box;
  cursor: pointer;
}
.ag-card-add-top {
  width: 80px;
  height: 80px;
  position: relative;
}
.ag-card-add:hover {
  border-color: var(--primary-color-weak);
  background: var(--primary-color-hover);
  transition: all 0.3s ease-in-out;
}
.ag-card-add:hover .ag-card-add-icon {
  opacity: 0;
  transition: all 0.2s ease-in-out;
}
.ag-card-add:hover .ag-card-add-icon-hover {
  opacity: 1;
  transition: all 0.5s ease-in-out;
}
.ag-card-add:hover .ag-card-add-text {
  color: var(--primary-color);
  transition: all 0.3s ease-in-out;
}
.ag-card-add-icon,
.ag-card-add-icon-hover {
  position: absolute;
  width: 80px;
  height: 80px;
}
.ag-card-add-icon {
  opacity: 1;
}
.ag-card-add-icon-hover {
  opacity: 0;
}
.ag-card-add-text {
  padding-top: 5px;
  font-size: 16px;
  color: var(--text-color-muted);
}
</style>
