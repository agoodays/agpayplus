<template>
  <div>
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
        @click="emit('addAgCard')"
      >
        <div class="ag-card-add" :style="{ height: height + 'px' }">
          <div class="ag-card-add-top">
            <img src="~@/assets/svg/add-icon.svg" alt="add-icon" class="ag-card-add-icon" />
            <img src="~@/assets/svg/add-icon-hover.svg" alt="add-icon" class="ag-card-add-icon-hover" />
          </div>
          <div class="ag-card-add-text">新建{{ name }}</div>
        </div>
      </a-col>

      <!-- 数据卡片 -->
      <a-col
        v-for="(item, key) in cardList"
        :key="key"
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
        v-model:current="pagination.current"
        :total="pagination.total"
        :page-size="pagination.pageSize"
        show-less-items
        @change="handlePageChange"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  span: {
    type: Object,
    default: () => ({ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 })
  },
  height: { type: Number, default: 200 },
  name: { type: String, default: '' },
  addAuthority: { type: Boolean, default: false },
  searchData: { type: Object, default: () => ({}) },
  reqCardListFunc: { type: Function, required: true },
  usePagination: { type: Boolean, default: false },
  pageSize: { type: Number, default: 10 }
})

const emit = defineEmits(['addAgCard', 'btnLoadClose'])

const cardList = ref([])
const pagination = ref({ current: 1, pageSize: props.pageSize, total: 0 })

const refCardList = (isToFirst = false) => {
  if (props.usePagination && isToFirst) {
    pagination.value.current = 1
  }

  let params = { ...props.searchData }

  if (props.usePagination) {
    params = {
      ...params,
      pageNumber: pagination.value.current,
      pageSize: pagination.value.pageSize
    }
  }

  if (typeof props.reqCardListFunc === 'function') {
    props
      .reqCardListFunc(params)
      .then((res) => {
        if (props.usePagination) {
          cardList.value = res.records || []
          pagination.value.total = res.total || 0
        } else {
          cardList.value = Array.isArray(res) ? res : res.records || []
          pagination.value.total = cardList.value.length
        }
        emit('btnLoadClose')
      })
      .catch((err) => {
        console.error('AgCard 加载失败:', err)
        cardList.value = []
        pagination.value.total = 0
        emit('btnLoadClose')
      })
  } else {
    console.error('AgCard: reqCardListFunc 不是一个函数')
    cardList.value = []
    pagination.value.total = 0
    emit('btnLoadClose')
  }
}

const handlePageChange = (page) => {
  pagination.value.current = page
  refCardList()
}

onMounted(() => {
  refCardList()
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
