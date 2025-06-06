<template>
  <a-drawer
    :visible="visible"
    :title=" isAdd ? '新增应用' : '修改应用'"
    :maskClosable="false"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%">

    <a-form-model ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10" v-if="!isAdd">
          <a-form-model-item label="应用 AppId" prop="appId">
            <a-input v-model="saveObject.appId" placeholder="请输入" :disabled="!isAdd" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="商户号" prop="mchNo">
            <ag-select
              v-model="saveObject.mchNo"
              :api="searchMch"
              valueField="mchNo"
              labelField="mchName"
              placeholder="商户号（搜索商户名称）"
              :disabled="!isAdd"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="应用名称" prop="appName">
            <a-input v-model="saveObject.appName" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" placeholder="请输入" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="状态" prop="state">
            <a-radio-group v-model="saveObject.state">
              <a-radio :value="1">
                启用
              </a-radio>
              <a-radio :value="0">
                停用
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="是否设置为默认应用" prop="defaultFlag">
            <a-radio-group v-model="saveObject.defaultFlag">
              <a-radio :value="0">
                否
              </a-radio>
              <a-radio :value="1">
                是
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
      </a-row>

      <!-- 签名配置板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left" style="color: rgb(26, 102, 255);">
            签名配置
            <!--<a-tag color="rgb(26, 102, 255)">签名配置</a-tag>-->
          </a-divider>
        </a-col>
      </a-row>
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="24">
            <a-form-model-item prop="appSignType">
              <template slot="label">
                <div>
                  <label title="支持的签名方式" style="margin-right: 4px">支持的签名方式</label>
                  <!-- 支持的签名方式 气泡弹窗 -->
                  <!-- title可省略，就不显示 -->
                  <a-popover placement="top">
                    <template slot="content">
                      <p>若需要使用系统测试或者商户通APP则必须支持MD5， 若仅通过API调用则根据需求进行选择。</p>
                    </template>
                    <template slot="title">
                      <span>签名方式</span>
                    </template>
                    <a-icon type="question-circle" />
                  </a-popover>
                </div>
              </template>
              <a-checkbox-group v-model="saveObject.appSignType" :options="appSignTypeOptions" />
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex" v-if="this.saveObject.appSignType?.includes('MD5')">
          <a-col :span="24">
            <a-form-model-item label="设置MD5秘钥" prop="appSecret" >
              <a-input v-model="saveObject.appSecret" :placeholder="saveObject.appSecret_ph" type="textarea" />
              <a-button type="primary" ghost @click="randomKey(false, 128, 0)"><a-icon type="file-sync" />随机生成私钥</a-button>
            </a-form-model-item>
          </a-col>
        </a-row>
        <a-row justify="space-between" type="flex" v-if="this.saveObject.appSignType?.includes('RSA2')">
          <a-col :span="24">
            <a-form-model-item label="设置RSA2应用公钥" prop="appRsa2PublicKey" >
              <a-input v-model="saveObject.appRsa2PublicKey" type="textarea" />
            </a-form-model-item>
          </a-col>
          <a-col :span="24">
            <a-form-model-item label="支付网关系统公钥（回调验签使用）" prop="sysRSA2PublicKey" >
              <a-input v-model="sysRSA2PublicKey" type="textarea" disabled="disabled" rows="6"/>
            </a-form-model-item>
          </a-col>
        </a-row>
      </div>
    </a-form-model>

    <div class="drawer-btn-center">
      <a-button @click="onClose" icon="close" :style="{ marginRight: '8px' }">取消</a-button>
      <a-button type="primary" @click="onSubmit" icon="check" >保存</a-button>
    </div>

  </a-drawer>
</template>

<script>
import AgSelect from '@/components/AgSelect/AgSelect'
import { API_URL_MCH_APP, API_URL_MCH_LIST, req, getSysRSA2PublicKey } from '@/api/manage'

export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  components: {
    AgSelect
  },
  data () {
    return {
      isAdd: true, // 新增 or 修改
      visible: false, // 抽屉开关
      appId: '', // 应用AppId
      saveObject: {}, // 数据对象
      appSignTypeOptions: [
        { label: 'MD5', value: 'MD5' },
        { label: 'RSA2', value: 'RSA2' }
      ],
      sysRSA2PublicKey: '',
      rules: {
        mchNo: [{ required: true, message: '请输入商户号', trigger: 'blur' }],
        appName: [{ required: true, message: '请输入应用名称', trigger: 'blur' }],
        appSecret: [{ required: true, message: '请输入MD5秘钥', trigger: 'blur' }],
        appRsa2PublicKey: [{ required: true, message: '请输入RSA2应用公钥', trigger: 'blur' }]
      }
    }
  },
  methods: {
    // 抽屉显示
    show (mchNo, appId) {
      this.isAdd = !appId
       // 数据清空
      this.saveObject = {
        state: 1,
        defaultFlag: 0,
        appSignType: ['MD5'],
        appSecret: '',
        mchNo: mchNo,
        appSecret_ph: '请输入'
      }

      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }

      this.rules.appSecret = [{ required: true, message: '请输入MD5秘钥', trigger: 'blur' }]

      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        that.appId = appId
        // 拉取详情
        req.getById(API_URL_MCH_APP, appId).then(res => {
          that.saveObject = res
          that.saveObject.appSecret_ph = res.appSecret
          that.saveObject.appSecret = ''
          const deleteAppSecretRules = !!that.saveObject.appSecret_ph
          if (deleteAppSecretRules) {
            delete that.rules.appSecret
          }
        })
      }

      this.visible = true

      getSysRSA2PublicKey().then(res => {
        that.sysRSA2PublicKey = res
      })
    },
    searchMch (params) {
      return req.list(API_URL_MCH_LIST, params)
    },
    // 表单提交
    onSubmit () {
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          delete that.saveObject.appSecret_ph
          // 请求接口
          if (that.isAdd) {
            req.add(API_URL_MCH_APP, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
            })
          } else {
            if (that.saveObject.appSecret === '') {
              delete that.saveObject.appSecret
            }
            req.updateById(API_URL_MCH_APP, that.appId, that.saveObject).then(res => {
              that.$message.success('修改成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
            })
          }
        }
      })
    },
    randomKey: function (randomFlag, min, max) { // 生成随机128位私钥
      let str = ''
      let range = min
      const arr = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z']
      // 随机产生
      if (randomFlag) {
        range = Math.round(Math.random() * (max - min)) + min
      }
      for (var i = 0; i < range; i++) {
        var pos = Math.round(Math.random() * (arr.length - 1))
        str += arr[ pos ]
      }
      this.saveObject.appSecret = str
    },
    onClose () {
      this.visible = false
    }
  }
}
</script>

<style scoped>
  ::v-deep(.ant-divider-inner-text) {
    color: rgb(26, 102, 255);
  }
</style>
