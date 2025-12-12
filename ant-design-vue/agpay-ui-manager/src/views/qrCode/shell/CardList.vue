<template>
  <div>
    <a-card style="margin-bottom: 10px;">
      <div class="table-page-search-wrapper">
        <a-form layout="inline" class="table-head-ground">
          <div class="table-layer">
            <ag-text-up :placeholder="'模板别名'" :msg="searchData.shellAlias" v-model="searchData.shellAlias" />
            <span class="table-page-search-submitButtons">
              <a-button type="primary" @click="searchFunc(true)" icon="search" :loading="btnLoading">查询</a-button>
              <a-button style="margin-left: 8px;" @click="() => this.searchData = {}" icon="reload">重置</a-button>
            </span>
          </div>
        </a-form>
      </div>
    </a-card>
    <AgCard
      ref="infoCard"
      @btnLoadClose="btnLoading=false"
      :reqCardListFunc="reqCardListFunc"
      :searchData="searchData"
      :span="agpayCard.span"
      :height="agpayCard.height"
      :name="agpayCard.name"
      :addAuthority="agpayCard.addAuthority"
      @addAgCard="addFunc"
      :use-pagination="true"
      :pageSize="11"
    >
      <div slot="cardContentSlot" slot-scope="{record}">
        <div :style="{'height': agpayCard.height + 'px'}" class="ag-card-content">
          <!-- 卡片自定义样式 -->
          <div class="ag-card-content-header" :style="{height: agpayCard.height - 100 + 'px'}">
            <img :style="{height: agpayCard.height - 100 + 'px', width: (agpayCard.height - 100)/1.415 + 'px'}" :src="record.shellImgViewUrl" v-if="$access('ENT_DEVICE_QRC_SHELL_VIEW')" @click="onPreview(record.shellImgViewUrl)" />
            <img :style="{height: agpayCard.height - 100 + 'px', width: (agpayCard.height - 100)/1.415 + 'px'}" :src="record.shellImgViewUrl" v-else>
          </div>
          <div class="ag-card-content-body" :style="{height: 50 + 'px'}">
            <div class="title">
              {{ record.shellAlias }}
            </div>
          </div>
          <!-- 卡片底部操作栏 -->
          <div class="ag-card-ops">
            <a-tooltip placement="top" title="编辑" v-if="$access('ENT_DEVICE_QRC_SHELL_EDIT')">
              <a-icon key="edit" type="edit" @click="editFunc(record.id)" />
            </a-tooltip>
            <a-tooltip placement="top" title="删除" v-if="$access('ENT_DEVICE_QRC_SHELL_DEL')">
              <a-icon key="delete" type="delete" @click="delFunc(record.id)" />
            </a-tooltip>
          </div>
        </div>
      </div>
    </AgCard>
    <!-- 新增页面组件 -->
    <InfoAddOrEdit ref="infoAddOrEdit" :callbackFunc="searchFunc"/>
  </div>
</template>
<script>
import AgCard from '@/components/AgCard/AgCard'
import { API_URL_QRC_SHELL_LIST, req } from '@/api/manage'
import InfoAddOrEdit from './AddOrEdit'
import AgTextUp from '@/components/AgTextUp/AgTextUp' // 文字上移组件

export default {
  name: 'QrCodeShellPage',
  components: { AgCard, InfoAddOrEdit, AgTextUp },
  data () {
    return {
      agpayCard: {
        name: '码牌模版',
        height: 300,
        span: { xxl: 6, xl: 4, lg: 4, md: 3, sm: 2, xs: 1 },
        addAuthority: this.$access('ENT_DEVICE_QRC_SHELL_ADD')
      },
      searchData: {},
      btnLoading: false
    }
  },
  methods: {
    // 请求table接口数据
    reqCardListFunc: (params) => {
      return req.list(API_URL_QRC_SHELL_LIST, params)
    },

    // 刷新card列表
    refCardList (isToFirst) {
      this.$refs.infoCard.refCardList(isToFirst)
    },

    searchFunc (isToFirst = false) { // 点击【查询】按钮点击事件
      this.btnLoading = true
      this.refCardList(isToFirst)
    },

    onPreview (url) {
      this.$viewerApi({
        images: [url],
        options: {
          initialViewIndex: 0
        }
      })
    },

    addFunc: function () { // 业务通用【新增】 函数
      this.$refs.infoAddOrEdit.show()
    },

    editFunc: function (recordId) { // 业务通用【修改】 函数
      console.log(recordId)
      this.$refs.infoAddOrEdit.show(recordId)
    },

    delFunc: function (recordId) {
      const that = this
      this.$infoBox.confirmDanger('确认删除？', '', () => {
        req.delById(API_URL_QRC_SHELL_LIST, recordId).then(res => {
          that.$message.success('删除成功！')
          that.refCardList()
        })
      })
    }
  }
}
</script>

<style lang="less" scoped>
  .ag-card-content {
    width: 100%;
    position: relative;
    background-color: @ag-card-back;
    border-radius: 6px;
    overflow:hidden;
  }
  .ag-card-ops {
    width: 100%;
    height: 50px;
    background-color: @ag-card-back;
    display: flex;
    flex-direction: row;
    justify-content: space-around;
    align-items: center;
    border-top: 1px solid @ag-back;
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
    font-family: PingFang SC, PingFang SC-Bold;
    font-weight: 700;
    color: #1a1a1a;
    letter-spacing: 1px;
  }
</style>
