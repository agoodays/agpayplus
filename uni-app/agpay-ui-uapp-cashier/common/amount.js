/**
 * 千分位格式化函数
 * @param {number|string} num - 需要格式化的数字或字符串
 * @returns {string} 格式化后的字符串
 */
export const formatThousands = (num) => {
	if (num === null || num === undefined) return '';

	const str = num.toString().trim();
	if (str === '') return '';

	// 处理负数
	let sign = '';
	let numberStr = str;
	if (str.startsWith('-')) {
		sign = '-';
		numberStr = str.slice(1);
	}

	// 分割整数和小数部分
	const parts = numberStr.split('.');
	let integerPart = parts[0] || '0';
	const decimalPart = parts[1] || '';

	// 添加千分位逗号
	let formattedInteger = '';
	while (integerPart.length > 3) {
		formattedInteger = `,${integerPart.slice(-3)}${formattedInteger}`;
		integerPart = integerPart.slice(0, -3);
	}
	formattedInteger = integerPart + formattedInteger;

	// 拼接结果
	let result = `${sign}${formattedInteger}`;
	if (decimalPart) {
		result += `.${decimalPart}`;
	} else if (str.endsWith('.')) {
		result += '.';
	}

	return result;
}