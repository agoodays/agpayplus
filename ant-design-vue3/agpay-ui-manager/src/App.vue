<template>
  <a-config-provider
      :theme="{
        token: {
          colorPrimary: appConfigStore.primaryColor,
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
import { computed } from 'vue';
import { useSpinStore } from '/@/store/modules/system/spin';
import { useAppConfigStore } from '/@/store/modules/system/app-config';

// 全局loading
let spinStore = useSpinStore();
const spinning = computed(() => spinStore.loading);

let appConfigStore = useAppConfigStore();
const colorPrimary = computed(() => appConfigStore.primaryColor);
console.log(appConfigStore.primaryColor)

</script>

<style scoped>

</style>
