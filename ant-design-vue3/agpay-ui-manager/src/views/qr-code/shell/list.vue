<template>
  <div>
    <a-card>
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-input v-model="searchData.shellAlias" placeholder="模板名称" />
            <span class="table-page-search-submitButtons">
              <a-button type="primary" icon="search" :loading="btnLoading" @click="searchFunc(true)">查询</a-button>
              <a-button style="margin-left: 8px" icon="reload" @click="() => (searchData = {})">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
      <div class="split-line" />
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
<script>
import { AgTable, AgTableActions, AgInput } from '@/components'
import { API_URL_QRC_SHELL_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './add-or-edit.vue'

const tableColumns = [
  {
    key: 'shellImgViewUrl',
    title: '模板预览图',
    width: 151,
    fixed: 'left',
    scopedSlots: { customRender: 'shellImgViewUrlSlot' }
  },
  { key: 'shellAlias', dataIndex: 'shellAlias', title: '模板名称' },
  { key: 'op', title: '操作', width: 160, fixed: 'right', align: 'center', scopedSlots: { customRender: 'opSlot' } }
]

export default {
  name: 'QrCodeShellPage',
  components: {
    'ag-table': AgTable,
    'ag-table-actions': AgTableActions,
    'ag-input': AgInput,
    InfoAddOrEdit
  },
  data() {
    return {
      tableColumns: tableColumns,
      searchData: {},
      btnLoading: false
    }
  },
  methods: {
    // 对接table接口函数
    reqTableDataFunc: (params) => {
      return req.list(API_URL_QRC_SHELL_LIST, params)
    },

    searchFunc(isToFirst = false) {
      // 点击查询按钮事件
      this.btnLoading = true
      this.$refs.infoTable.loadData()
    },

    onPreview(url) {
      this.$viewerApi({
        images: [url],
        options: {
          initialViewIndex: 0
        }
      })
    },

    addFunc: function () {
      // 业务通道.模板管理 新增
      this.$refs.infoAddOrEdit.show()
    },

    editFunc: function (recordId) {
      // 业务通道.模板管理 编辑
      this.$refs.infoAddOrEdit.show(recordId)
    },

    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确定删除吗', '', () => {
        req.delById(API_URL_QRC_SHELL_LIST, recordId).then((res) => {
          that.$message.success('删除成功')
          that.$refs.infoTable.loadData()
        })
      })
    }
  }
}
</script>
