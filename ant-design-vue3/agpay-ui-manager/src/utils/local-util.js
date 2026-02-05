/**
 * localStorage 工具类
 * 
 * 功能：
 * 1. 自动序列化/反序列化对象
 * 2. 支持设置过期时间
 * 3. 完善的错误处理
 * 4. 统一的 API
 */

// ==================== 基础存储 ====================

/**
 * 保存数据到 localStorage
 * @param {string} key - 存储键名
 * @param {any} value - 存储值（自动序列化对象）
 * @param {number} expires - 过期时间（毫秒），0 表示永不过期
 * @returns {boolean} 是否保存成功
 */
export const localSave = (key, value, expires = 0) => {
  try {
    const data = {
      value,
      expires: expires > 0 ? Date.now() + expires : 0
    }
    localStorage.setItem(key, JSON.stringify(data))
    return true
  } catch (error) {
    console.error(`localStorage.setItem('${key}') 失败:`, error)
    return false
  }
}

/**
 * 从 localStorage 读取数据
 * @param {string} key - 存储键名
 * @param {any} defaultValue - 默认值
 * @returns {any} 读取的值
 */
export const localRead = (key, defaultValue = null) => {
  try {
    const item = localStorage.getItem(key)
    if (!item) return defaultValue

    // 尝试解析为对象
    try {
      const data = JSON.parse(item)
      
      // 检查是否是新格式（带过期时间）
      if (data && typeof data === 'object' && 'value' in data) {
        // 检查是否过期
        if (data.expires > 0 && Date.now() > data.expires) {
          localRemove(key)
          return defaultValue
        }
        return data.value
      }
      
      // 兼容旧格式（直接存储的值）
      return data
    } catch {
      // 如果解析失败，返回原始字符串
      return item || defaultValue
    }
  } catch (error) {
    console.error(`localStorage.getItem('${key}') 失败:`, error)
    return defaultValue
  }
}

/**
 * 移除指定 key 的数据
 * @param {string} key - 存储键名
 * @returns {boolean} 是否移除成功
 */
export const localRemove = (key) => {
  try {
    localStorage.removeItem(key)
    return true
  } catch (error) {
    console.error(`localStorage.removeItem('${key}') 失败:`, error)
    return false
  }
}

/**
 * 清空所有数据
 * @returns {boolean} 是否清空成功
 */
export const localClear = () => {
  try {
    localStorage.clear()
    return true
  } catch (error) {
    console.error('localStorage.clear() 失败:', error)
    return false
  }
}

// ==================== 辅助方法 ====================

/**
 * 检查 key 是否存在
 * @param {string} key - 存储键名
 * @returns {boolean} 是否存在
 */
export const localHas = (key) => {
  return localStorage.getItem(key) !== null
}

/**
 * 获取所有 key
 * @returns {string[]} 所有键名数组
 */
export const localKeys = () => {
  const keys = []
  for (let i = 0; i < localStorage.length; i++) {
    keys.push(localStorage.key(i))
  }
  return keys
}

/**
 * 获取 localStorage 大小（KB）
 * @returns {number} 占用空间大小（KB）
 */
export const localSize = () => {
  let size = 0
  for (let i = 0; i < localStorage.length; i++) {
    const key = localStorage.key(i)
    const value = localStorage.getItem(key)
    size += key.length + value.length
  }
  return (size / 1024).toFixed(2)
}

/**
 * 清理过期数据
 * @returns {number} 清理的数量
 */
export const localClearExpired = () => {
  let count = 0
  const keys = localKeys()
  
  keys.forEach(key => {
    try {
      const item = localStorage.getItem(key)
      const data = JSON.parse(item)
      
      // 检查是否是带过期时间的格式
      if (data && typeof data === 'object' && 'expires' in data) {
        if (data.expires > 0 && Date.now() > data.expires) {
          localRemove(key)
          count++
        }
      }
    } catch {
      // 忽略解析错误
    }
  })
  
  return count
}

// ==================== 向后兼容 ====================

// 导出默认对象（兼容旧的导入方式）
export default {
  save: localSave,
  read: localRead,
  remove: localRemove,
  clear: localClear,
  has: localHas,
  keys: localKeys,
  size: localSize,
  clearExpired: localClearExpired
}

