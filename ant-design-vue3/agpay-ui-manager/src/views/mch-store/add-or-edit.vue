<template>
  <a-drawer
    v-model:open="localOpen"
    :title="isAdd ? '新增门店' : '修改门店'"
    :width="720"
    :mask-closable="false"
    :body-style="{ paddingBottom: '80px' }"
    @close="handleClose"
  >
    <a-form ref="infoForm" :model="saveObject" :rules="rules" layout="vertical">
      <!-- 商户号（仅新增时显示） -->
      <a-row v-if="isAdd" :gutter="16">
        <a-col :span="24">
          <a-form-item label="商户号" name="mchNo">
            <a-select
              v-model:value="saveObject.mchNo"
              placeholder="请选择商户"
              show-search
              :filter-option="false"
              @search="handleSearchMch"
            >
              <a-select-option v-for="item in mchList" :key="item.mchNo" :value="item.mchNo">
                {{ item.mchName }}
              </a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 门店基本信息 -->
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="门店名称" name="storeName">
            <a-input v-model:value="saveObject.storeName" placeholder="请输入门店名称" />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="联系人电话" name="contactPhone">
            <a-input v-model:value="saveObject.contactPhone" placeholder="请输入联系人电话" />
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 门店图片 -->
      <a-row :gutter="16">
        <a-col :span="8">
          <a-form-item label="门店LOGO" name="storeLogo">
            <ag-upload
              :urls="saveObject.storeLogo ? [saveObject.storeLogo] : []"
              list-type="picture-card"
              :before-upload="beforeUpload"
              @change="(info) => handleUploadChange(info, 'storeLogo')"
              @preview="handlePreview"
            >
              <div v-if="!saveObject.storeLogo">
                <plus-outlined />
                <div style="margin-top: 8px">上传</div>
              </div>
            </ag-upload>
          </a-form-item>
        </a-col>

        <a-col :span="8">
          <a-form-item label="门头照" name="storeOuterImg">
            <ag-upload
              :urls="saveObject.storeOuterImg ? [saveObject.storeOuterImg] : []"
              list-type="picture-card"
              :before-upload="beforeUpload"
              @change="(info) => handleUploadChange(info, 'storeOuterImg')"
              @preview="handlePreview"
            >
              <div v-if="!saveObject.storeOuterImg">
                <plus-outlined />
                <div style="margin-top: 8px">上传</div>
              </div>
            </ag-upload>
          </a-form-item>
        </a-col>

        <a-col :span="8">
          <a-form-item label="门店内景照" name="storeInnerImg">
            <ag-upload
              :urls="saveObject.storeInnerImg ? [saveObject.storeInnerImg] : []"
              list-type="picture-card"
              :before-upload="beforeUpload"
              @change="(info) => handleUploadChange(info, 'storeInnerImg')"
              @preview="handlePreview"
            >
              <div v-if="!saveObject.storeInnerImg">
                <plus-outlined />
                <div style="margin-top: 8px">上传</div>
              </div>
            </ag-upload>
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 备注 -->
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="备注" name="remark">
            <a-textarea v-model:value="saveObject.remark" placeholder="请输入备注" :rows="3" />
          </a-form-item>
        </a-col>
      </a-row>

      <a-divider />

      <!-- 地址信息 -->
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="选址省/市/区" name="areas">
            <a-cascader
              v-model:value="areas"
              :options="areaOptions"
              placeholder="请选择省市区"
              @change="handleAreaChange"
            />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="具体位置" name="address">
            <a-input v-model:value="saveObject.address" placeholder="请输入详细地址" />
          </a-form-item>
        </a-col>
      </a-row>

      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="经度" name="lng">
            <a-input-number v-model:value="saveObject.lng" placeholder="请输入经度" :precision="6" style="width: 100%" />
          </a-form-item>
        </a-col>

        <a-col :span="12">
          <a-form-item label="纬度" name="lat">
            <a-input-number v-model:value="saveObject.lat" placeholder="请输入纬度" :precision="6" style="width: 100%" />
          </a-form-item>
        </a-col>
      </a-row>

      <!-- TODO: 地图选址功能待实现 -->
      <a-alert
        message="地图选址功能"
        description="地图选址功能正在开发中，暂时请手动输入经纬度。"
        type="info"
        show-icon
        closable
      />
    </a-form>

    <!-- 图片预览 -->
    <a-modal v-model:open="previewOpen" :footer="null" @cancel="handleCancelPreview">
      <img :src="previewImage" style="width: 100%" alt="preview" />
    </a-modal>

    <!-- 底部按钮 -->
    <template #footer>
      <div style="text-align: center">
        <a-space>
          <a-button @click="handleClose">
            <close-outlined />
            取消
          </a-button>
          <a-button type="primary" :loading="loading" @click="handleSubmit">
            <check-outlined />
            保存
          </a-button>
        </a-space>
      </div>
    </template>
  </a-drawer>
</template>

<script setup>
import { mchStoreApi } from '@/api/business/mch-store/mch-store-api'
import { CheckOutlined, CloseOutlined, PlusOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { nextTick, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import AgUpload from '@/components/ag-upload'

const { t } = useI18n()

// 省市区数据（简化版，实际项目应该从接口获取或使用完整的省市区数据）
// TODO: 实现完整的省市区数据
const areaOptions = ref([])

// Props & Emits
const props = defineProps({
  open: {
    type: Boolean,
    default: false
  },
  recordId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:open', 'success'])

// State
const infoForm = ref(null)
const loading = ref(false)
const isAdd = ref(true)
const localOpen = ref(false)
const mchList = ref([])
const areas = ref([])



// 图片预览
const previewOpen = ref(false)
const previewImage = ref('')

// 表单数据
const saveObject = reactive({
  mchNo: '',
  storeName: '',
  contactPhone: '',
  storeLogo: '',
  storeOuterImg: '',
  storeInnerImg: '',
  remark: '',
  provinceCode: '',
  cityCode: '',
  districtCode: '',
  address: '',
  lng: null,
  lat: null
})

// 表单验证规则
const rules = {
  mchNo: [{ required: true, message: '请选择商户', trigger: 'change' }],
  storeName: [{ required: true, message: '请输入门店名称', trigger: 'blur' }],
  contactPhone: [{ pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }],
  areas: [{ required: true, message: '请选择省市区', trigger: 'change', type: 'array' }],
  address: [{ required: true, message: '请输入详细地址', trigger: 'blur' }]
}

// 监听 props.open 变化
watch(
  () => props.open,
  (val) => {
    localOpen.value = val
    if (val) {
      initForm()
    }
  }
)

// 监听 localOpen 变化
watch(localOpen, (val) => {
  emit('update:open', val)
})

/**
 * 初始化表单
 */
const initForm = async () => {
  isAdd.value = !props.recordId

  if (isAdd.value) {
    resetForm()
  } else {
    await loadDetail()
  }
}

/**
 * 加载详情数据
 */
const loadDetail = async () => {
  try {
    loading.value = true
    const res = await mchStoreApi.getById(props.recordId)

    Object.assign(saveObject, res)

    // 设置省市区
    if (res.provinceCode && res.cityCode && res.districtCode) {
      areas.value = [res.provinceCode, res.cityCode, res.districtCode]
    }

    
  } catch (error) {
    message.error(error.msg || t('common.loadDataFailed'))
  } finally {
    loading.value = false
  }
}

/**
 * 重置表单
 */
const resetForm = () => {
  Object.assign(saveObject, {
    mchNo: '',
    storeName: '',
    contactPhone: '',
    storeLogo: '',
    storeOuterImg: '',
    storeInnerImg: '',
    remark: '',
    provinceCode: '',
    cityCode: '',
    districtCode: '',
    address: '',
    lng: null,
    lat: null
  })

  areas.value = []

  nextTick(() => {
    infoForm.value?.clearValidate()
  })
}

/**
 * 搜索商户
 */
const handleSearchMch = async (keyword) => {
  try {
    const res = await mchStoreApi.queryMchPage({
      mchName: keyword,
      pageSize: 20
    })
    mchList.value = res.records || []
  } catch (error) {
    console.error('搜索商户失败:', error)
  }
}

/**
 * 省市区变化
 */
const handleAreaChange = (value) => {
  if (value && value.length === 3) {
    saveObject.provinceCode = value[0]
    saveObject.cityCode = value[1]
    saveObject.districtCode = value[2]
  }
}

/**
 * 上传前校验
 */
const beforeUpload = (file) => {
  const isImage = file.type.startsWith('image/')
  if (!isImage) {
    message.error(t('mchStore.onlyImageAllowed'))
  }
  const isLt2M = file.size / 1024 / 1024 < 2
  if (!isLt2M) {
    message.error(t('mchStore.imageMax2m'))
  }
  return isImage && isLt2M
}

/**
 * 上传变化
 */
const handleUploadChange = async (info, field) => {
  const { file } = info

  // 上传成功后更新表单数据
  if (file.status === 'done' && file.response) {
    saveObject[field] = file.response.data
    message.success(t('common.uploadSuccess'))
  } else if (file.status === 'error') {
    message.error(t('common.uploadFailed'))
  }
}

/**
 * 预览图片
 */
const handlePreview = (file) => {
  previewImage.value = file.url || file.thumbUrl
  previewOpen.value = true
}

/**
 * 取消预览
 */
const handleCancelPreview = () => {
  previewOpen.value = false
}

/**
 * 提交表单
 */
const handleSubmit = async () => {
  try {
    await infoForm.value.validate()

    loading.value = true

    const data = { ...saveObject }

    // 提交数据
    if (isAdd.value) {
      await mchStoreApi.add(data)
      message.success(t('common.addSuccess'))
    } else {
      await mchStoreApi.updateById(props.recordId, data)
      message.success(t('common.editSuccess'))
    }

    handleClose()
    emit('success')
  } catch (error) {
    if (error.errorFields) {
      // 表单验证失败
      return
    }
    console.error('提交失败:', error)
    message.error(error.msg || t('common.operationFailed'))
  } finally {
    loading.value = false
  }
}

/**
 * 关闭抽屉
 */
const handleClose = () => {
  emit('update:open', false)
}
</script>

<style lang="less" scoped>
:deep(.ant-upload-list-picture-card-container) {
  width: 100%;
  height: 100%;
}
</style>
