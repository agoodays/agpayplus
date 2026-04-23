<template>
  <a-drawer
    :open="visible"
    :title="isAdd ? '新增支付接口' : '修改支付接口'"
    :mask-closable="false"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%">
    <a-form
      ref="infoForm"
      :model="saveObject"
      layout="vertical"
      :rules="rules">
      <a-row :gutter="16">
        <a-col :span="24">
          <p class="ag-drawer-title">基本配置</p>
        </a-col>
        <a-col :span="12">
          <a-form-item label="接口代码" name="ifCode">
            <a-input v-model:value="saveObject.ifCode" placeholder="请输入" :disabled="!isAdd" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="接口名称" name="ifName">
            <a-input v-model:value="saveObject.ifName" placeholder="请输入" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持普通商户模式" name="isMchMode">
            <a-radio-group v-model:value="saveObject.isMchMode">
              <a-radio :value="1">
                支持
              </a-radio>
              <a-radio :value="0">
                不支持
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持服务商子商户模式" name="isIsvMode">
            <a-radio-group v-model:value="saveObject.isIsvMode">
              <a-radio :value="1">
                支持
              </a-radio>
              <a-radio :value="0">
                不支持
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="支付参数配置页面类型" name="configPageType">
            <a-radio-group v-model:value="saveObject.configPageType">
              <a-radio :value="1">
                根据接口配置定义描述渲染页面
              </a-radio>
              <a-radio :value="2">
                自定义页面
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持进件" name="isSupportApplyment">
            <a-radio-group v-model:value="saveObject.isSupportApplyment">
              <a-radio :value="1">
                支持
              </a-radio>
              <a-radio :value="0">
                不支持
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否开启进件" name="isOpenApplyment">
            <a-radio-group v-model:value="saveObject.isOpenApplyment" :disabled="!saveObject.isSupportApplyment">
              <a-radio :value="1">
                开启
              </a-radio>
              <a-radio :value="0">
                关闭
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持对账" name="isSupportCheckBill">
            <a-radio-group v-model:value="saveObject.isSupportCheckBill">
              <a-radio :value="1">
                支持
              </a-radio>
              <a-radio :value="0">
                不支持
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否开启对账" name="isOpenCheckBill">
            <a-radio-group v-model:value="saveObject.isOpenCheckBill" :disabled="!saveObject.isSupportCheckBill">
              <a-radio :value="1">
                开启
              </a-radio>
              <a-radio :value="0">
                关闭
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否支持提现" name="isSupportCashout">
            <a-radio-group v-model:value="saveObject.isSupportCashout">
              <a-radio :value="1">
                支持
              </a-radio>
              <a-radio :value="0">
                不支持
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="是否开启提现" name="isOpenCashout">
            <a-radio-group v-model:value="saveObject.isOpenCashout" :disabled="!saveObject.isSupportCashout">
              <a-radio :value="1">
                开启
              </a-radio>
              <a-radio :value="0">
                关闭
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isIsvMode == 1 && saveObject.configPageType === 1">
          <a-form-item label="服务商接口配置定义描述" name="isvParams">
            <a-input v-model:value="saveObject.isvParams" placeholder="请输入" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isIsvMode == 1 && saveObject.configPageType === 1">
          <a-form-item label="特约商户接口配置定义描述" name="isvsubMchParams">
            <a-input v-model:value="saveObject.isvsubMchParams" placeholder="请输入" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col :span="24" v-if="saveObject.isMchMode == 1 && saveObject.configPageType === 1">
          <a-form-item label="普通商户接口配置定义描述" name="normalMchParams">
            <a-input v-model:value="saveObject.normalMchParams" placeholder="请输入" type="textarea" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="状态" name="state">
            <a-radio-group v-model:value="saveObject.state">
              <a-radio :value="1">
                启用
              </a-radio>
              <a-radio :value="0">
                停用
              </a-radio>
            </a-radio-group>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="备注" name="remark">
            <a-input v-model:value="saveObject.remark" placeholder="请输入" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left"/>
        </a-col>
        <a-col :span="24">
          <p class="ag-drawer-title">支付方式</p>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="支持的支付方式" name="checkedList">
            <a-checkbox-group v-model:value="checkedList">
              <a-row v-for="(group, index) in groupedWays" :key="index">
                <h3>{{ group.name }}</h3>
                <a-col :span="6" v-for="(way, i) in group.ways" :key="i">
                  <a-checkbox :value="way.wayCode">{{ way.wayName }}</a-checkbox>
                </a-col>
              </a-row>
            </a-checkbox-group>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :span="24">
          <a-divider orientation="left"/>
        </a-col>
        <a-col :span="24">
          <p class="ag-drawer-title">页面展示</p>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="卡片icon" name="icon">
            <ag-upload
              :action="action"
              accept=".jpg, .jpeg, .png"
              bind-name="icon"
              :urls="[saveObject.icon]"
              @upload-success="uploadSuccess"
            >
              <template #uploadSlot="{loading}">
                <a-button class="ag-upload-btn"> <a-icon :type="loading ? 'loading' : 'upload'" /> 上传 </a-button>
              </template>
            </ag-upload>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="卡片背景色" name="bgColor">
            <a-input style="height: 66px; margin-top: 8px;" v-model:value="saveObject.bgColor" placeholder="请输入" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>

    <div class="drawer-btn-center">
      <a-button @click="onClose" icon="close" :style="{ marginRight: '8px' }">取消</a-button>
      <a-button type="primary" @click="onSubmit" icon="check" >保存</a-button>
    </div>

  </a-drawer>
</template>

<script setup>
import { ref, reactive, onMounted, defineProps } from 'vue'
import { API_URL_IFDEFINES_LIST, API_URL_PAYWAYS_LIST, req, upload } from '@/api/manage'

const props = defineProps({
  callbackFunc: { type: Function, default: () => () => ({}) }
})

const infoForm = ref(null)
const visible = ref(false)
const isAdd = ref(true)
const ifCode = ref('')
const action = upload.ifBG

const saveObject = reactive({
  isMchMode: 1,
  isIsvMode: 1,
  state: 1,
  configPageType: 1,
  isSupportApplyment: 0,
  isOpenApplyment: 0,
  isSupportCheckBill: 0,
  isOpenCheckBill: 0,
  isSupportCashout: 0,
  isOpenCashout: 0,
  bgColor: '#1a53ff'
})

const groupedWays = ref([])
const checkedList = ref([])

const rules = reactive({
  ifCode: [{ required: true, message: '请输入接口代码', trigger: 'blur' }],
  ifName: [{ required: true, message: '请输入接口名称', trigger: 'blur' }],
  normalMchParams: [{ validator: (rule, value, callback) => {
    if (saveObject.isMchMode === 1 && saveObject.configPageType === 1 && !value) {
      callback(new Error('请输入普通商户接口配置定义描述'))
    }
    callback()
  }, trigger: 'blur' }],
  isvParams: [{ validator: (rule, value, callback) => {
    if (saveObject.isIsvMode === 1 && saveObject.configPageType === 1 && !value) {
      callback(new Error('请输入服务商接口配置定义描述'))
    }
    callback()
  }, trigger: 'blur' }],
  isvsubMchParams: [{ validator: (rule, value, callback) => {
    if (saveObject.isIsvMode === 1 && saveObject.configPageType === 1 && !value) {
      callback(new Error('请输入特约商户接口配置定义描述'))
    }
    callback()
  }, trigger: 'blur' }],
  checkedList: [{ required: true, validator: (rule, value, callback) => {
    if (checkedList.value.length <= 0) {
      callback(new Error('请选择支付方式'))
    }
    callback()
  }, trigger: 'blur' }]
})

// 抽屉显示
const show = (ifCodeParam) => {
  isAdd.value = !ifCodeParam
  // 数据清空
  Object.assign(saveObject, {
    isMchMode: 1,
    isIsvMode: 1,
    state: 1,
    configPageType: 1,
    isSupportApplyment: 0,
    isOpenApplyment: 0,
    isSupportCheckBill: 0,
    isOpenCheckBill: 0,
    isSupportCashout: 0,
    isOpenCashout: 0,
    bgColor: '#1a53ff'
  })

  if (infoForm.value) {
    infoForm.value.resetFields()
  }

  if (!isAdd.value) { // 修改信息 延迟展示弹层
    ifCode.value = ifCodeParam
    // 拉取详情
    req.getById(API_URL_IFDEFINES_LIST, ifCodeParam).then(res => {
      Object.assign(saveObject, res)
      const newItems = [] // 多选框赋值
      res.wayCodes.forEach(item => {
        newItems.push(item.wayCode)
      })
      checkedList.value = newItems
    })
    visible.value = true
  } else {
    checkedList.value = [] // 多选框设置空
    visible.value = true // 展示弹层信息
  }
}

const onClose = () => {
  visible.value = false
}

// 表单提交
const onSubmit = () => {
  infoForm.value.validate().then(() => {
    saveObject.wayCodeStrs = checkedList.value.join(',')
    // 请求接口
    if (isAdd.value) {
      req.add(API_URL_IFDEFINES_LIST, saveObject).then(res => {
        import('ant-design-vue').then(({ message }) => {
          message.success('新增成功')
          visible.value = false
          props.callbackFunc() // 刷新列表
        })
      })
    } else {
      req.updateById(API_URL_IFDEFINES_LIST, ifCode.value, saveObject).then(res => {
        import('ant-design-vue').then(({ message }) => {
          message.success('修改成功')
          visible.value = false
          props.callbackFunc() // 刷新列表
        })
      })
    }
  }).catch(error => {
    console.error('验证失败:', error)
  })
}

const groupBy = (list, key) => {
  return list.reduce((acc, item) => {
    (acc[item[key]] = acc[item[key]] || []).push(item)
    return acc
  }, {})
}

const getGroupName = (wayType) => {
  switch (wayType) {
    case 'WECHAT':
      return '微信'
    case 'ALIPAY':
      return '支付宝'
    case 'YSFPAY':
      return '云闪付'
    case 'UNIONPAY':
      return '银联'
    case 'DCEPPAY':
      return '数字人民币'
    default:
      return '其他'
  }
}

// 支付方式列表
const payWayList = () => {
  req.list(API_URL_PAYWAYS_LIST, { 'pageSize': '-1' }).then(res => {
    const ways = res.records

    const groupedWaysData = groupBy(ways, 'wayType')

    // 指定顺序排序
    const order = ['WECHAT', 'ALIPAY', 'YSFPAY', 'UNIONPAY', 'DCEPPAY', 'OTHER']
    const sortedGroupedWays = Object.fromEntries(
        Object.entries(groupedWaysData)
            .sort(([keyA], [keyB]) => {
              const indexA = order.indexOf(keyA)
              const indexB = order.indexOf(keyB)
              return indexA - indexB
            })
    )

    groupedWays.value = []
    for (const wayType in sortedGroupedWays) {
      const group = {
        name: getGroupName(wayType),
        ways: []
      }
      for (const way of sortedGroupedWays[wayType]) {
        group.ways.push({ wayCode: way.wayCode, wayName: way.wayName })
      }
      groupedWays.value.push(group)
    }
  })
}

// 上传文件成功回调方法，参数fileList为已经上传的文件列表，name是自定义参数
const uploadSuccess = (name, fileList) => {
  const [firstItem] = fileList
  saveObject[name] = firstItem?.url
}

onMounted(() => {
  payWayList()
})
</script>

<style lang="less" scoped>
  .ag-drawer-title {
    font-size: 16px;
    font-weight: 600;
    width: 100%;
    margin-bottom: 15px;
  }

  .ag-upload-btn {
    height: 66px;
    margin-top: 8px;
  }
</style>