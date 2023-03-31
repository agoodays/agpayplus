<template>
  <a-drawer
    :visible="visible"
    :title="true ? '团队详情' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="40%"
  >
    <a-row justify="space-between" type="flex">
      <a-col :sm="10">
        <a-descriptions>
          <a-descriptions-item label="团队编号">
            {{ detailData.teamNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="10">
        <a-descriptions>
          <a-descriptions-item label="团队名称">
            {{ detailData.teamName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
    </a-row>
  </a-drawer>
</template>

<script>
  import { API_URL_UR_TEAM_LIST, req } from '@/api/manage'
  export default {
    props: {
      callbackFunc: { type: Function, default: () => () => ({}) }
    },

    data () {
      return {
        btnLoading: false,
        detailData: {}, // 数据对象
        recordId: null, // 更新对象ID
        visible: false // 是否显示弹层/抽屉
      }
    },
    created () {
    },
    methods: {
      show: function (recordId) { // 弹层打开事件
        if (this.$refs.infoFormModel !== undefined) {
          this.$refs.infoFormModel.resetFields()
        }
        const that = this
        that.recordId = recordId
        req.getById(API_URL_UR_TEAM_LIST, recordId).then(res => {
          that.detailData = res
        })
        this.visible = true
      },
      onClose () {
        this.visible = false
      }
    }
  }
</script>

<style lang="less">
.detail-upload-list-inline .ant-upload-list-item-card-actions.picture {
  display: none;
}
</style>
