<template>
  <a-input-search
    v-model:value="value"
    placeholder="input search text"
    style="width: 200px"
    @search="onSearch"
  />
</template>

<script>
export default {
  name: 'AgSearchInfoInput'
}
</script>

<style scoped>

</style>
