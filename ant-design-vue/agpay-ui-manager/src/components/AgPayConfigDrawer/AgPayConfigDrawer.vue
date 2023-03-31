<template>
  <a-drawer
    :visible="visible"
    :title="true ? '支付配置' : ''"
    @close="onClose"
    :body-style="{ paddingBottom: '80px' }"
    width="80%">
    <a-tabs v-model="activeKey">
      <a-tab-pane :key="1" tab="参数及费率的填写">
        <div class="search">
          <a-input class="rate-input" placeholder="搜索渠道名称" v-model="searchData.ifName"/>
          <a-input class="rate-input" placeholder="搜索渠道代码" v-model="searchData.ifCode"/>
          <a-button type="primary" icon="search" @click="searchFunc">查询</a-button>
          <a-button style="margin-left: 8px" icon="reload" @click="() => this.searchData = {}">重置</a-button>
        </div>
        <div class="pay-list-wrapper" :style="{ 'height': isShowMore ? 'auto' : '110px' }">
          <div class="pay-item-wrapper" v-for="(item, key) in payList" :key="key">
            <div class="pay-content" :class="{ 'pay-selected' : selectedIfCode === item.ifCode }" @click="selectedIfCode = item.ifCode">
              <div class="pay-img" :style="{ backgroundColor: item.bgColor }">
                <img :src="item.icon" alt="">
                <div class="pay-state-dot" :style="{ backgroundColor: item.state ? '#29CC96FF' : '#D9D9D9FF' }"></div>
              </div>
              <div class="pay-info">
                <div class="pay-title">{{ item.ifName }}</div>
                <div class="pay-code" >{{ item.ifCode }}</div>
              </div>
            </div>
          </div>
        </div>
        <div class="tab-wrapper" v-if="selectedIfCode">
          <div class="tab-content">
            <div class="tab-item" :class="{ 'tab-selected' : selectedTab === 'param' }" @click="selectedTab = 'param'">参数配置</div>
            <div class="tab-item" :class="{ 'tab-selected' : selectedTab === 'rate' }" @click="selectedTab = 'rate'">费率配置</div>
          </div>
          <div class="open-close" @click="isShowMore = !isShowMore">
            {{ isShowMore ? '收起' : '展开' }}
            <a-icon :type="isShowMore ? 'up' : 'down'" />
          </div>
        </div>
        <div class="content-box" v-if="selectedIfCode">
          <div v-if="selectedTab === 'param'">
            {{ selectedIfCode }} —— 参数配置
          </div>
          <div v-else>
            {{ selectedIfCode }} —— 费率配置
          </div>
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>

<script>
const payList = [
  {
    ifCode: 'wxpay',
    normalMchParams: '[{"name":"mchId", "desc":"微信支付商户号", "type": "text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"key", "desc":"API密钥", "type": "textarea","verify":"required","star":"1"},{"name":"apiVersion", "desc":"微信支付API版本", "type": "radio","values":"V2,V3","titles":"V2,V3","verify":"required"},{"name":"apiV3Key", "desc":"API V3秘钥（V3接口必填）", "type": "textarea","verify":"","star":"1"},{"name":"serialNo", "desc":"序列号（V3接口必填）", "type": "textarea","verify":"","star":"1" },{"name":"cert", "desc":"API证书(.p12格式)", "type": "file","verify":""},{"name":"apiClientKey", "desc":"私钥文件(.pem格式)", "type": "file","verify":""}]',
    isIsvMode: 1,
    ifName: '微信支付官方',
    isOpenCheckBill: 1,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/wxpay.png',
    isSupportCheckBill: 1,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '微信官方通道',
    configPageType: 2,
    isMchMode: 1,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:32:23',
    wayCodes: [
      {
        wayCode: 'WX_APP'
      },
      {
        wayCode: 'WX_H5'
      },
      {
        wayCode: 'WX_NATIVE'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"subMchId","desc":"子商户ID","type":"text","verify":"required"},{"name":"subMchAppId","desc":"子商户公众号AppId(置空表示使用服务商)","type":"text"},{"name":"subMchLiteAppId","desc":"子商户小程序AppId(置空表示使用服务商)","type":"text"},{"name":"subMchOpenAppId","desc":"子商户App软件微信开放平台中AppId(使用app支付必填)","type":"text"}]',
    bgColor: '#02c067',
    isvParams: '[{"name":"mchId","desc":"微信支付商户号","type":"text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"key","desc":"API密钥","type":"textarea","verify":"required","star":"1"},{"name":"apiVersion","desc":"微信支付API版本","type":"radio","verify":"required","values":"V2,V3","titles":"V2,V3"},{"name":"apiV3Key","desc":"API V3秘钥（V3接口必填）","type":"textarea","verify":"","star":"1"},{"name":"serialNo","desc":"序列号（V3接口必填）","type":"textarea","verify":"","star":"1"},{"name":"cert","desc":"API证书(.p12格式)","type":"file","verify":""},{"name":"apiClientKey","desc":"私钥文件(.pem格式)","type":"file","verify":""},{"name":"test","desc":"test","type":"tupleList","verify":"required","tupleDefine":"{\\"test01\\":\\"test01\\",\\"test02\\":\\"test02\\"}"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'alipay',
    normalMchParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"","values":"1,0","titles":"沙箱环境,生产环境","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"privateKey", "desc":"应用私钥", "type": "textarea","verify":"required","star":"1"},{"name":"alipayPublicKey", "desc":"支付宝公钥(不使用证书时必填)", "type": "textarea","star":"1"},{"name":"signType","desc":"接口签名方式(推荐使用RSA2)","type":"radio","verify":"","values":"RSA,RSA2","titles":"RSA,RSA2","verify":"required"},{"name":"useCert","desc":"公钥证书","type":"radio","verify":"","values":"1,0","titles":"使用证书（请使用RSA2私钥）,不使用证书"},{"name":"appPublicCert","desc":"应用公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayPublicCert","desc":"支付宝公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayRootCert","desc":"支付宝根证书（.crt格式）","type":"file","verify":""}]',
    isIsvMode: 1,
    ifName: '支付宝官方',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/alipay.png',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '支付宝官方通道',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:32:24',
    wayCodes: [
      {
        wayCode: 'ALI_APP'
      },
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_PC'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'ALI_WAP'
      }
    ],
    isvsubMchParams: '[{"name":"appAuthToken", "desc":"子商户app_auth_token", "type": "text","readonly":"readonly"},{"name":"refreshToken", "desc":"子商户刷新token", "type": "hidden","readonly":"readonly"},{"name":"expireTimestamp", "desc":"authToken有效期（13位时间戳）", "type": "hidden","readonly":"readonly"}]',
    bgColor: '#cccccc',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"","values":"1,0","titles":"沙箱环境,生产环境","verify":"required"},{"name":"pid","desc":"合作伙伴身份（PID）","type":"text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"privateKey", "desc":"应用私钥", "type": "textarea","verify":"required","star":"1"},{"name":"alipayPublicKey", "desc":"支付宝公钥(不使用证书时必填)", "type": "textarea","star":"1"},{"name":"signType","desc":"接口签名方式(推荐使用RSA2)","type":"radio","verify":"","values":"RSA,RSA2","titles":"RSA,RSA2","verify":"required"},{"name":"useCert","desc":"公钥证书","type":"radio","verify":"","values":"1,0","titles":"使用证书（请使用RSA2私钥）,不使用证书"},{"name":"appPublicCert","desc":"应用公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayPublicCert","desc":"支付宝公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayRootCert","desc":"支付宝根证书（.crt格式）","type":"file","verify":""}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'ysfpay',
    isIsvMode: 1,
    ifName: '云闪付官方',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/ysfpay.png',
    isSupportCheckBill: 1,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '云闪付官方通道',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:32:25',
    wayCodes: [
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'WX_BAR'
      }
    ],
    isvsubMchParams: '[{"name":"merId","desc":"云闪付商户号","type":"text","verify":"required"}, {"name":"acqMerId","desc":"收单机构商户号","type":"text","verify":""}]',
    bgColor: 'red',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"","values":"1,0","titles":"沙箱环境,生产环境","verify":"required"},{"name":"serProvId","desc":"服务商开发ID[serProvId]","type":"text","verify":"required"},{"name":"isvPrivateCertFile","desc":"服务商私钥文件（.pfx格式）","type":"file","verify":"required"},{"name":"isvPrivateCertPwd","desc":"服务商私钥文件密码","type":"text","verify":"required","star":"1"},{"name":"ysfpayPublicKey","desc":"云闪付开发公钥（证书管理页面可查询）","type":"textarea","verify":"required","star":"1"},{"name":"acqOrgCode","desc":"可用支付机构编号","type":"text","verify":"required"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'sftpay',
    isIsvMode: 1,
    ifName: '微信收付通',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/wxpay.png',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '微信电商收付通',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 0,
    createdAt: '2022-12-11 21:32:26',
    wayCodes: [
      {
        wayCode: 'WX_APP'
      },
      {
        wayCode: 'WX_NATIVE'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'WX_H5'
      }
    ],
    isvsubMchParams: '[{"name":"subMchId","desc":"子商户ID","type":"text","verify":"required"},{"name":"subMchAppId","desc":"子账户appID(线上支付必填)","type":"text","verify":""}]',
    bgColor: '#04BE02',
    isvParams: '[{"name":"mchId", "desc":"微信支付商户号", "type": "text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"oauth2Url", "desc":"oauth2地址（置空将使用官方）", "type": "text"},{"name":"apiV3Key", "desc":"API V3秘钥", "type": "textarea","verify":"","star":"1"},{"name":"serialNo", "desc":"序列号", "type": "textarea","verify":"","star":"1"},{"name":"cert", "desc":"API证书(.p12格式)", "type": "file","verify":""},{"name":"apiClientCert", "desc":"证书文件(apiclient_cert.pem证书文件)", "type": "file","verify":""},{"name":"apiClientKey", "desc":"私钥文件(apiclient_key.pem证书文件)", "type": "file","verify":""}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2022-12-11 21:32:26'
  },
  {
    ifCode: 'shengpay',
    normalMchParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"mchId","desc":"商户ID","type":"text","verify":"required"},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"盛付通公钥","type":"textarea","verify":"required","star":"1"},{"name":"wxLiteAppId","desc":"微信小程序appId","type":"text"}]',
    isIsvMode: 1,
    ifName: '盛付通',
    isOpenCheckBill: 1,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/shengpay.png',
    isSupportCheckBill: 1,
    isOpenCashout: 0,
    isSupportCashout: 1,
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    'configState': 1,
    createdAt: '2022-12-11 21:35:28',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'AUTO_POS'
      },
      {
        wayCode: 'UP_BAR'
      },
      {
        wayCode: 'UP_JSAPI'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'UP_WAP'
      },
      {
        wayCode: 'WEIXIN_BAR_PAY'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_H5'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"subMchId","desc":"子商户号","type":"text","verify":"required"}, {"name":"subMchLiteAppId","desc":"子商户小程序AppId(置空表示使用服务商)","type":"text"}]',
    bgColor: '#3775B3',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"mchId","desc":"商户ID","type":"text","verify":"required"},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"appPublicKey","desc":"应用公钥（分账使用）","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"盛付通公钥","type":"textarea","verify":"required","star":"1"},{"name":"wxLiteAppId","desc":"微信小程序appId","type":"text"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'fuioupay',
    isIsvMode: 1,
    ifName: '富友支付',
    isOpenCheckBill: 0,
    icon: 'https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/fuioupay.png',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '富友支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:35:31',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      },
      {
        wayCode: 'AUTO_POS'
      }
    ],
    isvsubMchParams: '[{"name":"mchntCd","desc":"二级商户号","type":"text","verify":"required"},{"name":"wxSubAppid","desc":"商户公众号appid","type":"text","verify":""},{"name":"wxSubLiteAppid","desc":"商户小程序appid","type":"text","verify":""}]',
    bgColor: '#b7d0f0',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"insCd","desc":"机构号","type":"text","verify":"required"},{"name":"insCd4OrderPrefix","desc":"机构编码(4位，用于订单前缀)","type":"text","verify":"required"},{"name":"termId","desc":"默认终端号(未通过本系统在富友侧进行终端报备时，使用该默认值)","type":"text","verify":""},{"name":"md5Key","desc":"进件md5Key","type":"text","verify":"required","star":"1"},{"name":"privateKey","desc":"私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"富友公钥","type":"textarea","verify":"required","star":"1"},{"name":"applyFtpUser","desc":"进件FTP帐号","type":"text","verify":""},{"name":"applyFtpPwd","desc":"进件FTP密码","type":"text","verify":"","star":"1"},{"name":"shareFtpUser","desc":"分账FTP帐号","type":"text","verify":""},{"name":"shareFtpPwd","desc":"分账FTP密码","type":"text","verify":"","star":"1"},{"name":"wxAppid","desc":"公众号appid","type":"text","verify":""},{"name":"wxLiteAppid","desc":"商户小程序appid","type":"text","verify":""},{"name":"ymSysId","desc":"【云秘POS】系统ID（sysId）","type":"text","verify":""},{"name":"ymPosMd5Key","desc":"【云秘POS】md5Key","type":"text","verify":"","star":"1"},{"name":"ymPosPrivateKey","desc":"【云秘POS】私钥","type":"textarea","verify":"required","star":"1"},{"name":"ymPosPublicKey","desc":"【云秘POS】富友公钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 1,
    state: 0,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'hmpay',
    isIsvMode: 1,
    ifName: '河马付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/sandpay.png',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '河马付',
    configPageType: 2,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:35:31',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"subAppId","desc":"子商户ID（sub_app_id）","type":"text","verify":"required"},{"name":"wxLiteAppId","desc":"微信appId","type":"text","verify":""}]',
    bgColor: '#1A2131',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"appId","desc":"服务商ID（app_id）","type":"text","verify":"required"},{"name":"privateKey","desc":"服务商私钥（privateKey）","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"平台公钥（platPublicKey）","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'lklpay',
    normalMchParams: '[\t{\n\t\t"name": "publicCert",\n\t\t"desc": "拉卡拉公钥证书（.cer格式）",\n\t\t"type": "file",\n\t\t"verify": "required"\n\t}\n]',
    isIsvMode: 1,
    ifName: '拉卡拉支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/lklpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '拉卡拉支付',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:35:32',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'AUTO_POS'
      },
      {
        wayCode: 'DDDD'
      },
      {
        wayCode: 'FT_APP'
      },
      {
        wayCode: 'FUIOUPAY'
      },
      {
        wayCode: 'UMS'
      },
      {
        wayCode: 'UP_JSAPI'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"merCupNo","desc":"商户号（merCupNo）","type":"text","verify":"required"},{"name":"termNo","desc":"终端号（termId）","type":"text","verify":"required"},{"name":"subMchId","desc":"微信子商户号（subMchId）","type":"text","verify":""}]',
    bgColor: '#00AFEC',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"orgCode","desc":"机构号","type":"text","verify":"required"},{"name":"appId","desc":"appId","type":"text","verify":"required"},{"name":"wxChannelId","desc":"微信渠道号","type":"text","verify":""},{"name":"wxOpenUrl","desc":"微信渠道拓展二维码URL","type":"text","verify":""},{"name":"aliChannelExtUrl","desc":"支付宝渠道拓展二维码URL","type":"text","verify":""},{"name":"serialNo","desc":"证书序列号","type":"text","verify":"required"},{"name":"privateCert","desc":"应用私钥证书（.pem格式）","type":"file","verify":"required"},{"name":"publicCert","desc":"拉卡拉公钥证书（.cer格式）","type":"file","verify":"required"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'fbpay',
    normalMchParams: '[{"name":"mchNo","desc":"商户号","type":"text","verify":"required"},{"name":"storeNo","desc":"门店号","type":"text","verify":""},{"name":"privateKey","desc":"商户私钥","type":"textarea","verify":"required","star":"1"}]',
    isIsvMode: 1,
    ifName: '付呗支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/fbpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '付呗支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 0,
    createdAt: '2022-12-11 21:35:33',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"subMchNo","desc":"子商户号","type":"text","verify":"required"},{"name":"storeNo","desc":"门店号","type":"text","verify":""},{"name":"wxSubAppId","desc":"公众号appId","type":"text","verify":""}]',
    bgColor: '#E83E43',
    isvParams: '[{"name":"vendorSn","desc":"服务商ID","type":"text","verify":"required"},{"name":"privateKey","desc":"服务商私钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 0,
    state: 1,
    updatedAt: '2022-12-11 21:35:33'
  },
  {
    ifCode: 'pfpay',
    normalMchParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"mchNo","desc":"商户号","type":"text","verify":"required"},{"name":"storeNo","desc":"门店编号","type":"text","verify":"required"},{"name":"terminalNo","desc":"终端号","type":"text","verify":"required"},{"name":"appClientId","desc":"应用标识(appClientId)","type":"text","verify":"required"},{"name":"clientSecret","desc":"请求秘钥(clientSecret)","type":"text","verify":"required","star":"1"},{"name":"mchPrivateKey","desc":"商户RSA私钥","type":"textarea","verify":"required","star":"1"},{"name":"pfpayPublicKey","desc":"浦发RSA公钥","type":"textarea","verify":"required","star":"1"}]',
    isIsvMode: 1,
    ifName: '浦发银行',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/pfpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '浦发银行',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 0,
    createdAt: '2022-12-11 21:35:34',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_NATIVE'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"mchNo","desc":"商户号","type":"text","verify":"required"},{"name":"storeNo","desc":"门店编号","type":"text","verify":"required"},{"name":"terminalNo","desc":"终端号","type":"text","verify":"required"},{"name":"appClientId","desc":"应用标识(appClientId)","type":"text","verify":"required"},{"name":"clientSecret","desc":"请求秘钥(clientSecret)","type":"text","verify":"required","star":"1"},{"name":"mchPrivateKey","desc":"商户RSA私钥","type":"textarea","verify":"required","star":"1"},{"name":"pfpayPublicKey","desc":"浦发RSA公钥","type":"textarea","verify":"required","star":"1"}]',
    bgColor: '#074195',
    isvParams: '[]',
    isSupportApplyment: 0,
    state: 1,
    updatedAt: '2022-12-11 21:35:34'
  },
  {
    ifCode: 'utmpay',
    isIsvMode: 1,
    ifName: '银联条码支付前置平台',
    isOpenCheckBill: 1,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/upacp.png',
    isSupportCheckBill: 1,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '银联条码支付前置平台',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:35:35',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'UMS'
      },
      {
        wayCode: 'UP_BAR'
      },
      {
        wayCode: 'UP_JSAPI'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'WEIXIN_BAR_PAY'
      },
      {
        wayCode: 'WX_APP'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'WX_NATIVE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"merchantId","desc":"商户号","type":"text","verify":"required"},{"name":"subMchAppId","desc":"子商户公众号AppId","type":"text","verify":""},{"name":"subMchLiteAppId","desc":"子商户小程序AppId","type":"text","verify":""},{"name":"subMchOpenAppId","desc":"微信开放平台移动应用AppId(app支付必填)","type":"text","verify":""}]',
    bgColor: '#1A1C33',
    isvParams: '[{"name":"partner","desc":"机构号","type":"text","verify":"required"},{"name":"md5Key","desc":"md5Key","type":"text","verify":"required","star":"1"},{"name":"appId","desc":"服务商公众号AppId","type":"text","verify":""},{"name":"liteAppId","desc":"服务商小程序AppId","type":"text","verify":""}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'sqbpay',
    normalMchParams: '',
    isIsvMode: 1,
    ifName: '收钱吧',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/sqbpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '收钱吧',
    configPageType: 2,
    isMchMode: 0,
    isOpenApplyment: 0,
    createdAt: '2022-12-11 21:35:36',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'WX_NATIVE'
      }
    ],
    isvsubMchParams: '',
    bgColor: '#F5D446',
    isvParams: '',
    isSupportApplyment: 0,
    state: 1,
    updatedAt: '2022-12-11 21:35:36'
  },
  {
    ifCode: 'allinpay',
    normalMchParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"signType","desc":"签名类型","type":"radio","verify":"required","values":"RSA,SM2","titles":"RSA,SM2"},{"name":"cusid","desc":"商户ID","type":"text","verify":"required"},{"name":"appId","desc":"appId","type":"text","verify":"required"},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"通联公钥","type":"textarea","verify":"required","star":"1"}]',
    isIsvMode: 1,
    ifName: '通联支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/allinpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '通联支付',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 1,
    createdAt: '2022-12-11 21:35:39',
    wayCodes: [
      {
        wayCode: 'ALI_APP'
      },
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'UP_BAR'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'WX_NATIVE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"cusid","desc":"商户号","type":"text","verify":"required"}]',
    bgColor: '#1A1C33',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"signType","desc":"签名类型","type":"radio","verify":"required","values":"RSA,SM2","titles":"RSA,SM2"},{"name":"orgid","desc":"代理商ID","type":"text","verify":"required"},{"name":"appId","desc":"appId","type":"text","verify":"required"},{"name":"expandUser","desc":"拓展人（商户进件）","type":"text","verify":""},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"通联公钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'mbpay',
    normalMchParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"merAccount","desc":"商户标识","type":"text","verify":"required"},{"name":"merNo","desc":"商户编号","type":"text","verify":"required"},{"name":"privateKey","desc":"商户私钥","type":"textarea","verify":"required","star":"1"},{"name":"mbPublicKey","desc":"米花平台公钥","type":"textarea","verify":"required","star":"1"}]',
    isIsvMode: 1,
    ifName: '米花支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/mbpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '米花支付',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 0,
    createdAt: '2022-12-11 21:35:41',
    wayCodes: [
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'ALI_WAP'
      },
      {
        wayCode: 'WX_NATIVE'
      }
    ],
    isvsubMchParams: '[{"name":"merAccount","desc":"商户标识","type":"text","verify":"required"},{"name":"merNo","desc":"商户编号","type":"text","verify":"required"}]',
    bgColor: '#EE8100',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"privateKey","desc":"商户私钥","type":"textarea","verify":"required","star":"1"},{"name":"mbPublicKey","desc":"米花平台公钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 0,
    state: 1,
    updatedAt: '2022-12-11 21:35:41'
  },
  {
    ifCode: 'cloudpay',
    normalMchParams: '',
    isIsvMode: 1,
    ifName: '支付宝云支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/alipay.png',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '支付宝云支付',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 0,
    createdAt: '2022-12-11 21:35:45',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"cpMid","desc":"云支付商户编号","type":"text","verify":"required"},{"name":"cpStoreId","desc":"云支付门店编号","type":"text","verify":"required"}]',
    bgColor: '#1779FF',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"bAppId","desc":"云支付应用ID","type":"text","verify":"required"},{"name":"priKey","desc":"网关加签私钥","type":"textarea","verify":"required","star":"1"},{"name":"pubKey","desc":"网关验签公钥","type":"textarea","verify":"required","star":"1"},{"name":"wxAppid","desc":"公众号appid","type":"text","verify":"required"},{"name":"wxLiteAppid","desc":"小程序appid","type":"text","verify":"required"}]',
    isSupportApplyment: 0,
    state: 1,
    updatedAt: '2022-12-11 21:35:45'
  },
  {
    ifCode: 'hkpay',
    isIsvMode: 1,
    ifName: '海科融通支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/hkpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '海科融通支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2022-12-31 17:13:42',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"merchNo","desc":"商户编号","type":"text","verify":"required"}]',
    bgColor: '#2D2F92',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"agentNo","desc":"服务商编号","type":"text","verify":"required"},{"name":"accessId","desc":"接入机构标识accessid","type":"text","verify":"required"},{"name":"accessKey","desc":"服务商的接入秘钥","type":"text","verify":"required"},{"name":"channelNoWx","desc":"微信渠道号[服务商通过海科在(微信)申请的渠道编号]","type":"text","verify":"required"},{"name":"channelNoAli","desc":"支付宝渠道号[服务商自行申请的支付宝渠道号]","type":"text","verify":"required"},{"name":"wxAppId","desc":"微信公众账号appid","type":"text","verify":"required"},{"name":"wxUrl","desc":"微信开户意愿地址","type":"text","verify":"required"},{"name":"aliUrl","desc":"支付宝商家认证地址","type":"text","verify":"required"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:39:36'
  },
  {
    ifCode: 'icbcpay',
    isIsvMode: 1,
    ifName: '工行支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/icbcpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '工行支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 0,
    createdAt: '2023-01-12 04:53:51',
    wayCodes: [
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"merId","desc":"商户号","type":"text","verify":"required"}]',
    bgColor: '#DA261F',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"appId","desc":"APPID","type":"text","verify":""},{"name":"merPrtclNo","desc":"收单产品协议编号","type":"text","verify":"required"},{"name":"wxAppId","desc":"微信公众号APPID","type":"text","verify":""},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"交行公钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:53:51'
  },
  {
    ifCode: 'dgpay',
    normalMchParams: '[{"name":"huifuId","desc":"商户号","type":"text","verify":"required"},{"name":"productId","desc":"产品ID","type":"text","verify":"required"},{"name":"rsaPrivateKey","desc":"商户私钥","type":"textarea","verify":"required","star":"1"},{"name":"rsaPublicKey","desc":"斗拱公钥","type":"textarea","verify":"required","star":"1"}]',
    isIsvMode: 1,
    ifName: '斗拱支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/dgpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '斗拱支付',
    configPageType: 1,
    isMchMode: 1,
    isOpenApplyment: 1,
    createdAt: '2023-01-12 04:53:52',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'AUTO_POS'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'WEIXIN_BAR_PAY'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_NATIVE'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"huifuId","desc":"商户号","type":"text","verify":"required"}]',
    bgColor: '#B5DCFF',
    isvParams: '[{"name":"settleCycle","desc":"商户结算周期","type":"radio","verify":"required","values":"T1,D1","titles":"T1,D1"},{"name":"settleFee","desc":"D1结算费率（填写值为 0.00-100.00 之间）","type":"text","verify":""},{"name":"mchSettManual","desc":"商户手动取现","type":"radio","verify":"required","values":"0,T1,D1,D0","titles":"关闭,T1,D1,D0"},{"name":"productId","desc":"产品ID","type":"radio","verify":"required","values":"PAYUN,EDUSTD,KAZX","titles":"PAYUN,EDUSTD,KAZX"},{"name":"sysId","desc":"服务商号","type":"text","verify":"required"},{"name":"wxOpenUrl","desc":"微信渠道拓展二维码URL","type":"text","verify":""},{"name":"aliChannelExtUrl","desc":"支付宝渠道拓展二维码URL","type":"text","verify":""},{"name":"agreementModel","desc":"【电子协议】协议模板号","type":"text","verify":""},{"name":"agreementName","desc":"【电子协议】协议模板名称","type":"text","verify":""},{"name":"rsaPrivateKey","desc":"商户私钥","type":"textarea","verify":"required","star":"1"},{"name":"rsaPublicKey","desc":"斗拱公钥","type":"textarea","verify":"required","star":"1"},{"name":"webhookPrivateKey","desc":"webhook终端秘钥（智能POS需配置此项）","type":"textarea","verify":"","star":"1"},{"name":"posPublicKey","desc":"智能POS公钥（智能POS需配置此项）","type":"textarea","verify":"","star":"1"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:53:52'
  },
  {
    ifCode: 'umpay',
    isIsvMode: 1,
    ifName: '联动优势',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/umpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '联动优势',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2023-01-12 04:53:52',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"storeId","desc":"商户号","type":"text","verify":"required"}]',
    bgColor: '#E60033',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"appId","desc":"服务商appId","type":"text","verify":"required"},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"联动公钥","type":"textarea","verify":"required","star":"1"},{"name":"wxOpenUrl","desc":"微信渠道拓展二维码URL","type":"text","verify":""},{"name":"aliChannelExtUrl","desc":"支付宝渠道拓展二维码URL","type":"text","verify":""},{"name":"wxChannelNo","desc":"微信渠道号 （多渠道时上送）","type":"text","verify":""},{"name":"aliChannelNo","desc":"支付宝渠道号 （多渠道时上送）","type":"text","verify":""}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:53:52'
  },
  {
    ifCode: 'bcmpay',
    isIsvMode: 1,
    ifName: '交行支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/bcmpay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '交行支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2023-01-12 04:53:53',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      },
      {
        wayCode: 'YSF_BAR'
      },
      {
        wayCode: 'YSF_JSAPI'
      }
    ],
    isvsubMchParams: '[{"name":"mchtId","desc":"商户号","type":"text","verify":"required"}]',
    bgColor: '#00377A',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"partnerId","desc":"partnerId","type":"text","verify":""},{"name":"appId","desc":"appId","type":"text","verify":"required"},{"name":"wxAppId","desc":"微信公众号APPID","type":"text","verify":""},{"name":"wxOpenUrl","desc":"微信开通意愿二维码url","type":"text","verify":""},{"name":"aliChannelExtUrl","desc":"支付宝开通意愿二维码url","type":"text","verify":""},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"交行公钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:53:53'
  },
  {
    ifCode: 'hnapay',
    isIsvMode: 1,
    ifName: '新生支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/hnapay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '新生支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2023-01-12 04:53:54',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"merchantNo","desc":"商户号","type":"text","verify":"required"}]',
    bgColor: '#E37629',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"orgNo","desc":"服务商编号","type":"text","verify":"required"},{"name":"wxAppid","desc":"公众号appid","type":"text","verify":""},{"name":"wxOpenUrl","desc":"微信开通意愿二维码url","type":"text","verify":""},{"name":"aliChannelExtUrl","desc":"支付宝开通意愿二维码url","type":"text","verify":""},{"name":"privateKey","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"publicKey","desc":"新生公钥","type":"textarea","verify":"required","star":"1"}]',
    isSupportApplyment: 1,
    state: 1,
    updatedAt: '2023-01-12 04:53:54'
  },
  {
    ifCode: 'zftpay',
    isIsvMode: 1,
    ifName: '支付宝直付通',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/alipay.png',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '支付宝直付通',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 0,
    createdAt: '2023-01-13 08:23:14',
    wayCodes: [
      {
        wayCode: 'ALI_APP'
      },
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_PC'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'ALI_WAP'
      }
    ],
    isvsubMchParams: '[{"name":"smid","desc":"子商户号","type":"text","verify":"required"}]',
    bgColor: '#1779FF',
    isvParams: '[{"name":"sandbox","desc":"环境配置","type":"radio","verify":"required","values":"1,0","titles":"沙箱环境,生产环境"},{"name":"settType","desc":"结算方式","type":"radio","verify":"required","values":"0,1","titles":"自动结算,主动结算"},{"name":"pid2","desc":"合作伙伴身份（PID）","type":"text","verify":"required"},{"name":"appId","desc":"应用App ID","type":"text","verify":"required"},{"name":"privateKey3","desc":"应用私钥","type":"textarea","verify":"required","star":"1"},{"name":"alipayPublicKey","desc":"支付宝公钥(不使用证书时必填)","type":"textarea","verify":"","star":"1"},{"name":"signType","desc":"接口签名方式(推荐使用RSA2)","type":"radio","verify":"required","values":"RSA,RSA2","titles":"RSA,RSA2"},{"name":"useCert","desc":"公钥证书","type":"radio","verify":"","values":"1,0","titles":"使用证书（请使用RSA2私钥）,不使用证书"},{"name":"appPublicCert","desc":"应用公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayPublicCert","desc":"支付宝公钥证书（.crt格式）","type":"file","verify":""},{"name":"alipayRootCert","desc":"支付宝根证书（.crt格式）","type":"file","verify":""}]',
    isSupportApplyment: 0,
    state: 1,
    updatedAt: '2023-01-13 08:23:14'
  },
  {
    ifCode: 'yspay',
    normalMchParams: '',
    isIsvMode: 1,
    ifName: '银盛支付',
    isOpenCheckBill: 0,
    icon: 'http://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/yspay.svg',
    isSupportCheckBill: 0,
    isOpenCashout: 0,
    isSupportCashout: 0,
    remark: '银盛支付',
    configPageType: 1,
    isMchMode: 0,
    isOpenApplyment: 1,
    createdAt: '2023-03-30 18:24:46',
    wayCodes: [
      {
        wayCode: 'ALI_BAR'
      },
      {
        wayCode: 'ALI_JSAPI'
      },
      {
        wayCode: 'ALI_LITE'
      },
      {
        wayCode: 'ALI_QR'
      },
      {
        wayCode: 'UP_BAR'
      },
      {
        wayCode: 'UP_QR'
      },
      {
        wayCode: 'YSF_JSAPI'
      },
      {
        wayCode: 'WX_BAR'
      },
      {
        wayCode: 'WX_JSAPI'
      },
      {
        wayCode: 'WX_LITE'
      }
    ],
    isvsubMchParams: '[{"name":"settType","desc":"下单类型","type":"radio","verify":"required","values":"0,1,2","titles":"默认配置,T1/D1,D0"},{"name":"mercId","desc":"商户号","type":"text","verify":"required"},{"name":"mercCnm","desc":"商户名称","type":"text","verify":""}]',
    bgColor: '#004DA0',
    isvParams: '[{"name":"signType","desc":"加密方式","type":"radio","verify":"required","values":"RSA,SM","titles":"RSA,SM2"},{"name":"settType","desc":"到账方式","type":"radio","verify":"required","values":"1,2","titles":"T1/D1,D0"},{"name":"partnerId","desc":"服务商号","type":"text","verify":"required"},{"name":"src","desc":"银盛机构号","type":"text","verify":"required"},{"name":"aesKey","desc":"AES秘钥","type":"text","verify":"required"},{"name":"wxOpenUrl","desc":"微信渠道拓展二维码URL","type":"text","verify":""},{"name":"wxAppId","desc":"微信公众号/小程序appId","type":"text","verify":""},{"name":"aliChannelExtUrl","desc":"支付宝渠道拓展二维码URL","type":"text","verify":""},{"name":"privateKeyPassword","desc":"私钥证书密码","type":"text","verify":"required"},{"name":"privateKeyFile","desc":"私钥证书（.pfx/.sm2）","type":"file","verify":"required"},{"name":"publicKeyFile","desc":"银盛公钥证书（.cer）","type":"file","verify":"required"}]',
    isSupportApplyment: 1,
    state: 0,
    updatedAt: '2023-03-30 18:24:46'
  }
]

export default {
  name: 'AgPayConfigDrawer',
  data () {
    return {
      visible: false, // 是否显示弹层/抽屉
      recordId: null, // 更新对象ID
      btnLoading: false,
      isShowMore: true,
      activeKey: 1,
      selectedIfCode: null,
      selectedTab: 'param',
      payList,
      searchData: {}
    }
  },
  methods: {
    show: function (recordId) { // 弹层打开事件
      this.recordId = recordId
      this.btnLoading = false
      this.isShowMore = true
      this.activeKey = 1
      this.selectedIfCode = null
      this.selectedTab = 'param'

      this.visible = true
    },
    onClose () {
      this.visible = false
    },
    searchFunc () {

    },
    paySelected (code) {
      this.selectedIfCode = code
    }
  }
}
</script>

<style scoped>
  .table-box {
    display: flex
  }

  .table-box .table-item {
    width: 50%
  }

  .if-name {
    display: flex;
    align-items: center
  }

  .if-name .back {
    margin-right: 20px;
    width: 30px;
    height: 30px;
    border-radius: 7px;
    display: flex;
    justify-content: center;
    align-items: center
  }

  .if-name .back img {
    width: 16px;
    height: 16px
  }

   .ant-table-wrapper {
    margin: 0
  }

  .search {
    display: flex;
    margin-top: 30px;
    margin-left: 50px
  }

  .search .rate-input {
    width: 200px;
    margin-right: 10px
  }

  .pay-list-wrapper {
    display: flex;
    flex-wrap: wrap;
    padding: 0 40px;
    margin-top: 20px;
    overflow: hidden
  }

  .pay-item-wrapper {
    padding: 10px;
    min-width: 220px;
    width: 20%
  }

  .pay-content {
    position: relative;
    display: flex;
    align-items: center;
    width: 100%;
    height: 90px;
    border-radius: 5px;
    border: 1px solid #dedede;
    background: #fff;
    cursor: pointer
  }

  .pay-content .pay-img {
    flex-shrink: 0;
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-left: 20px;
    margin-right: 12px;
    background-color: #0853ad;
    width: 50px;
    height: 50px;
    border-radius: 50%
  }

  .pay-content .pay-img img {
    width: 50%
  }

  .pay-content .pay-img .pay-state-dot {
    box-sizing: content-box;
    display: block;
    position: absolute;
    bottom: -3px;
    right: -3px;
    width: 12px;
    height: 12px;
    background-color: rgb(217, 217, 217);
    border: 3px solid #fff;
    border-radius: 50%
  }

  .pay-content .pay-info .pay-title {
    font-size: 14px;
    font-weight: 600
  }

  .pay-content .pay-info .pay-code {
    font-size: 13px;
    color: #1a1919
  }

  .pay-selected {
    border: 2px solid #1a79ff;
    background: rgba(25,121,255,.05)
  }

  .pay-selected:after {
    content: "\221a";
    position: absolute;
    right: 0;
    top: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    background-color: #1a79ff;
    color: #fff;
    font-size: 18px;
    font-weight: 700;
    border-radius: 0 0 0 5px
  }

  .tab-wrapper {
    position: relative;
    height: 50px
  }

  .tab-wrapper:after {
    content: "";
    display: block;
    position: absolute;
    top: 50%;
    width: 100%;
    height: 1px;
    background-color: #d9d9d9
  }

  .tab-wrapper .open-close {
    width: 80px;
    height: 36px;
    cursor: pointer;
    border-radius: 5px;
    background: #fff;
    border: 1px solid #e8e8e8;
    border-top: none;
    position: absolute;
    top: 50%;
    left: 50%;
    z-index: 1;
    margin-left: -40px;
    margin-top: -18px;
    display: flex;
    justify-content: center;
    align-items: center
  }

  .tab-wrapper .open-close:after,.tab-wrapper .open-close:before {
    content: "";
    display: block;
    position: absolute;
    top: 0;
    z-index: 10;
    width: 10px;
    height: 18px;
    background-color: #fff
  }

  .tab-wrapper .open-close:after {
    left: -1px
  }

  .tab-wrapper .open-close:before {
    right: -1px
  }

  .tab-content {
    position: relative;
    margin-top: 30px;
    display: flex;
    align-items: center;
    justify-content: space-around;
    z-index: 1;
    margin-left: 50px;
    width: max-content;
    padding: 0 5px;
    height: 50px;
    border-radius: 5px;
    background-color: #f7f7f7;
    border: 1px solid #d9d9d9;
    font-size: 14px;
    color: gray
  }

  .tab-content .tab-item {
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    width: 119px;
    height: 40px;
    cursor: pointer
  }

  .tab-selected {
    color: #000;
    box-shadow: 0 1px 4px #0000001a;
    background-color: #fff
  }

  .content-box {
    padding: 30px 50px
  }
</style>
