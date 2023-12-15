<template>
  <div style="background: #fff;border-radius:10px">
    <a-tabs v-model="parentKey" @change="selectParentTabs">
      <a-tab-pane key="0" tab="基本信息">
        <div class="account-settings-info-view">
          <a-row :gutter="16">
            <a-col :md="16" :lg="16">
              <a-form-model ref="infoFormModel" :model="saveObject" :label-col="{span: 9}" :wrapper-col="{span: 10}" :rules="rules">
                <a-form-model-item label="用户登录名:">
                  <a-input v-model="saveObject.loginUsername" disabled/>
                </a-form-model-item>
                <a-form-model-item label="用户姓名：" prop="realname">
                  <a-input v-model="saveObject.realname" />
                </a-form-model-item>
                <a-form-model-item label="手机号：" prop="telphone">
                  <a-input v-model="saveObject.telphone" disabled/>
                </a-form-model-item>
                <a-form-model-item label="请选择性别：">
                  <a-radio-group v-model="saveObject.sex">
                    <a-radio :value="1">男</a-radio>
                    <a-radio :value="2">女</a-radio>
                  </a-radio-group>
                </a-form-model-item>
              </a-form-model>
              <a-form-item style="display:flex;justify-content:center">
                <a-button type="primary" @click="changeInfo" icon="check-circle" :loading="btnLoading">更新基本信息</a-button>
              </a-form-item>
            </a-col>
            <a-col :md="8" :lg="8" :style="{ minHeight: '180px',margin:'0 auto' }">
              <!-- 原始头像上传，带有图片裁剪
                <div class="ant-upload-preview" @click="$refs.modal.edit(1)" >
                <a-icon type="cloud-upload-o" class="upload-icon"/>
                <div class="mask">
                  <a-icon type="plus" />
                </div>
                <img :src="saveObject.avatarUrl"/>
              </div> -->
              <div class="ant-upload-preview" >
                <img
                  :src="saveObject.avatarUrl"
                  style="border: 1px solid rgba(0,0,0,0.08);cursor: pointer;"
                  @click="imgPreview({ url:saveObject.avatarUrl })"
                />
                <div style="margin-top:10px">
                  <a-upload
                    name="file"
                    :headers="headers"
                    :action="action"
                    :accept="accept"
                    :showUploadList="false"
                    :before-upload="beforeUpload"
                    @change="handleChange"
                    @preview="imgPreview($event)"
                  >
                    <a-button style="margin-left: 5px;"> <a-icon :type="loading ? 'loading' : 'upload'" /> {{ loading ? '正在上传' : '更换头像' }} </a-button>
                  </a-upload>
                </div>
              </div>
            </a-col>
          </a-row>
          <avatar-modal ref="modal" @ok="setavatar"/>
        </div>
      </a-tab-pane>
      <a-tab-pane key="1" tab="安全信息">
        <div class="account-settings-info-view">
          <a-tabs v-model="childKey" tab-position="left" @change="selectChildTabs">
            <a-tab-pane key="0" tab="修改密码">
              <div class="account-settings-info-view">
                <a-row :gutter="16">
                  <a-col :md="16" :lg="16">
                    <a-form-model ref="pwdFormModel" :model="updateObject" :label-col="{span: 9}" :wrapper-col="{span: 10}" :rules="rulesPass">
                      <a-form-model-item label="原密码：" prop="originalPwd">
                        <a-input-password v-model="updateObject.originalPwd" placeholder="请输入原密码" />
                      </a-form-model-item>
                      <a-form-model-item label="新密码：" prop="newPwd">
                        <a-input-password v-model="updateObject.newPwd" placeholder="请输入新密码" />
                      </a-form-model-item>
                      <a-form-model-item label="确认新密码：" prop="confirmPwd">
                        <a-input-password v-model="updateObject.confirmPwd" placeholder="确认新密码" />
                      </a-form-model-item>
                    </a-form-model>
                    <a-form-item style="display:flex;justify-content:center">
                      <a-button type="primary" icon="safety-certificate" @click="confirm" :loading="btnLoading">更新密码</a-button>
                    </a-form-item>
                  </a-col>
                </a-row>
              </div>
            </a-tab-pane>
            <a-tab-pane key="1" tab="预留信息">
              <div class="account-settings-info-view">
                <a-row :gutter="16">
                  <a-col :md="16" :lg="16">
                    <a-form-model :label-col="{span: 9}" :wrapper-col="{span: 10}" :rules="rulesPass">
                      <a-form-model-item label="预留信息：" prop="safeWord">
                        <a-input v-model="safeWord" placeholder="请输入新的预留信息" />
                      </a-form-model-item>
                    </a-form-model>
                    <a-form-item style="display:flex;justify-content:center">
                      <a-button type="primary" icon="check-circle" @click="changeSafeWordInfo" :loading="btnLoading">确认更新</a-button>
                    </a-form-item>
                  </a-col>
                </a-row>
              </div>
            </a-tab-pane>
          </a-tabs>
        </div>
      </a-tab-pane>
    </a-tabs>
  </div>
</template>
<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import { getInfo } from '@/api/login'
import { Base64 } from 'js-base64'
import { updateUserInfo, updateUserPass, getUserInfo, upload } from '@/api/manage'
import AvatarModal from './AvatarModal'
import store from '@/store'
import appConfig from '@/config/appConfig'
import storage from '@/utils/agpayStorageWrapper'
import 'viewerjs/dist/viewer.css'

function getHeaders () {
  const headers = {}
  headers[appConfig.ACCESS_TOKEN_NAME] = `Bearer ${storage.getToken()}` // token
  return headers
}

export default {
  components: {
    AvatarModal, AgUpload
  },
  data () {
    return {
      loading: false, // 上传状态
      size: 10, // 文件大小限制
      action: upload.avatar, // 上传图标地址
      headers: getHeaders(), // 放入token
      accept: '.jpg, .jpeg, .png',
      btnLoading: false,
      parentKey: this.$route.params.parentKey ?? '0',
      childKey: this.$route.params.childKey ?? '0',
      saveObject: {
        loginUsername: '', // 登录名
        realname: '', //  真实姓名
        sex: '',
        avatarUrl: '' // 头像地址
      },
      updateObject: {
        originalPwd: '', // 原密码
        newPwd: '', //  新密码
        confirmPwd: '' //  确认密码
      },
      safeWord: store.state.user.safeWord,
      recordId: store.state.user.userId, // 拿到ID
      rules: {
        realname: [{ required: true, message: '请输入真实姓名', trigger: 'blur' }]
      },
      rulesPass: {
        originalPwd: [{ required: true, message: '请输入原密码', trigger: 'blur' }],
        newPwd: [{ min: 6, max: 12, required: true, message: '请输入6-12位新密码', trigger: 'blur' }],
        confirmPwd: [{ required: true, message: '请输入确认新密码', trigger: 'blur' }, {
          validator: (rule, value, callBack) => {
            this.updateObject.newPwd === value ? callBack() : callBack('新密码与确认密码不一致')
          }
        }]
      }
    }
  },
  created () {
    this.detail()
  },
  methods: {
    setavatar (url) {
      this.option.img = url
    },
    detail () { // 获取基本信息
      const that = this
      getUserInfo().then(res => {
        that.saveObject = res
      })
    },
    changeSafeWordInfo () { // 更新基本信息事件
      const that = this
      if (that.safeWord.length <= 0) {
        that.$message.error('信息内容不可为空')
        return
      }
      that.btnLoading = true // 打开按钮上的 loading
      that.confirmLoading = true // 显示loading
      updateUserInfo({ safeWord: that.safeWord }).then(res => {
        that.btnLoading = false // 关闭按钮刷新
        return getInfo()
      }).then(bizData => {
        // console.log(bizData)
        bizData.safeWord = that.safeWord
        store.commit('SET_USER_INFO', bizData) // 调用vuex设置用户基本信息
        that.$message.success('修改成功')
      }).catch(res => {
        that.btnLoading = false
      })
    },
    changeInfo () { // 更新基本信息事件
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          this.$infoBox.confirmPrimary('确认更新信息吗？', '', () => {
            // 请求接口
            that.btnLoading = true // 打开按钮上的 loading
              updateUserInfo(that.saveObject).then(res => {
                that.btnLoading = true // 打开按钮上的 loading
                return getInfo()
              })
              .then(bizData => {
                bizData.avatarUrl = that.saveObject.avatarUrl
                bizData.realname = that.saveObject.realname
                that.btnLoading = false
                store.commit('SET_USER_INFO', bizData) // 调用vuex设置用户基本信息
                that.$message.success('修改成功')
                console.log('cg')
              }).catch(err => {
                console.log(err)
                that.btnLoading = false // 打开按钮上的 loading
              })
            })
        }
      })
    },
    confirm (e) { // 确认更新密码
      const that = this
      this.$refs.pwdFormModel.validate(valid => {
        if (valid) { // 验证通过
          this.$infoBox.confirmPrimary('确认更新密码吗？', '', () => {
              // 请求接口
              that.btnLoading = true // 打开按钮上的 loading
              that.confirmLoading = true // 显示loading
              const recordId = that.recordId // 用户ID
              const originalPwd = Base64.encode(that.updateObject.originalPwd)
              const confirmPwd = Base64.encode(that.updateObject.confirmPwd)
              // this.$delete(this.updateObject, 'newPwd')
              updateUserPass({ recordId, originalPwd, confirmPwd }).then(res => {
                that.$message.success('修改成功')
                // 退出登录
                this.$store.dispatch('Logout').then(() => {
                  this.$router.push({ name: 'login' })
                })
              }).catch(res => {
                that.confirmLoading = false
                that.btnLoading = false
              })
            })
        }
      })
    },
    selectParentTabs (key) { // 清空必填提示
      this.parentKey = key
      this.$route.params.parentKey = key
      if (this.$refs.pwdFormModel !== undefined) {
        this.$refs.pwdFormModel.resetFields()
      }
    },
    selectChildTabs (key) {
      this.childKey = key
      this.$route.params.childKey = key
    },
    // 上传回调
    handleChange (info) {
      // 限制文件数量
      /* let fileList = [...info.fileList]
      fileList = fileList.length > this.num ? fileList.splice(0 - this.num) : fileList // 取最新加入的元素
      fileList = fileList.map(file => {
        if (file.response) {
          file.url = file.response.data
        }
        return file
      }) */
      const res = info.file.response

      if (info.file.status === 'uploading') {
        this.loading = true
      }
      if (info.file.status === 'done') {
        if (res.code !== 0) {
          this.$message.error(res.msg)
        }
        this.loading = false
        this.uploadSuccess(res.data)
      } else if (info.file.status === 'error') {
        console.log(info)
        this.$message.error(`上传失败`)
      }
    },
    imgPreview (info) {
      // console.log(info)
      this.$viewerApi({
        images: [info.url],
        options: {
          initialViewIndex: 0
        }
      })
    },
    // 上传图片前的校验
    beforeUpload (file) {
      const validate = file.size / 1024 / 1024 < this.size
      if (!validate) {
        this.$message.error('文件应小于' + this.size + 'M!')
      }
      return validate
    },
    // 上传文件成功回调方法，参数value为文件地址，name是自定义参数
    uploadSuccess (value) {
      this.saveObject.avatarUrl = value
      this.$forceUpdate()
    }
  }
}

</script>
<style lang="less" scoped>

  .avatar-upload-wrapper {
    height: 200px;
    width: 100%;
  }

  .ant-upload-preview {
    text-align:center ;
    position: relative;
    margin: 0 auto;
    width: 100%;
    // max-width: 180px;
    border-radius: 50%;
    // box-shadow: 0 0 4px #ccc;

    .upload-icon {
      position: absolute;
      top: 0;
      right: 10px;
      font-size: 1.4rem;
      padding: 0.5rem;
      background: rgba(222, 221, 221, 0.7);
      border-radius: 50%;
      border: 1px solid rgba(0, 0, 0, 0.2);
    }
    .mask {
      opacity: 0;
      position: absolute;
      background: rgba(0,0,0,0.4);
      cursor: pointer;
      transition: opacity 0.4s;

      &:hover {
        opacity: 1;
      }

      i {
        font-size: 2rem;
        position: absolute;
        top: 50%;
        left: 50%;
        margin-left: -1rem;
        margin-top: -1rem;
        color: #d6d6d6;
      }
    }

    img, .mask {
      width: 150px;
      height: 150px;
      border-radius: 50%;
      overflow: hidden;
    }
  }
</style>
