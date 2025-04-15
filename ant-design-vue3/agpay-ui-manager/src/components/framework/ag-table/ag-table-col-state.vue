<template>
  <div>
    <template v-if="!showSwitchType">
      <div v-if="state === 0"><a-badge status="error" text="停用" /></div>
      <div v-else-if="state === 1"><a-badge status="processing" text="启用" /></div>
      <div v-else><a-badge status="warning" text="未知" /></div>
    </template>

    <template v-if="showSwitchType">
      <a-switch class="els" :checked="switchChecked" @change="onChangeInner" />
    </template>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue';

// Props
defineProps({
  state: { type: Number, default: -1 }, // 初始化列表数据，默认 -1
  showSwitchType: { type: Boolean, default: false }, // 默认 badge
  onChange: {
    type: Function,
    default: (checked) => {
      return new Promise((resolve) => {
        resolve();
      });
    },
  },
});

// Local state
const switchChecked = ref(false);

// Watchers
watch(
  () => state,
  (newVal) => {
    switchChecked.value = newVal === 1;
  }
);

// Lifecycle hooks
onMounted(() => {
  switchChecked.value = state === 1;
});

// Methods
const onChangeInner = (checked) => {
  switchChecked.value = checked;

  // 回调输出 0 和 1; 成功不需要处理，失败需要将状态变更为原始状态
  onChange(checked ? 1 : 0)
    .then(() => {
      // 成功逻辑（如果有其他逻辑需要处理，可以在这里添加）
    })
    .catch(() => {
      // 失败时恢复原始状态
      switchChecked.value = !checked;
    });
};
</script>

<style scoped>
/* 样式可以根据需要调整 */
</style>