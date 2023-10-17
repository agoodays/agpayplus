<!-- 详情抽屉 -->
<template>
  <a-drawer
    width="40%"
    :closable="true"
    :visible="visible"
    title="详情"
    @close="visible = false"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
  >
    <a-divider orientation="left" style="color: rgb(26, 102, 255);">基本信息</a-divider>
    <a-row justify="space-between" type="flex">
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="服务商号">{{ detailData.isvNo }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="商户号">{{ detailData.mchNo }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="接收方绑定ID">{{ detailData.receiverId }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="应用APPID">{{ detailData.appId }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="分账状态">
        <a-tag v-if="detailData.state === 0" :key="detailData.state" color="orange">暂停分账</a-tag>
        <a-tag v-if="detailData.state === 1" :key="detailData.state" color="blue">正常分账</a-tag>
      </a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="支付接口">{{ detailData.ifCode }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="分账比例">{{ (detailData.divisionProfit * 100).toFixed(2) + '%' }}</a-descriptions-item></a-descriptions></a-col>
    </a-row>
    <a-divider orientation="left" style="color: rgb(26, 102, 255);">账户信息</a-divider>
    <a-row justify="space-between" type="flex">
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="接收方账号别名">{{ detailData.receiverAlias }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="账户类型">{{ detailData.accType === 0 ? '个人' : '商户' }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="分账组ID">{{ detailData.receiverGroupId }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="分账组名称">{{ detailData.receiverGroupName }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="分账接收账号名称">{{ detailData.accName }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="12"><a-descriptions><a-descriptions-item label="分账接收账号">{{ detailData.accNo }}</a-descriptions-item></a-descriptions></a-col>
    </a-row>
    <a-divider orientation="left" style="color: rgb(26, 102, 255);">账户参数信息</a-divider>
    <a-row justify="space-between" type="flex">
      <a-col :sm="24"><a-descriptions><a-descriptions-item label="渠道账号参数">{{ detailData.channelAccNo }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="24"><a-descriptions><a-descriptions-item label="绑定响应参数">{{ detailData.channelBindResult }}</a-descriptions-item></a-descriptions></a-col>
      <a-col :sm="24"><a-descriptions><a-descriptions-item label="创建时间">{{ detailData.createdAt }}</a-descriptions-item></a-descriptions></a-col>
    </a-row>
  </a-drawer>
</template>
<script>
import { API_URL_DIVISION_RECEIVER, req } from '@/api/manage'

export default {
  data () {
    return {
      visible: false,
      detailData: {}
    }
  },
  methods: {
    show: function (recordId) {
      const that = this
      req.getById(API_URL_DIVISION_RECEIVER, recordId).then(res => {
        that.detailData = res
      })
      this.visible = true
    }
  }
}
</script>
