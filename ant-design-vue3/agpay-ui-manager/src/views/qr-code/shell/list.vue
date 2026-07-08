<template>
  <div>
    <a-card>
      <ag-search v-model="searchData" :search-loading="btnLoading" @search="queryFunc" @reset="resetFunc">
        <template #formItem>
          <ag-input v-model="searchData.shellAlias" placeholder="模板名称" />
        </template>
      </ag-search>
      <!-- 列表渲染 -->
      <ag-table
        ref="infoTable"
        :init-data="true"
        :on-load="reqTableDataFunc"
        :columns="tableColumns"
        :params="searchData"
        row-key="id"
        @btn-load-close="btnLoading = false"
      >
        <template #topLeftSlot>
          <div>
            <a-button
              v-if="$access('ENT_DEVICE_QRC_SHELL_ADD')"
              type="primary"
              icon="plus"
              class="mg-b-30"
              @click="addFunc"
              >新增</a-button
            >
          </div>
        </template>

        <template #shellImgViewUrlSlot="{ record }">
          <img
            v-if="$access('ENT_DEVICE_QRC_SHELL_VIEW')"
            width="119"
            :src="record.shellImgViewUrl"
            @click="onPreview(record.shellImgViewUrl)"
          />
          <img v-else width="119" :src="record.shellImgViewUrl" />
        </template>
        <template #opSlot="{ record }">
          <!-- 操作按钮 -->
          <ag-table-actions>
            <a-button v-if="$access('ENT_DEVICE_QRC_SHELL_VIEW')" type="link" @click="onPreview(record.shellImgViewUrl)"
              >预览</a-button
            >
            <a-button v-if="$access('ENT_DEVICE_QRC_SHELL_EDIT')" type="link" @click="editFunc(record.id)"
              >编辑</a-button
            >
            <a-button
              v-if="$access('ENT_DEVICE_QRC_SHELL_DEL')"
              type="link"
              style="color: red"
              @click="delFunc(record.id)"
              >删除</a-button
            >
          </ag-table-actions>
        </template>
      </ag-table>
    </a-card>
    <!-- 新增/编辑页面弹窗  -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callback-func="searchFunc" />
  </div>
</template>
<script setup>
import { qrcShellApi } from '@/api/business/qr-code/qrc-shell-api'
import { AgInput, AgSearch, AgTable, AgTableActions } from '@/components'
import { reactive, ref } from 'vue'
import InfoAddOrEdit from './add-or-edit.vue'

const tableColumns = [
  {
    key: 'shellImgViewUrl',
    title: '模板预览图',
    width: 151,
    fixed: 'left',
    customRender: 'shellImgViewUrlSlot'
  },
  { key: 'shellAlias', dataIndex: 'shellAlias', title: '模板名称' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', customRender: 'opSlot' }
]

const infoTable = ref(null)
const infoAddOrEdit = ref(null)
const searchData = reactive({})
const btnLoading = ref(false)

const reqTableDataFunc = (params) => qrcShellApi.queryCardList(params)

function queryFunc() {
  btnLoading.value = true
  infoTable.value.loadData()
}

function searchFunc() {
  infoTable.value.loadData()
}

function resetFunc() {
  Object.keys(searchData).forEach((key) => {
    delete searchData[key]
  })
}

function onPreview(url) {
  window.$viewerApi({
    images: [url],
    options: {
      initialViewIndex: 0
    }
  })
}

function addFunc() {
  infoAddOrEdit.value?.show()
}

function editFunc(recordId) {
  infoAddOrEdit.value?.show(recordId)
}

function delFunc(recordId) {
  window.$infoBox.confirmDanger('确定删除吗', '', () => {
    qrcShellApi.delById(recordId).then(() => {
      window.$message.success('删除成功')
      reloadTable()
    })
  })
}
</script>
