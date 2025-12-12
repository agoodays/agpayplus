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
        @click="$emit('addAgCard')"
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
    <div v-if="usePagination" style="text-align: center; margin-top: 20px;">
      <a-pagination
        v-model="pagination.current"
        :total="pagination.total"
        :page-size="pagination.pageSize"
        show-less-items
        @change="handlePageChange"
      />
    </div>
  </div>
</template>

<script>
export default {
  name: 'AgCard',
  props: {
    span: {
      type: Object,
      default: () => ({ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 })
    },
    height: { type: Number, default: 200 },
    name: { type: String, default: '' },
    addAuthority: { type: Boolean, default: false },
    searchData: { type: Object, default: () => ({}) },
    reqCardListFunc: { type: Function, required: true },
    usePagination: { type: Boolean, default: false }, // 新增：是否启用分页
    pageSize: { type: Number, default: 10 } // 默认每页条数
  },
  data () {
    return {
      cardList: [],
      pagination: { current: 1, pageSize: this.pageSize, total: 0 } // 分页器数据（用于分页器显示）
    }
  },
  created () {
    this.refCardList()
  },
  methods: {
    /**
     * 重新加载数据
     * @param {Boolean} isToFirst - 是否跳转到第一页（仅在分页模式下生效）
     */
    refCardList (isToFirst = false) {
      const that = this
      if (this.usePagination && isToFirst) {
        this.pagination.current = 1
      }

      let params = { ...this.searchData }

      if (this.usePagination) {
        params = {
          ...params,
          pageNumber: this.pagination.current,
          pageSize: this.pagination.pageSize
        }
      }

      this.reqCardListFunc(params)
        .then(res => {
          if (this.usePagination) {
            // 分页模式：res 应包含 data 和 total
            this.cardList = res.records || []
            this.pagination.total = res.total || 0
          } else {
            // 非分页模式：res 可直接是数组
            this.cardList = Array.isArray(res) ? res : (res.records || [])
            this.pagination.total = this.cardList.length
          }
          // 请求成功后，关闭查询按钮的loading
          that.$emit('btnLoadClose')
        })
        .catch(err => {
          console.error('AgCard 加载失败:', err)
          this.cardList = []
          this.pagination.total = 0
          // that.$message?.error?.('加载数据失败')
          // 请求成功后，关闭查询按钮的loading
          that.$emit('btnLoadClose')
        })
    },

    /**
    * 分页切换
    */
    handlePageChange (page) {
      this.pagination.current = page
      this.refCardList()
    }
  }
}
</script>

<style lang="less" scoped>
.ag-card-add {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border: 2px dashed rgba(0, 0, 0, 0.15);
  background: rgba(0, 0, 0, 0.03);
  border-radius: 6px;
  box-sizing: border-box;
  cursor: pointer;
}
.ag-card-add-top {
  width: 80px;
  height: 80px;
  position: relative;
}
.ag-card-add:hover {
  border-color: rgba(25, 83, 255, 0.3);
  background: rgba(25, 83, 255, 0.06);
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
  color: rgba(25, 83, 255, 1);
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
  color: rgba(0, 0, 0, 0.35);
}
</style>
