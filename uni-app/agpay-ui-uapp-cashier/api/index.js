import { post } from "@/utils/http";

// 获取url API
export const getRedirectUrl = (data) => post("/api/cashier/redirectUrl", data);

// 获取用户ID API
export const getChannelUserId = (data) => post("/api/cashier/channelUserId", data);

// 调起支付接口, 获取订单信息 API
export const getPayOrderInfo = (data) => post("/api/cashier/payOrderInfo", data);

// 调起支付接口, 获取支付数据包 API
export const getPayPackage = (data) => post("/api/cashier/pay", data);

// 取消订单 API
export const cancelPay = (data) => post("/api/cashier/cancelPay", data);
