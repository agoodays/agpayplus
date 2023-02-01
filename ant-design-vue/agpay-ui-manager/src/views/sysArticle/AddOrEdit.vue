<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增公告' : '修改公告' "
    @close="onClose"
    :body-style="{ paddingBottom: '80px' }"
    width="60%"
    class="drawer-width"
  >
    <a-form-model v-if="visible" ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="公告标题" prop="title">
            <a-input
                placeholder="请输入公告标题"
                v-model="saveObject.title"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="公告副标题" prop="subtitle">
            <a-input
                placeholder="请输入公告副标题"
                v-model="saveObject.subtitle"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="公告范围" prop="articleRange">
            <a-checkbox-group v-model="saveObject.articleRange" :options="articleRangeOptions" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="发布人" prop="publisher">
            <a-input
                placeholder="请输入发布人"
                v-model="saveObject.publisher"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="24">
          <a-form-model-item label="公告内容" prop="content">
<!--            <AgEditor :height="438" v-model="saveObject.content"></AgEditor>-->
            <AgEditor :height="438" :modelValue="saveObject.content" @update:modelValue="saveObject.content = $event"></AgEditor>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <div class="drawer-btn-center" >
      <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose" style="margin-right:8px">
        取消
      </a-button>
      <a-button type="primary" icon="check-circle" @click="onSubmit" :loading="btnLoading">
        保存
      </a-button>
    </div>
  </a-drawer>

</template>

<script>
import AgEditor from '@/components/AgEditor/AgEditor'
import { API_URL_ARTICLE_LIST, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function }
  },
  components: { AgEditor },
  data () {
    const checkArticleRange = (rule, value, callback) => { // 是否选择了公告范围
      if (!value.length) {
        callback(new Error('请选择公告范围'))
      }
      callback()
    }
    return {
      btnLoading: false,
      isAdd: true, // 新增 or 修改页面标志
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      articleRangeOptions: [
        { label: '商户', value: 'MCH' },
        { label: '代理商', value: 'AGENT' }
      ],
      rules: {
        title: [{ required: true, message: '请输入公告标题', trigger: 'blur' }],
        subtitle: [{ required: true, message: '请输入公告副标题', trigger: 'blur' }],
        publisher: [{ required: true, message: '请填写发布人', trigger: 'blur' }],
        articleRange: [{ required: true, validator: checkArticleRange, trigger: 'blur' }]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = { }
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        console.log(555)
        that.recordId = recordId
        req.getById(API_URL_ARTICLE_LIST, recordId).then(res => {
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
            req.add(API_URL_ARTICLE_LIST, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          } else {
            req.updateById(API_URL_ARTICLE_LIST, that.recordId, that.saveObject).then(res => {
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
