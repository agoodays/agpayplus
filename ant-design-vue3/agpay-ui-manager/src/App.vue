<script setup>
import { computed } from 'vue';
import { useSpinStore } from '/@/store/modules/system/spin';

// 全局loading
let spinStore = useSpinStore();
const spinning = computed(() => spinStore.loading);
</script>

<template>
  <a-config-provider
      :theme="{
        token: {
          colorPrimary: '#00b96b',
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

<style scoped>

</style>
