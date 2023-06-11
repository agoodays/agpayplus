<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增码牌' : '修改码牌' "
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
    class="drawer-width">
    <a-form-model ref="infoFormModel" :model="saveObject" :label-col="{span: 6}" :wrapper-col="{span: 15}" :rules="rules">
      <a-form-model-item label="批次号：" prop="batchId">
        <a-input v-model="saveObject.batchId" :disabled="!isAdd" />
      </a-form-model-item>
      <a-form-model-item label="创建数量：" prop="addNum">
        <a-input v-model="saveObject.addNum" />
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
import { API_URL_QRC_LIST, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },

  data () {
    return {
      isAdd: true, // 新增 or 修改页面标志
      visible: false, // 是否显示弹层/抽屉
      btnLoading: false,
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      rules: {
        batchId: [
          { required: true, message: '请输入批次号', trigger: 'blur' }
        ],
        addNum: [
          { required: true, message: '请输入创建数量', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = {} // 数据清空

      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }

      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        that.recordId = recordId
        req.getById(API_URL_QRC_LIST, recordId).then(res => { that.saveObject = res })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
      }
    },
    onClose () {
      this.visible = false
    },
    handleOkFunc: function () { // 点击【确认】按钮事件
        const that = this
        this.$refs.infoFormModel.validate(valid => {
          if (valid) { // 验证通过
            // 请求接口

            if (that.isAdd) {
              req.add(API_URL_QRC_LIST, that.saveObject).then(res => {
                that.$message.success('新增成功')
                that.visible = false
                that.callbackFunc() // 刷新列表
              })
            } else {
              req.updateById(API_URL_QRC_LIST, that.recordId, that.saveObject).then(res => {
                that.$message.success('修改成功')
                that.visible = false
                that.callbackFunc() // 刷新列表
              })
            }
          }
        })
    }
  }
}
</script>
