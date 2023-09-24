<template>
  <div>
    <div class="table-page-search-wrapper">
      <a-form layout="inline" class="table-head-ground">
        <div class="table-layer">
          <slot name="formItem"></slot>
          <a-form-item label="" class="table-head-layout">
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" @click="queryFunc" :loading="btnLoading">搜索</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="resetFunc">重置</a-button>
            </span>
          </a-form-item>
        </div>
      </a-form>
    </div>
    <div class="split-line">
      <div class="btns" @click="setIsShowMore">
        <div>
          {{ isShowMore ? '收起' : '更多' }}筛选 <a-icon :type="isShowMore ? 'up' : 'down'" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AgSearchForm',
  props: {
    searchData: { type: Object, default: null },
    isShowMore: { type: Boolean, default: false },
    btnLoading: { type: Boolean, default: false }
  },
  data () {
    return {}
  },
  methods: {
    updateSearchData (key, value) {
      this.$emit('update-search-data', {
        ...this.searchData,
        [key]: value
      })
    },
    setIsShowMore () {
      this.$emit('set-is-show-more', !this.isShowMore)
    },
    queryFunc () {
      this.$emit('query-func')
    },
    resetFunc () {
      this.$emit('update-search-data', {})
    }
  }
}
</script>

<style scoped>

</style>
