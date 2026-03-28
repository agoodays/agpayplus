/*
 * 格式化工具类
 *
 */
import dayjs from 'dayjs'

/**
 * 格式化数字，每三位添加逗号
 * @param {number|string} value - 要格式化的数字
 * @returns {string} 格式化后的数字字符串
 */
export function numberFormat(value) {
  if (!value) {
    return '0'
  }
  const intPartFormat = value.toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,') // 将整数部分逢三一断
  return intPartFormat
}

/**
 * 格式化日期
 * @param {string|Date} dateStr - 日期字符串或Date对象
 * @param {string} pattern - 日期格式，默认为 'YYYY-MM-DD HH:mm:ss'
 * @returns {string} 格式化后的日期字符串
 */
export function formatDate(dateStr, pattern = 'YYYY-MM-DD HH:mm:ss') {
  return dayjs(dateStr).format(pattern)
}

// 保持与原项目的 moment 过滤器兼容
export const momentFormat = formatDate
