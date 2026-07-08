<template>
  <a-drawer
    :mask-closable="false"
    :visible="visible"
    :title="isAdd ? '新增模板' : '修改模板'"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="80%"
    class="drawer-width"
    @close="onClose"
  >
    <a-row>
      <a-col span="14">
        <a-form
          ref="infoForm"
          :model="saveObject"
          :label-col="{ span: 4 }"
          :wrapper-col="{ span: 20 }"
          :rules="rules"
        >
          <a-form-item label="模板别名：" name="shellAlias">
            <a-input v-model:value="saveObject.shellAlias" />
          </a-form-item>
          <a-form-item label="选择渲染模板：" name="styleCode">
            <a-radio-group v-model:value="saveObject.styleCode" size="small" button-style="solid" @change="onChange">
              <a-radio-button value="shellA">模板A</a-radio-button>
              <a-radio-button value="shellB">模板B</a-radio-button>
            </a-radio-group>
          </a-form-item>
          <a-form-item label="显示ID：" name="showIdFlag">
            <a-radio-group
              v-model:value="saveObject.configInfo.showIdFlag"
              size="small"
              button-style="solid"
              @change="onChange"
            >
              <a-radio-button :value="true">显示</a-radio-button>
              <a-radio-button :value="false">隐藏</a-radio-button>
            </a-radio-group>
          </a-form-item>
          <a-form-item label="支付方式：" name="payType">
            <a-row v-for="(item, index) in saveObject.configInfo.payTypeList" :key="index">
              <a-col>
                <a-radio-group v-model:value="item.name" :options="payTypeOptions" @change="onPayTypeChange($event, index)" />
                <span
                  ><span>名称：</span><a-input v-model:value="item.alias" size="small" style="width: 60px" @change="onChange"
                /></span>
                <a-button size="small" @click="removePayTypeItem(index)">删除</a-button>
                <a-button
                  v-if="
                    saveObject.configInfo.payTypeList.length <= 4 &&
                    index === saveObject.configInfo.payTypeList.length - 1
                  "
                  size="small"
                  @click="addPayTypeItem"
                  >新增</a-button
                >
                <div v-if="item.name === 'custom'">
                  <ag-upload
                    :action="action"
                    accept=".jpg, .jpeg, .png"
                    :bind-name="`${index},imgUrl`"
                    :urls="[item.imgUrl]"
                    @upload-success="payTypeImgUploadSuccess"
                  >
                    <template #uploadSlot="{ loading }">
                      <a-button class="ag-upload-btn">
                        <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                      </a-button>
                    </template>
                  </ag-upload>
                  <div class="agpay-tip-text">
                    <span>(建议尺寸： 120 X 120)</span>
                  </div>
                </div>
              </a-col>
            </a-row>
            <a-row v-if="saveObject.configInfo.payTypeList.length <= 0">
              <a-button size="small" @click="addPayTypeItem">新增</a-button>
            </a-row>
          </a-form-item>
          <a-form-item label="背景颜色：" name="bgColor">
            <a-row>
              <a-col>
                <a-radio-group v-model:value="saveObject.configInfo.bgColor" @change="onChange">
                  <a-radio :value="'var(--primary-color)'" style="color: var(--primary-color)">蓝色</a-radio>
                  <a-radio :value="'var(--error-color)'" style="color: var(--error-color)">红色</a-radio>
                  <a-radio :value="'var(--success-color)'" style="color: var(--success-color)">绿色</a-radio>
                  <a-radio :value="'custom'" :style="{ color: saveObject.configInfo.customBgColor }"> 自定义 </a-radio>
                </a-radio-group>
              </a-col>
            </a-row>
            <a-row>
              <a-col>
                <colorPicker
                  v-if="saveObject.configInfo.bgColor === 'custom'"
                  v-model:modelValue="saveObject.configInfo.customBgColor"
                  style="height: 66px; margin-top: 8px"
                  @change="onChange"
                />
              </a-col>
            </a-row>
          </a-form-item>
          <a-form-item label="主logo：" name="logoImgUrl">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="logoImgUrl"
              :urls="[saveObject.configInfo.logoImgUrl]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
            <span class="agpay-tip-text">{{ logoImgTipText }}</span>
          </a-form-item>
          <a-form-item label="二维码上的logo：" name="qrInnerImgUrl">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="qrInnerImgUrl"
              :urls="[saveObject.configInfo.qrInnerImgUrl]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
            <div class="agpay-tip-text">
              <span>{{ qrInnerImgTipText }}</span>
            </div>
          </a-form-item>
        </a-form>
      </a-col>
      <a-col span="10">
        <div style="display: flex; justify-content: center">
          <div>
            <img
              :src="saveObject.shellImgViewUrl"
              style="max-width: 400px; border: 1px solid darkgrey"
              @click="onPreview"
            />
          </div>
        </div>
      </a-col>
    </a-row>
    <div class="drawer-btn-center">
      <a-button icon="close" :style="{ marginRight: '8px' }" style="margin-right: 8px" @click="onClose">
        取消
      </a-button>
      <a-button type="primary" icon="check" :loading="btnLoading" @click="handleOkFunc"> 保存 </a-button>
    </div>
  </a-drawer>
</template>
<script setup>
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
const icons = { LoadingOutlined, UploadOutlined }
import { qrcShellApi } from '@/api/business/qr-code/qrc-shell-api'
import AgUpload from '@/components/ag-upload'
import { upload } from '@/lib/ag-axios'
import { message } from 'ant-design-vue'
import { ref } from 'vue'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const payTypeOptions = [
  { value: 'wxpay', label: '微信' },
  { value: 'alipay', label: '支付宝' },
  { value: 'ysfpay', label: '云闪付' },
  { value: 'unionpay', label: '银联' },
  { value: 'custom', label: '自定义' }
]

function createDefaultSaveObject() {
  return {
    styleCode: 'shellA',
    configInfo: {
      showIdFlag: true,
      payTypeList: [
        { imgUrl: '', name: 'wxpay', alias: '微信' },
        { imgUrl: '', name: 'alipay', alias: '支付宝' },
        { imgUrl: '', name: 'ysfpay', alias: '云闪付' },
        { imgUrl: '', name: 'unionpay', alias: '银联' }
      ],
      bgColor: 'var(--primary-color)',
      customBgColor: 'var(--text-color)'
    }
  }
}

const infoForm = ref(null)
const isAdd = ref(true)
const visible = ref(false)
const btnLoading = ref(false)
const action = upload.form
const logoImgTipText = ref('(显示在顶部，透明图片，建议尺寸：924 X 282)')
const qrInnerImgTipText = ref('(建议尺寸：100 X 100)')
const saveObject = ref(createDefaultSaveObject())
const recordId = ref(null)

const rules = {
  shellAlias: [{ required: true, message: '请输入模板别名', trigger: 'blur' }],
  styleCode: [{ required: true, message: '请输入选择渲染模板', trigger: 'blur' }]
}

async function show(currentRecordId) {
  isAdd.value = !currentRecordId
  saveObject.value = createDefaultSaveObject()
  infoForm.value?.resetFields?.()

  if (!isAdd.value) {
    recordId.value = currentRecordId
    const res = await qrcShellApi.getById(currentRecordId)
    saveObject.value = res
    visible.value = true
    return
  }

  visible.value = true
  onChange()
}

function onClose() {
  visible.value = false
}

function onPayTypeChange(e, index) {
  const selectedOption = payTypeOptions.find((option) => option.value === e.target.value)
  if (selectedOption) {
    saveObject.value.configInfo.payTypeList.forEach((item, i) => {
      if (i === index) {
        item.imgUrl = ''
        item.name = selectedOption.value
        item.alias = selectedOption.value === 'custom' ? '' : selectedOption.label
      }
    })
  }
  onChange()
}

function updateLogoImgTipText() {
  switch (saveObject.value.styleCode) {
    case 'shellA':
      logoImgTipText.value = '(显示在顶部，透明图片，建议尺寸：924 X 282)'
      break
    case 'shellB':
      logoImgTipText.value = '(显示在顶部，建议尺寸：548 X 148)'
      break
    default:
      logoImgTipText.value = '(显示在顶部，透明图片，建议尺寸：924 X 282)'
  }
}

async function onChange() {
  updateLogoImgTipText()
  const res = await qrcShellApi.previewImage(saveObject.value)
  saveObject.value.shellImgViewUrl = res
}

function removePayTypeItem(index) {
  saveObject.value.configInfo.payTypeList.splice(index, 1)
  onChange()
}

function addPayTypeItem() {
  saveObject.value.configInfo.payTypeList.push({
    imgUrl: '',
    name: 'wxpay',
    alias: '微信'
  })
  onChange()
}

function onPreview() {
  window.$viewerApi({
    images: [saveObject.value.shellImgViewUrl],
    options: {
      initialViewIndex: 0
    }
  })
}

function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  saveObject.value.configInfo[name] = firstItem?.url
  onChange()
}

function payTypeImgUploadSuccess(name, fileList) {
  const [firstItem] = fileList
  const [targetIndex, targetKey] = name.split(',')
  saveObject.value.configInfo.payTypeList[targetIndex][targetKey] = firstItem?.url
  onChange()
}

async function validateForm() {
  if (!infoForm.value?.validate) {
    return true
  }
  try {
    await infoForm.value.validate()
    return true
  } catch {
    return false
  }
}

async function handleOkFunc() {
  const valid = await validateForm()
  if (!valid) return

  const params = { ...saveObject.value, shellImgViewUrl: undefined }
  if (isAdd.value) {
    await qrcShellApi.add(params)
    message.success('新增成功')
  } else {
    await qrcShellApi.updateById(recordId.value, params)
    message.success('修改成功')
  }

  visible.value = false
  props.callbackFunc()
}

defineExpose({
  show,
  onClose
})
</script>

<style lang="less" scoped>
.agpay-tip-text:before {
  content: '';
  width: 0;
  height: 0;
  border: 10px solid transparent;
  border-bottom-color: var(--warning-color);
  position: absolute;
  top: -20px;
  left: 30px;
}

.agpay-tip-text {
  font-size: 12px !important;
  border-radius: 5px;
  background: var(--warning-color);
  color: var(--text-on-primary) !important;
  padding: 5px 10px;
  display: inline-block;
  max-width: 100%;
  position: relative;
  margin-top: 15px;
  line-height: 1.5715;
}

.ag-upload-btn {
  height: 66px;
}

.m-colorPicker {
  height: 66px;
  width: 100%;
  margin-top: 8px;
  border: 1px solid #d9d9d9;
  border-radius: 4px;

  :deep(.colorBtn) {
    height: 48px;
    width: calc(100% - 16px);
    margin: 8px;
    border-radius: 4px;
  }

  :deep(.box.open) {
    z-index: 3;
  }

  :deep(.bd h3:nth-of-type(3)) {
    cursor: pointer;
  }
}
</style>
