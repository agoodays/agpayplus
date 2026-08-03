<template>
  <ag-drawer
    v-model:open="localOpen"
    :title="isAdd ? '新增门店' : '修改门店'"
    width="60%"
    :mask-closable="false"
    @close="handleClose"
    :show-confirm="true"
    :confirm-loading="loading"
    @confirm="handleConfirm"
  >
    <a-form ref="infoForm" :model="saveObject" :rules="rules" layout="vertical">
      <!-- 商户号（仅新增时显示） -->
      <a-row v-if="isAdd" :gutter="16">
        <a-col :span="24">
          <a-form-item label="商户号" name="mchNo">
              <!-- <ag-select
                v-model="saveObject.mchNo"
                placeholder="商户号（搜索商户名称）"
                allow-clear
                :options="mchOptions"
                :show-search="true"
                :filter-option="false"
                @search="handleSearchMch"
              /> -->
            <ag-select-infinite
              v-model="saveObject.mchNo"
              placeholder="请选择商户"
              allow-clear
              :fetch-data="fetchMerchants"
              :field-names="{ label: 'mchName', value: 'mchNo' }"
            >
              <template #option="{ option }">
                {{ option.mchName }}[{{ option.mchNo }}]
              </template>
            </ag-select-infinite>
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 门店基本信息 -->
      <a-row :gutter="16">
        <a-col :span="10">
          <a-form-item label="门店名称" name="storeName">
            <a-input v-model:value="saveObject.storeName" placeholder="请输入门店名称" />
          </a-form-item>
        </a-col>

        <a-col :span="10">
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
              bind-name="storeLogo"
              :action="action"
              :before-upload="beforeUpload"
              :urls="[saveObject.storeLogo]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>

        <a-col :span="8">
          <a-form-item label="门头照" name="storeOuterImg">
            <ag-upload
              bind-name="storeOuterImg"
              :action="action"
              :before-upload="beforeUpload"
              :urls="[saveObject.storeOuterImg]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>

        <a-col :span="8">
          <a-form-item label="门店内景照" name="storeInnerImg">
            <ag-upload
              bind-name="storeInnerImg"
              :action="action"
              :before-upload="beforeUpload"
              :urls="[saveObject.storeInnerImg]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{ loading }">
                <a-button class="ag-upload-btn"> <component :is="loading ? icons.LoadingOutlined : icons.UploadOutlined" /> 上传 </a-button>
              </template>
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
        <a-col :span="10">
          <a-form-item label="选址省/市/区" name="areas">
            <a-cascader
              v-model:value="areas"
              :options="areaOptions"
              placeholder="请选择省市区"
              @change="handleAreaChange"
            />
          </a-form-item>
        </a-col>

        <a-col :span="10">
          <a-form-item label="具体位置" name="address">
            <a-input id="address" v-model:value="saveObject.address" placeholder="请输入详细地址" />
          </a-form-item>
        </a-col>

        <a-col :span="4">
          <a-form-item label="经纬度" name="lnglat">
            <a-input v-model:value="lnglat" disabled />
          </a-form-item>
        </a-col>
      </a-row>

      <!-- 地图选址 -->
      <a-row :gutter="16">
        <a-col :span="24">
          <a-collapse :activeKey="mapActiveKey">
            <a-collapse-panel key="1" header="地图选址">
              <div id="amap-container"></div>
            </a-collapse-panel>
          </a-collapse>
        </a-col>
      </a-row>
    </a-form>

    <!-- 图片预览 -->
    <a-modal v-model:open="previewOpen" :footer="null" @cancel="handleCancelPreview">
      <img :src="previewImage" style="width: 100%" alt="preview" />
    </a-modal>
  </ag-drawer>
</template>

<script setup>
/**
 * 门店新增/编辑组件
 * 功能：门店信息的新增和编辑，包含地图选址功能
 * 使用高德地图API实现地图选址、地址搜索、地理编码等功能
 */
import { mchStoreApi } from '@/api/business/mch-store/mch-store-api'
import { basicApi } from '@/api/system/basic-api'
import { AgDrawer, AgSelectInfinite, AgUpload } from '@/components'
import { upload } from '@/lib/ag-axios'
import AMapLoader from '@amap/amap-jsapi-loader'
import { LoadingOutlined, UploadOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, nextTick, onUnmounted, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
/** 图标集合 */
const icons = { LoadingOutlined, UploadOutlined }

const { t } = useI18n()

/** Props 定义 */
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

/** 组件事件定义 */
const emit = defineEmits(['update:open', 'success'])

/** 表单引用 */
const infoForm = ref(null)

/** 上传地址 */
const action = upload.form

/** 加载状态 */
const loading = ref(false)

/** 是否为新增模式 */
const isAdd = ref(true)

/** 本地抽屉打开状态 */
const localOpen = ref(false)

/** 省市区级联数据 */
const areaOptions = ref([])

/** 选中的省市区 */
const areas = ref([])

/** 经纬度显示 */
const lnglat = ref('')

/** 地图折叠状态 */
const mapActiveKey = ref(['1'])

/** 图片预览状态 */
const previewOpen = ref(false)
const previewImage = ref('')

/** 地图相关实例 */
const map = ref(null)
const marker = ref(null)
const amap = ref(null)
const district = ref(null)
const polygons = ref([])
const mapConfig = ref(null)

/** 表单数据 */
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

/** 表单验证规则 */
const rules = {
  mchNo: [{ required: true, message: '请选择商户', trigger: 'change' }],
  storeName: [{ required: true, message: '请输入门店名称', trigger: 'blur' }],
  contactPhone: [{ required: true, pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }],
  storeLogo: [{ required: true, message: '请上传门店LOGO', trigger: 'change' }],
  storeOuterImg: [{ required: true, message: '请上传门头照', trigger: 'change' }],
  storeInnerImg: [{ required: true, message: '请上传门店内景照', trigger: 'change' }],
  areas: [{ required: true, message: '请选择省市区', trigger: 'change', type: 'array' }],
  lnglat: [{ required: true, message: '请输入经纬度', trigger: 'blur' }],
  address: [{ required: true, message: '请输入详细地址', trigger: 'blur' }]
}

/**
 * 监听 props.open 变化，控制抽屉显示
 */
watch(
  () => props.open,
  async (val) => {
    localOpen.value = val
    if (val) {
      await initForm()
    }
  }
)

/**
 * 监听 localOpen 变化，同步到父组件
 */
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

    // 设置经纬度显示
    if (res.lng && res.lat) {
      lnglat.value = `${res.lng},${res.lat}`
    }

    // 初始化地图
    await nextTick()
    initAMap()
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
  lnglat.value = ''

  nextTick(() => {
    infoForm.value?.clearValidate()
    initAMap()
  })
}

const mchList = ref([])

/**
 * 商户选项（用于下拉选择）
 */
const mchOptions = computed(() => {
  return mchList.value.map(item => ({
    value: item.mchNo,
    label: item.mchName
  }))
})

/**
 * 搜索商户
 * @param {string} keyword - 搜索关键词
 */
const handleSearchMch = async (keyword) => {
  if (!keyword) {
    mchList.value = []
    return
  }

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
 * 获取商户列表（用于 ag-select-infinite 组件）
 * @param {Object} params - 查询参数
 * @param {Number} params.pageNumber - 当前页码
 * @param {Number} params.pageSize - 每页数量
 * @param {String} params.keyword - 搜索关键词
 * @returns {Promise<Object>} 返回数据格式：{ records: [], total: 0 }
 */
const fetchMerchants = async ({ pageNumber, pageSize, keyword }) => {
  try {
    const res = await mchStoreApi.queryMchPage({
      mchName: keyword,
      pageNumber,
      pageSize
    })
    return {
      records: res.records || [],
      total: res.total || 0
    }
  } catch (error) {
    console.error('获取商户列表失败:', error)
    return {
      records: [],
      total: 0
    }
  }
}

/**
 * 省市区变化处理
 */
const handleAreaChange = (value) => {
  saveObject.address = ''
  lnglat.value = ''

  // 移除地图标记
  if (marker.value) {
    map.value.remove(marker.value)
    marker.value = null
  }

  if (value && value.length) {
    // 设置省市区代码
    setAreas(value)
    // 绘制行政区划边界
    aMapPolygon(value)
  } else {
    // 清空省市区代码
    saveObject.provinceCode = ''
    saveObject.cityCode = ''
    saveObject.districtCode = ''
    // 清除地图上所有覆盖物
    clearPolygons()
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
 * 上传成功处理
 * @param {String} name - 绑定的字段名
 * @param {Array} fileList - 文件列表
 */
function uploadSuccess(name, fileList) {
  const [firstItem] = fileList
  saveObject[name] = firstItem?.url
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
const handleConfirm = async () => {
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

/**
 * 组件卸载时清理地图资源
 */
onUnmounted(() => {
  clearPolygons()
  if (map.value) {
    map.value.destroy()
    map.value = null
  }
})

/**
 * 初始化高德地图
 */
const initAMap = async () => {
  try {
    // 获取地图配置
    mapConfig.value = await basicApi.getMapConfig()

    // 设置安全密钥
    window._AMapSecurityConfig = {
      securityJsCode: mapConfig.value.apiMapWebSecret
    }

    // 加载高德地图API
    amap.value = await AMapLoader.load({
      key: mapConfig.value.apiMapWebKey,
      language: 'zh_cn',
      version: '2.0',
      plugins: ['AMap.ElasticMarker', 'AMap.ToolBar', 'AMap.Scale', 'AMap.Geolocation', 'AMap.PlaceSearch', 'AMap.AutoComplete', 'AMap.DistrictSearch'],
      resizeEnable: true,
      AMapUI: {
        version: '1.1',
        plugins: []
      },
      Loca: {
        version: '2.0'
      }
    })

    // 创建地图实例
    map.value = new amap.value.Map('amap-container', {
      zoom: 8.5,
      center: [116.397455, 39.909187]
    })

    // 添加工具条
    const toolBar = new amap.value.ToolBar({
      position: 'LT',
      autoPosition: true,
      locate: false,
      ruler: false
    })
    map.value.addControl(toolBar)

    // 添加比例尺
    const scale = new amap.value.Scale()
    map.value.addControl(scale)

    // 添加定位控件
    const geolocation = new amap.value.Geolocation({
      enableHighAccuracy: true,
      timeout: 10000,
      maximumAge: 0,
      convert: true,
      showButton: true,
      buttonPosition: 'LB',
      buttonOffset: new amap.value.Pixel(10, 20),
      showMarker: false,
      showCircle: true,
      panToLocation: true,
      zoomToAccuracy: true
    })
    map.value.addControl(geolocation)

    // 地址输入提示
    const autoComplete = new amap.value.AutoComplete({
      input: 'address'
    })

    // 监听地址选择
    autoComplete.on('select', (e) => {
      const lnglat = { lng: e.poi.location.lng, lat: e.poi.location.lat }
      aMapGeocode(lnglat, e.poi.name)
    })

    // 监听地图点击事件
    map.value.on('click', (ev) => {
      const lnglat = ev.lnglat
      aMapGeocode(lnglat)
    })

    // 初始化行政区划查询
    const opts = {
      level: 'country',
      subdistrict: 3,
      showbiz: false,
      extensions: 'base'
    }
    district.value = new amap.value.DistrictSearch(opts)

    // 查询中国行政区划，生成省市区级联数据
    district.value.search('中国', (status, result) => {
      if (status === 'complete') {
        setAreasData(result.districtList[0])
      }
    })

    // 如果是编辑模式且有经纬度，设置地图标记
    if (!isAdd.value && saveObject.lng && saveObject.lat) {
      const lnglat = { lng: saveObject.lng, lat: saveObject.lat }
      const areas = [saveObject.provinceCode, saveObject.cityCode, saveObject.districtCode]
      aMapMarker(lnglat, saveObject.address, areas)
    }
  } catch (error) {
    console.error('初始化高德地图失败:', error)
    message.error('地图加载失败，请稍后重试')
  }
}

/**
 * 设置省市区级联数据
 * @param {Object} data - 行政区划数据
 */
const setAreasData = (data) => {
  areaOptions.value = []
  const subList = data.districtList
  if (subList) {
    areaOptions.value = genAreasOption(subList)
  }
}

/**
 * 生成省市区级联选项
 * @param {Array} data - 行政区划数据列表
 * @returns {Array} 级联选项数组
 */
const genAreasOption = (data) => {
  const options = []
  for (const i in data?.sort((a, b) => a.adcode - b.adcode)) {
    const item = data[i]
    const optionItem = {
      value: item.adcode,
      label: item.name,
      level: item.level,
      citycode: item.citycode
    }
    if (item?.districtList?.length && (item.level === 'province' || item.level === 'city')) {
      const subOptions = genAreasOption(item.districtList)
      optionItem.children = subOptions
    }
    if (item.level === 'province' || item.level === 'city' || item.level === 'district') {
      options.push(optionItem)
    }
  }
  return options
}

/**
 * 设置省市区代码
 * @param {Array} value - 选中的省市区值数组
 */
const setAreas = (value) => {
  saveObject.provinceCode = ''
  saveObject.cityCode = ''
  saveObject.districtCode = ''

  for (const i in value) {
    if (value[i]) {
      const node = getNodeById(areaOptions.value, value[i])
      switch (node?.level) {
        case 'province':
          saveObject.provinceCode = node.value
          break
        case 'city':
          saveObject.cityCode = node.value
          break
        case 'district':
          saveObject.districtCode = node.value
          break
      }
    }
  }

  const code = value[value.length - 1]
  return getNodeById(areaOptions.value, code)
}

/**
 * 根据ID获取节点
 * @param {Array} list - 树形列表
 * @param {string} id - 节点ID
 * @returns {Object} 节点对象
 */
const getNodeById = (list, id) => {
  const nodes = getAllParentBySubId(list, id)
  for (const i in nodes) {
    if (nodes[i].value === id) {
      return nodes[i]
    }
  }
}

/**
 * 根据子节点ID获取所有父节点
 * @param {Array} list - 树形列表
 * @param {string} id - 子节点ID
 * @returns {Array} 父节点数组
 */
const getAllParentBySubId = (list, id) => {
  for (const i in list) {
    if (list[i].value === id) {
      return [list[i]]
    }
    if (list[i].children) {
      const node = getAllParentBySubId(list[i].children, id)
      if (node !== undefined) {
        return node.concat(list[i])
      }
    }
  }
}

/**
 * 获取父节点ID路径
 * @param {Array} treeData - 树形数据
 * @param {string} id - 节点ID
 * @returns {string} 父节点ID路径，用逗号分隔
 */
const getParentIds = (treeData, id) => {
  let str = ''
  const joinStr = ','

  for (const i in treeData) {
    const item = treeData[i]
    if (item.value === id) {
      return item.value
    }
    if (item.children) {
      str = item.value + joinStr + getParentIds(item.children, id)
      if (str === item.value + joinStr) {
        str = ''
      } else {
        return str
      }
    }
  }
  return str
}

/**
 * 地理编码（逆地理编码）
 * @param {Object} lnglat - 经纬度对象
 * @param {string} address - 地址名称（可选）
 */
const aMapGeocode = async (lnglat, address) => {
  try {
    const res = await fetchJsonp('https://restapi.amap.com/v3/geocode/regeo', {
      params: {
        platform: 'JS',
        key: mapConfig.value.apiMapWebKey,
        jscode: mapConfig.value.apiMapWebSecret,
        language: 'zh_cn',
        location: `${lnglat.lng},${lnglat.lat}`,
        s: 'rsv3'
      }
    })

    if (res.status === '1') {
      const formattedAddress = address || res.regeocode.formatted_address
      const areaIds = getParentIds(areaOptions.value, res.regeocode.addressComponent.adcode)
      aMapMarker(lnglat, formattedAddress, areaIds.split(','))
    } else {
      message.error('地理编码失败')
    }
  } catch (error) {
    console.error('地理编码失败:', error)
    message.error('地理编码失败，请重试')
  }
}

/**
 * 在地图上添加标记
 * @param {Object} lnglat - 经纬度对象
 * @param {string} address - 地址名称
 * @param {Array} areas - 省市区ID数组
 */
const aMapMarker = (lnglat, address, areas) => {
  if (areas?.filter(d => d).length) {
    // 移除旧标记
    if (marker.value) {
      map.value.remove(marker.value)
    }

    // 更新表单数据
    saveObject.address = address
    saveObject.lng = lnglat.lng
    saveObject.lat = lnglat.lat
    lnglat.value = `${saveObject.lng},${saveObject.lat}`

    // 创建新标记
    marker.value = new amap.value.Marker({
      position: new amap.value.LngLat(lnglat.lng, lnglat.lat),
      title: address
    })

    // 添加标记到地图
    map.value.add(marker.value)

    // 更新省市区选择
    aMapPolygon(areas, lnglat)
  }
}

/**
 * 绘制行政区划边界
 * @param {Array} areas - 省市区ID数组
 * @param {Object} lnglat - 经纬度对象（可选）
 */
const aMapPolygon = (areas, lnglat) => {
  // 清除旧的边界
  clearPolygons()

  // 设置省市区代码
  const node = setAreas(areas)

  if (!node || !district.value) return

  // 设置行政区划级别
  district.value.setLevel(node.level)
  district.value.setExtensions('all')

  // 查询行政区划边界
  district.value.search(node.value, (status, result) => {
    if (status && result.districtList[0]?.boundaries) {
      const bounds = result.districtList[0].boundaries
      for (let i = 0, l = bounds.length; i < l; i++) {
        const polygon = new amap.value.Polygon({
          map: map.value,
          strokeWeight: 1,
          strokeColor: '#0091ea',
          fillColor: '#80d8ff',
          fillOpacity: 0.2,
          path: bounds[i]
        })
        polygon.on('click', (ev) => {
          aMapGeocode(ev.lnglat)
        })
        polygons.value.push(polygon)
      }

      // 更新地图视野
      if (lnglat) {
        map.value.setZoomAndCenter(14, [lnglat.lng, lnglat.lat])
      } else {
        map.value.setFitView()
      }
    }
  })
}

/**
 * 清除地图上的行政区划边界
 */
const clearPolygons = () => {
  for (let i = 0, l = polygons.value.length; i < l; i++) {
    polygons.value[i].setMap(null)
  }
  polygons.value = []
}

/**
 * JSONP请求封装
 * @param {string} url - 请求URL
 * @param {Object} options - 请求选项
 * @returns {Promise} Promise对象
 */
const fetchJsonp = (url, options = {}) => {
  return new Promise((resolve, reject) => {
    const callbackName = `jsonp_${Date.now()}_${Math.random().toString(36).substr(2)}`
    const params = new URLSearchParams(options.params || {})
    params.set('callback', callbackName)

    const script = document.createElement('script')
    script.src = `${url}?${params.toString()}`
    script.type = 'text/javascript'

    window[callbackName] = (data) => {
      try {
        resolve(data)
      } finally {
        delete window[callbackName]
        document.body.removeChild(script)
      }
    }

    script.onerror = (error) => {
      delete window[callbackName]
      document.body.removeChild(script)
      reject(error)
    }

    document.body.appendChild(script)
  })
}
</script>

<style lang="less" scoped>
/** 地图容器样式 */
#amap-container {
  padding: 0;
  margin: 0;
  width: 100%;
  height: 500px;
  position: relative;
}

/** 上传按钮样式 */
:deep(.ant-upload-list-picture-card-container) {
  width: 100%;
  height: 100%;
}

/** 抽屉按钮居中 */
:deep(.drawer-btn-center) {
  z-index: 200;
}
</style>
