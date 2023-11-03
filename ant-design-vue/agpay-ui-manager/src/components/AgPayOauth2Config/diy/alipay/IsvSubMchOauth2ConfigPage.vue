<template>
  <div></div>
</template>

<script>
export default {
  name: 'MchOauth2ConfigPage',
  props: {
    ifParams: { type: Object, default: () => ({}) }
  },
  methods: {
    updateIfParams (key, value) {
      this.$emit('update-if-params', {
        ...this.ifParams,
        [key]: value
      })
    },
    updateIfParamsLiteParams (key, value) {
      this.$emit('update-form-data', {
        ...this.ifParams,
        liteParams: {
          ...this.ifParams.liteParams,
          [key]: value
        }
      })
    },
    validate: function validate (callback) {
      return this.$refs.infoFormModel.validate(callback)
    },
    resetFields: function resetFields () {
      this.$refs.infoFormModel.resetFields()
    }
  }
}
</script>

<style scoped>

</style>
