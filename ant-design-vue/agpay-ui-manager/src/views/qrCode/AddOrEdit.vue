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
    <a-form-model ref="infoFormModel" :model="saveObject" :label-col="{span: 6}" :wrapper-col="{span: 18}" :rules="rules">
      <a-form-model-item label="批次号：" prop="batchId">
        <a-row>
          <a-col span="14"><a-input-number v-model="saveObject.batchId" :disabled="!isAdd" style="width: 100%" /></a-col>
          <a-col span="6"><a-button type="primary" @click="onToday" size="small" style="margin-left: 20px">今天</a-button></a-col>
        </a-row>
        <div class="agpay-tip-text">
          <span>( 数字格式， 二维码编号的前缀， 建议采用： YYYYMMDD+次数表示 )</span>
        </div>
      </a-form-model-item>
      <a-form-model-item label="创建数量：" prop="addNum">
        <a-input-number v-model="saveObject.addNum" :min="1" :max="500" />
      </a-form-model-item>
      <a-form-model-item label="选择模板" prop="qrcShellId">
        <a-select v-model="saveObject.qrcShellId" placeholder="请选择模板">
          <a-select-option value="" key="">无</a-select-option>
          <a-select-option v-for="d in shellList" :value="d.id" :key="d.id">
            <span class="icon-style"><img class="icon" :src="d.shellImgViewUrl" alt=""></span> {{ d.shellAlias }}
          </a-select-option>
        </a-select>
      </a-form-model-item>
      <a-form-model-item label="状态" prop="state">
        <a-radio-group v-model="saveObject.state">
          <a-radio :value="1">
            启用
          </a-radio>
          <a-radio :value="0">
            禁用
          </a-radio>
        </a-radio-group>
      </a-form-model-item>
      <a-form-model-item label="固定金额" prop="fixedFlag">
        <a-radio-group v-model="saveObject.fixedFlag">
          <a-radio :value="0">
            任意金额
          </a-radio>
          <a-radio :value="1">
            固定金额
          </a-radio>
        </a-radio-group>
<!--        <a-input v-if="saveObject.fixedFlag===1" v-model="saveObject.fixedPayAmount" type="number" addon-after="元" style="width: 150px"/>-->
        <span v-if="saveObject.fixedFlag===1"><a-input-number v-model="saveObject.fixedPayAmount" addon-after="元"/>元</span>
      </a-form-model-item>
      <a-form-model-item prop="entryPage">
        <template slot="label">
          <span>
            <label title="选择页面类型" style="margin-right: 4px">选择页面类型</label>
            <!-- 选择页面类型 气泡弹窗 -->
            <!-- title可省略，就不显示 -->
            <a-popover placement="top">
              <template slot="content">
                <p>谨慎选择， 一经填写不可变更。</p>
              </template>
              <a-icon type="question-circle" />
            </a-popover>
          </span>
        </template>
        <a-radio-group v-model="saveObject.entryPage">
          <a-radio :value="'default'">
            默认
            <a-popover placement="top">
              <template slot="content">
                <p>未指定，取决于二维码是否绑定到微信侧</p>
              </template>
              <a-icon type="question-circle" />
            </a-popover>
          </a-radio>
          <a-radio :value="'h5'">
            固定H5页面
          </a-radio>
          <a-radio :value="'lite'">
            固定小程序页面
          </a-radio>
        </a-radio-group>
      </a-form-model-item>
      <a-form-model-item prop="alipayWayCode">
        <template slot="label">
          <span>
            <label title="支付宝支付方式" style="margin-right: 4px">支付宝支付方式</label>
            <!-- 支付宝支付方式 气泡弹窗 -->
            <!-- title可省略，就不显示 -->
            <a-popover placement="top">
              <template slot="content">
                <p>仅H5呈现时生效</p>
              </template>
              <a-icon type="question-circle" />
            </a-popover>
          </span>
        </template>
        <a-radio-group v-model="saveObject.alipayWayCode">
          <a-radio :value="'ALI_JSAPI'">
            ALI_JSAPI
          </a-radio>
          <a-radio :value="'ALI_WAP'">
            ALI_WAP
          </a-radio>
        </a-radio-group>
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
import { API_URL_QRC_SHELL_LIST, API_URL_QRC_LIST, req } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },

  data () {
    return {
      isAdd: true, // 新增 or 修改页面标志
      visible: false, // 是否显示弹层/抽屉
      btnLoading: false,
      shellList: null, // 代理商下拉列表
      recordId: null, // 更新对象ID
      saveObject: {}, // 数据对象
      rules: {
        batchId: [
          { required: true, message: '请输入批次号', trigger: 'blur' }
        ],
        addNum: [
          { required: true, message: '请输入创建数量', trigger: 'blur' },
          {
            validator: (rule, value, callBack) => {
              if (value < 1 || value > 500) {
                callBack('数量请介于1-500之间')
              }
              callBack()
            },
            trigger: 'blur'
          }
        ]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = {
        addNum: 1,
        state: 1,
        fixedFlag: 0,
        entryPage: 'default',
        alipayWayCode: 'ALI_JSAPI'
      } // 数据清空

      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }

      const that = this
      req.list(API_URL_QRC_SHELL_LIST, { 'pageSize': -1, 'state': 1 }).then(res => { // 模板下拉选择列表
        that.shellList = res.records
      })
      req.get(API_URL_QRC_LIST + '/batchIdDistinctCount').then(res => { // 模板下拉选择列表
        that.saveObject.batchId = res
      })
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
    onToday () {
      const today = new Date()
      const year = today.getFullYear().toString()// .substr(-2)
      const month = (today.getMonth() + 1).toString().padStart(2, '0')
      const day = today.getDate().toString().padStart(2, '0')
      this.saveObject.batchId = +`${year}${month}${day}00`
      console.log(this.saveObject.batchId)
      this.$forceUpdate()
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

<style lang="less">
  .icon-style {
    border-radius: 5px;
    padding-left: 2px;
    padding-right: 2px
  }

  .icon {
    width: 18.37px;
    height: 26px;
    margin-bottom: 3px
  }

  .agpay-tip-text:before {
    content: "";
    width: 0;
    height: 0;
    border: 10px solid transparent;
    border-bottom-color: #ffeed8;
    position: absolute;
    top: -20px;
    left: 30px;
  }
  .agpay-tip-text {
    font-size: 10px !important;
    border-radius: 5px;
    background: #ffeed8;
    color: #c57000 !important;
    padding: 5px 10px;
    display: inline-block;
    max-width: 100%;
    position: relative;
    margin-top: 15px;
  }
</style>
