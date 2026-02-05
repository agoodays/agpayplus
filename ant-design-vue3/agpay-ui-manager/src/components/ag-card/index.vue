<template>
  <a-card :title="title" :bordered="bordered">
    <slot />
  </a-card>
</template>

<script setup>
const props = defineProps({
  title: { type: String, default: '' },
  bordered: { type: Boolean, default: true }
})
</script>

<style scoped>
</style>