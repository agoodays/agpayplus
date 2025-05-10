import { defineStore } from "pinia";

export const usePayStore = defineStore("pay", {
	state: () => ({
		token: uni.getStorageSync("token") || ""
	}),
	actions: {
		setToken(token) {
			this.token = token;
			uni.setStorageSync("token", token);
		},
		clearToken() {
			this.token = "";
			uni.removeStorageSync("token");
		}
	}
});
