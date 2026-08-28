// 定义全局自增ID
let atomicLong = 1

/** 生成自增序列号（不重复） **/
export function genRowKey() {
  return new Date().getTime() + '_' + atomicLong++
}
