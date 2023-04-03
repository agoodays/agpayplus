<template>
  <div v-if="visible">
    <a-form-model ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-model-item label="支付接口费率" prop="ifRate">
            <a-input v-model="saveObject.ifRate" type="number" :step="0.01" placeholder="请输入" suffix="%" />
          </a-form-model-item>
        </a-col>
        <a-col :span="12">
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
        <a-col :span="24">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" placeholder="请输入" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <a-divider orientation="left">
      <a-tag color="#FF4B33">
        {{ saveObject.ifCode }} 服务商参数配置
      </a-tag>
    </a-divider>
    <a-form-model ref="paramFormModel" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16">
        <a-col v-for="(item, key) in ifDefineArray" :key="key" :span="item.type === 'text' ? 12 : 24">
          <a-form-model-item :label="item.desc" :prop="item.name" v-if="item.type === 'text' || item.type === 'textarea'">
            <a-input v-if="item.star === '1'" v-model="ifParams[item.name]" :placeholder="ifParams[item.name + '_ph']" :type="item.type" />
            <a-input v-else v-model="ifParams[item.name]" placeholder="请输入" :type="item.type" />
          </a-form-model-item>
          <a-form-model-item :label="item.desc" :prop="item.name" v-else-if="item.type === 'radio'">
            <a-radio-group v-model="ifParams[item.name]">
              <a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
                {{ radioItem.title }}
              </a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item :label="item.desc" :prop="item.name" v-else-if="item.type === 'file'">
            <AgUpload
              :action="action"
              :bind-name="item.name"
              :urls="[ifParams[item.name]]"
              listType="text"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> {{ loading ? '正在上传' : '点击上传' }} </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
      </a-row>
    </a-form-model>
    <div class="drawer-btn-center">
      <a-button type="primary" icon="check" @click="onSubmit" :loading="btnLoading">
        保存
      </a-button>
    </div>
  </div>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import { API_URL_PAYCONFIGS_LIST, req, upload } from '@/api/manage'

export default {
  name: 'AgPayConfigPanel',
  components: {
    AgUpload
  },
  props: {
    configMode: { type: String, default: null },
    callbackFunc: { type: Function, default: () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      infoId: null, // 更新对象ID
      action: upload.cert, // 上传文件地址
      visible: false, // 一级抽屉开关
      ifDefineArray: {}, // 支付接口定义描述
      saveObject: {}, // 保存的对象
      ifParams: {}, // 参数配置对象
      rules: {
        infoId: [{ required: true, trigger: 'blur' }],
        ifCode: [{ required: true, trigger: 'blur' }],
        ifRate: [{ required: false, pattern: /^(([1-9]{1}\d{0,1})|(0{1}))(\.\d{1,4})?$/, message: '请输入0-100之间的数字，最多四位小数', trigger: 'blur' }]
      },
      ifParamsRules: {}
    }
  },
  methods: {
    show: function (infoId, record) {
      this.infoId = infoId
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      if (this.$refs.paramFormModel !== undefined) {
        this.$refs.paramFormModel.resetFields()
      }
      this.ifParams = {} // 参数配置对象
      this.ifDefineArray = {} // 支付接口定义描述

      // 数据初始化
      this.saveObject = {
        infoId: infoId,
        ifCode: record.ifCode,
        state: record.ifConfigState === 0 ? 0 : 1
      }
      this.getParamsConfig(record)
      this.visible = true
    },
    hide () {
      this.visible = false
    },
    generateRules () {
      const rules = {}
      let newItems = []
      this.ifDefineArray.forEach(item => {
        newItems = []
        if (item.verify === 'required' && item.star !== '1') {
          newItems.push({
            required: true,
            message: '请输入' + item.desc,
            trigger: 'blur'
          })
          rules[item.name] = newItems
        }
      })
      this.ifParamsRules = rules
    },
    getParamsConfig (record) {
      const that = this
      const params = Object.assign({}, { configMode: that.$props.configMode, infoId: that.infoId, ifCode: record.ifCode })
      req.get(API_URL_PAYCONFIGS_LIST + '/interfaceSavedConfigs', params).then(res => {
        if (res && res.ifParams) {
          this.saveObject = res
          this.ifParams = JSON.parse(res.ifParams)
        }

        const newItems = [] // 重新加载支付接口配置定义描述json
        JSON.parse(record.isvParams).forEach(item => {
          const radioItems = [] // 存放单选框value title
          if (item.type === 'radio') {
            const valueItems = item.values.split(',')
            const titleItems = item.titles.split(',')

            for (const i in valueItems) {
              // 检查参数是否为数字类型 然后赋值给radio值
              let radioVal = valueItems[i]
              if (!isNaN((radioVal))) { radioVal = Number(radioVal) }

              radioItems.push({
                value: radioVal,
                title: titleItems[i]
              })
            }
          }

          if (item.star === '1') {
            that.ifParams[item.name + '_ph'] = that.ifParams[item.name] ? that.ifParams[item.name] : '请输入'
            if (that.ifParams[item.name]) {
              that.ifParams[item.name] = ''
            }
          }

          newItems.push({
            name: item.name,
            desc: item.desc,
            type: item.type,
            verify: item.verify,
            values: radioItems,
            star: item.star // 脱敏标识 1-是
          })
        })
        that.ifDefineArray = newItems // 重新赋值接口定义描述
        that.generateRules()
        that.$forceUpdate()
      })
    },
    // 表单提交
    onSubmit () {
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        this.$refs.paramFormModel.validate(valid2 => {
          if (valid && valid2) { // 验证通过
            that.btnLoading = true
            const reqParams = {}
            reqParams.infoId = that.saveObject.infoId
            reqParams.ifCode = that.saveObject.ifCode
            reqParams.ifRate = that.saveObject.ifRate
            reqParams.state = that.saveObject.state
            reqParams.remark = that.saveObject.remark

            switch (that.$props.configMode) {
              case 'mgrIsv':
                reqParams.infoType = 1
                break
              case 'mgrAgent':
              case 'agentSubagent':
                reqParams.infoType = 4
                break
              case 'mgrMch':
              case 'agentMch':
              case 'agentSelf':
              case 'mchSelfApp1':
              case 'mchSelfApp2':
                reqParams.infoType = 3
                break
            }

            // 支付参数配置不能为空
            if (Object.keys(that.ifParams).length === 0) {
              this.$message.error('参数不能为空！')
              return
            }
            // 脱敏数据为空时，删除该key
            this.ifDefineArray.forEach(item => {
              if (item.star === '1' && that.ifParams[item.name] === '') {
                that.ifParams[item.name] = undefined
              }
              that.ifParams[item.name + '_ph'] = undefined
            })
            reqParams.ifParams = JSON.stringify(that.ifParams)
            // 请求接口
            req.add(API_URL_PAYCONFIGS_LIST + '/interfaceParams', reqParams).then(res => {
              that.$message.success('保存成功')
              that.callbackFunc()
              that.btnLoading = false
            })
          }
        })
      })
    },
    // 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
    uploadSuccess (name, fileList) {
      const [firstItem] = fileList
      this.ifParams[name] = firstItem?.url
      this.$forceUpdate()
    }
  }
}
</script>

<style scoped>
  .drawer-btn-center {
    position: fixed;
    width: 80%;
  }
</style>
