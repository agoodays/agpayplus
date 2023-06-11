<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增模板' : '修改模板' "
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    class="drawer-width">
    <a-row>
      <a-col span="14">
        <a-form-model ref="infoFormModel" :model="saveObject" :label-col="{span: 6}" :wrapper-col="{span: 15}" :rules="rules">
          <a-form-model-item label="模板别名：" prop="shellAlias">
            <a-input v-model="saveObject.shellAlias"/>
          </a-form-model-item>
          <a-form-model-item label="选择渲染模板：" prop="styleCode">
            <a-radio-group v-model="saveObject.styleCode" size="small" button-style="solid">
              <a-radio-button value="shellA">模板A</a-radio-button>
              <a-radio-button value="shellB">模板B</a-radio-button>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="显示ID：" prop="showIdFlag">
            <a-radio-group v-model="saveObject.configInfo.showIdFlag" size="small" button-style="solid">
              <a-radio-button :value="true">显示</a-radio-button>
              <a-radio-button :value="false">隐藏</a-radio-button>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="支付方式：" prop="payType">
            <a-radio-group :options="payTypeOptions" v-for="item in saveObject.configInfo.payTypeList" :key="item.Key" v-model="item.name" />
          </a-form-model-item>
          <a-form-model-item label="背景颜色：" prop="bgColor">
            <a-radio-group v-model="saveObject.bgColor">
              <a-radio :value="'#1a53ff'" style="color: #1a53ff">蓝色</a-radio>
              <a-radio :value="'#ff0000'" style="color: #ff0000">红色</a-radio>
              <a-radio :value="'#09bb07'" style="color: #09bb07">绿色</a-radio>
              <a-radio :value="'custom'" :style="{ color:saveObject.customBgColor }">
                自定义 <colorPicker v-if="saveObject.bgColor === 'custom'" v-model="saveObject.customBgColor" style="margin-top: 12px;"/>
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="主logo：" prop="logoImgUrl">
            <AgUpload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="logoImgUrl"
              :urls="[saveObject.configInfo.logoImgUrl]"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
          <a-form-model-item label="二维码上的logo：" prop="qrInnerImgUrl">
            <AgUpload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="qrInnerImgUrl"
              :urls="[saveObject.configInfo.qrInnerImgUrl]"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-form-model>
      </a-col>
      <a-col span="10">
        <div style="display: flex; justify-content: center;">
          <div>
            <img src="https://localhost:9817/api/qrc/shell/stylea.png" style="max-width: 400px; border: 1px solid darkgrey;">
          </div>
        </div>
      </a-col>
    </a-row>
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
import AgUpload from '@/components/AgUpload/AgUpload'
import { API_URL_QRC_SHELL_LIST, req, upload } from '@/api/manage'

const payTypeOptions = [
  { value: 'wechat', label: '微信' },
  { value: 'alipay', label: '支付宝' },
  { value: 'ysfpay', label: '云闪付' },
  { value: 'unionpay', label: '银联' },
  { value: 'custom', label: '自定义' }
]
export default {
  components: {
    AgUpload
  },
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      isAdd: true, // 新增 or 修改页面标志
      visible: false, // 是否显示弹层/抽屉
      btnLoading: false,
      payTypeOptions: payTypeOptions,
      action: upload.form, // 上传图标地址
      saveObject: {
        styleCode: 'shellA',
        configInfo: {
          showIdFlag: true,
          payTypeList: [
            { imgUrl: '', name: 'wechat', alias: '微信' },
            { imgUrl: '', name: 'alipay', alias: '支付宝' },
            { imgUrl: '', name: 'ysfpay', alias: '云闪付' },
            { imgUrl: '', name: 'unionpay', alias: '银联' }
          ]
        }
      }, // 数据对象
      recordId: null, // 更新对象ID
      rules: {
        shellAlias: [
          { required: true, message: '请输入模板别名', trigger: 'blur' }
        ],
        styleCode: [
          { required: true, message: '请输入选择渲染模板', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = {
        styleCode: 'shellA',
        configInfo: {
          showIdFlag: true,
          payTypeList: [
            { imgUrl: '', name: 'wechat', alias: '微信' },
            { imgUrl: '', name: 'alipay', alias: '支付宝' },
            { imgUrl: '', name: 'ysfpay', alias: '云闪付' },
            { imgUrl: '', name: 'unionpay', alias: '银联' }
          ]
        }
      } // 数据初始化
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }

      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        that.recordId = recordId
        req.getById(API_URL_QRC_SHELL_LIST, recordId).then(res => { that.saveObject = res })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
      }
    },
    onClose () {
      this.visible = false
    },
    // 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
    uploadSuccess (name, fileList) {
      const [firstItem] = fileList
      this.saveObject.configInfo[name] = firstItem?.url
      this.$forceUpdate()
    },
    handleOkFunc: function () { // 点击【确认】按钮事件
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          // 请求接口
          if (that.isAdd) {
            req.add(API_URL_QRC_SHELL_LIST, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
            })
          } else {
            req.updateById(API_URL_QRC_SHELL_LIST, that.recordId, that.saveObject).then(res => {
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

<style lang="less">
  .ag-upload-btn {
    height: 66px;
  }
</style>
