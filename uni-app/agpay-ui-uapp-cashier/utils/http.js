const BASE_URL = "https://localhost:9819";

// 通用请求方法封装
const request = (url, method, data = {}, cache = false, headers = {}) => {
	return new Promise((resolve, reject) => {

		// 缓存处理
		const cacheKey = `${method}:${url}:${JSON.stringify(data)}`;
		if (cache) {
			const cachedData = uni.getStorageSync(cacheKey);
			if (cachedData) {
				console.log("返回缓存数据：", cacheKey);
				return resolve(cachedData);
			}
		}

		uni.request({
			url: BASE_URL + url,
			method,
			data,
			header: {
				"Content-Type": "application/json",
				...headers
			},
			success: (res) => {
				// 接口实际返回数据 格式为：{code: '', msg: '', data: ''}， res.data 是axios封装对象的返回数据；
				console.log(res)
				const resData = res.data
				console.log(resData)
				// 相应结果不为0， 说明异常
				if (resData.code !== 0) {
					uni.reLaunch({ url: "/pages/error/error?message=" + resData.msg });
					reject(resData);
				} else {
					if (cache) {
						uni.setStorageSync(cacheKey, resData);
					}
					resolve(resData.data);
				}
			},
			fail: (err) => {
				uni.showToast({ title: "网络错误", icon: "none" });
				reject(err);
			}
		});
	});
};

// 导出 GET / POST / PUT / DELETE 方法
export const get = (url, params = {}, cache = false, headers = {}) => request(url, "GET", params, cache, headers);
export const post = (url, data = {}, headers = {}) => request(url, "POST", data, false, headers);
export const put = (url, data = {}, headers = {}) => request(url, "PUT", data, false, headers);
export const del = (url, data = {}, headers = {}) => request(url, "DELETE", data, false, headers);