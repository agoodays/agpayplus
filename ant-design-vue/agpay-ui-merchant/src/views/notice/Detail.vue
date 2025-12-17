<template>
  <a-drawer
    :visible="visible"
    :title="true ? '公告详情' : ''"
    @close="onClose"
    :drawer-style="{ overflow: 'hidden' }"
    :body-style="{ paddingBottom: '80px', overflow: 'auto' }"
    width="60%"
  >
    <div class="article-container">
      <div class="title">{{ detailData.title }}</div>
      <div class="author">
        <span class="auther-text"><a-icon type="user" /> {{ detailData.publisher }}</span>
        <span><a-icon type="history" /> {{ detailData.publishTime }}</span>
      </div>
      <div class="content" v-html="detailData.content"></div>
    </div>
  </a-drawer>
</template>

<script>
  import { API_URL_ARTICLE_LIST, req } from '@/api/manage'
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
        const that = this
        that.recordId = recordId
        req.getById(API_URL_ARTICLE_LIST, recordId).then(res => {
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
  .article-container {
    max-width: 100%;
    width: 800px;
    margin: 0 auto;
    padding: 26px 6px 6px;

    .title {
      height: 50px;
      line-height: 1.75;
      font-size: 30px;
      font-weight: 700;
      letter-spacing: 3px;
      margin-bottom: 15px
    };

    .subtitle {
      box-sizing: border-box;
      width: 400px;
      font-size: 15px
    };

    .author {
      width: 400px;
      color: #969696;
      margin-bottom: 15px
    };

    .author .auther-text {
      margin-right: 20px
    };

    .content {
      margin: 45px auto 0;
      line-height: 1.5
    }
  }
</style>
