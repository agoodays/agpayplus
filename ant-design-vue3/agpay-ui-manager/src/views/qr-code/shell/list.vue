<template>
  <div>
    <a-card :bordered="false">
      <!-- 搜索表单 -->
      <ag-search v-model="searchData" :search-loading="loading" @search="searchFunc" @reset="resetFunc">
        <template #base="{ colSpan }">
          <a-col v-bind="colSpan">
            <a-form-item label="">
              <ag-input v-model="searchData.shellAlias" label="模板名称" placeholder="请输入模板名称" />
            </a-form-item>
          </a-col>
        </template>
      </ag-search>
      
      <!-- 列表渲染 -->
      <ag-table
        ref="tableRef"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="id"
      >
        <template #toolbar-left>
          <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_ADD')" type="primary" @click="addFunc">
            <template #icon><PlusOutlined /></template>
            新增
          </a-button>
        </template>

        <template #shellImgViewUrlSlot="{ record }">
          <img v-if="hasPermission('ENT_DEVICE_QRC_SHELL_VIEW')" width="119" :src="record.shellImgViewUrl" @click="onPreview(record.shellImgViewUrl)" />
          <img v-else width="119" :src="record.shellImgViewUrl" />
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_VIEW')" type="link" @click="onPreview(record.shellImgViewUrl)">预览</a-button>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_EDIT')" type="link" @click="editFunc(record.id)">编辑</a-button>
            <a-button v-if="hasPermission('ENT_DEVICE_QRC_SHELL_DEL')" type="link" style="color: red" @click="delFunc(record.id)">删除</a-button>
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <add-or-edit v-model:open="addOrEditOpen" :record-id="editRecordId" @success="searchFunc" />
  </div>
</template>
<script setup>
/**
 * 二维码模板列表页面组件
 * 功能：展示二维码模板列表，支持搜索、预览、新增、编辑、删除操作
 */
import { PlusOutlined } from '@ant-design/icons-vue'
import { qrcShellApi } from '@/api/business/qr-code/qrc-shell-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { usePermission } from '@/composables/useCommon'
import { reactive, ref } from 'vue'
import AddOrEdit from './add-or-edit.vue'
import { message } from 'ant-design-vue'

// 权限检查
const { hasPermission } = usePermission()

/**
 * 表格列配置
 */
const tableColumns = [
  { key: 'shellImgViewUrl', title: '模板预览图', width: 151, fixed: 'left', customRender: 'shellImgViewUrlSlot' },
  { key: 'shellAlias', dataIndex: 'shellAlias', title: '模板名称' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

/**
 * 组件引用
 */
const tableRef = ref(null)
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
 * 请求表格数据函数
 * @param {Object} params - 查询参数
 * @returns {Promise<Object>} 表格数据
 */
const reqTableDataFunc = async (params) => {
  return await qrcShellApi.queryCardList(params)
}

/**
 * 搜索函数
 */
const searchFunc = () => {
  loading.value = true
  tableRef.value?.loadData()
}

/**
 * 重置搜索条件
 */
const resetFunc = () => {
  Object.keys(searchData).forEach((key) => {
    delete searchData[key]
  })
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
  infoBox.confirmDanger('确定删除吗', '', async () => {
    try {
      await qrcShellApi.delById(recordId)
      message.success('删除成功')
      tableRef.value?.reload()
    } catch (error) {
      console.error('删除模板失败:', error)
    }
  })
}
</script>
