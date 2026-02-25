/*
 * 时间操作类
 *
 */
import { translateWithFallback } from '/@/utils/i18n-util'

export function timeFix () {
  const time = new Date()
  const hour = time.getHours()
  if (hour < 9) {
    return translateWithFallback('time.greetingEarlyMorning', '早上好')
  }
  if (hour <= 11) {
    return translateWithFallback('time.greetingMorning', '上午好')
  }
  if (hour <= 13) {
    return translateWithFallback('time.greetingNoon', '中午好')
  }
  if (hour < 20) {
    return translateWithFallback('time.greetingAfternoon', '下午好')
  }
  return translateWithFallback('time.greetingEvening', '晚上好')
}
