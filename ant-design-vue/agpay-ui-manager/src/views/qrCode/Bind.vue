<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title="'绑定码牌'"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    class="drawer-width">
    <a-form-model ref="infoFormModel" :model="saveObject" :label-col="{span: 6}" :wrapper-col="{span: 18}" :rules="rules">
      <a-form-model-item label="商户号" prop="mchNo">
        <a-select v-model="saveObject.mchNo" placeholder="请选择商户" @change="mchNoChange">
          <a-select-option value="" key="">请选择商户</a-select-option>
          <a-select-option v-for="d in mchList" :value="d.mchNo" :key="d.mchNo">
            {{ d.mchName + " [ ID: " + d.mchNo + " ]" }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
      <a-form-model-item label="应用" prop="appId">
        <a-select v-model="saveObject.appId" placeholder="请选择应用">
          <a-select-option value="" key="">请选择应用</a-select-option>
          <a-select-option v-for="d in appList" :value="d.appId" :key="d.appId">
            {{ d.appName + " [ AppId: " + d.appId + " ]" }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
      <a-form-model-item label="门店" prop="storeId">
        <a-select v-model="saveObject.storeId" placeholder="请选择门店">
          <a-select-option value="" key="">请选择门店</a-select-option>
          <a-select-option v-for="d in storeList" :value="d.storeId" :key="d.storeId">
            {{ d.storeName + " [ ID: " + d.storeId + " ]" }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
    </a-form-model>
    <div class="drawer-btn-center" >
      <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose" style="margin-right:8px">
        取消
      </a-button>
      <a-button type="primary" icon="check" @click="handleOkFunc" :loading="btnLoading">
        保存
      </a-button>
    </div>
  </a-drawer>
</template>
<script>
import { API_URL_QRC_LIST, API_URL_MCH_LIST, API_URL_MCH_APP, API_URL_MCH_STORE, req } from '@/api/manage'

export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      btnLoading: false,
      recordId: null, // 更新对象ID
      saveObject: {}, // 数据对象
      mchList: null, // 商户下拉列表
      appList: null, // 应用下拉列表
      storeList: null, // 门店下拉列表
      rules: {
        mchNo: [{ required: true, message: '请选择商户', trigger: 'blur' }],
        appId: [{ required: true, message: '请选择应用', trigger: 'blur' }],
        storeId: [{ required: true, message: '请选择门店', trigger: 'blur' }]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      const that = this
      that.recordId = recordId
      req.getById(API_URL_QRC_LIST, recordId).then(res => {
        that.saveObject = res
        if (res.mchNo) {
          that.mchNoChange()
        }
      })

      req.list(API_URL_MCH_LIST, { pageSize: -1, state: 1 }).then(res => { // 下拉选择列表
        that.mchList = res.records
      })
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    mchNoChange () {
      const that = this
      if (that.saveObject.mchNo) {
        req.list(API_URL_MCH_APP, { mchNo: that.saveObject.mchNo, pageSize: -1, state: 1 }).then(res => { // 下拉选择列表
          that.appList = res.records
        })
        req.list(API_URL_MCH_STORE, { mchNo: that.saveObject.mchNo, pageSize: -1, state: 1 }).then(res => { // 下拉选择列表
          that.storeList = res.records
        })
      } else {
        that.appList = null
        that.storeList = null
        that.saveObject.appId = null
        that.saveObject.storeId = null
      }
    },
    handleOkFunc: function () { // 点击【确认】按钮事件
        const that = this
        this.$refs.infoFormModel.validate(valid => {
          if (valid) { // 验证通过
            // 请求接口
            req.updateById(API_URL_QRC_LIST + '/bind', that.recordId, that.saveObject).then(res => {
              that.$message.success('绑定成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
            })
          }
        })
    }
  }
}
</script>

<style lang="less">
</style>
