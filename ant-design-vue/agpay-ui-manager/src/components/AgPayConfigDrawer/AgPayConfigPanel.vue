<template>
  <div v-if="visible">
    <a-form-model ref="infoFormModel" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-model-item label="支付接口费率" prop="ifRate">
            <a-input v-model="saveObject.ifRate" placeholder="请输入" suffix="%" />
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
    <div class="drawer-btn-center" v-if="selectedIfCode">
      <a-button icon="close" :style="{ marginRight: '8px' }" @click="onClose" style="margin-right:8px">
        取消
      </a-button>
      <a-button type="primary" icon="check" @click="handleOkFunc" :loading="btnLoading">
        保存
      </a-button>
    </div>
  </div>
</template>

<script>
import AgUpload from '@/components/AgUpload/AgUpload'
import { upload } from '@/api/manage'

export default {
  components: {
    AgUpload
  },
  data () {
    return {
      btnLoading: false,
      infoId: null,
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
      this.visible = true
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
