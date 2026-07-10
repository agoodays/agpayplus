<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc" @reset="resetFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.shellAlias" label="模板别名" placeholder="请输入模板别名" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
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
      @add-ag-card="addFunc"
    >
      <template #cardContentSlot="{ record }">
        <div>
          <div :style="{ height: agpayCard.height + 'px' }" class="ag-card-content">
            <!-- 卡片自定义样式 -->
            <div class="ag-card-content-header" :style="{ height: agpayCard.height - 100 + 'px' }">
              <img
                v-if="hasPermission('ENT_DEVICE_QRC_SHELL_VIEW')"
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
              <a-tooltip v-if="hasPermission('ENT_DEVICE_QRC_SHELL_EDIT')" placement="top" title="编辑">
                <icons.EditOutlined />
              </a-tooltip>
              <a-tooltip v-if="hasPermission('ENT_DEVICE_QRC_SHELL_DEL')" placement="top" title="删除">
                <icons.DeleteOutlined />
              </a-tooltip>
            </div>
          </div>
        </div>
      </template>
    </ag-card>
    <!-- 新增页面组件 -->
    <add-or-edit v-model:open="addOrEditOpen" :record-id="editRecordId" @success="searchFunc" />
  </div>
</template>
<script setup>
/**
 * 二维码模板卡片列表页面组件
 * 功能：展示二维码模板卡片列表，支持搜索、预览、新增、编辑、删除操作
 */
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue'
import { qrcShellApi } from '@/api/business/qr-code/qrc-shell-api'
import { AgCard, AgInput, AgSearch } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { reactive, ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import { message } from 'ant-design-vue'

const icons = { DeleteOutlined, EditOutlined }

// 权限检查
const { hasPermission } = usePermission()

/**
 * 组件引用
 */
const infoCard = ref(null)
const addOrEditOpen = ref(false)
const editRecordId = ref(null)

/**
 * 搜索表单数据
 */
const searchData = reactive({})

/**
 * 加载状态
 */
const loading = ref(false)

/**
 * 卡片配置
 */
const agpayCard = reactive({
  name: '码牌模版',
  height: 360,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
  addAuthority: hasPermission('ENT_DEVICE_QRC_SHELL_ADD')
})

/**
 * 请求卡片列表数据
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 卡片列表数据
 */
const reqCardListFunc = async (params) => {
  return await qrcShellApi.queryCardList(params)
}

/**
 * 刷新卡片列表
 * @param {boolean} isToFirst - 是否跳转到第一页
 */
const refCardList = (isToFirst) => {
  infoCard.value?.refCardList(isToFirst)
}

/**
 * 搜索函数
 */
const searchFunc = () => {
  loading.value = true
  refCardList(true)
}

/**
 * 预览图片
 * @param {string} url - 图片URL
 */
const onPreview = (url) => {
  window.$viewerApi({
    images: [url],
    options: {
      initialViewIndex: 0
    }
  })
}

/**
 * 新增模板
 */
const addFunc = () => {
  editRecordId.value = null
  addOrEditOpen.value = true
}

/**
 * 编辑模板
 * @param {string} recordId - 模板ID
 */
const editFunc = (recordId) => {
  editRecordId.value = recordId
  addOrEditOpen.value = true
}

/**
 * 删除模板
 * @param {string} recordId - 模板ID
 */
const delFunc = async (recordId) => {
  const { infoBox } = await import('@/utils/info-box')
  infoBox.confirmDanger('确认删除？', '', async () => {
    try {
      await qrcShellApi.delById(recordId)
      message.success('删除成功！')
      refCardList()
    } catch (error) {
      console.error('删除模板失败:', error)
    }
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
