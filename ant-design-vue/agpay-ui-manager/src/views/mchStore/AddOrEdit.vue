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
        <a-col :span="10">
          <a-form-model-item label="备注" prop="remark">
            <a-input v-model="saveObject.remark" style="height: 70px" placeholder="请输入备注" type="textarea" />
          </a-form-model-item>
        </a-col>
      </a-row>
<!--      <a-row justify="space-between" type="flex">
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
<!--            <a-cascader placeholder="请选择省市区" :options="areasOptions" :value="[saveObject.provinceCode, saveObject.cityCode, saveObject.areaCode]" @change="areasChange" />-->
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
<!--            <a-input :value="saveObject.lng?.length || saveObject.lat?.length ? saveObject.lng + ',' + saveObject.lat : ''" @change="lngLatChange" />-->
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
import AMapLoader from '@amap/amap-jsapi-loader'
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
      areasOptions: [
        {
          value: '110000',
          label: '北京市',
          children: [
            {
              value: '110100',
              label: '北京市',
              children: [
                {
                  value: '110105',
                  label: '朝阳区',
                  code: 110105
                }
              ]
            }
          ]
        },
        {
          value: '120000',
          label: '天津市',
          children: [
            {
              value: '120100',
              label: '天津市',
              children: [
                {
                  value: '120101',
                  label: '和平区',
                  code: 120101
                }
              ]
            }
          ]
        }
      ],
      areas: [],
      lnglat: null,
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
  mounted () {
    // this.initAMap()
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
          that.initAMap()
          that.areas = [that.saveObject.provinceCode, that.saveObject.cityCode, that.saveObject.areaCode]
          if (that.saveObject.lng?.length || that.saveObject.lat?.length) {
            that.lnglat = that.saveObject.lng + ',' + that.saveObject.lat
          }
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
        that.initAMap()
      }
    },
    // DOM初始化完成进行地图初始化
    initAMap: function () {
      const that = this

      getMapConfig().then(res => {
        console.log(res)
        that.mapConfig = res

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
            console.log(e)
            // placeSearch.setCity(e.poi.adcode)
            // placeSearch.search(e.poi.name) // 关键字查询查询
            const lnglat = { lng: e.poi.location.lng, lat: e.poi.location.lat }
            that.aMapMarker(that, lnglat, e.poi.name)
          })
          // that.map.addControl(auto)

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

          if (that.saveObject.lng?.length && that.saveObject.lat?.length) {
            // 创建一个 Marker 实例：
            that.marker = new that.amap.Marker({
              position: new that.amap.LngLat(that.saveObject.lng.length, that.saveObject.lat), // 经纬度对象，也可以是经纬度构成的一维数组[116.39, 39.9]
              title: that.saveObject.address
            })
            // 将创建的点标记添加到已有的地图实例：
            that.map.add(that.marker)
          }

          that.map.on('click', function (ev) {
            // // 触发事件的对象
            // const target = ev.target
            // console.log(target)
            // 触发事件的地理坐标，AMap.LngLat 类型
            const lnglat = ev.lnglat
            // console.log(lnglat)
            // console.log(lnglat.lng)
            // console.log(lnglat.lat)
            // // 触发事件的像素坐标，AMap.Pixel 类型
            // const pixel = ev.pixel
            // console.log(pixel)
            // // 触发事件类型
            // const type = ev.type
            // console.log(type)

            // that.$jsonp('https://restapi.amap.com/v3/geocode/regeo?platform=JS&s=rsv3&logversion=2.0&key=6cebea39ba50a4c9bc565baaf57d1c8b&sdkversion=2.0.5.14&appname=https://mgr.s.jeepay.com/store&csid=93D8C382-D595-412A-8612-FE3A08DEE2C0&jscode=dccbb5a56d2a1850eda2b6e67f8f2f13&key=6cebea39ba50a4c9bc565baaf57d1c8b&s=rsv3&language=zh_cn&location=116.448763,39.955928')
            //     .then(res => {
            //       console.log(res)
            //     })

            that.$jsonp('https://restapi.amap.com/v3/geocode/regeo', {
              platform: 'JS',
              key: that.mapConfig.apiMapWebKey,
              // jscode: 'dccbb5a56d2a1850eda2b6e67f8f2f13',
              language: 'zh_cn',
              location: lnglat.toString(),
              s: 'rsv3'
            }).then(res => {
              console.log(res)
              that.aMapMarker(that, lnglat, res.regeocode.formatted_address)
            })
          })

          // 行政区划查询
          const opts = {
            subdistrict: 1, // 返回下一级行政区
            showbiz: false // 最后一级返回街道信息
          }
          const district = new AMap.DistrictSearch(opts) // 注意：需要使用插件同步下发功能才能这样直接使用
          district.search('中国', function (status, result) {
            if (status === 'complete') {
              console.log(result.districtList[0])
            }
          })

          if (!this.isAdd) {
            const lnglat = { lng: that.saveObject.lng, lat: that.saveObject.lat }
            that.aMapMarker(that, lnglat, that.saveObject.address)
          }
        }).catch(e => {
          console.log(e)
        })
      })
    },
    aMapMarker: function (that, lnglat, address) {
      console.log(lnglat)
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
    },
    areasChange (value, selectedOptions) {
      console.log(value)
      console.log(selectedOptions)
      if (value.length > 2) {
        this.saveObject.provinceCode = value[0]
        this.saveObject.cityCode = value[1]
        this.saveObject.areaCode = value[2]
      } else {
        this.saveObject.provinceCode = ''
        this.saveObject.cityCode = ''
        this.saveObject.areaCode = ''
      }
      console.log(this.saveObject.provinceCode)
      console.log(this.saveObject.cityCode)
      console.log(this.saveObject.areaCode)
    },
    lngLatChange (e) {
      console.log(this.saveObject)
      console.log(e)
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
    padding: 0px;
    margin: 0px;
    width: 100%;
    height: 600px;
    position: relative;
  }

  .upload-list-inline .ant-btn {
    height: 66px;
  }
</style>
