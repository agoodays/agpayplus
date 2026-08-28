# ChannelUserModal 渠道用户ID获取弹窗组件

## 概述

渠道用户ID获取弹窗组件，通过展示二维码让用户扫码获取渠道用户ID（微信OpenId/支付宝UserId）。使用WebSocket实时监听扫码结果。

## 组件说明

该组件提供以下功能：
- 展示微信/支付宝二维码
- 使用 ReconnectingWebSocket 实时监听扫码结果
- 扫码成功后自动关闭弹窗并触发回调

## 使用示例

```vue
<template>
  <channel-user-modal @change-channel-user-id="handleChannelUserId" />
</template>

<script setup>
import { ref } from 'vue'
import ChannelUserModal from '@/components/channel-user'

const channelUserModalRef = ref(null)

const showChannelUserModal = async (appId, ifCode) => {
  await channelUserModalRef.value.showModal(appId, ifCode, { customData: 'xxx' })
}

const handleChannelUserId = ({ channelUserId, extObject }) => {
  console.log('获取到渠道用户ID:', channelUserId)
  console.log('扩展对象:', extObject)
}
</script>
```

## Props

无

## 事件

| 事件名 | 说明 | 参数 |
|--------|------|------|
| changeChannelUserId | 用户扫码成功后触发 | { channelUserId, extObject } |

## 方法

| 方法名 | 说明 | 参数 |
|--------|------|------|
| showModal | 显示弹窗并获取二维码 | appId: String, ifCode: String, extObj: Object |

### showModal 参数说明

| 参数 | 类型 | 说明 |
|------|------|------|
| appId | String | 应用ID |
| ifCode | String | 支付接口代码（wxpay/alipay） |
| extObj | Object | 扩展对象，将原样返回 |

## 功能特性

1. **WebSocket连接**：使用 ReconnectingWebSocket 实现自动重连
2. **二维码展示**：使用 a-qrcode 组件生成二维码
3. **扫码监听**：实时监听扫码结果，成功后自动关闭弹窗
4. **扩展对象传递**：支持传递扩展对象，扫码成功后原样返回

## 注意事项

1. 需要后端提供二维码获取接口和WebSocket服务
2. 每次调用showModal前会关闭之前的WebSocket连接
3. WebSocket地址通过 `basicApi.getWebSocketPrefix()` 获取
4. 依赖 `a-qrcode` 和 `reconnectingwebsocket` 包