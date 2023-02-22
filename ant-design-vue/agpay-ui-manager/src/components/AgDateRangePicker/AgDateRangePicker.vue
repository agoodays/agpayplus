<template>
  <div>
    <a-select
        v-if="optionValue!=='customDateTime'"
        v-model="optionValue"
        placeholder=""
        @change="optionChange"
        style="width: 100%"
        ref="dateSelect"
    >
      <a-select-option v-for="o in options" :value="o.value" :key="o.value">
        {{ o.name }}
      </a-select-option>
    </a-select>
    <a-popover
      placement="bottom"
      trigger="hover"
      :visible="dateRangeTipIsShow"
    >
      <template #content>
        <span style="white-space:nowrap;">{{ dateRangeTip }}</span>
      </template>
      <a-range-picker
        v-if="optionValue==='customDateTime'"
        @change="onChange"
        @mouseenter="onMouseenter"
        @mouseleave="onMouseleave"
        style="width: 100%"
        ref="dateRangePicker"
      >
        <a-icon slot="suffixIcon" type="sync"/>
        <template #renderExtraFooter>
          <span style="cursor: pointer;" @click="onClick">
            <span role="img" aria-label="left-circle" class="anticon anticon-left-circle">
              <a-icon type="left-circle" />
            </span>
            返回日期下拉框
          </span>
        </template>
      </a-range-picker>
    </a-popover>
  </div>
</template>

<script>
export default {
  name: 'AgDateRangePicker',
  props: {
    value: { type: String, default: '' },
    options: {
      type: Array,
      default: () => ([
        { name: '全部时间', value: '' },
        { name: '今天', value: 'today' },
        { name: '昨天', value: 'yesterday' },
        { name: '近7天', value: 'near2now_7' },
        { name: '近30天', value: 'near2now_30' },
        { name: '自定义时间', value: 'customDateTime' }
      ])
    }
  },
  data () {
    return {
      optionValue: this.value,
      optionOriginValue: this.value,
      dateRangeTipIsShow: false,
      dateRangeTip: ''
    }
  },
  methods: {
    optionChange () {
      if (this.optionValue !== 'customDateTime') {
        this.optionOriginValue = this.optionValue
        this.$emit('change', this.optionValue)
      }
    },
    onChange (date, dateString) {
      const start = dateString[0] // 开始时间
      const end = dateString[1] // 结束时间
      if (start.length && end.length) {
        this.dateRangeTip = `搜索时间： ${start} 00:00:00 ~ ${end} 23:59:59`
        this.$emit('change', `${this.optionValue}_${start} 00:00:00_${end} 23:59:59`)
      } else {
        this.dateRangeTip = ''
      }
    },
    onMouseenter () {
      this.dateRangeTipIsShow = this.dateRangeTip.length && true
    },
    onMouseleave () {
      this.dateRangeTipIsShow = false
    },
    onClick () {
      this.optionValue = this.optionOriginValue
      this.$emit('change', this.optionValue)
    }
  }
}
</script>

<style scoped lang="less">

</style>
