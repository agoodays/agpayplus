<template>
  <page-header-wrapper>
    <a-card :bordered="false" style="width: 500px;padding: 30px;">
      <div>
        <p>商户中心</p>
      </div>
      <a-divider />
      <div class="item">
        <span class="label">商户名称</span>
        <span class="desc">{{ saveObject.mchName }}</span>
      </div>
      <div class="item">
        <span class="label">商户简称</span>
        <span class="desc">{{ saveObject.mchShortName }}</span>
      </div>
      <div class="item">
        <span class="label">登录名</span>
        <span class="desc">{{ saveObject.loginUsername }}</span>
      </div>
      <div class="item">
        <span class="label">商户号</span>
        <span class="desc">{{ saveObject.mchNo }}</span>
      </div>
      <div class="item">
        <span class="label">商户类型</span>
        <span class="desc">{{ saveObject.type === 1 ? '普通商户': '特约商户' }}</span>
      </div>
      <div class="item">
        <span class="label">服务商号</span>
        <span class="desc">{{ saveObject.isvNo }}</span>
      </div>
      <div class="item">
        <span class="label">注册时间</span>
        <span class="desc">{{ saveObject.createdAt }}</span>
      </div>
    </a-card>
  </page-header-wrapper>
</template>

<script>
import { getMainUserInfo } from '@/api/manage'
export default {
  data () {
    return {
      saveObject: {}
    }
  },
  created () {
    this.detail()
  },
  methods: {
    detail () { // 获取基本信息
      const that = this
      getMainUserInfo().then(res => {
        that.saveObject = res
        // console.log(res)
      })
    }
  }
}
</script>

<style scoped>
  p {
    color: #000000d9;
    font-size: 16px;
    font-weight: 500;
    margin: 0 5px
  }

  .desc {
    font-weight: 500;
    font-size: 14px;
    letter-spacing: .05em;
    color: #262626
  }

  .label {
    font-weight: 500;
    font-size: 14px;
    letter-spacing: .05em;
    color: #999
  }

  .item {
    display: flex;
    justify-content: space-between;
    padding: 8px
  }
</style>
