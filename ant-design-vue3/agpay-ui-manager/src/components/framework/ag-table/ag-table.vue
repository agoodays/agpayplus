<template>
  <div>
    <div class="ag-table-top-row" v-if="isShowTableTop">
      <div class="ag-table-top-left">
        <a-button
          icon="area-chart"
          class="statistics"
          v-if="isEnableDataStatistics"
          @click="toggleDataStatistics"
        >
          {{ isShowDataStatistics ? "关闭" : "数据" }}统计
        </a-button>
        <slot name="topLeftSlot"></slot>
      </div>
      <div class="operation-icons">
        <span class="pd-0-20" v-if="isShowAutoRefresh">
          <span style="margin-right: 10px; color: rgb(169, 179, 177);">
            自动刷新：
            <span style="margin-right: 5px; color: rgb(0, 0, 0);">{{ countdown }}s</span>
          </span>
          <a-switch v-model="enableAutoRefresh" />
        </span>
        <a-tooltip placement="top">
          <template #title>
            <span>数据导出</span>
          </template>
          <span
            v-if="isShowDownload"
            @click="downloadData"
            class="pd-0-20"
            style="cursor: pointer; font-size: 16px; color: #000;"
          >
            <a-icon type="download" />
          </span>
        </a-tooltip>
        <a-dropdown :trigger="['click']">
          <a-tooltip placement="top">
            <template #title>
              <span>表格密度</span>
            </template>
            <span class="pd-0-20" style="cursor: pointer; font-size: 16px; color: #000;">
              <a-icon type="column-height" />
            </span>
          </a-tooltip>
          <template #overlay>
            <a-menu class="ant-pro-drop-down menu">
              <a-menu-item :key="0" @click="size = 'default'">默认</a-menu-item>
              <a-menu-item :key="1" @click="size = 'middle'">宽松</a-menu-item>
              <a-menu-item :key="2" @click="size = 'small'">紧促</a-menu-item>
            </a-menu>
          </template>
        </a-dropdown>
        <a-dropdown :trigger="['click']">
          <a-tooltip placement="top">
            <template #title>
              <span>列设置</span>
            </template>
            <span class="pd-0-20" style="cursor: pointer; font-size: 16px; color: #000;">
              <a-icon type="setting" />
            </span>
          </a-tooltip>
          <template #overlay>
            <a-menu class="ant-pro-drop-down menu">
              <a-checkbox-group v-model="visibleColumns">
                <a-menu-item v-for="column in allColumns" :key="column.key">
                  <a-checkbox :value="column.key" :key="column.key">{{ column.title }}</a-checkbox>
                </a-menu-item>
              </a-checkbox-group>
            </a-menu>
          </template>
        </a-dropdown>
      </div>
    </div>
    <slot name="dataStatisticsSlot" v-if="isShowDataStatistics" :countData="countData"></slot>
    <a-table
      :columns="displayedColumns"
      :size="size"
      :data-source="apiResData.records"
      :pagination="pagination"
      :loading="showLoading"
      @change="handleTableChange"
      :row-selection="rowSelection"
      :rowKey="rowKey"
      :scroll="{ x: scrollX }"
      :customRow="customRow"
    >
      <template
        v-for="colCustom in columnsCustomSlots"
        :slot="colCustom.customRender"
        slot-scope="record"
      >
        <slot :name="colCustom.customRender" :record="record"></slot>
      </template>
    </a-table>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue';

// Props
defineProps({
  defaultCountdown: { type: Number, default: 180 },
  isShowTableTop: { type: Boolean, default: true },
  autoRefresh: { type: Boolean, default: false },
  isShowAutoRefresh: { type: Boolean, default: false },
  isShowDownload: { type: Boolean, default: false },
  isEnableDataStatistics: { type: Boolean, default: false },
  initData: { type: Boolean, default: true },
  tableColumns: { type: Array, default: null },
  reqTableDataFunc: { type: Function, default: () => () => ({}) },
  reqTableCountFunc: { type: Function, default: () => () => ({}) },
  reqDownloadDataFunc: { type: Function, default: () => () => ({}) },
  currentChange: { type: Function, default: (v1, v2) => {} },
  searchData: { type: Object, default: null },
  countInitData: { type: Object, default: null },
  pageSize: { type: Number, default: 10 },
  rowSelection: { type: Object, default: null },
  rowKey: { type: [String, Function], default: 'id' },
  scrollX: { type: Number, default: 500 },
  tableRowCrossColor: { type: Boolean, default: false },
});

// Local state
const allColumns = ref([]);
const visibleColumns = ref([]);
const apiResData = ref({ total: 0, records: [] });
const countData = ref({});
const iPage = ref({ pageNumber: 1, pageSize: 10 });
const pagination = ref({
  total: 0,
  current: 1,
  pageSize: 10,
  showSizeChanger: true,
  showTotal: (total) => `共${total}条`,
});
const countdown = ref(180);
const enableAutoRefresh = ref(false);
const isShowDataStatistics = ref(false);
const showLoading = ref(false);
const size = ref('default');

// Computed properties
const columnsCustomSlots = computed(() =>
  tableColumns.filter((item) => item.scopedSlots).map((item) => item.scopedSlots)
);
const displayedColumns = computed(() =>
  allColumns.value.filter((column) => visibleColumns.value.includes(column.key))
);

// Methods
const toggleDataStatistics = () => {
  isShowDataStatistics.value = !isShowDataStatistics.value;
};

const handleTableChange = (pagination, filters, sorter) => {
  iPage.value = {
    pageSize: pagination.pageSize,
    pageNumber: pagination.current,
    sortField: sorter.columnKey,
    sortOrder: sorter.order,
    ...filters,
  };
  refTable();
};

const refTable = (isToFirst = false) => {
  if (isToFirst) {
    iPage.value.pageNumber = 1;
    pagination.value.current = 1;
  }
  showLoading.value = true;
  reqTableDataFunc({ ...iPage.value, ...searchData }).then((resData) => {
    pagination.value.total = resData.total;
    apiResData.value = resData;
    showLoading.value = false;
  });
};

const customRow = (record, index) => {
  if (!tableRowCrossColor) {
    return {};
  }
  return { style: { 'background-color': index % 2 === 0 ? '#FCFCFC' : '#FFFFFF' } };
};

// Lifecycle hooks
onMounted(() => {
  allColumns.value = tableColumns;
  visibleColumns.value = tableColumns.map((column) => column.key);
  if (initData) {
    refTable(true);
  }
});
</script>

<style lang="less" scoped>
/* 样式可以根据需要调整 */
</style>