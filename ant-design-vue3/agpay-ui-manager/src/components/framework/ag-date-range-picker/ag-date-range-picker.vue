<template>
  <div>
    <a-select
      v-if="optionValue !== 'customDateTime'"
      v-model="optionValue"
      placeholder=""
      @change="optionChange"
      class="full-width"
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
        <span class="nowrap">{{ dateRangeTip }}</span>
      </template>
      <a-range-picker
        v-if="optionValue === 'customDateTime'"
        v-model="dateRangeValue"
        @change="onChange"
        class="full-width"
        ref="dateRangePicker"
        :open="dateRangeOpen"
        @openChange="handleDateRangeOpenChange"
        :format="format"
        :ranges="{
          本月: [moment().startOf('month'), moment()],
          本年: [moment().startOf('year'), moment()]
        }"
      >
        <a-icon slot="suffixIcon" type="sync" />
        <template #renderExtraFooter>
          <div class="render-extra-footer">
            <span class="ant-tag ant-tag-blue render-extra-action" @click="onClick">
              <a-icon type="left-circle" /> 返回日期下拉框
            </span>
          </div>
        </template>
      </a-range-picker>
    </a-popover>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import moment from 'moment';

const props = defineProps({
  value: { type: String, default: '' },
  format: { type: String, default: 'YYYY-MM-DD' },
  options: {
    type: Array,
    default: () => [
      { name: '全部时间', value: '' },
      { name: '今天', value: 'today' },
      { name: '昨天', value: 'yesterday' },
      { name: '近7天', value: 'near2now_7' },
      { name: '近30天', value: 'near2now_30' },
      { name: '自定义时间', value: 'customDateTime' }
    ]
  }
});

const emit = defineEmits(['change']);

const dateRangeValue = ref([]);
const dateRangeTip = ref('');
const dateRangeTipIsShow = ref(false);
const dateRangeOpen = ref(false);

const optionValue = ref('');
const optionOriginValue = ref('');

const setDateRangeValue = (value, start, end) => {
  switch (value) {
    case 'today':
      start = moment().startOf('day');
      end = moment();
      break;
    case 'yesterday':
      start = moment().startOf('day').subtract(1, 'days');
      end = moment().endOf('day').subtract(1, 'days');
      break;
    case value?.startsWith('near2now'):
      const day = +value.split('_')[1];
      start = moment().startOf('day').subtract(day - 1, 'days');
      end = moment().endOf('day');
      break;
    default:
      if (start?.length > 0 && end?.length > 0) {
        start = moment(start);
        end = moment(end);
      }
      break;
  }
  if (start && end) {
    dateRangeValue.value = [start, end];
    dateRangeTip.value = `搜索时间： ${start.format('YYYY-MM-DD')} 00:00:00 ~ ${end.format('YYYY-MM-DD')} 23:59:59`;
  } else {
    dateRangeValue.value = [];
    dateRangeTip.value = '';
  }
};

const handleHoverChange = (visible) => {
  if (dateRangeTip.value.length > 0) {
    dateRangeTipIsShow.value = visible;
  } else {
    dateRangeTipIsShow.value = false;
  }
};

const handleDateRangeOpenChange = (open) => {
  dateRangeOpen.value = open;
};

const optionChange = () => {
  if (optionValue.value !== 'customDateTime') {
    optionOriginValue.value = optionValue.value;
    setDateRangeValue(optionValue.value);
    emit('change', optionValue.value);
  } else {
    handleDateRangeOpenChange(true);
  }
};

const onChange = (date, dateString) => {
  const start = dateString[0]; // 开始时间
  const end = dateString[1]; // 结束时间
  if (start.length && end.length) {
    emit('change', `${optionValue.value}_${start} 00:00:00_${end} 23:59:59`);
  } else {
    emit('change', '');
    handleHoverChange(false);
  }
  setDateRangeValue(null, start, end);
};

const onClick = () => {
  handleHoverChange(false);
  optionValue.value = optionOriginValue.value;
  setDateRangeValue(optionValue.value);
  emit('change', optionValue.value);
};

// 初始化
const [option, startDate, endDate] = props.value.split('_');
const _optionValue = option === 'customDateTime' ? option : props.value;
optionValue.value = _optionValue;
optionOriginValue.value = _optionValue === 'customDateTime' ? '' : _optionValue;
setDateRangeValue(_optionValue, startDate, endDate);
</script>

<style scoped lang="less">
.full-width {
  width: 100%;
}
.nowrap {
  white-space: nowrap;
}
.render-extra-footer {
  text-align: right;
}
.render-extra-action {
  cursor: pointer;
}
</style>