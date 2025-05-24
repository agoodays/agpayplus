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
    showValue: { type: Boolean, default: true } // 新增，是否显示valueField
  },
  data () {
    return {
      data: [],
      loading: false,
      searchTimeout: null // 防抖定时器
    }
  },
  methods: {
    handleSearch (val) {
      if (this.searchTimeout) {
        clearTimeout(this.searchTimeout)
      }
      if (!val) {
        this.data = []
        return
      }
      // 300ms防抖
      this.searchTimeout = setTimeout(() => {
        this.loading = true
        this.api(val).then(list => {
          if (!list || list.length === 0) {
            // 没有结果时，直接将输入值作为选项
            this.data = [{
              [this.valueField]: val,
              [this.labelField]: val
            }]
            this.handleChange(val)
          } else {
            this.data = list
          }
        }).finally(() => {
          this.loading = false
        })
      }, 300)
    },
    handleChange (val) {
      this.$emit('input', val)
      this.$emit('change', val)
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
