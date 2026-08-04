<template>
  <div>
    <a-card :bordered="false">
      <ag-search v-model="searchData" :search-loading="computedLoading" @search="searchFunc" @reset="searchFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.shellAlias" label="模板别名" placeholder="请输入模板别名" />
            </a-form-item>
          </a-col>
        </template>
        <template #extra>
          <a-space>
            <a-button-group>
              <a-button
                :type="viewMode === 'card' ? 'primary' : 'default'"
                @click="switchViewMode('card')"
              >
                <appstore-outlined /> 卡片
              </a-button>
              <a-button
                :type="viewMode === 'list' ? 'primary' : 'default'"
                @click="switchViewMode('list')"
              >
                <bars-outlined /> 列表
              </a-button>
            </a-button-group>
          </a-space>
        </template>
      </ag-search>

      <template v-if="viewMode === 'list'">
        <ag-table
          ref="tableRef"
          row-key="id"
          state-key="qr_code_shell"
          :on-load="loadTableData"
          :columns="tableColumns"
          :search-data="searchData"
        >
          <template #toolbar-left>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_ADD')" type="primary" @click="openCreate">
              <plus-outlined /> 新增
            </a-button>
          </template>

          <template #shellImgViewUrlSlot="{ record }">
            <img
              :width="119"
              :src="record.shellImgViewUrl"
              :class="{ 'cursor-pointer': hasPermission('ENT_DEVICE_QRC_SHELL_VIEW') }"
              @click="hasPermission('ENT_DEVICE_QRC_SHELL_VIEW') && handlePreview(record.shellImgViewUrl)"
            />
          </template>

          <template #opSlot="{ record }">
            <ag-table-actions>
              <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_VIEW')" type="link" @click="handlePreview(record.shellImgViewUrl)">预览</a-button>
              <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_EDIT')" type="link" @click="openEdit(record.id)">修改</a-button>
              <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_DEL')" type="link" danger @click="confirmDelete(record.id)">删除</a-button>
            </ag-table-actions>
          </template>
        </ag-table>
      </template>

      <template v-else>
        <ag-card
          ref="cardRef"
          :load-data="loadCardData"
          :search-data="searchData"
          :span="cardConfig.span"
          :height="cardConfig.height"
          :name="cardConfig.name"
          :add-authority="cardConfig.addAuthority"
          :use-pagination="true"
          :page-size="11"
          @add="openCreate"
        >
          <template #cardContentSlot="{ record }">
            <div class="shell-card-wrapper">
              <div :style="{ height: cardConfig.height + 'px' }" class="shell-card-content">
                <div class="shell-card-content-header" :style="{ height: cardConfig.height - 100 + 'px' }">
                  <img
                    v-if="hasPermission('ENT_DEVICE_QRC_SHELL_VIEW')"
                    :style="{ height: cardConfig.height - 100 + 'px', width: (cardConfig.height - 100) / 1.415 + 'px' }"
                    :src="record.shellImgViewUrl"
                    @click="handlePreview(record.shellImgViewUrl)"
                  />
                  <img
                    v-else
                    :style="{ height: cardConfig.height - 100 + 'px', width: (cardConfig.height - 100) / 1.415 + 'px' }"
                    :src="record.shellImgViewUrl"
                  />
                </div>
                <div class="shell-card-content-body">
                  <div class="shell-card-title">{{ record.shellAlias }}</div>
                </div>
                <div class="shell-card-operations">
                  <a-tooltip v-if="hasPermission('ENT_DEVICE_QRC_SHELL_EDIT')" placement="top" title="编辑">
                    <a-button type="text" @click="openEdit(record.id)">
                      <edit-outlined />
                    </a-button>
                  </a-tooltip>
                  <a-tooltip v-if="hasPermission('ENT_DEVICE_QRC_SHELL_DEL')" placement="top" title="删除">
                    <a-button type="text" danger @click="confirmDelete(record.id)">
                      <delete-outlined />
                    </a-button>
                  </a-tooltip>
                </div>
              </div>
            </div>
          </template>
        </ag-card>
      </template>
    </a-card>

    <add-or-edit v-model:open="modalOpen" :record-id="currentRecordId" @success="handleModalSuccess" />
  </div>
</template>

<script setup>
/**
 * 码牌模板列表页面
 * 支持列表视图和卡片视图两种展示模式，可切换。
 */
import { qrcShellApi } from '@/api/business/qr-code/qrc-shell-api'
import { AgCard, AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { useCrudTablePage } from '@/composables/useCrudTablePage'
import { viewerApi } from '@/utils/viewer-api'
import { AppstoreOutlined, BarsOutlined, DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons-vue'
import { message } from 'ant-design-vue'
import { computed, ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'

/** 权限检查 */
const { hasPermission } = usePermission()

/**
 * CRUD 表格页面状态
 * 复用 tableRef/searchData/modalOpen/currentRecordId/confirmDelete 等
 * 注：删除后需根据当前视图刷新（list/card），通过 onDeleted 回调处理。
 */
const {
  tableRef,
  searchData,
  modalOpen,
  currentRecordId,
  openCreate,
  openEdit,
  confirmDelete
} = useCrudTablePage({
  deleteAction: async (recordId) => {
    await qrcShellApi.delById(recordId)
    message.success('删除成功')
  },
  deleteConfirmTitle: '确认删除？',
  deleteSuccessMessage: '删除成功',
  onDeleted: () => refreshList()
})

/** AgCard 组件引用（卡片视图专用） */
const cardRef = ref(null)

/** 当前视图模式：'list' 列表视图 | 'card' 卡片视图 */
const viewMode = ref('card')

/** 卡片加载状态（独立于表格 loading） */
const cardLoading = ref(false)

/** 表格列配置 */
const tableColumns = [
  { key: 'shellImgViewUrl', title: '模板预览图', width: 151, fixed: 'left', customRender: 'shellImgViewUrlSlot' },
  { key: 'shellAlias', dataIndex: 'shellAlias', title: '模板别名' },
  { key: 'op', title: '操作', width: 100, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/** AgCard 组件配置参数 */
const cardConfig = {
  name: '码牌模版',
  height: 360,
  span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
  addAuthority: hasPermission('ENT_DEVICE_QRC_SHELL_ADD')
}

/** 计算当前加载状态（兼容表格和卡片两种视图） */
const computedLoading = computed(() => {
  return cardLoading.value || tableRef.value?.isLoading?.value || false
})

/**
 * 请求表格数据
 * @param {Object} params - 请求参数
 * @returns {Promise<Object>} 列表数据
 */
async function loadTableData(params) {
  return await qrcShellApi.queryCardList(params)
}

/**
 * 请求卡片列表数据
 * @param {Object} params - 请求参数
 * @returns {Promise<Object>} 列表数据
 */
async function loadCardData(params) {
  cardLoading.value = true
  try {
    return await qrcShellApi.queryCardList(params)
  } finally {
    cardLoading.value = false
  }
}

/**
 * 切换视图模式
 * @param {'list' | 'card'} mode - 视图模式
 */
function switchViewMode(mode) {
  viewMode.value = mode
  searchFunc()
}

/**
 * 刷新列表/卡片
 * @param {boolean} [isToFirst=false] - 是否跳转到第一页
 */
function refreshList(isToFirst = false) {
  if (viewMode.value === 'list') {
    tableRef.value?.reload()
  } else {
    cardRef.value?.reload(isToFirst)
  }
}

/** 搜索函数 */
function searchFunc() {
  refreshList(true)
}

/** 新增/编辑弹窗保存成功后的统一处理 */
function handleModalSuccess() {
  searchFunc()
}

/**
 * 预览图片
 * @param {string} url - 图片URL
 */
function handlePreview(url) {
  viewerApi({
    images: [url],
    options: {
      initialViewIndex: 0
    }
  })
}
</script>

<style lang="less" scoped>
.cursor-pointer {
  cursor: pointer;
}

.shell-card-wrapper {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  background-color: var(--base-bg-color);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  transition: box-shadow 0.3s;

  &:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
  }
}

.shell-card-content {
  width: 100%;
  position: relative;
  background-color: var(--base-bg-color);
}

.shell-card-content-header {
  width: 100%;
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  padding: 16px 0;
}

.shell-card-content-body {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 8px 0;
}

.shell-card-title {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-color);
  letter-spacing: 1px;
}

.shell-card-operations {
  width: 100%;
  height: 60px;
  background-color: var(--surface-subtle);
  display: flex;
  flex-direction: row;
  justify-content: center;
  align-items: center;
  gap: 24px;
  border-top: 1px solid var(--border-color);

  :deep(.ant-btn) {
    padding: 4px 8px;
    font-size: 16px;

    &:hover {
      color: var(--primary-color);
    }
  }
}
</style>
