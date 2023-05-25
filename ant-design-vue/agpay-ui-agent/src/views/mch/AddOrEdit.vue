<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title="isAdd ? '新增商户' : '修改商户'"
    @close="onClose"
    class="drawer-width"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%">
    <a-form-model v-if="visible" ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="商户名称" prop="mchName">
            <a-input
              placeholder="请输入商户名称"
              v-model="saveObject.mchName"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="登录名" prop="loginUsername">
            <a-input
              placeholder="请输入商户登录名"
              v-model="saveObject.loginUsername"
              :disabled="!this.isAdd"
            />
          </a-form-model-item>
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="商户简称" prop="mchShortName">
            <a-input
              placeholder="请输入商户简称"
              v-model="saveObject.mchShortName"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人姓名" prop="contactName">
            <a-input
              placeholder="请输入联系人姓名"
              v-model="saveObject.contactName"
            />
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="联系人邮箱" prop="contactEmail">
            <a-input
              placeholder="请输入联系人邮箱"
              v-model="saveObject.contactEmail"
            >
            </a-input>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人手机号" prop="contactTel">
            <a-input
              placeholder="请输入联系人手机号"
              v-model="saveObject.contactTel"
            >
            </a-input>
            <p class="agpay-tip-text">(同步更改登录手机号)</p>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10" style="position:relative">
          <a-form-model-item prop="mchLevel">
            <template slot="label">
              <div>
                <label title="商户级别" style="margin-right: 4px">商户级别</label>
                <!-- 商户级别 气泡弹窗 -->
                <a-popover placement="top">
                  <template slot="content">
                    <p>M0商户：简单模式（页面简洁，仅基础收款功能）</p>
                    <p>M1商户：高级模式（支持api调用， 支持配置应用及分账、转账功能）</p>
                  </template>
                  <template slot="title">
                    <span>商户级别</span>
                  </template>
                  <a-icon type="question-circle" />
                </a-popover>
              </div>
            </template>
            <!-- 商户级别 气泡弹窗 -->
            <a-radio-group v-model="saveObject.mchLevel">
              <a-radio value="M0">
                M0
              </a-radio>
              <a-radio value="M1">
                M1
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
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
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" placeholder="请输入备注" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>

      <!-- 账户安全板块 -->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left">
            <a-tag color="#FF4B33">
              账户安全
            </a-tag>
          </a-divider>
        </a-col>
      </a-row>

      <div>
        <a-row justify="space-between" type="flex" v-if="this.isAdd">
          <a-col :span="10">
            <a-form-model-item label="是否发送开通提醒" prop="isNotify">
              <a-radio-group v-model="saveObject.isNotify">
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
        <a-row justify="space-between" type="flex" v-if="this.isAdd">
          <a-col :span="10">
            <a-form-model-item label="密码设置" prop="passwordType">
              <a-radio-group v-model="saveObject.passwordType">
                <a-radio value="default">
                  默认密码
                </a-radio>
                <a-radio value="custom">
                  自定义密码
                </a-radio>
              </a-radio-group>
            </a-form-model-item>
          </a-col>
          <a-col :span="10" v-if="saveObject.passwordType === 'custom'">
            <a-form-model-item label="登录密码" prop="loginPassword">
              <a-input placeholder="请输入登录密码" v-model="saveObject.loginPassword"/>
            </a-form-model-item>
            <a-button icon="file-sync" :style="{ marginRight: '8px', color: '#4278ff', borderColor: '#4278ff' }" @click="genRandomPassword">
              随机生成密码
            </a-button>
          </a-col>
        </a-row>
      </div>

      <!-- 重置密码板块 -->
      <div>
        <a-row justify="space-between" type="flex">
          <a-col :span="10">
            <a-form-model-item label="" v-if="resetIsShow" >
              重置密码：<a-checkbox v-model="sysPassword.resetPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
          <a-col :span="10">
            <a-form-model-item label="" v-if="sysPassword.resetPass">
              恢复默认密码：<a-checkbox v-model="sysPassword.defaultPass" @click="isResetPass"></a-checkbox>
            </a-form-model-item>
          </a-col>
        </a-row>

        <a-row justify="space-between" type="flex" v-if="sysPassword.resetPass && !this.sysPassword.defaultPass">
          <a-col :span="10">
            <a-form-model-item label="新密码：" prop="newPwd">
              <a-input-password autocomplete="new-password" v-model="newPwd" :disabled="sysPassword.defaultPass"/>
            </a-form-model-item>
          </a-col>

          <a-col :span="10">
            <a-form-model-item label="确认新密码：" prop="confirmPwd">
              <a-input-password autocomplete="new-password" v-model="sysPassword.confirmPwd" :disabled="sysPassword.defaultPass"/>
            </a-form-model-item>
          </a-col>
        </a-row>
      </div>

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
import { API_URL_MCH_LIST, req } from '@/api/manage'
import { Base64 } from 'js-base64'
export default {

  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },

  data () {
    const checkIsvNo = (rule, value, callback) => { // 校验类型为特约商户是否选择了服务商
      if (this.saveObject.type === 2 && !value) {
        callback(new Error('请选择服务商'))
      }
      callback()
    }
    return {
      passwordLength: 6, // 密码长度
      includeUpperCase: true, // 包含大写字母
      includeNumber: false, // 包含数字
      includeSymbol: false, // 包含符号
      newPwd: '', //  新密码
      resetIsShow: false, // 重置密码是否展现
      sysPassword: {
        resetPass: false, // 重置密码
        defaultPass: true, // 使用默认密码
        confirmPwd: '' //  确认密码
      },
      btnLoading: false,
      isAdd: true, // 新增 or 修改页面标志
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      rules: {
        mchName: [{ required: true, message: '请输入商户名称', trigger: 'blur' }],
        loginUsername: [{ required: true, pattern: /^[a-zA-Z][a-zA-Z0-9]{5,17}$/, message: '请输入字母开头，长度为6-18位的登录名', trigger: 'blur' }],
        loginPassword: [{ required: false, trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            if (this.saveObject.passwordType === 'custom') {
              if (this.saveObject.loginPassword.length < 6 || this.saveObject.loginPassword.length > 12) {
                callBack('请输入6-12位密码')
              }
            }
            callBack()
          }
        }], // 登录密码
        mchShortName: [{ required: true, message: '请输入商户简称', trigger: 'blur' }],
        contactName: [{ required: true, message: '请输入联系人姓名', trigger: 'blur' }],
        isvNo: [{ validator: checkIsvNo, trigger: 'blur' }],
        contactEmail: [{ required: false, pattern: /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/, message: '请输入正确的邮箱地址', trigger: 'blur' }],
        contactTel: [{ required: true, pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }],
        newPwd: [{ required: false, trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            if (!this.sysPassword.defaultPass) {
              if (this.newPwd.length < 6 || this.newPwd.length > 12) {
                callBack('请输入6-12位新密码')
              }
            }
            callBack()
          }
        }], // 新密码
        confirmPwd: [{ required: false, trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            if (!this.sysPassword.defaultPass) {
              this.newPwd === this.sysPassword.confirmPwd ? callBack() : callBack('新密码与确认密码不一致')
            } else {
              callBack()
            }
          }
        }] // 确认新密码
      }
    }
  },
  created () {
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = { 'state': 1, 'type': 1, 'mchLevel': 'M0', 'refundMode': ['plat', 'api'], 'isNotify': 0, 'passwordType': 'default', 'loginPassword': '' } // 数据清空
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        console.log(555)
        that.resetIsShow = true // 展示重置密码板块
        that.recordId = recordId
        req.getById(API_URL_MCH_LIST, recordId).then(res => {
          that.saveObject = res
        })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
      }
    },
    // 随机生成六位数密码
    genRandomPassword: function () {
      if (!this.passwordLength) return

      let password = ''
      let characters = 'abcdefghijklmnopqrstuvwxyz'
      if (this.includeUpperCase) characters += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
      if (this.includeNumber) characters += '0123456789'
      if (this.includeSymbol) characters += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"
      for (let i = 0; i < this.passwordLength; i++) {
        password += characters.charAt(Math.floor(Math.random() * characters.length))
      }

      this.saveObject.loginPassword = password
    },
    handleOkFunc: function () { // 点击【确认】按钮事件
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          // 请求接口
          if (that.isAdd) {
            this.btnLoading = true
            req.add(API_URL_MCH_LIST, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          } else {
            that.sysPassword.confirmPwd = Base64.encode(that.sysPassword.confirmPwd)
            console.log(that.sysPassword.confirmPwd)
            Object.assign(that.saveObject, that.sysPassword) // 拼接对象
            console.log(that.saveObject)
            req.updateById(API_URL_MCH_LIST, that.recordId, that.saveObject).then(res => {
              that.$message.success('修改成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
              that.resetIsShow = true // 展示重置密码板块
              that.sysPassword.resetPass = false
              that.sysPassword.defaultPass = true	// 是否使用默认密码默认为true
              that.resetPassEmpty(that) // 清空密码
            }).catch(res => {
              that.btnLoading = false
              that.resetIsShow = true // 展示重置密码板块
              that.sysPassword.resetPass = false
              that.sysPassword.defaultPass = true	// 是否使用默认密码默认为true
              that.resetPassEmpty(that) // 清空密码
            })
          }
        }
      })
    },
    onClose () {
      this.visible = false
      this.resetIsShow = false // 取消重置密码板块展示
      this.sysPassword.resetPass = false
      this.resetPassEmpty(this)
      this.sysPassword.defaultPass = true	// 是否使用默认密码默认为true
    },
    searchFunc: function () { // 点击【查询】按钮点击事件
      this.$refs.infoTable.refTable(true)
    },
    // 使用默认密码重置是否为true
    isResetPass () {
      if (!this.sysPassword.defaultPass) {
        this.newPwd = ''
        this.sysPassword.confirmPwd = ''
      }
    },
    // 保存后清空密码
    resetPassEmpty (that) {
      that.newPwd = ''
      that.sysPassword.confirmPwd = ''
    },
    refundModeChange (checkedValues) {
      console.log('checked = ', checkedValues)
      if (checkedValues.length === 1 && checkedValues[0] === 'plat') {
        this.saveObject.refundMode = ['plat', 'api']
      }
    },
    agentNoChange () {
      if (this.saveObject.agentNo) {
        this.saveObject.isvNo = this.agentList?.find(a => a.agentNo === this.saveObject.agentNo)?.isvNo
      }
    }
  }
}
</script>
<style lang="less">
  .typePopover {
    position: absolute;
    top: 0;
    left: 62px;
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
