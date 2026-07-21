<template>
  <ag-drawer
    title="填写参数"
    width="40%"
    :closable="true"
    :mask-closable="false"
    v-model:open="localOpen"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    @close="handleClose"
  >
    <a-form ref="infoinfoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state" :options="stateOptions" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入" type="textarea" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <a-divider orientation="left">
      <a-tag color="#FF4B33">{{ saveObject.ifCode }} 商户参数配置</a-tag>
    </a-divider>
    <a-form ref="mchParaminfoForm" :model="ifParams" layout="vertical" :rules="ifParamsRules">
      <a-row :gutter="16">
        <a-col v-for="(item, key) in mchParams" :key="key" :span="item.type === 'text' ? 12 : 24">
          <a-form-item :label="item.desc" :name="item.name" v-if="item.type === 'text' || item.type === 'textarea'">
            <a-input v-model:value="ifParams[item.name]" :placeholder="item.star === '1' ? (ifParams[item.name + '_ph'] || '请输入') : '请输入'" :type="item.type" />
          </a-form-item>
          <a-form-item :label="item.desc" :name="item.name" v-else-if="item.type === 'radio'">
            <a-radio-group v-model:value="ifParams[item.name]">
              <a-radio v-for="(radioItem, radioKey) in item.values" :key="radioKey" :value="radioItem.value">
                {{ radioItem.title }}
              </a-radio>
            </a-radio-group>
          </a-form-item>
          <a-form-item :label="item.desc" :name="item.name" v-else-if="item.type === 'file'">
            <ag-upload
              :action="action"
              :bind-name="item.name"
              :urls="[ifParams[item.name]]"
              list-type="picture"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
    <div class="drawer-btn-center">
      <a-button :style="{ marginRight: '8px' }" @click="onClose">
        <template #icon><CloseOutlined /></template>
        取消
      </a-button>
      <a-button type="primary" @click="onSubmit" :loading="loading">
        <template #icon><CheckOutlined /></template>
        保存
      </a-button>
    </div>
  </ag-drawer>
</template>

<script setup>
/**
 * 商户支付配置添加/编辑组件
 * 功能：配置商户支付接口参数
 */
import { AgDrawer, AgUpload } from '@/components'
import { ref, reactive, watch, computed } from 'vue'
import { message } from 'ant-design-vue'
import { CheckOutlined, CloseOutlined, LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { mchAppApi } from '@/api/business/mch-app/mch-app-api'
import { upload } from '@/lib/ag-axios'
import { getStateOptions } from '@/constants/common-const'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// 获取翻译后的下拉选项
const stateOptions = computed(() => getStateOptions(t))

const icons = { LoadingOutlined, UploadOutlined };

/** Props 定义 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  appId: {
    type: String,
    default: ''
  },
  record: {
    type: Object,
    default: () => ({})
  }
});

/** 事件定义 */
const emit = defineEmits(['update:open', 'success']);

const infoinfoForm = ref(null);
const mchParaminfoForm = ref(null);
const loading = ref(false);
const localOpen = ref(false);
const ifCode = ref(null);
const mchType = ref(null);
const action = ref(upload.cert);
const mchParams = ref({});
const saveObject = reactive({
 infoId: null,
 ifCode: null,
 state: 1,
 remark: ''
});
const ifParams = reactive({});
const rules = {
 infoId: [{ required: true, trigger: 'blur' }],
 ifCode: [{ required: true, trigger: 'blur' }]
};
const ifParamsRules = ref({});

/** 监听 open 属性变化 */
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val && props.appId && props.record.ifCode) {
      resetForm()
      getMchPayConfig(props.record)
    }
  }
)

/** 监听本地 open 变化，同步 emit */
watch(localOpen, (val) => {
  emit('update:open', val)
})

/** 重置表单数据 */
const resetForm = () => {
  ifCode.value = props.record.ifCode
  mchType.value = props.record.mchType
  Object.keys(saveObject).forEach(key => {
    saveObject[key] = null
  })
  Object.keys(ifParams).forEach(key => {
    delete ifParams[key]
  })
  mchParams.value = {}
  saveObject.infoId = props.appId
  saveObject.ifCode = props.record.ifCode
  saveObject.state = props.record.ifConfigState === 0 ? 0 : 1
  if (mchParaminfoForm.value) {
    mchParaminfoForm.value.resetFields()
  }
};
/**
 * 获取商户支付配置
 * @param {Object} record - 记录对象
 */
const getMchPayConfig = async (record) => {
  try {
    const res = await mchAppApi.getMchPayConfigUnique(saveObject.infoId, saveObject.ifCode)
    if (res && res.ifParams) {
      Object.assign(saveObject, res)
      const parsedParams = JSON.parse(res.ifParams)
      Object.assign(ifParams, parsedParams)
    }
    const newItems = []
    let radioItems = []
    const mchParamsStr = mchType.value === 1 ? record.normalMchParams : record.isvsubMchParams
    JSON.parse(mchParamsStr).forEach(item => {
      radioItems = []
      if (item.type === 'radio') {
        const valueItems = item.values.split(',')
        const titleItems = item.titles.split(',')
        for (let i = 0; i < valueItems.length; i++) {
          let radioVal = valueItems[i]
          if (!isNaN(radioVal)) {
            radioVal = Number(radioVal)
          }
          radioItems.push({
            value: radioVal,
            title: titleItems[i]
          })
        }
      }
      if (item.star === '1') {
        ifParams[item.name + '_ph'] = ifParams[item.name] ? ifParams[item.name] : '请输入'
        if (ifParams[item.name]) {
          ifParams[item.name] = ''
        }
      }
      newItems.push({
        name: item.name,
        desc: item.desc,
        type: item.type,
        verify: item.verify,
        values: radioItems,
        star: item.star
      })
    })
    mchParams.value = newItems
    generoterRules()
  } catch (error) {
    console.error('获取商户支付配置失败:', error)
  }
}

/**
 * 提交表单
 */
const onSubmit = async () => {
  try {
    await infoinfoForm.value.validate()
    await mchParaminfoForm.value.validate()
    
    loading.value = true
    const reqParams = {}
    reqParams.infoId = saveObject.infoId
    reqParams.ifCode = saveObject.ifCode
    reqParams.state = saveObject.state
    reqParams.remark = saveObject.remark
    
    if (Object.keys(ifParams).length === 0) {
      message.error('参数不能为空！')
      return
    }
    
    Object.keys(mchParams.value).forEach(key => {
      const item = mchParams.value[key]
      if (item.star === '1' && ifParams[item.name] === '') {
        ifParams[item.name] = undefined
      }
      if (ifParams[item.name + '_ph'] !== undefined) {
        delete ifParams[item.name + '_ph']
      }
    })
    
    reqParams.ifParams = JSON.stringify(ifParams)
    
    if (Object.keys(reqParams).length === 0) {
      message.error('参数不能为空！')
      return
    }
    
    await mchAppApi.addMchPayConfig(reqParams)
    message.success('保存成功')
    localOpen.value = false
    emit('success')
  } catch (error) {
    console.error('提交失败:', error)
  } finally {
    loading.value = false
  }
}

const uploadSuccess = (name, fileList) => {
  const [firstItem] = fileList;
  ifParams[name] = firstItem?.url;
};

const generoterRules = () => {
  const rules = {};
  Object.keys(mchParams.value).forEach(key => {
    const item = mchParams.value[key];
    const newItems = [];
    if (item.verify === 'required' && item.star !== '1') {
      newItems.push({
        required: true,
        message: '请输入' + item.desc,
        trigger: 'blur'
      });
      rules[item.name] = newItems;
    }
  });
  ifParamsRules.value = rules;
};

/** 处理关闭 */
const handleClose = () => {
  localOpen.value = false;
};
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: #fff;
  border-radius: 6px;
  overflow:hidden;
  border: 1px solid #e8e8e8;
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: #fff;
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid #e8e8e8;
  position: absolute;
  bottom: 0;
}
.ag-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
}
.ag-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: start;
  align-items: center;
}
.title {
  font-size: 16px;
  font-family: PingFang SC, PingFang SC-Bold;
  font-weight: 700;
  color: #1a1a1a;
  letter-spacing: 1px;
}
</style>
