<template>
  <page-header-wrapper>
    <AgPayCard
      ref="infoCard"
      :reqCardListFunc="reqCardListFunc"
      :span="agpayCard.span"
      :height="agpayCard.height"
      :name="agpayCard.name"
      :addAuthority="agpayCard.addAuthority"
      @addAgPayCard="addOrEdit"
    >
      <div slot="cardContentSlot" slot-scope="{record}">
        <div :style="{'height': agpayCard.height + 'px'}" class="agpay-card-content">
          <!-- 卡片自定义样式 -->
          <div class="agpay-card-content-header" :style="{backgroundColor: record.bgColor, height: agpayCard.height/2 + 'px'}">
            <img v-if="record.icon" :src="record.icon" :style="{height: agpayCard.height/5 + 'px'}">
          </div>
          <div class="agpay-card-content-body" :style="{height: (agpayCard.height/2 - 50) + 'px'}">
            <div class="title">
              {{ record.ifName }}
            </div>
          </div>
          <!-- 卡片底部操作栏 -->
          <div class="agpay-card-ops">
            <a-tooltip placement="top" title="编辑">
              <a-icon key="edit" type="edit" @click="addOrEdit(record.ifCode)" />
            </a-tooltip>
            <a-tooltip placement="top" title="删除">
              <a-icon key="delete" type="delete" @click="del(record.ifCode)" />
            </a-tooltip>
          </div>
        </div>
      </div>
    </AgPayCard>

    <!-- 新增页面组件  -->
    <PayIfDefineAddOrEdit ref="payIfDefineAddOrEdit" :callbackFunc="refCardList"/>
  </page-header-wrapper>
</template>

<script>
import AgPayCard from '@/components/AgPayCard/AgPayCard'
import { API_URL_IFDEFINES_LIST, req } from '@/api/manage'
import PayIfDefineAddOrEdit from './AddOrEdit'

export default {
  name: 'IfDefinePage',
  components: {
    AgPayCard,
    PayIfDefineAddOrEdit
  },
  data () {
    return {
      agpayCard: {
        name: '支付接口',
        height: 300,
        span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
        addAuthority: this.$access('ENT_PC_IF_DEFINE_ADD')
      }
    }
  },
  methods: {
    // 请求支付接口定义数据
    reqCardListFunc () {
      return req.list(API_URL_IFDEFINES_LIST)
    },
    // 刷新card列表
    refCardList () {
      this.$refs.infoCard.refCardList()
    },
    addOrEdit (ifCode) {
      this.$refs.payIfDefineAddOrEdit.show(ifCode)
    },
    del (ifCode) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        req.delById(API_URL_IFDEFINES_LIST, ifCode).then(res => {
          that.$message.success('删除成功！')
          that.refCardList()
        })
      })
    }
  }
}
</script>

<style lang="less" scoped>
  .agpay-card-content {
    width: 100%;
    position: relative;
    background-color: @ag-card-back;
    border-radius: 6px;
    overflow:hidden;
  }
  .agpay-card-ops {
    width: 100%;
    height: 50px;
    background-color: @ag-card-back;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: center;
    border-top: 1px solid @ag-back;
    position: absolute;
    bottom: 0;
  }
  .agpay-card-content-header {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
  }
  .agpay-card-content-body {
    display: flex;
    flex-direction: column;
    justify-content: space-around;
    align-items: center;
  }
  .title {
    font-size: 16px;
    font-family: PingFang SC, PingFang SC-Bold;
    font-weight: 700;
    color: #1a1a1a;
    letter-spacing: 1px;
  }
</style>
