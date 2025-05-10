/**
 * 支付令牌验证工具
 * 验证规则：64位十六进制字符串（不区分大小写）
 */

/**
 * 验证 JeepayToken 格式
 * @param {string} token - 待验证的支付令牌
 * @returns {boolean} 是否有效
 */
export const validateToken = (token) => {
	// 基础类型检查
	if (typeof token !== 'string') return false

	// 严格格式验证
	return /^[a-f0-9]{64}$/i.test(token)
}

/**
 * 增强版 Token 验证（带详细错误信息）
 * @param {string} token 
 * @returns {{valid: boolean, reason?: string}}
 */
export const validateTokenWithReason = (token) => {
	if (typeof token !== 'string') {
		return {
			valid: false,
			reason: 'INVALID_TYPE: Token 必须为字符串类型'
		}
	}

	if (token.length !== 64) {
		return {
			valid: false,
			reason: `LENGTH_MISMATCH: 需要64位字符 (当前: ${token.length}位)`
		}
	}

	if (!/^[a-f0-9]+$/i.test(token)) {
		const invalidChars = [...new Set(token.match(/[^a-f0-9]/gi))]
		return {
			valid: false,
			reason: `INVALID_CHARS: 包含非法字符 [${invalidChars.join(',')}]`
		}
	}

	return {
		valid: true
	}
}

// // 测试用例（开发时使用）
// if (process.env.NODE_ENV === 'development') {
//	 const testCases = [
//		 { token: 'e9134b1a2a26572554cfdb889a7ac230f0392ee7bc9ef5cd36418f3273c8bf3d', valid: true },
//		 { token: 'E9134B1A2A26572554CFDB889A7AC230F0392EE7BC9EF5CD36418F3273C8BF3D', valid: true }, // 大写
//		 { token: 'e9134b1a2a26572554cfdb889a7ac230', valid: false }, // 32位
//		 { token: 123456, valid: false }, // 数字类型
//		 { token: '', valid: false }, // 空字符串
//		 { token: 'g'.repeat(64), valid: false }, // 非法字符
//		 { token: null, valid: false },
//		 { token: undefined, valid: false }
//	 ]

//	 testCases.forEach(({ token, valid }) => {
//		 console.assert(
//			 validateToken(token) === valid,
//			 `Token 验证失败: ${token} (预期: ${valid})`
//		 )
//	 })
// }