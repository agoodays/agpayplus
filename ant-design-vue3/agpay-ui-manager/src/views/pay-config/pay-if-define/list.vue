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
              <icons.EditOutlined />
            </a-tooltip>
            <a-tooltip placement="top" title="删除">
              <icons.DeleteOutlined />
            </a-tooltip>
          </div>
        </div>
      </template>
    </ag-card>
    <!-- 新增页面组件  -->
    <add-or-edit v-model:open="modalOpen" :if-code="currentIfCode" @success="handleSuccess" />
  </div>
</template>

<script setup>
/**
 * 支付接口定义列表页面组件
 * 功能：展示支付接口定义卡片列表，支持新增、编辑、删除操作
 */
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue'
import { payConfigApi } from '@/api/business/pay-config/pay-config-api'
import { reactive, ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import { message } from 'ant-design-vue'

const icons = { DeleteOutlined, EditOutlined }

/**
 * 卡片组件引用
 */
const infoCard = ref(null)

/**
 * 弹窗状态
 */
const modalOpen = ref(false)
const currentIfCode = ref('')

/**
 * 卡片配置
 */
const agpayCard = reactive({
  name: '支付接口',
  height: 300,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
  addAuthority: true
})

/**
 * 请求支付接口定义数据
 * @returns {Promise<Object>} 支付接口定义列表
 */
const reqCardListFunc = async () => {
  return await payConfigApi.queryIfDefineList()
}

/**
 * 刷新卡片列表
 */
const refCardList = () => {
  infoCard.value?.refCardList()
}

/**
 * 新增或编辑支付接口定义
 * @param {string} ifCode - 接口编码
 */
const addOrEdit = (ifCode) => {
  currentIfCode.value = ifCode || ''
  modalOpen.value = true
}

/**
 * 操作成功回调
 */
const handleSuccess = () => {
  refCardList()
}

/**
 * 删除支付接口定义
 * @param {string} ifCode - 接口编码
 */
const del = async (ifCode) => {
  const { infoBox } = await import('@/utils/info-box')
  infoBox.confirmDanger('确认删除？', '', async () => {
    try {
      await payConfigApi.delIfDefineById(ifCode)
      message.success('删除成功！')
      refCardList()
    } catch (error) {
      console.error('删除支付接口定义失败:', error)
    }
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