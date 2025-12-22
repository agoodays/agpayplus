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
        :value="dateRangeValue"
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
    return {
      // 内部状态
      optionValue: '',
      optionOriginValue: '',
      dateRangeValue: [],
      dateRangeTip: '',
      dateRangeTipIsShow: false,
      dateRangeOpen: false
    }
  },
  watch: {
    // 监听外部传入的 value 变化
    value: {
      immediate: true,
      handler (newVal) {
        this.updateFromValue(newVal)
      }
    }
  },
  created () {
    // 初始化
    this.updateFromValue(this.value)
  },
  methods: {
    // 从 value 解析数据
    parseValue (value) {
      if (!value || value === '') {
        return ['', null, null]
      }
      const parts = value.split('_')
      if (parts[0] === 'customDateTime' && parts.length >= 3) {
        const startTime = parts[1]
        const endTime = parts[2]
        const startDate = startTime.split(' ')[0]
        const endDate = endTime.split(' ')[0]
        return ['customDateTime', startDate, endDate]
      } else {
        return [value, null, null]
      }
    },

    // 设置日期范围值
    setDateRangeValue (value, start, end) {
      let startMoment = null
      let endMoment = null

      if (value === 'today') {
        startMoment = moment().startOf('day')
        endMoment = moment()
      } else if (value === 'yesterday') {
        startMoment = moment().startOf('day').subtract(1, 'days')
        endMoment = moment().endOf('day').subtract(1, 'days')
      } else if (value && typeof value === 'string' && value.startsWith('near2now')) {
        // 处理 near2now 格式
        const day = +value.split('_')[1]
        startMoment = moment().startOf('day').subtract(day - 1, 'days')
        endMoment = moment().endOf('day')
      } else {
        // 自定义时间或其他情况
        if (start && start.length > 0 && end && end.length > 0) {
          startMoment = moment(start)
          endMoment = moment(end)
        }
      }

      if (startMoment && endMoment) {
        this.dateRangeValue = [startMoment, endMoment]
        this.dateRangeTip = `搜索时间： ${startMoment.format('YYYY-MM-DD')} 00:00:00 ~ ${endMoment.format('YYYY-MM-DD')} 23:59:59`
      } else {
        this.dateRangeValue = []
        this.dateRangeTip = ''
      }
    },

    // 根据传入的 value 更新内部状态
    updateFromValue (value) {
      const [optionValue, startDate, endDate] = this.parseValue(value)

      if (!value || value === '') {
        this.optionValue = ''
        this.optionOriginValue = ''
        this.setDateRangeValue('')
        return
      }

      if (optionValue === 'customDateTime') {
        this.optionValue = 'customDateTime'
        this.setDateRangeValue(null, startDate, endDate)
      } else {
        this.optionValue = optionValue
        this.optionOriginValue = optionValue
        this.setDateRangeValue(optionValue)
      }
    },

    // 选项变化
    optionChange () {
      if (this.optionValue !== 'customDateTime') {
        this.optionOriginValue = this.optionValue
        this.setDateRangeValue(this.optionValue)
        this.$emit('change', this.optionValue)
      } else {
        this.dateRangeOpen = true
      }
    },

    // 日期范围变化
    onChange (date, dateString) {
      const start = dateString[0]
      const end = dateString[1]
      if (start && start.length && end && end.length) {
        this.$emit('change', `${this.optionValue}_${start} 00:00:00_${end} 23:59:59`)
      } else {
        this.$emit('change', '')
        this.handleHoverChange(false)
      }
      this.setDateRangeValue(null, start, end)
    },

    // 点击返回
    onClick () {
      this.handleHoverChange(false)
      this.optionValue = this.optionOriginValue
      this.setDateRangeValue(this.optionValue)
      this.$emit('change', this.optionValue)
    },

    // 悬停变化
    handleHoverChange (visible) {
      if (this.dateRangeTip && this.dateRangeTip.length > 0) {
        this.dateRangeTipIsShow = visible
      } else {
        this.dateRangeTipIsShow = false
      }
    },

    // 日期选择器打开变化
    handleDateRangeOpenChange (open) {
      this.dateRangeOpen = open
    },
    moment
  }
}
</script>

<style scoped lang="less">

</style>
