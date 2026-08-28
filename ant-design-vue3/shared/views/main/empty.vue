<template>
  <div class="empty">
    <a-empty description="暂无数据" />
  </div>
</template>

<script setup></script>

<style scoped lang="less">
.empty {
  position: absolute;
  top: 0;
  left: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  width: 100%;
  z-index: 1;
  pointer-events: none;
}
</style>
