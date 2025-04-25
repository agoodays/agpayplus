<template>
  <div>
    <div class="content">
      <a-card :bordered="false" class="mch-info" style="padding: 30px;">
        <div class="title">
          <b>商户中心</b>
          <a-button icon="copy" type="link" @click="copyFunc" style="padding-right: 0">复制商户信息</a-button>
        </div>
        <div class="item">
          <span class="label">商户名称</span>
          <span class="desc">{{ mchInfo.mchName }}</span>
        </div>
        <div class="item">
          <span class="label">商户简称</span>
          <span class="desc">{{ mchInfo.mchShortName }}</span>
        </div>
        <div class="item">
          <span class="label">登录名</span>
          <span class="desc">{{ mchInfo.loginUsername }}</span>
        </div>
        <div class="item">
          <span class="label">商户号</span>
          <span class="desc">{{ mchInfo.mchNo }}</span>
        </div>
        <div class="item">
          <span class="label">商户类型</span>
          <span class="desc">{{ mchInfo.type === 1 ? '普通商户': '特约商户' }}</span>
        </div>
        <div class="item" v-if="mchInfo.isvNo">
          <span class="label">服务商号</span>
          <span class="desc">{{ mchInfo.isvNo }}</span>
        </div>
        <div class="item">
          <span class="label">注册时间</span>
          <span class="desc">{{ mchInfo.createdAt }}</span>
        </div>
      </a-card>
      <a-card :bordered="false" class="other-box ali-auth" style="padding: 30px;">
        <div class="title">
          <b>支付宝代运营授权请求</b>
        </div>
        <a-alert message="注意！！！仅当使用支付宝如意lite产品时使用" type="info" />
        <a-form-model ref="infoFormModel" :model="alipayAuthData" :rules="rules" :label-col="{ span: 4 }" :wrapper-col="{ span: 18 } ">
          <a-form-model-item label="授权方式" prop="authType">
            <a-radio-group v-model="alipayAuthData.authType">
              <a-radio :value="'qrcode'">使用支付宝扫授权码</a-radio>
              <a-radio :value="'apply'">发送支付宝授权消息</a-radio>
            </a-radio-group>
          </a-form-model-item>
          <a-form-model-item label="支付宝账号" prop="alipayAccount">
            <a-input placeholder="请输入支付宝账号(一般为手机或邮箱)" v-model="alipayAuthData.alipayAccount"/>
          </a-form-model-item>
          <a-form-model-item :wrapper-col="{ span: 18, offset: 4 }">
            <a-button icon="check-circle" type="primary" @click="alipayAuthFunc">发起授权</a-button>
          </a-form-model-item>
        </a-form-model>
      </a-card>
    </div>
  </div>
</template>

<script>
import { getMainUserInfo } from '@/api/manage'
export default {
  data () {
    return {
      mchInfo: {},
      alipayAuthData: {},
      rules: {
        authType: [{ required: true, trigger: 'blur', message: '请选择授权方式' }],
        alipayAccount: [{ required: true, message: '请输入支付宝账号', trigger: 'blur' }]
      }
    }
  },
  created () {
    this.detail()
  },
  methods: {
    copyFunc () {
      const data = [
        { title: '商户名称', value: 'mchName' },
        { title: '商户简称', value: 'mchShortName' },
        { title: '登录名', value: 'loginUsername' },
        { title: '商户号', value: 'mchNo' },
        { title: '商户类型', value: 'type' },
        { title: '服务商号', value: 'isvNo' },
        { title: '注册时间', value: 'createdAt' }
      ]

      const getText = (c) => {
        if (c.value === 'type') {
          return `${c.title}: ${this.mchInfo[c.value] === 1 ? '普通商户' : '特约商户' }`
        } else {
          return `${c.title}: ${this.mchInfo[c.value]}`
        }
      }

      const text = data.map((c) => getText(c)).join('\n')

      if (navigator.clipboard) {
        navigator.clipboard.writeText(text).then(() => {
          this.$message.success('复制成功')
        }).catch((error) => {
          console.log('复制失败:', error)
        })
      } else {
        console.log('当前浏览器不支持剪贴板操作！')
      }
    },
    detail () { // 获取基本信息
      const that = this
      getMainUserInfo().then(res => {
        that.mchInfo = res
        // console.log(res)
      })
    },
    alipayAuthFunc: function () {
      const that = this
      this.$refs.infoFormModel.validate(valid => {
        if (valid) { // 验证通过
          console.log('发送成功', that.alipayAuthData)
          that.$infoBox.modalSuccess('发送成功', `授权消息已发送至支付宝：${that.alipayAuthData.alipayAccount}，请前往支付宝App处理`)
        }
      })
    }
  }
}
</script>

<style scoped>
  .content {
    width: 100%;
    display: flex;
    flex-wrap: wrap
  }

  .other-box {
    box-sizing: border-box;
    padding: 30px;
    margin-bottom: 30px
  }

  .other-box .main-item,.other-box .other-item {
    width: 100%
  }

  .other-box .main-item .item,.other-box .other-item .item {
    width: 25%
  }

  .other-box .main-item p,.other-box .other-item p {
    margin: 0!important
  }

  .other-box .main-item .item-title,.other-box .other-item .item-title {
    white-space: nowrap;
    font-weight: 400;
    font-size: 13px;
    letter-spacing: .05em;
    color: #0009
  }

  .other-box .main-item .item-text,.other-box .other-item .item-text {
    font-weight: 400;
    font-size: 50px;
    letter-spacing: .05em;
    color: #1A53FF
  }

  .other-box .other-item {
    display: flex
  }

  .other-box .other-item .item-text {
    font-weight: 400;
    font-size: 25px;
    letter-spacing: .05em;
    color: #000!important
  }

  ::v-deep(.ant-card-body) {
    height: 100%
  }

  ::v-deep(.ant-form-item){
    margin: 12px 0;
  }

  .box-flex {
    flex-grow: 1;
    display: flex;
    flex-wrap: wrap;
    height: 100%;
    align-content: space-between
  }

  .box-flex>div {
    width: 100%
  }

  .title {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px
  }

  .title b {
    font-size: 16px;
    font-weight: 600;
    margin-right: 10px;
    color: black;
  }

  .title .date {
    width: 215px
  }

  .mch-info {
    margin-bottom: 30px
  }

  .mch-info .desc {
    font-weight: 500;
    font-size: 14px;
    letter-spacing: .05em;
    color: #262626
  }

  .mch-info .label {
    font-weight: 500;
    font-size: 14px;
    color: #999;
    margin-right: 10px
  }

  .mch-info .item {
    display: flex;
    justify-content: space-between;
    padding-bottom: 20px
  }

  .mch-info .item:nth-last-child(1) {
    padding: 0
  }

  .mch-info,.ali-auth {
    width: 100%
  }

  @media screen and (min-width: 1024px) {
    .mch-info {
      order: 1;
      width: 280px;
      flex-grow: 1
    }

    .ali-auth {
      order: 3;
      width: 100%
    }
  }

  @media screen and (min-width: 1430px) {
    .mch-info {
      width: 350px;
      margin-right: 30px
    }

    .ali-auth {
      width: 65%;
      flex-grow: 1
    }

    .mch-info {
      order: 0
    }

    .ali-auth {
      order: 1
    }
  }
</style>
