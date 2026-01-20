<template>
  <a-row :gutter="[24,24]" class="ag-card-row">
    <!-- 卡片默认新增框 -->
    <a-col
        :xxl="24/span.xxl"
        :xl="24/span.xl"
        :lg="24/span.lg"
        :md="24/span.md"
        :sm="24/span.sm"
        :xs="24/span.xs"
        @click="$emit('addAgCard')"
        v-if="addAuthority"
    >
      <div class="ag-card-add" :style="{'height': height + 'px'}">
        <div class="ag-card-add-top">
          <img src="~@/assets/svg/add-icon.svg" alt="add-icon" class="ag-card-add-icon">
          <img src="~@/assets/svg/add-icon-hover.svg" alt="add-icon" class="ag-card-add-icon-hover">
        </div>
        <div class="ag-card-add-text">
          新建{{ name }}
        </div>
      </div>
    </a-col>
    <!-- 数据 -->
    <a-col
        v-for="(item, key) in cardList"
        :key="key"
        :xxl="24/span.xxl"
        :xl="24/span.xl"
        :lg="24/span.lg"
        :md="24/span.md"
        :sm="24/span.sm"
        :xs="24/span.xs"
    >
      <slot name="cardContentSlot" :record="item"></slot>
      <slot name="cardOpSlot" :record="item"></slot>
    </a-col>
  </a-row>
</template>

<script setup>
import { ref, onMounted } from 'vue';

const props = defineProps({
  span: { type: Object, default: () => ({ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }) },
  height: { type: Number, default: 200 },
  name: { type: String, default: '' },
  addAuthority: { type: Boolean, default: false },
  reqCardListFunc: { type: Function, default: () => () => ({}) }
});

const cardList = ref([]);

const refCardList = async () => {
  const resData = await props.reqCardListFunc();
  cardList.value = resData || [];
};

onMounted(refCardList);
</script>

.style-placeholder{}
<style lang="less" scoped>
.ag-card-row {
  width: 100%;
}
.ag-card-add {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  border: 2px dashed var(--border-dashed);
  background: var(--surface-variant);
  border-radius: var(--border-radius);
  box-sizing: border-box;
  cursor: pointer;
}
.ag-card-add-top {
  width: 80px;
  height: 80px;
  position: relative;
}
.ag-card-add:hover {
  border-color: var(--primary-color-hover);
  background: var(--primary-color-weak);
  transition: all 0.3s ease-in-out;
}
.ag-card-add:hover .ag-card-add-icon {
  opacity: 0;
  transition: all 0.2s ease-in-out;
}
.ag-card-add:hover .ag-card-add-icon-hover {
  opacity: 1;
  transition: all 0.5s ease-in-out;
}
.ag-card-add:hover .ag-card-add-text {
  color: var(--primary-color-hover);
  transition: all 0.3s ease-in-out;
}
.ag-card-add-icon {
  position: absolute;
  width: 80px;
  height: 80px;
  opacity: 1;
}
.ag-card-add-icon-hover {
  position: absolute;
  width: 80px;
  height: 80px;
  opacity: 0;
}
.ag-card-add-text {
  padding-top: 5px;
  font-size: 16px;
  color: var(--text-color-weak);
}
</style>
