<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增团队' : '修改团队' "
    @close="onClose"
    class="drawer-width"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-form-model v-if="visible" ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="团队名称" prop="teamName">
            <a-input
              placeholder="请输入团队名称"
              v-model="saveObject.teamName"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="团队编号" prop="teamNo">
            <a-input
              placeholder="请输入团队编号"
              v-model="saveObject.teamNo"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="团队编号" prop="statRangeType">
            <a-select v-model="saveObject.statRangeType" placeholder="统计周期" default-value="year">
              <a-select-option value="year">年</a-select-option>
              <a-select-option value="quarter">季度</a-select-option>
              <a-select-option value="month">月</a-select-option>
              <a-select-option value="week">周</a-select-option>
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
import { API_URL_UR_TEAM_LIST, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },

  data () {
    const checkStatRangeType = (rule, value, callback) => { // 是否选择了统计周期
      if (this.isAdd && !value) {
        callback(new Error('请选择统计周期'))
      }
      callback()
    }
    return {
      btnLoading: false,
      isAdd: true, // 新增 or 修改页面标志
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      rules: {
        teamName: [{ required: true, message: '请输入团队名称', trigger: 'blur' }],
        teamNo: [{ required: true, message: '请输入团队编号', trigger: 'blur' }],
        statRangeType: [{ required: true, validator: checkStatRangeType, trigger: 'blur' }]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = { 'statRangeType': 'year' }
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        console.log(555)
        that.recordId = recordId
        req.getById(API_URL_UR_TEAM_LIST, recordId).then(res => {
          that.saveObject = res
        })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
      }
    },
    onSubmit: function () { // 点击【保存】按钮事件
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          // 请求接口
          if (that.isAdd) {
            this.btnLoading = true
            req.add(API_URL_UR_TEAM_LIST, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          } else {
            req.updateById(API_URL_UR_TEAM_LIST, that.recordId, that.saveObject).then(res => {
              that.$message.success('修改成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          }
        }
      })
    },
    onClose () {
      this.visible = false
    }
  }
}
</script>

<style lang="less">
  .upload-list-inline .ant-btn {
    height: 66px;
  }
</style>
