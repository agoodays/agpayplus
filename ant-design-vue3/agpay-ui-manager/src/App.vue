<template>
  <a-config-provider
      :theme="{
        token: {
          colorPrimary: colorPrimary,
        },
      }"
  >
    <!---全局loading--->
    <a-spin :spinning="spinning" size="large">
      <!--- 路由 -->
      <RouterView />
    </a-spin>
  </a-config-provider>
</template>

<script setup>
import { computed, ref, onMounted, watchEffect } from 'vue';
import { useSpinStore } from '/@/store/modules/system/spin';

// 全局loading
let spinStore = useSpinStore();
const spinning = computed(() => spinStore.loading);

const colorPrimary = ref('');
onMounted(() => {
  watchEffect(() => {
    const rootStyles = getComputedStyle(document.documentElement);
    console.log(rootStyles)
    colorPrimary.value = rootStyles.getPropertyValue('--ant-primary-color');
    console.log(colorPrimary.value)
  });
});

</script>

<style scoped>

</style>
