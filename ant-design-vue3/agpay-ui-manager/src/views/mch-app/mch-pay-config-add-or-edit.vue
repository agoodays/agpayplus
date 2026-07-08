<template>
  <a-drawer
    title="填写参数"
    width="40%"
    :closable="true"
    :mask-closable="false"
    :visible="visible"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    @close="onClose"
  >
    <a-form ref="infoinfoForm" :model="saveObject" layout="vertical" :rules="rules">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state">
              <a-radio :value="1">启用</a-radio>
              <a-radio :value="0">停用</a-radio>
            </a-radio-group>
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
      <a-button :style="{ marginRight: '8px' }" @click="onClose" icon="close">取消</a-button>
      <a-button type="primary" @click="onSubmit" icon="check" :loading="btnLoading">保存</a-button>
    </div>
  </a-drawer>
</template>

<script setup>import { ref, reactive } from 'vue';
import { message } from 'ant-design-vue';
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue';
import { AgUpload as agUpload } from '@/components/ag-upload';
import { mchAppApi } from '@/api/business/mch-app/mch-app-api';
import { upload } from '@/lib/ag-axios';
const icons = { LoadingOutlined, UploadOutlined };
const props = defineProps({
 callbackFunc: { type: Function, default: () => ({}) }
});
const infoinfoForm = ref(null);
const mchParaminfoForm = ref(null);
const btnLoading = ref(false);
const visible = ref(false);
const appId = ref(null);
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
const show = (appIdVal, record) => {
 appId.value = appIdVal;
 ifCode.value = record.ifCode;
 mchType.value = record.mchType;
 Object.keys(saveObject).forEach(key => {
 saveObject[key] = null;
 });
 Object.keys(ifParams).forEach(key => {
 delete ifParams[key];
 });
 mchParams.value = {};
 saveObject.infoId = appIdVal;
 saveObject.ifCode = record.ifCode;
 saveObject.state = record.ifConfigState === 0 ? 0 : 1;
 if (mchParaminfoForm.value) {
 mchParaminfoForm.value.resetFields();
 }
 getMchPayConfig(record);
};
const getMchPayConfig = (record) => {
 mchAppApi.getMchPayConfigUnique(saveObject.infoId, saveObject.ifCode).then(res => {
 if (res && res.ifParams) {
 Object.assign(saveObject, res);
 const parsedParams = JSON.parse(res.ifParams);
 Object.assign(ifParams, parsedParams);
 }
 const newItems = [];
 let radioItems = [];
 const mchParamsStr = mchType.value === 1 ? record.normalMchParams : record.isvsubMchParams;
 JSON.parse(mchParamsStr).forEach(item => {
 radioItems = [];
 if (item.type === 'radio') {
 const valueItems = item.values.split(',');
 const titleItems = item.titles.split(',');
 for (let i = 0; i < valueItems.length; i++) {
 let radioVal = valueItems[i];
 if (!isNaN(radioVal)) {
 radioVal = Number(radioVal);
 }
 radioItems.push({
 value: radioVal,
 title: titleItems[i]
 });
 }
 }
 if (item.star === '1') {
 ifParams[item.name + '_ph'] = ifParams[item.name] ? ifParams[item.name] : '请输入';
 if (ifParams[item.name]) {
 ifParams[item.name] = '';
 }
 }
 newItems.push({
 name: item.name,
 desc: item.desc,
 type: item.type,
 verify: item.verify,
 values: radioItems,
 star: item.star
 });
 });
 mchParams.value = newItems;
 visible.value = true;
 generoterRules();
 });
};
const onSubmit = () => {
 infoinfoForm.value.validate().then(() => {
 mchParaminfoForm.value.validate().then(() => {
 btnLoading.value = true;
 const reqParams = {};
 reqParams.infoId = saveObject.infoId;
 reqParams.ifCode = saveObject.ifCode;
 reqParams.state = saveObject.state;
 reqParams.remark = saveObject.remark;
 if (Object.keys(ifParams).length === 0) {
 message.error('参数不能为空！');
 return;
 }
 Object.keys(mchParams.value).forEach(key => {
 const item = mchParams.value[key];
 if (item.star === '1' && ifParams[item.name] === '') {
 ifParams[item.name] = undefined;
 }
 if (ifParams[item.name + '_ph'] !== undefined) {
 delete ifParams[item.name + '_ph'];
 }
 });
 reqParams.ifParams = JSON.stringify(ifParams);
 if (Object.keys(reqParams).length === 0) {
 message.error('参数不能为空！');
 return;
 }
 mchAppApi.addMchPayConfig(reqParams).then(() => {
 message.success('保存成功');
 visible.value = false;
 btnLoading.value = false;
 props.callbackFunc();
 });
 }).catch(() => {
 btnLoading.value = false;
 });
 }).catch(() => {
 btnLoading.value = false;
 });
};
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
const onClose = () => {
 visible.value = false;
};
defineExpose({ show });
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
