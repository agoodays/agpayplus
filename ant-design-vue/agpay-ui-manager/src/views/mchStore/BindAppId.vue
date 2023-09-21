<template>
  <a-drawer
    :visible="visible"
    :title=" true ? '应用分配' : '' "
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-form-model :model="saveObject" ref="infoFormModel" layout="vertical">
      <a-row justify="space-between" type="flex">
        <a-col :span="16">
          <a-form-model-item label="请选择要绑定的应用" prop="bindAppId">
            <a-select v-model="saveObject.bindAppId">
              <a-select-option key="">(空)</a-select-option>
              <a-select-option v-for="(item) in mchAppList" :key='item.appId'>{{ item.appName }} [{{ item.appId }}]</a-select-option>
            </a-select>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <div class="drawer-btn-center" >
      <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose" style="margin-right:8px">
        取消
      </a-button>
      <a-button type="primary" icon="check" @click="onSubmit" :loading="btnLoading">
        保存
      </a-button>
    </div>
  </a-drawer>
</template>

<script>
import { API_URL_MCH_APP, API_URL_MCH_STORE, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      mchAppList: [], // app列表
      saveObject: {} // 数据对象
    }
  },
  mounted () {
    req.list(API_URL_MCH_APP, { pageSize: -1 }).then(res => {
      this.mchAppList = res.records
    })
  },
  methods: {
    show: function (recordId, bindAppId) { // 弹层打开事件
      this.recordId = recordId
      this.saveObject = {
        bindAppId: bindAppId || ''
      }
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    onSubmit: function () { // 点击【保存】按钮事件
      const that = this
      that.saveObject.bindAppId = that.saveObject.bindAppId || null
      that.btnLoading = true
      this.$infoBox.confirmPrimary('确认保存应用吗？', '', () => {
        // 请求接口
        req.updateById(API_URL_MCH_STORE, that.recordId, that.saveObject).then(res => {
          that.$message.success('绑定应用成功')
          that.visible = false
          that.callbackFunc() // 刷新列表
          that.btnLoading = false
        }).catch(res => {
          that.btnLoading = false
        })
      })
    }
  }
}
</script>
