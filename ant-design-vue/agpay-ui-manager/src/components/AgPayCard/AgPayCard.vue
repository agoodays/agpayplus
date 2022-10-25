<template>
  <div>
    <a-row :gutter="[24,24]" style="width:100%">
      <!-- 卡片默认新增框 -->
      <a-col
        :xxl="24/span.xxl"
        :xl="24/span.xl"
        :lg="24/span.lg"
        :md="24/span.md"
        :sm="24/span.sm"
        :xs="24/span.xs"
        @click="$emit('addAgPayCard')"
        v-if="addAuthority"
      >
        <div class="agpay-card-add" :style="{'height': height + 'px'}">
          <div class="agpay-card-add-top">
            <img src="~@/assets/svg/add-icon.svg" alt="add-icon" class="agpay-card-add-icon">
            <img src="~@/assets/svg/add-icon-hover.svg" alt="add-icon" class="agpay-card-add-icon-hover">
          </div>
          <div class="agpay-card-add-text">
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
  </div>
</template>

<script>
export default {
  name: 'AgPayCard',
  props: {
    span: { type: Object, default: () => ({ xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 }) },
    height: { type: Number, default: 200 },
    name: { type: String, default: '' },
    addAuthority: { type: Boolean, default: false },
    reqCardListFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      cardList: []
    }
  },
  created () {
    this.refCardList()
  },
  methods: {
    refCardList () {
      const that = this
      this.reqCardListFunc().then(resData => {
        that.cardList = resData
      })
    }
  }
}
</script>

<style lang="less" scoped>
  .agpay-card-add {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    border: 2px dashed rgba(0, 0, 0, 0.15);
    background: rgba(0, 0, 0, 0.03);
    border-radius: 6px;
    box-sizing: border-box;
    cursor: pointer;
  }
  .agpay-card-add-top {
    width: 80px;
    height: 80px;
    position: relative;
  }
  .agpay-card-add:hover {
    border-color: rgba(25,83,255,0.3);
    background: rgba(25,83,255,0.06);
    transition: all 0.3s ease-in-out;
  }
   .agpay-card-add:hover .agpay-card-add-icon {
     opacity: 0;
     transition: all 0.2s ease-in-out;
   }
   .agpay-card-add:hover .agpay-card-add-icon-hover {
     opacity: 1;
     transition: all 0.5s ease-in-out;
   }
   .agpay-card-add:hover .agpay-card-add-text {
     color: rgba(25,83,255,1);
     transition: all 0.3s ease-in-out;
   }
   .agpay-card-add-icon {
     position: absolute;
     width: 80px;
     height: 80px;
     opacity: 1;
   }
   .agpay-card-add-icon-hover {
     position: absolute;
     width: 80px;
     height: 80px;
     opacity: 0;
   }
  .agpay-card-add-text {
    padding-top: 5px;
    font-size: 16px;
    color: rgba(0, 0, 0, 0.35);
  }
</style>
