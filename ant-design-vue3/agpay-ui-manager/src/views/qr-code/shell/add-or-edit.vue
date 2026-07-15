<template>
  <ag-drawer
    width="60%"
    v-model:open="localOpen"
    :mask-closable="false"
    :title="isAdd ? '新增模板' : '修改模板'"
    :show-confirm="true"
    :confirm-loading="loading"
    @close="handleClose"
    @confirm="handleConfirm"
  >
    <a-row>
      <a-col span="14">
        <a-form
          ref="infoForm"
          layout="horizontal"
          :model="saveObject"
          :label-col="{ span: 4 }"
          :wrapper-col="{ span: 20 }"
        >
          <a-form-item label="模板别名" name="shellAlias" :rules="rules.shellAlias">
            <a-input v-model:value="saveObject.shellAlias" placeholder="请输入模板别名" />
          </a-form-item>

          <a-form-item label="选择渲染模板" name="styleCode" :rules="rules.styleCode">
            <a-radio-group v-model:value="saveObject.styleCode" size="small" button-style="solid" @change="onChange">
              <a-radio-button value="shellA">模板A</a-radio-button>
              <a-radio-button value="shellB">模板B</a-radio-button>
            </a-radio-group>
          </a-form-item>

          <a-form-item label="显示ID">
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

          <a-form-item label="支付方式">
            <a-row v-for="(item, index) in saveObject.configInfo.payTypeList" :key="index">
              <a-col>
                <a-radio-group v-model:value="item.name" :options="payTypeOptions" @change="(e) => onPayTypeChange(e, index)" />
                <span>
                  <span>名称：</span>
                  <a-input style="width: 60px" v-model:value="item.alias" size="small" placeholder="名称" @change="onChange" />
                </span>
                <a-button size="small" @click="removePayTypeItem(index)">删除</a-button>
                <a-button
                  v-if="saveObject.configInfo.payTypeList.length <= 4 && index === saveObject.configInfo.payTypeList.length - 1"
                  size="small"
                  type="primary"
                  @click="addPayTypeItem"
                >新增</a-button>
                <div v-if="item.name === 'custom'" style="margin-top: 8px">
                  <ag-upload
                    :action="action"
                    accept=".jpg, .jpeg, .png"
                    :bind-name="`${index},imgUrl`"
                    :urls="[item.imgUrl]"
                    @upload-success="payTypeImgUploadSuccess"
                  >
                    <template #uploadSlot="{ loading }">
                      <a-button class="ag-upload-btn">
                        <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传图标
                      </a-button>
                    </template>
                  </ag-upload>
                  <p class="agpay-tip-text">(建议尺寸： 120 X 120)</p>
                </div>
              </a-col>
            </a-row>
            <a-row v-if="saveObject.configInfo.payTypeList.length <= 0" style="margin-top: 8px">
              <a-button size="small" type="primary" @click="addPayTypeItem">新增支付方式</a-button>
            </a-row>
          </a-form-item>

          <a-form-item label="背景颜色">
            <a-row>
              <a-col>
                <a-radio-group v-model:value="saveObject.configInfo.bgColor" @change="onChange">
                  <a-radio :value="'#1a53ff'" style="color: #1a53ff">蓝色</a-radio>
                  <a-radio :value="'#ff0000'" style="color: #ff0000">红色</a-radio>
                  <a-radio :value="'#09bb07'" style="color: #09bb07">绿色</a-radio>
                  <a-radio :value="'custom'" :style="{ color: saveObject.configInfo.customBgColor }">自定义</a-radio>
                </a-radio-group>
              </a-col>
            </a-row>
            <a-row>
              <a-col :span="24">
                <color-picker
                  v-if="saveObject.configInfo.bgColor === 'custom'"
                  v-model="saveObject.configInfo.customBgColor"
                  @change="onChange"
                />
              </a-col>
            </a-row>
          </a-form-item>

          <a-form-item label="主logo">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="logoImgUrl"
              :urls="[saveObject.configInfo.logoImgUrl]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
            <p class="agpay-tip-text">{{ logoImgTipText }}</p>
          </a-form-item>

          <a-form-item label="二维码上的logo">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="qrInnerImgUrl"
              :urls="[saveObject.configInfo.qrInnerImgUrl]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn">
                  <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传
                </a-button>
              </template>
            </ag-upload>
            <p class="agpay-tip-text">{{ qrInnerImgTipText }}</p>
          </a-form-item>
        </a-form>
      </a-col>

      <a-col span="10">
        <div class="preview-container">
          <div class="preview-header">
            <span class="preview-title">预览效果</span>
          </div>
          <div class="preview-content">
            <img
              :src="saveObject.shellImgViewUrl"
              class="preview-image"
              @click="onPreview"
            />
          </div>
        </div>
      </a-col>
    </a-row>
  </ag-drawer>
</template>

<script setup>
import { AgDrawer, AgUpload } from '@/components'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { qrcShellApi } from '@/api/business/qr-code/qrc-shell-api'
import { upload } from '@/lib/ag-axios'
import { message } from 'ant-design-vue'
import { ref, watch } from 'vue'
import { viewerApi } from '@/utils/viewer-api'

/**
 * 图标组件映射
 */
const icons = { LoadingOutlined, UploadOutlined }

/**
 * 组件属性定义
 */
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: [String, Number],
    default: null
  }
})

/**
 * 组件事件定义
 */
const emit = defineEmits(['update:open', 'success'])

/**
 * 表单引用
 */
const infoForm = ref(null)

/**
 * 本地抽屉打开状态，避免直接修改 props
 */
const localOpen = ref(props.open)

/**
 * 支付方式选项配置
 */
const payTypeOptions = [
  { value: 'wxpay', label: '微信' },
  { value: 'alipay', label: '支付宝' },
  { value: 'ysfpay', label: '云闪付' },
  { value: 'unionpay', label: '银联' },
  { value: 'custom', label: '自定义' }
]

/**
 * 创建默认表单数据对象
 * @returns {Object} 默认表单数据
 */
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
      bgColor: '#1a53ff',
      customBgColor: '#000000'
    }
  }
}

/**
 * 是否为新增模式
 */
const isAdd = ref(true)

/**
 * 按钮加载状态
 */
const loading = ref(false)

/**
 * 文件上传接口地址
 */
const action = upload.form

/**
 * 主logo提示文本
 */
const logoImgTipText = ref('(显示在顶部，透明图片，建议尺寸：924 X 282)')

/**
 * 二维码内部logo提示文本
 */
const qrInnerImgTipText = ref('(建议尺寸：100 X 100)')

/**
 * 表单保存数据对象
 */
const saveObject = ref(createDefaultSaveObject())

/**
 * 表单验证规则
 */
const rules = {
  shellAlias: [{ required: true, message: '请输入模板别名', trigger: 'blur' }],
  styleCode: [{ required: true, message: '请选择渲染模板', trigger: 'change' }]
}

/**
 * 监听抽屉打开状态变化
 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val) {
      await loadData()
    }
  }
)

/**
 * 加载表单数据
 * 新增时初始化默认数据，编辑时从接口获取数据
 */
async function loadData() {
  isAdd.value = !props.recordId
  saveObject.value = createDefaultSaveObject()

  if (!isAdd.value) {
    try {
      const res = await qrcShellApi.getById(props.recordId)
      saveObject.value = { ...res }
    } catch (error) {
      console.error('加载模板数据失败:', error)
      message.error('加载模板数据失败')
    }
    return
  }

  await onChange()
}

/**
 * 关闭抽屉
 */
function handleClose() {
  emit('update:open', false)
}

/**
 * 支付方式变更处理
 * @param {Event} e - 事件对象
 * @param {number} index - 支付方式索引
 */
function onPayTypeChange(e, index) {
  const selectedOption = payTypeOptions.find((option) => option.value === e.target.value)
  if (selectedOption) {
    saveObject.value.configInfo.payTypeList[index].imgUrl = ''
    saveObject.value.configInfo.payTypeList[index].name = selectedOption.value
    saveObject.value.configInfo.payTypeList[index].alias = selectedOption.value === 'custom' ? '' : selectedOption.label
  }
  onChange()
}

/**
 * 更新主logo提示文本
 * 根据所选模板类型显示不同的建议尺寸
 */
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

/**
 * 表单数据变更处理
 * 更新提示文本并请求预览图片
 */
async function onChange() {
  updateLogoImgTipText()
  try {
    const res = await qrcShellApi.previewImage(saveObject.value)
    saveObject.value.shellImgViewUrl = res
  } catch (error) {
    console.error('预览图片生成失败:', error)
  }
}

/**
 * 删除支付方式项
 * @param {number} index - 要删除的索引
 */
function removePayTypeItem(index) {
  saveObject.value.configInfo.payTypeList.splice(index, 1)
  onChange()
}

/**
 * 新增支付方式项
 */
function addPayTypeItem() {
  saveObject.value.configInfo.payTypeList.push({
    imgUrl: '',
    name: 'wxpay',
    alias: '微信'
  })
  onChange()
}

/**
 * 预览图片
 */
function onPreview() {
  if (!saveObject.value.shellImgViewUrl) {
    message.warning('暂无预览图片')
    return
  }
  viewerApi({
    images: [saveObject.value.shellImgViewUrl],
    options: {
      initialViewIndex: 0
    }
  })
}

/**
 * 上传文件成功回调
 * @param {string} name - 字段名称
 * @param {Array} fileList - 文件列表
 */
function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  saveObject.value.configInfo[name] = firstItem?.url
  onChange()
}

/**
 * 支付方式图标上传成功回调
 * @param {string} name - 索引和字段名组合（格式：index,fieldName）
 * @param {Array} fileList - 文件列表
 */
function payTypeImgUploadSuccess(name, fileList) {
  const [firstItem] = fileList
  const [targetIndex, targetKey] = name.split(',')
  saveObject.value.configInfo.payTypeList[targetIndex][targetKey] = firstItem?.url
  onChange()
}

/**
 * 提交表单处理
 */
async function handleConfirm() {
  if (infoForm.value) {
    try {
      await infoForm.value.validate()
    } catch {
      return
    }
  }

  loading.value = true
  try {
    const params = { ...saveObject.value, shellImgViewUrl: undefined }
    if (isAdd.value) {
      await qrcShellApi.add(params)
      message.success('新增成功')
    } else {
      await qrcShellApi.updateById(props.recordId, params)
      message.success('修改成功')
    }
    emit('success')
    emit('update:open', false)
  } catch (error) {
    console.error('保存模板失败:', error)
    message.error('保存失败，请重试')
  } finally {
    loading.value = false
  }
}
</script>

<style lang="less" scoped>
.preview-container {
  display: flex;
  flex-direction: column;
  height: 100%;
  border-radius: 8px;
  overflow: hidden;
  margin-left: 10px;
}

.preview-header {
  padding: 12px 16px;
}

.preview-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-color);
}

.preview-content {
  flex: 1;
  display: flex;
  justify-content: center;
  padding: 16px;
}

.preview-image {
  max-width: 100%;
  max-height: 580px;
  border-radius: 4px;
  cursor: pointer;
  transition: transform 0.2s;
  border: 1px solid darkgrey;

  &:hover {
    transform: scale(1.02);
  }
}
</style>
