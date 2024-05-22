import CryptoJS from 'crypto-js';

export const Base64 = {
    /**
     * Base64 加密
     * @param data
     * @returns {string}
     */
    encode: function (data) {
        return CryptoJS.enc.Base64.stringify(CryptoJS.enc.Utf8.parse(data));
    },
    /**
     * Base64 解密
     * @param data
     * @returns {*}
     */
    decode: function (data) {
        return CryptoJS.enc.Utf8.stringify(CryptoJS.enc.Base64.parse(data));
    }
}
