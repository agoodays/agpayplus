<template>
  <a-drawer
    :visible="visible"
    :title="true ? '代理商详情' : ''"
    @close="onClose"
    :body-style="{ paddingBottom: '80px' }"
    width="40%"
  >
    <a-row justify="space-between" type="flex">
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="代理商号">
            {{ detailData.agentNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="代理商名称">
            {{ detailData.agentName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="登录名">
            {{ detailData.loginUsername }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="代理商简称">
            {{ detailData.agentShortName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="上级代理商号">
            {{ detailData.pid }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="服务商号">
            {{ detailData.isvNo }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="服务商名称">
            {{ this.isvName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="联系人姓名">
            {{ detailData.contactName }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="联系人手机号">
            {{ detailData.contactTel }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="联系人邮箱">
            {{ detailData.contactEmail }}
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="是否允许发展下级">
            <a-tag :color="detailData.addAgentFlag === 1?'green':'volcano'">
              {{ detailData.addAgentFlag === 0?'否':detailData.addAgentFlag === 1?'是':'未知' }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="状态">
            <a-tag :color="detailData.state === 1?'green':'volcano'">
              {{ detailData.state === 0?'禁用':detailData.state === 1?'启用':'未知' }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="钱包余额">
            <a-tag :color="detailData.balanceAmount > 0?'green':'volcano'">
              {{ detailData.balanceAmount }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="不可用金额">
            <a-tag :color="detailData.unAmount > 0?'green':'volcano'">
              {{ detailData.unAmount }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
      <a-col :sm="12">
        <a-descriptions>
          <a-descriptions-item label="在途佣金">
            <a-tag :color="detailData.auditProfitAmount > 0?'green':'volcano'">
              {{ detailData.auditProfitAmount }}
            </a-tag>
          </a-descriptions-item>
        </a-descriptions>
      </a-col>
    </a-row>
    <a-row justify="start" type="flex">
      <a-col :sm="24">
        <a-form-model-item label="备注">
          <a-input
            type="textarea"
            disabled="disabled"
            style="height: 50px"
            v-model="detailData.remark"
          />
        </a-form-model-item>
      </a-col>
    </a-row>

    <!-- 账户信息板块 -->
    <a-row justify="space-between" type="flex">
      <a-col :sm="24">
        <a-divider orientation="left">
          <a-tag color="#FF4B33">账户信息
          </a-tag>
        </a-divider>
      </a-col>
    </a-row>
    <div>
      <a-row justify="space-between" type="flex">
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="代理商类型">
              {{ detailData.agentType === 1 ? '个人': '企业' }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item label="收款账户类型">
              {{ detailData.settAccountTypeName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
      <a-row justify="space-between" type="flex">
        <a-col :sm="12" v-if="detailData.settAccountType==='BANK_PUBLIC'">
          <a-descriptions>
            <a-descriptions-item label="对公账户名称">
              {{ detailData.settAccountName }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12">
          <a-descriptions>
            <a-descriptions-item :label="detailData.settAccountNoLabel">
              {{ detailData.settAccountNo }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12" v-if="detailData.settAccountType==='BANK_PUBLIC'">
          <a-descriptions>
            <a-descriptions-item label="开户银行名称">
              {{ detailData.settAccountBank }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
        <a-col :sm="12" v-if="detailData.settAccountType==='BANK_PUBLIC'">
          <a-descriptions>
            <a-descriptions-item label="开户行支行名称">
              {{ detailData.settAccountSubBank }}
            </a-descriptions-item>
          </a-descriptions>
        </a-col>
      </a-row>
    </div>

    <!-- 资料信息板块 -->
    <a-row justify="space-between" type="flex">
      <a-col :span="24">
        <a-divider orientation="left">
          <a-tag color="#FF4B33">
            资料信息
          </a-tag>
        </a-divider>
      </a-col>
    </a-row>
    <div>
      <a-row justify="space-between" type="flex">
        <!-- 企业 -->
        <a-col :span="10" v-if="detailData.agentType === 2">
          <a-form-model-item label="营业执照照片" prop="licenseImg">
            <div v-if="detailData.licenseImg">
              <a-upload
                :default-file-list="getDefaultFileList(detailData.licenseImg)"
                :showUploadList="{ showPreviewIcon:false, showRemoveIcon:false, showDownloadIcon:false }"
                list-type="picture"
                class="detail-upload-list-inline"
                @preview="imgPreview($event)"
              />
            </div>
          </a-form-model-item>
        </a-col>
        <!-- 企业对公 -->
        <a-col :span="10" v-if="detailData.agentType === 2 && detailData.settAccountType === 'BANK_PUBLIC'">
          <a-form-model-item label="开户许可证照片" prop="permitImg">
            <div v-if="detailData.permitImg">
              <a-upload
                :default-file-list="getDefaultFileList(detailData.permitImg)"
                :showUploadList="{ showPreviewIcon:false, showRemoveIcon:false, showDownloadIcon:false }"
                list-type="picture"
                class="detail-upload-list-inline"
                @preview="imgPreview($event)"
              />
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item :label="'['+this.imgLabel+']身份证人像面照片'" prop="idcard1Img">
            <div v-if="detailData.idcard1Img">
              <a-upload
                :default-file-list="getDefaultFileList(detailData.idcard1Img)"
                :showUploadList="{ showPreviewIcon:false, showRemoveIcon:false, showDownloadIcon:false }"
                list-type="picture"
                class="detail-upload-list-inline"
                @preview="imgPreview($event)"
              />
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item :label="'['+this.imgLabel+']身份证国徽面照片'" prop="idcard2Img">
            <div v-if="detailData.idcard2Img">
              <a-upload
                :default-file-list="getDefaultFileList(detailData.idcard2Img)"
                :showUploadList="{ showPreviewIcon:false, showRemoveIcon:false, showDownloadIcon:false }"
                list-type="picture"
                class="detail-upload-list-inline"
                @preview="imgPreview($event)"
              />
            </div>
          </a-form-model-item>
        </a-col>
        <a-col :span="10">
          <a-form-model-item label="[联系人]手持身份证照片" prop="idcardInHandImg">
            <div v-if="detailData.idcardInHandImg">
              <a-upload
                :default-file-list="getDefaultFileList(detailData.idcardInHandImg)"
                :showUploadList="{ showPreviewIcon:false, showRemoveIcon:false, showDownloadIcon:false }"
                list-type="picture"
                class="detail-upload-list-inline"
                @preview="imgPreview($event)"
              />
            </div>
          </a-form-model-item>
        </a-col>
        <!-- 个人对私/企业对私 -->
        <a-col :span="10" v-if="detailData.settAccountType === 'BANK_PRIVATE'">
          <a-form-model-item :label="'['+this.imgLabel+']银行卡照片'" prop="bankCardImg">
            <div v-if="detailData.bankCardImg">
              <a-upload
                :default-file-list="getDefaultFileList(detailData.bankCardImg)"
                :showUploadList="{ showPreviewIcon:false, showRemoveIcon:false, showDownloadIcon:false }"
                list-type="picture"
                class="detail-upload-list-inline"
                @preview="imgPreview($event)"
              />
            </div>
          </a-form-model-item>
        </a-col>
      </a-row>
    </div>
  </a-drawer>
</template>

<script>
import { API_URL_AGENT_LIST, API_URL_ISV_LIST, req } from '@/api/manage'
import 'viewerjs/dist/viewer.css'
export default {
  name: 'Detail',
  props: {
    callbackFunc: { type: Function, default: () => () => ({}) }
  },
  data () {
    return {
      btnLoading: false,
      detailData: {}, // 数据对象
      recordId: null, // 更新对象ID
      visible: false, // 是否显示弹层/抽屉
      isvList: null, // 服务商下拉列表
      isvName: '', // 服务商名称
      imgLabel: '联系人'
    }
  },
  created () {
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.detailData = { 'state': 1, 'addAgentFlag': 1, 'type': 1, 'settAccountTypeName': '个人微信', 'settAccountNoLabel': '个人微信号' } // 数据清空
      if (this.$refs.infoFormModel !== undefined) {
        this.$refs.infoFormModel.resetFields()
      }
      const that = this
      that.recordId = recordId
      req.getById(API_URL_AGENT_LIST, recordId).then(res => {
        that.detailData = res
        switch (that.detailData.settAccountType) {
          case 'WX_CASH':
            that.detailData.settAccountTypeName = '个人微信'
            that.detailData.settAccountNoLabel = '个人微信号'
            break
          case 'ALIPAY_CASH':
            that.detailData.settAccountTypeName = '个人支付宝'
            that.detailData.settAccountNoLabel = '支付宝账号'
            break
          case 'BANK_PRIVATE':
            that.detailData.settAccountTypeName = '对私账户'
            that.detailData.settAccountNoLabel = '收款银行卡号'
            break
          case 'BANK_PUBLIC':
            that.detailData.settAccountTypeName = '对公账户'
            that.detailData.settAccountNoLabel = '对公账号'
            break
        }
        if (that.detailData.agentType === 2) {
          this.imgLabel = '法人'
        } else {
          this.imgLabel = '联系人'
        }
      })
      req.list(API_URL_ISV_LIST, { 'pageSize': null }).then(res => { // 服务商下拉选择列表
        that.isvList = res.records
        for (let i = 0; i < that.isvList.length; i++) {
          if (that.detailData.isvNo === that.isvList[i].isvNo) {
            that.isvName = that.isvList[i].isvName
          }
        }
      })
      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    imgPreview (info) {
      // console.log(info)
      this.$viewerApi({
        images: [info.url],
        options: {
          initialViewIndex: 0
        }
      })
    },
    getDefaultFileList (url) {
      if (!url) {
        return []
      }
      return [{
        uid: '-1',
        name: url.split('/').pop(),
        status: 'done',
        url: url,
        thumbUrl: url
      }]
    }
  }
}
</script>

<style lang="less">
  //.detail-upload-list-inline .ant-upload-list-item-card-actions.picture {
  //  display: none;
  //}
</style>
