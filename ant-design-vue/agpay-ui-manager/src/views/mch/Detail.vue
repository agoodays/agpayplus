<template>
  <a-drawer
    :visible="visible"
    :title=" true ? '商户详情' : '' "
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-row justify="space-between" type="flex">
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="商户号">
            {{ detailData.mchNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="商户名称">
            {{ detailData.mchName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="登录名">
            {{ detailData.loginUsername }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="商户简称">
            {{ detailData.mchShortName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12" v-if="detailData.type === 2">
        <a-descriptions>
          <a-descriptions-item label="服务商号">
            {{ detailData.isvNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12" v-if="detailData.type === 2">
        <a-descriptions>
          <a-descriptions-item label="服务商名称">
            {{ detailData.isvName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12" v-if="detailData.type === 2">
        <a-descriptions>
          <a-descriptions-item label="代理商号">
            {{ detailData.agentNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12" v-if="detailData.type === 2">
        <a-descriptions>
          <a-descriptions-item label="代理商名称">
            {{ detailData.agentName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="商户类型">
            <a-tag :color="detailData.type === 1 ? 'green' : 'orange'">
              {{ detailData.type === 1 ? '普通商户': '特约商户' }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="联系人姓名">
            {{ detailData.contactName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="联系人手机号">
            {{ detailData.contactTel }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="联系人邮箱">
            {{ detailData.contactEmail }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="状态">
            <a-tag :color="detailData.state === 1?'green':'volcano'">
              {{ detailData.state === 0?'禁用':detailData.state === 1?'启用':'未知' }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
    </a-row>
    <a-row justify="start" type="flex">
      <a-col :sm="24">
        <a-form-model-item label="备注">
          <a-input
            type="textarea"
            disabled="disabled"
            style="height: 50px"
            v-model="detailData.remark"
          />
        </a-form-model-item>
      </a-col>
    </a-row>
  </a-drawer>
</template>

<script>
import { API_URL_MCH_LIST, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      detailData: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      isvList: null // 服务商下拉列表
    }
  },
  created () {
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.detailData = { 'state': 1, 'type': 1 } // 数据清空
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      that.recordId = recordId
      req.getById(API_URL_MCH_LIST, recordId).then(res => {
        that.detailData = res
      })
      this.visible = true
    },
    onClose () {
      this.visible = false
    }
  }
}
</script>
