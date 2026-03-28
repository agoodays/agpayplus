<template>
  <div>
    <a-card style="margin-bottom: 10px">
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <a-form-item label="">
              <ag-input v-model:value="searchData.shellAlias" placeholder="模板别名" />
            </a-form-item>
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" :loading="btnLoading" @click="searchFunc(true)">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => (searchData = {})">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
    </a-card>
    <ag-card
      ref="infoCard"
      :req-card-list-func="reqCardListFunc"
      :search-data="searchData"
      :span="agpayCard.span"
      :height="agpayCard.height"
      :name="agpayCard.name"
      :add-authority="agpayCard.addAuthority"
      :use-pagination="true"
      :page-size="11"
      @btn-load-close="btnLoading = false"
      @add-ag-card="addFunc"
    >
      <template #cardContentSlot="{ record }">
        <div>
          <div :style="{ height: agpayCard.height + 'px' }" class="ag-card-content">
            <!-- 卡片自定义样式 -->
            <div class="ag-card-content-header" :style="{ height: agpayCard.height - 100 + 'px' }">
              <img
                v-if="$access('ENT_DEVICE_QRC_SHELL_VIEW')"
                :style="{ height: agpayCard.height - 100 + 'px', width: (agpayCard.height - 100) / 1.415 + 'px' }"
                :src="record.shellImgViewUrl"
                @click="onPreview(record.shellImgViewUrl)"
              />
              <img
                v-else
                :style="{ height: agpayCard.height - 100 + 'px', width: (agpayCard.height - 100) / 1.415 + 'px' }"
                :src="record.shellImgViewUrl"
              />
            </div>
            <div class="ag-card-content-body" :style="{ height: 50 + 'px' }">
              <div class="title">
                {{ record.shellAlias }}
              </div>
            </div>
            <!-- 卡片底部操作栏 -->
            <div class="ag-card-ops">
              <a-tooltip v-if="$access('ENT_DEVICE_QRC_SHELL_EDIT')" placement="top" title="编辑">
                <a-icon key="edit" type="edit" @click="editFunc(record.id)" />
              </a-tooltip>
              <a-tooltip v-if="$access('ENT_DEVICE_QRC_SHELL_DEL')" placement="top" title="删除">
                <a-icon key="delete" type="delete" @click="delFunc(record.id)" />
              </a-tooltip>
            </div>
          </div>
        </div>
      </template>
    </ag-card>
    <!-- 新增页面组件 -->
    <info-add-or-edit ref="infoAddOrEdit" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { ref, reactive, computed } from 'vue'
import { AgCard, AgInput } from '@/components'
import { API_URL_QRC_SHELL_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'

// 响应式数据
const infoCard = ref(null)
const infoAddOrEdit = ref(null)
const searchData = reactive({})
const btnLoading = ref(false)

// 卡片配置
const agpayCard = reactive({
  name: '码牌模版',
  height: 360,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
  addAuthority: window.$access('ENT_DEVICE_QRC_SHELL_ADD')
})

// 请求卡片列表数据
const reqCardListFunc = (params) => {
  return req.list(API_URL_QRC_SHELL_LIST, params)
}

// 刷新card列表
const refCardList = (isToFirst) => {
  infoCard.value.refCardList(isToFirst)
}

// 搜索函数
const searchFunc = (isToFirst = false) => {
  // 点击【查询】按钮点击事件
  btnLoading.value = true
  refCardList(isToFirst)
}

// 预览图片
const onPreview = (url) => {
  window.$viewerApi({
    images: [url],
    options: {
      initialViewIndex: 0
    }
  })
}

// 新增函数
const addFunc = () => {
  // 业务通用【新增】 函数
  infoAddOrEdit.value.show()
}

// 编辑函数
const editFunc = (recordId) => {
  // 业务通用【修改】 函数
  console.log(recordId)
  infoAddOrEdit.value.show(recordId)
}

// 删除函数
const delFunc = (recordId) => {
  window.$infoBox.confirmDanger('确认删除？', '', () => {
    req.delById(API_URL_QRC_SHELL_LIST, recordId).then((res) => {
      window.$message.success('删除成功！')
      refCardList()
    })
  })
}
</script>

<style lang="less" scoped>
.ag-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
  border-radius: 6px;
  overflow: hidden;
}
.ag-card-ops {
  width: 100%;
  height: 50px;
  background-color: var(--base-bg-color);
  display: flex;
  flex-direction: row;
  justify-content: space-around;
  align-items: center;
  border-top: 1px solid var(--layout-bg);
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
  justify-content: space-around;
  align-items: center;
}
.title {
  font-size: 16px;
  font-family:
    PingFang SC,
    PingFang SC-Bold;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}
</style>
