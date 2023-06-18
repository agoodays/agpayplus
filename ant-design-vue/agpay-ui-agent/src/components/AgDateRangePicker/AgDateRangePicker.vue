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
      @visibleChange="handleHoverChange"
    >
      <template #content>
        <span style="white-space:nowrap;">{{ dateRangeTip }}</span>
      </template>
      <a-range-picker
        v-if="optionValue==='customDateTime'"
        v-model="dateRangeValue"
        @change="onChange"
        style="width: 100%"
        ref="dateRangePicker"
        :open="dateRangeOpen"
        @openChange="handleDateRangeOpenChange"
        :format="format"
        :ranges="{
          // 今天: [moment().startOf('day'), moment()],
          // 昨天: [moment().startOf('day').subtract(1,'days'), moment().endOf('day').subtract(1, 'days')],
          // 最近三天: [moment().startOf('day').subtract(2, 'days'), moment().endOf('day')],
          // 最近一周: [moment().startOf('day').subtract(1, 'weeks'), moment()],
          本月: [moment().startOf('month'), moment()],
          本年: [moment().startOf('year'), moment()]
        }"
      >
        <a-icon slot="suffixIcon" type="sync"/>
        <template #renderExtraFooter style="float: right;">
          <span class="ant-tag ant-tag-blue" style="cursor: pointer;" @click="onClick">
            <a-icon type="left-circle" /> 返回日期下拉框
          </span>
        </template>
      </a-range-picker>
    </a-popover>
  </div>
</template>

<script>
import { ref } from 'vue'
import moment from 'moment'
export default {
  name: 'AgDateRangePicker',
  props: {
    value: { type: String, default: '' },
    format: { type: String, default: 'YYYY-MM-DD' },
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
    const dateRangeValue = ref([])
    const dateRangeTip = ref('')
    const setDateRangeValue = (value, start, end) => {
      switch (value) {
        case 'today':
          start = moment().startOf('day')
          end = moment()
          break
        case 'yesterday':
          start = moment().startOf('day').subtract(1, 'days')
          end = moment().endOf('day').subtract(1, 'days')
          break
        case 'near2now_7':
          start = moment().startOf('day').subtract(6, 'days')
          end = moment().endOf('day')
          break
        case 'near2now_30':
          start = moment().startOf('day').subtract(29, 'days')
          end = moment().endOf('day')
          break
        default:
          if (start?.length > 0 && end?.length > 0) {
            start = moment(start)
            end = moment(end)
          }
          break
      }
      if (start && end) {
        dateRangeValue.value = [start, end]
        dateRangeTip.value = `搜索时间： ${start.format('YYYY-MM-DD')} 00:00:00 ~ ${end.format('YYYY-MM-DD')} 23:59:59`
      } else {
        dateRangeValue.value = []
        dateRangeTip.value = ''
      }
    }
    setDateRangeValue(this.value)
    const dateRangeTipIsShow = ref(false)
    const handleHoverChange = visible => {
      if (dateRangeTip.value.length > 0) {
        dateRangeTipIsShow.value = visible
      } else {
        dateRangeTipIsShow.value = false
      }
    }
    const dateRangeOpen = ref(false)
    const handleDateRangeOpenChange = open => {
      dateRangeOpen.value = open
    }
    return {
      optionValue: this.value,
      optionOriginValue: this.value,
      dateRangeValue,
      setDateRangeValue,
      dateRangeTip,
      dateRangeTipIsShow,
      handleHoverChange,
      dateRangeOpen,
      handleDateRangeOpenChange
    }
  },
  methods: {
    optionChange () {
      if (this.optionValue !== 'customDateTime') {
        this.optionOriginValue = this.optionValue
        this.setDateRangeValue(this.optionValue)
        this.$emit('change', this.optionValue)
      } else {
        this.handleDateRangeOpenChange(true)
      }
    },
    moment,
    onChange (date, dateString) {
      const start = dateString[0] // 开始时间
      const end = dateString[1] // 结束时间
      if (start.length && end.length) {
        this.$emit('change', `${this.optionValue}_${start} 00:00:00_${end} 23:59:59`)
      } else {
        this.$emit('change', '')
        this.handleHoverChange(false)
      }
      this.setDateRangeValue(null, start, end)
    },
    onClick () {
      this.handleHoverChange(false)
      this.optionValue = this.optionOriginValue
      this.setDateRangeValue(this.optionValue)
      this.$emit('change', this.optionValue)
    }
  }
}
</script>

<style scoped lang="less">

</style>
