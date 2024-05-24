/*
 *  登录
 *
 */
import { request, getRequest } from '/@/lib/axios';
import { LOGIN_METHOD_ENUM } from '/@/constants/system/login-const';
import { Base64 } from '/@/lib/encrypt';

export const loginApi = {
    /**
     * 登录
     */
    login: ({ loginMethod, username, password, mobile, vercode, vercodeToken }) => {
        let data, url
        if (loginMethod === LOGIN_METHOD_ENUM.MESSAGE) {
            data = {
                phone: Base64.encode(mobile), // 手机号
                code: Base64.encode(vercode) // 验证码值
            }
            url = '/api/anon/auth/phoneCode'
        } else {
            data = {
                ia: Base64.encode(username), // 账号
                ip: Base64.encode(password), // 密码
                vc: Base64.encode(vercode), // 验证码值
                vt: Base64.encode(vercodeToken), // 验证码token
                lt: Base64.encode('WEB') // 登录类型
            }
            url = '/api/anon/auth/validate'
        }
        return request({
            url: url,
            method: 'post',
            data: data
        }, true, false, false)
    },

    /**
     * 退出登录
     */
    logout: () => {
        return request({ url: 'api/current/logout', method: 'post' }, true, true, true)
    },

    /**
     * 获取验证码
     */
    getVercode: () => {
        return request({ url: '/api/anon/auth/vercode', method: 'get' }, true, true, true)
    },

    /**
     * 获取登录信息
     */
    getCurrentInfo: () => {
        return getRequest('/api/current/user');
    },

    /**
     * 找回密码
     * @param phone
     * @param code
     * @param confirmPwd
     * @returns {*}
     */
    forget:({ phone, code, confirmPwd })=>{
        const data = {
            phone: Base64.encode(phone), // 手机号
            code: Base64.encode(code), // 验证码
            newPwd: Base64.encode(confirmPwd) // 密码
        }
        return request.request({
            url: '/api/anon/cipher/retrieve',
            method: 'post',
            data: data
        }, true, true, true)
    },
    /**
     * 发送短信验证码
     * @param data
     * @returns {*}
     */
    sendcode: (data) => {
        return request.request({
            url: '/api/anon/sms/code',
            method: 'post',
            data: data
        }, true, true, true)
    },
};
