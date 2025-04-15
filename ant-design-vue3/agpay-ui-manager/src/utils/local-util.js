/*
 * localStorage 相关操作
 *
 */

export const localSave = (key, value) => {
  localStorage.setItem(key, value);
};

export const localRead = (key) => {
  return localStorage.getItem(key) || '';
};

export const localClear = () => {
  localStorage.clear();
};

export const localRemove = (key) => {
  localStorage.removeItem(key);
};
