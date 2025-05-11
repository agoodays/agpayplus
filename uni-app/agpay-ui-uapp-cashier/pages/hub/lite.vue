<template>
	<view>
		<text>正在跳转至收银台...</text>
	</view>
</template>

<script>
	import config from '@/config/index';
	import * as api from '@/api/index';

	export default {
		data() {
			return {};
		},
		onLoad(options) {
			// 调用方法进行环境判断与跳转
			this.redirectToCheckout();
		},
		methods: {
			redirectToCheckout() {
				if (config.payWay.wayCode === 'wxpay') {
					// 调用获取 openid 的函数
					getOpenId();
				}
				if (config.payWay.wayCode === 'alipay') {
					// 调用获取 user_id 的函数
					getUserId();
				}
			},
			// 获取用户 openid
			getOpenId() {
				// 获取 code
				uni.login({
					provider: 'weixin', // 使用微信登录
					success: (res) => {
						const code = res.code; // 获取临时登录凭证 code
						console.log('code:', code);

						// 将 code 发送到服务器
						this.getChannelUserId({
							code: code
						});
					},
					fail: (err) => {
						console.error('登录失败:', err);
					}
				});
			},
			// 获取用户 user_id
			getUserId() {
				// 获取 authCode
				my.getAuthCode({
					scopes: ['auth_user'], // 授权范围
					success: (res) => {
						const authCode = res.authCode; // 获取授权码
						console.log('authCode:', authCode);

						// 将 authCode 发送到服务器
						this.getChannelUserId({
							auth_code: authCode
						});
					},
					fail: (err) => {
						console.error('获取 authCode 失败:', err);
					}
				});
			},
			getChannelUserId(redirectData) {
				api.getChannelUserId({
					wayCode: config.payWay.wayCode,
					token: config.tokenValue,
					...redirectData
				}).then(res => {
					console.log(res)
					// 设置 channelUserId
					config.channelUserId = res;
					uni.setStorageSync('channelUserId', res);

					// 跳转到统一的收银台页面
					uni.redirectTo({
						url: '/pages/payway/index',
					});
				}).catch(res => {
					uni.redirectTo({
						url: '/pages/error/index?errInfo=' + res.msg
					});
				});
			}
		}
	}
</script>

<style>
	/* 样式可以根据需求自定义 */
</style>