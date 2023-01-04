<template>
  <div>
    <a-range-picker
      @change="onChange"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DD HH:mm:ss">
      <a-icon slot="suffixIcon" type="sync"/>
    </a-range-picker>
    <label>{{ placeholder }}</label>
  </div>
</template>

<script>
export default {
  name: 'AgDateRangePicker',
  props: {
    msg: { type: String, default: '' },
    placeholder: { type: String, default: '' }
  },
  methods: {
    onChange (date, dateString) {
    }
  }
}
</script>

<style scoped lang="less">

</style>
