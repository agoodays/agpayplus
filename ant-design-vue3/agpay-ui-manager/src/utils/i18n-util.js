import { i18n } from '/@/i18n'

export function translate(key, params) {
  return i18n?.global?.t?.(key, params)
}

export function translateWithFallback(key, fallback, params) {
  const value = translate(key, params)
  if (!value || value === key) {
    return fallback
  }
  return value
}
