<template>
  <a-drawer
    :maskClosable="false"
    :visible="visible"
    :title=" isAdd ? '新增门店' : '修改门店' "
    @close="onClose"
    :body-style="{ paddingBottom: '80px' }"
    width="60%"
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
            <a-input placeholder="请输入门店名称" v-model="saveObject.storeName"/>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="联系人电话" prop="contactPhone">
            <a-input placeholder="请输入联系人电话" v-model="saveObject.contactPhone"/>
          </a-form-model-item>
        </a-col>
      </a-row>

      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="门店LOGO" prop="storeLogo">
            <AgUpload
              :action="action"
              bind-name="storeLogo"
              :urls="[this.saveObject.storeLogo]"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="门头照" prop="storeOuterImg">
            <AgUpload
              :action="action"
              bind-name="storeOuterImg"
              :urls="[this.saveObject.storeOuterImg]"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="门店内景照" prop="storeInnerImg">
            <AgUpload
              :action="action"
              bind-name="storeInnerImg"
              :urls="[this.saveObject.storeInnerImg]"
              @uploadSuccess="uploadSuccess"
            >
              <template slot="uploadSlot" slot-scope="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </AgUpload>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" style="height: 70px" placeholder="请输入备注" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>
      <!--<a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" placeholder="请输入备注" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>-->
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left"></a-divider>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="10">
          <a-form-model-item label="选址省/市/区" prop="areas">
            <a-cascader placeholder="请选择省市区" :options="areasOptions" v-model="areas" @change="areasChange" />
            <!--<a-cascader placeholder="请选择省市区" :options="areasOptions" :value="[saveObject.provinceCode, saveObject.cityCode, saveObject.areaCode]" @change="areasChange" />-->
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="具体位置" prop="remark">
            <a-input id="address" v-model="saveObject.address" />
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="经纬度" prop="lngLat">
            <a-input v-model="lnglat" @change="lngLatChange" disabled="disabled" />
            <!--<a-input :value="saveObject.lng?.length || saveObject.lat?.length ? saveObject.lng + ',' + saveObject.lat : ''" @change="lngLatChange" />-->
          </a-form-model-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-collapse :activeKey="1">
            <a-collapse-panel key="1" header="地图选址">
              <div id="amap-container"></div>
            </a-collapse-panel>
          </a-collapse>
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
import { API_URL_MCH_STORE, API_URL_MCH_LIST, req, upload, getMapConfig } from '@/api/manage'
import AgUpload from '@/components/AgUpload/AgUpload'
import AMapLoader from '@amap/amap-jsapi-loader'
import 'viewerjs/dist/viewer.css'

export default {
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  components: {
    AgUpload
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
      polygons: [],
      areasOptions: [],
      areas: [],
      lnglat: null,
      action: upload.form, // 上传文件地址
      rules: {
        storeName: [{ required: true, message: '请输入门店名称', trigger: 'blur' }],
        mchNo: [{ validator: checkMchNo, trigger: 'blur' }],
        contactPhone: [{ required: true, pattern: /^1\d{10}$/, message: '请输入正确的手机号', trigger: 'blur' }]
      }
    }
  },
  mounted () {
    // this.initAMap()
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.isAdd = !recordId
      this.areas = []
      this.lnglat = null
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
          that.initAMap()
        })
        this.visible = true
      } else {
        that.visible = true // 立马展示弹层信息
        that.initAMap()
      }
    },
    // DOM初始化完成进行地图初始化
    initAMap: function () {
      const that = this
      getMapConfig().then(res => {
        // console.log(res)
        that.mapConfig = res
        window._AMapSecurityConfig.securityJsCode = that.mapConfig.apiMapWebSecret
        AMapLoader.load({
          key: that.mapConfig.apiMapWebKey, // 申请好的Web端开发者Key，首次调用 load 时必填
          language: 'zh_cn',
          version: '2.0', // 指定要加载的 JSAPI 的版本，缺省时默认为 1.4.15
          plugins: ['AMap.ElasticMarker', 'AMap.ToolBar', 'AMap.Scale', 'AMap.Geolocation', 'AMap.PlaceSearch', 'AMap.AutoComplete', 'AMap.DistrictSearch'], // 需要使用的的插件列表，如比例尺'AMap.Scale'等
          resizeEnable: true,
          AMapUI: {
            version: '1.1',
            plugins: []
          },
          Loca: {
            version: '2.0'
          }
        }).then((AMap) => {
          that.amap = AMap
          that.map = new AMap.Map('amap-container', // 设置地图容器id
              {
                // viewMode: '2D', // 是否为3D地图模式
                zoom: 8.5, // 初始化地图级别
                // zooms: [2, 22],
                center: [116.397455, 39.909187] // 初始化地图中心点位置
              })

          // 输入提示
          const auto = new AMap.AutoComplete({
            input: 'address'
          })
          // // 构造地点查询类
          // const placeSearch = new AMap.PlaceSearch({
          //   map: that.map
          // })
          // 注册监听，当选中某条记录时会触发
          auto.on('select', function (e) {
            // console.log(e)
            // placeSearch.setCity(e.poi.adcode)
            // placeSearch.search(e.poi.name) // 关键字查询查询
            const lnglat = { lng: e.poi.location.lng, lat: e.poi.location.lat }
            that.aMapGeocode(that, lnglat, e.poi.name)
          })

          // 工具条
          const toolBar = new AMap.ToolBar({ // toolBar
            // 这里可以添加自己想要的参数  ，上面有官方文档的链接
            position: 'LT', // LT:左上角;RT:右上角;LB:左下角;RB:右下角;默认位置：LT
            autoPosition: true, // 是否自动定位  默认为false
            locate: false, // 是否显示定位按钮，默认为 true
            ruler: false
          })
          // 在地图上显示工具条方法
          that.map.addControl(toolBar)

          // 比例尺
          const scale = new AMap.Scale()
          that.map.addControl(scale)

          // 定位
          const geolocation = new AMap.Geolocation({
            enableHighAccuracy: true, // 是否使用高精度定位，默认:true
            timeout: 10000, // 超过10秒后停止定位，默认：无穷大
            maximumAge: 0, // 定位结果缓存0毫秒，默认：0
            convert: true, // 自动偏移坐标，偏移后的坐标为高德坐标，默认：true
            showButton: true, // 显示定位按钮，默认：true
            buttonPosition: 'LB', // 定位按钮停靠位置，默认：'LB'，左下角
            buttonOffset: new AMap.Pixel(10, 20), // 定位按钮与设置的停靠位置的偏移量，默认：Pixel(10, 20)
            showMarker: false, // 定位成功后在定位到的位置显示点标记，默认：true
            showCircle: true, // 定位成功后用圆圈表示定位精度范围，默认：true
            panToLocation: true, // 定位成功后将定位到的位置作为地图中心点，默认：true
            zoomToAccuracy: true // 定位成功后调整地图视野范围使定位位置及精度范围视野内可见，默认：false
          })
          that.map.addControl(geolocation)

          that.map.on('click', function (ev) {
            // console.log(ev)
            // 触发事件的地理坐标，AMap.LngLat 类型
            const lnglat = ev.lnglat
            that.aMapGeocode(that, lnglat)
          })

          // 行政区划查询
          const opts = {
            level: 'country', // 关键字对应的行政区级别，country表示国家
            subdistrict: 3, // 显示下级行政区级数（行政区级别包括：国家、省/直辖市、市、区/县4个级别），商圈为区/县下一 级，可选值：0、1、2、3，默认值：1 0：不返回下级行政区 1：返回下一级行政区 2：返回下两级行政区 3：返回下三级行政区
            showbiz: false, // 是否显示商圈，默认值true
            extensions: 'base' // 是否返回行政区边界坐标点，默认值：base
          }
          that.district = new AMap.DistrictSearch(opts) // 注意：需要使用插件同步下发功能才能这样直接使用
          that.district.search('中国', function (status, result) {
            // console.log(status)
            // console.log(result)
            if (status === 'complete') {
              // console.log(that.genDistrictList(result.districtList, null))
              that.setAreasData(result.districtList[0])
            }
          })

          if (!this.isAdd && that.saveObject.lng?.length && that.saveObject.lat?.length) {
            const lnglat = { lng: that.saveObject.lng, lat: that.saveObject.lat }
            const areas = [that.saveObject.provinceCode, that.saveObject.cityCode, that.saveObject.areaCode]
            that.aMapMarker(that, lnglat, that.saveObject.address, areas)
          }
        }).catch(e => {
          console.log(e)
        })
      })
    },
    setCode: function (code) {
      const that = this
      const node = that.getNodeById(that.areasOptions, code)
      switch (node.level) {
        case 'province':
          that.saveObject.provinceCode = node.value
          break
        case 'city':
          that.saveObject.cityCode = node.value
          break
        case 'district':
          that.saveObject.areaCode = node.value
          break
      }
    },
    setAreas: function (value) {
      const that = this
      that.areas = value
      that.saveObject.provinceCode = null
      that.saveObject.cityCode = null
      that.saveObject.areaCode = null
      for (const i in value) {
        if (value[i]?.length) {
          that.setCode(value[i])
        }
      }
      const code = value[value.length - 1]
      const node = that.getNodeById(that.areasOptions, code)
      // console.log(node)
      return node
    },
    setAreasData: function (data) {
      const that = this
      that.areasOptions = []
      const subList = data.districtList
      if (subList) {
        // console.log(subList)
        that.areasOptions = that.genAreasOption(subList)
        // console.log(that.areasOptions)
      }
    },
    genDistrictList: function (data, pcode) {
      let options = []
      for (const i in data?.sort((a, b) => a.adcode - b.adcode)) {
        const item = data[i]
        // console.log(item)
        const optionItem = {
          code: item.adcode,
          name: item.name,
          level: item.level,
          lng: item.center.lng,
          lat: item.center.lat,
          city_code: item.citycode,
          parent_code: pcode
        }
        options.push(optionItem)
        const subOptions = this.genDistrictList(item.districtList, item.adcode)
        options = options.concat(subOptions)
      }
      // console.log(options)
      return options
    },
    genAreasOption: function (data) {
      const options = []
      for (const i in data?.sort((a, b) => a.adcode - b.adcode)) {
        const item = data[i]
        // console.log(item)
        const optionItem = {
          value: item.adcode,
          label: item.name,
          level: item.level,
          citycode: item.citycode
        }
        if (item?.districtList?.length && (item.level === 'province' || item.level === 'city')) {
          const subOptions = this.genAreasOption(item.districtList)
          // console.log(subOptions)
          optionItem.children = subOptions
        }
        if (item.level === 'province' || item.level === 'city' || item.level === 'district') {
          options.push(optionItem)
        }
      }
      // console.log(options)
      return options
    },
    getParentIds: function (treeData, id) {
      const that = this
      let str = ''
      const joinStr = ','

      for (const i in treeData) {
        const item = treeData[i]
        if (item.value === id) {
          return item.value
        }
        if (item.children) {
          str = item.value + joinStr + that.getParentIds(item.children, id)
          if (str === item.value + joinStr) {
            str = ''
          } else {
            return str
          }
        }
      }
      return str
    },
    // https://www.cnblogs.com/wangliko/p/14271202.html
    // 根据ID获取该节点的所有父节点的对象
    getAllParentBySubId: function (list, id) {
      const that = this
      for (const i in list) {
        if (list[i].value === id) {
          return [list[i]]
        }
        if (list[i].children) {
          const node = that.getAllParentBySubId(list[i].children, id)
          if (node !== undefined) {
            return node.concat(list[i])
          }
        }
      }
    },
    // 根据ID获取该节点的对象
    getNodeById: function (list, id) {
      const that = this
      const nodes = that.getAllParentBySubId(list, id)
      for (const i in nodes) {
        if (nodes[i].value === id) {
          return nodes[i]
        }
      }
    },
    // 根据ID获取所有子节点的对象
    getAllNodeById: function (list, newNodeId = []) {
      const that = this
      for (const i in list) {
        newNodeId.push(list[i])
        if (list[i].children) {
          that.getAllNodeById(list[i].children, newNodeId)
        }
      }
      return newNodeId
    },
    aMapGeocode: function (that, lnglat, address) {
      that.$jsonp('https://restapi.amap.com/v3/geocode/regeo', {
        platform: 'JS',
        key: that.mapConfig.apiMapWebKey,
        jscode: that.mapConfig.apiMapWebSecret,
        language: 'zh_cn',
        location: lnglat.lng + ',' + lnglat.lat,
        s: 'rsv3'
      }).then(res => {
        // console.log(res)
        address = address?.length ? address : res.regeocode.formatted_address
        const areas = that.getParentIds(that.areasOptions, res.regeocode.addressComponent.adcode)
        // console.log(areas.split(','))
        that.aMapMarker(that, lnglat, address, areas.split(','))
      })
    },
    aMapMarker: function (that, lnglat, address, areas) {
      if (areas?.filter(d => d)?.length) {
        if (that.marker) {
          that.map.remove(that.marker)
        }
        that.saveObject.address = address
        that.saveObject.lng = lnglat.lng
        that.saveObject.lat = lnglat.lat
        that.lnglat = that.saveObject.lng + ',' + that.saveObject.lat

        const AMap = that.amap

        // 创建一个 Marker 实例：
        that.marker = new AMap.Marker({
          position: new AMap.LngLat(lnglat.lng, lnglat.lat), // 经纬度对象，也可以是经纬度构成的一维数组[116.39, 39.9]
          title: address
        })
        // 将创建的点标记添加到已有的地图实例：
        that.map.add(that.marker)
        that.aMapPolygon(areas, lnglat)
        // console.log([this.saveObject.provinceCode, this.saveObject.cityCode, this.saveObject.areaCode])
      }
    },
    aMapPolygon: function (areas, lnglat) {
      const that = this
      // 清除地图上所有覆盖物
      for (let i = 0, l = that.polygons.length; i < l; i++) {
        that.polygons[i].setMap(null)
      }
      const node = that.setAreas(areas)
      that.district.setLevel(node.level) // 行政区级别
      that.district.setExtensions('all')
      // 行政区查询
      // 按照adcode进行查询可以保证数据返回的唯一性
      that.district.search(node.value, function (status, result) {
        if (status) {
          // 获取区域的边界信息
          const bounds = result.districtList[0].boundaries
          const AMap = that.amap
          if (bounds) {
            for (let i = 0, l = bounds.length; i < l; i++) {
              const polygon = new AMap.Polygon({
                map: that.map,
                strokeWeight: 1,
                strokeColor: '#0091ea',
                fillColor: '#80d8ff',
                fillOpacity: 0.2,
                path: bounds[i]
              })
              polygon.on('click', function (ev) {
                // console.log(ev)
                that.aMapGeocode(that, ev.lnglat)
              })
              that.polygons.push(polygon)
            }
            if (lnglat) {
              // 更新地图中心点位置
              that.map.setZoomAndCenter(14, [lnglat.lng, lnglat.lat])
            } else {
              // 地图自适应
              that.map.setFitView()
            }
          }
        }
      })
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
    // 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
    uploadSuccess (name, fileList) {
      console.log({ name, fileList })
      const [firstItem] = fileList
      this.saveObject[name] = firstItem?.url
      this.$forceUpdate()
    },
    areasChange (value, selectedOptions) {
      // console.log(value)
      // console.log(selectedOptions)
      const that = this
      that.saveObject.address = null
      that.lnglat = null
      if (that.marker) {
        that.map.remove(that.marker)
      }
      if (value?.length) {
        that.aMapPolygon(value)
      } else {
        this.saveObject.provinceCode = null
        this.saveObject.cityCode = null
        this.saveObject.areaCode = null

        // 清除地图上所有覆盖物
        for (let i = 0, l = that.polygons.length; i < l; i++) {
          that.polygons[i].setMap(null)
        }
      }
      // console.log([this.saveObject.provinceCode, this.saveObject.cityCode, this.saveObject.areaCode])
    },
    lngLatChange (e) {
      // console.log(e)
      const lngAndLat = e.target.value.split(',')
      if (lngAndLat.length > 1) {
        this.saveObject.lng = lngAndLat[0]
        this.saveObject.lat = lngAndLat[1]
      } else {
        this.saveObject.lng = ''
        this.saveObject.lat = ''
      }
    }
  }
}
</script>

<style lang="less">
  #amap-container {
    padding: 0;
    margin: 0;
    width: 100%;
    height: 600px;
    position: relative;
  }

  .ag-upload-btn {
    height: 66px;
  }
</style>
