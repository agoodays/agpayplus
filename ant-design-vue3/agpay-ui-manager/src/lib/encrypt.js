import CryptoJS from 'crypto-js';

const Base64 = {
    encode: function (data) {
        // Base64 加密
        return CryptoJS.enc.Utf8.stringify(words)
    },

    decode: function (data) {
        // Base64 解密
        return CryptoJS.enc.Base64.parse(data);
    }
}
