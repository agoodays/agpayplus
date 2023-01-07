<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增门店' : '修改门店' "
    @close="onClose"
    :body-style="{ paddingBottom: '80px' }"
    width="40%"
    class="drawer-width"
  >
    <a-form-model v-if="visible" ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-form-model-item label="商户号" prop="mchNo" v-if="isAdd">
            <a-select v-model="saveObject.mchNo" placeholder="请选择商户">
              <a-select-option value="" key="">请选择商户</a-select-option>
              <a-select-option v-for="d in mchList" :value="d.mchNo" :key="d.mchNo">
                {{ d.mchName + " [ ID: " + d.mchNo + " ]" }}
              </a-select-option>
            </a-select>
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="门店名称" prop="storeName">
            <a-input
                placeholder="请输入门店名称"
                v-model="saveObject.storeName"
            />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人电话" prop="contactPhone">
            <a-input
                placeholder="请输入联系人电话"
                v-model="saveObject.contactPhone"
            />
          </a-form-model-item>
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="门店LOGO" prop="storeLogo">
            <div v-if="this.imgDefaultFileList.storeLogo">
              <a-upload
                  :file-list="this.imgDefaultFileList.storeLogo"
                  list-type="picture"
                  class="default-upload-list-inline"
                  @change="handleChange($event, 'storeLogo')"
              >
              </a-upload>
            </div>
            <div v-else>
              <a-upload
                  :action="action"
                  list-type="picture"
                  class="upload-list-inline"
                  @change="handleChange($event, 'storeLogo')"
              >
                <a-button icon="upload" v-if="this.imgIsShow.storeLogo">
                  上传
                </a-button>
              </a-upload>
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="门头照" prop="storeOuterImg">
            <div v-if="this.imgDefaultFileList.storeOuterImg">
              <a-upload
                  :file-list="this.imgDefaultFileList.storeOuterImg"
                  list-type="picture"
                  class="default-upload-list-inline"
                  @change="handleChange($event, 'storeOuterImg')"
              >
              </a-upload>
            </div>
            <div v-else>
              <a-upload
                  :action="action"
                  list-type="picture"
                  class="upload-list-inline"
                  @change="handleChange($event, 'storeOuterImg')"
              >
                <a-button icon="upload" v-if="this.imgIsShow.storeOuterImg">
                  上传
                </a-button>
              </a-upload>
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="门店内景照" prop="storeInnerImg">
            <div v-if="this.imgDefaultFileList.storeInnerImg">
              <a-upload
                  :file-list="this.imgDefaultFileList.storeInnerImg"
                  list-type="picture"
                  class="default-upload-list-inline"
                  @change="handleChange($event, 'storeInnerImg')"
              >
              </a-upload>
            </div>
            <div v-else>
              <a-upload
                  :action="action"
                  list-type="picture"
                  class="upload-list-inline"
                  @change="handleChange($event, 'storeInnerImg')"
              >
                <a-button icon="upload" v-if="this.imgIsShow.storeInnerImg">
                  上传
                </a-button>
              </a-upload>
            </div>
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
import { API_URL_MCH_STORE, API_URL_MCH_LIST, req, upload } from '@/api/manage'
export default {
  props: {
    callbackFunc: { type: Function }
  },

  data () {
    const checkMchNo = (rule, value, callback) => { // 是否选择了商户
      if (this.isAdd && !value) {
        callback(new Error('请选择商户'))
      }
      callback()
    }
    return {
      btnLoading: false,
      isAdd: true, // 新增 or 修改页面标志
      saveObject: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      mchList: null, // 商户下拉列表
      action: upload.form, // 上传文件地址
      imgDefaultFileList: {
        storeLogo: null,
        storeOuterImg: null,
        storeInnerImg: null
      },
      imgIsShow: {
        storeLogo: true,
        storeOuterImg: true,
        storeInnerImg: true
      },
      rules: {
        storeName: [{ required: true, message: '请输入门店名称', trigger: 'blur' }],
        mchNo: [{ validator: checkMchNo, trigger: 'blur' }],
        contactPhone: [{ required: true, pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }]
      }
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.saveObject = {}
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      req.list(API_URL_MCH_LIST, { 'pageSize': -1, 'state': 1 }).then(res => { // 商户下拉选择列表
        that.mchList = res.records
      })
      if (!this.isAdd) { // 修改信息 延迟展示弹层
        console.log(555)
        that.recordId = recordId
        req.getById(API_URL_MCH_STORE, recordId).then(res => {
          that.saveObject = res
          Object.keys(that.imgDefaultFileList).forEach((field) => {
            const url = that.saveObject[field]
            if (!url) {
              this.imgIsShow[field] = true
              return null
            }
            this.imgIsShow[field] = false
            that.imgDefaultFileList[field] = [{
              uid: '-1',
              name: url.split('/').pop(),
              status: 'done',
              url: url,
              thumbUrl: url
            }]
          })
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
            req.add(API_URL_MCH_STORE, that.saveObject).then(res => {
              that.$message.success('新增成功')
              that.visible = false
              that.callbackFunc() // 刷新列表
              that.btnLoading = false
            }).catch(res => {
              that.btnLoading = false
            })
          } else {
            req.updateById(API_URL_MCH_STORE, that.recordId, that.saveObject).then(res => {
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
    },
    // 上传回调
    handleChange (info, name) {
      console.log(info)
      if (info.fileList.length) {
        this.imgIsShow[name] = false
      } else {
        this.imgIsShow[name] = true
        this.saveObject[name] = ''
      }

      const res = info.file.response

      if (info.file.status === 'uploading') {
        this.loading = true
      }
      if (info.file.status === 'done') {
        if (res.code !== 0) {
          this.$message.error(res.msg)
        }
        this.loading = false
        this.saveObject[name] = res.data
        info.file.name = res.data.split('/').pop()
        info.file.url = res.data
        info.file.thumbUrl = res.data
        const fileinfo = info.fileList.find(f => f.lastModified === info.file.lastModified)
        fileinfo.name = res.data.split('/').pop()
        fileinfo.url = res.data
        fileinfo.thumbUrl = res.data
      } else if (info.file.status === 'error') {
        console.log(info)
        this.$message.error(`上传失败`)
      } else if (info.file.status === 'removed') {
        this.imgDefaultFileList[name] = null
      }
    }
  }
}
</script>

<style lang="less">
  .upload-list-inline .ant-btn {
    height: 66px;
  }
</style>
