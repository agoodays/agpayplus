<template>
  <a-select
    show-search
    :value="value"
    :placeholder="placeholder"
    :default-active-first-option="false"
    :show-arrow="false"
    :filter-option="false"
    :not-found-content="loading ? '加载中...' : '无匹配数据'"
    @search="handleSearch"
    @change="handleChange"
    @popupScroll="handlePopupScroll"
    :disabled="disabled"
    allowClear
  >
    <a-select-option
      v-for="d in data"
      :key="d[valueField]"
      :value="d[valueField]"
      :title="showValue && d[labelField] !== d[valueField] ? (d[labelField] + ' [ ' + d[valueField] + ' ]') : d[labelField]"
    >
      <template v-if="showValue && d[labelField] !== d[valueField]">
        {{ d[labelField] + " [ " + d[valueField] + " ]" }}
      </template>
      <template v-else>
        {{ d[labelField] }}
      </template>
    </a-select-option>
    <a-select-option v-if="!hasMore && data.length > 0" key="all-loaded" disabled value="">
      已加载全部
    </a-select-option>
  </a-select>
</template>

<script>
export default {
  name: 'AgSelect',
  props: {
    value: [String, Number],
    api: { type: Function, required: true }, // 远程搜索API函数，返回Promise
    valueField: { type: String, default: 'value' }, // 选项value字段
    labelField: { type: String, default: 'text' }, // 选项label字段
    placeholder: { type: String, default: '请输入关键字搜索' },
    disabled: { type: Boolean, default: false }, // 新增支持disabled
    showValue: { type: Boolean, default: true }, // 新增，是否显示valueField
    pageSize: { type: Number, default: 10 } // 默认每页条数
  },
  data () {
    return {
      data: [],
      loading: false,
      searchTimeout: null,
      hasMore: true,
      lastSearch: '',
      loadMoreLock: false,
      iPage: { pageNumber: 1, pageSize: this.pageSize } // 初始化iPage
    }
  },
  watch: {
    value: {
      immediate: true,
      handler (val) {
        if (
          val &&
          !this.data.some(d => d[this.valueField] === val)
        ) {
          const params = { pageNumber: 1, pageSize: 1 }
          params[this.valueField] = val
          this.loading = true
          this.api(params).then(res => {
            const list = res.records || []
            if (list.length) {
              if (!this.data.some(d => d[this.valueField] === val)) {
                this.data.unshift(list[0])
              }
            }
          }).finally(() => {
            this.loading = false
          })
        }
      }
    }
  },
  methods: {
    handleSearch (val) {
      if (this.searchTimeout) {
        clearTimeout(this.searchTimeout)
      }
      if (!val) {
        this.data = []
        this.iPage.pageNumber = 1
        this.hasMore = true
        this.lastSearch = ''
        return
      }
      // 300ms防抖
      this.searchTimeout = setTimeout(() => {
        this.loading = true
        this.iPage.pageNumber = 1
        this.hasMore = true
        this.lastSearch = val
        const params = { pageNumber: this.iPage.pageNumber, pageSize: this.iPage.pageSize }
        params[this.labelField] = val
        this.api(params).then(res => {
          const list = res.records || []
          this.data = list
          if (typeof res.hasNext !== 'undefined') {
            this.hasMore = !!res.hasNext
          } else {
            this.hasMore = list.length === this.iPage.pageSize
          }
          if (!this.hasMore && list.length === 0) {
            this.data = [{
              [this.valueField]: val,
              [this.labelField]: val
            }]
            this.handleChange(val)
          }
        }).finally(() => {
          this.loading = false
        })
      }, 300)
    },
    handlePopupScroll (e) {
      if (!this.hasMore) return
      const target = e.target
      if (
        this.hasMore &&
        !this.loading &&
        !this.loadMoreLock &&
        target.scrollHeight > target.offsetHeight &&
        target.scrollTop + target.offsetHeight >= target.scrollHeight - 10
      ) {
        this.loadMoreLock = true
        this.loadMore()
      }
    },
    loadMore () {
      this.loading = true
      const nextPage = this.iPage.pageNumber + 1
      const params = { pageNumber: nextPage, pageSize: this.iPage.pageSize }
      params[this.labelField] = this.lastSearch
      this.api(params).then(res => {
        const list = res.records || []
        const existKeys = new Set(this.data.map(d => d[this.valueField]))
        const newList = list.filter(d => !existKeys.has(d[this.valueField]))
        if (newList.length) {
          this.data = this.data.concat(newList)
          this.iPage.pageNumber = nextPage
        }
        if (typeof res.hasNext !== 'undefined') {
          this.hasMore = !!res.hasNext
        } else if (typeof res.total !== 'undefined' && typeof res.current !== 'undefined') {
          this.hasMore = res.current * this.iPage.pageSize < res.total
        } else {
          this.hasMore = newList.length === this.iPage.pageSize
        }
      }).finally(() => {
        this.loading = false
        this.loadMoreLock = false
      })
    },
    handleChange (value) {
      // 找到当前选中的数据对象
      const selected = this.data.find(d => d[this.valueField] === value) || null
      this.$emit('input', value)
      this.$emit('change', value, selected)
    }
  },
  beforeDestroy () {
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout)
    }
  }
}
</script>

<style scoped>

</style>
