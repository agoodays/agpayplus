<template>
  <div>
    <ag-card
      ref="infoCard"
      :req-card-list-func="reqCardListFunc"
      :span="agpayCard.span"
      :height="agpayCard.height"
      :name="agpayCard.name"
      :add-authority="agpayCard.addAuthority"
      @add-ag-card="addOrEdit"
    >
      <template #cardContentSlot="{record}">
        <div :style="{'height': agpayCard.height + 'px'}" class="ag-card-content">
          <!-- 卡片自定义样式 -->
          <div class="ag-card-content-header" :style="{backgroundColor: record.bgColor, height: agpayCard.height/2 + 'px'}">
            <img v-if="record.icon" :src="record.icon" :style="{height: agpayCard.height/5 + 'px'}">
          </div>
          <div class="ag-card-content-body" :style="{height: (agpayCard.height/2 - 50) + 'px'}">
            <div class="title">
              {{ record.ifName }}
            </div>
          </div>
          <!-- 卡片底部操作栏 -->
          <div class="ag-card-ops">
            <a-tooltip placement="top" title="编辑">
              <a-icon key="edit" type="edit" @click="addOrEdit(record.ifCode)" />
            </a-tooltip>
            <a-tooltip placement="top" title="删除">
              <a-icon key="delete" type="delete" @click="del(record.ifCode)" />
            </a-tooltip>
          </div>
        </div>
      </template>
    </ag-card>
    <!-- 新增页面组件  -->
    <pay-if-define-add-or-edit ref="payIfDefineAddOrEdit" :callback-func="refCardList"/>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { API_URL_IFDEFINES_LIST, req } from '@/api/manage'
import PayIfDefineAddOrEdit from './add-or-edit.vue'

const infoCard = ref(null)
const payIfDefineAddOrEdit = ref(null)

const agpayCard = reactive({
  name: '支付接口',
  height: 300,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
  addAuthority: true // 暂时设置为 true，后续根据权限系统调整
})

// 请求支付接口定义数据
const reqCardListFunc = () => {
  return req.list(API_URL_IFDEFINES_LIST)
}

// 刷新card列表
const refCardList = () => {
  infoCard.value?.refCardList()
}

const addOrEdit = (ifCode) => {
  payIfDefineAddOrEdit.value.show(ifCode)
}

const del = (ifCode) => {
  import('ant-design-vue').then(({ message }) => {
    import('@/utils/info-box').then(({ infoBox }) => {
      infoBox.confirmDanger('确认删除？', '', () => {
        return req.delById(API_URL_IFDEFINES_LIST, ifCode).then(res => {
          message.success('删除成功！')
          refCardList()
        })
      })
    })
  })
}
</script>

<style lang="less" scoped>
  .ag-card-content {
    width: 100%;
    position: relative;
    background-color: #f5f5f5;
    border-radius: 6px;
    overflow:hidden;
  }
  .ag-card-ops {
    width: 100%;
    height: 50px;
    background-color: #f5f5f5;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: center;
    border-top: 1px solid #e8e8e8;
    position: absolute;
    bottom: 0;
  }
  .ag-card-content-header {
    width: 100%;
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
  }
  .ag-card-content-body {
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