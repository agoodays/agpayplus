/*
 *  ajax请求
 *
 */
import axios from 'axios';
import { message, Modal } from 'ant-design-vue';
import { AgLoading } from '/@/components/framework/ag-loading';
import { useUserStore } from '/@/store/modules/system/user';
// import { decryptData, encryptData } from './encrypt';
// import { DATA_TYPE_ENUM } from '../constants/common-const';
import { ACCESS_TOKEN_NAME } from '/@/constants/system/token-const';
import _ from 'lodash';

// 退出系统
function logout() {
  useUserStore().logout();
  location.reload(); // 退出时 重置缓存
}

class AgAxios {
  constructor (baseUrl = import.meta.env.VITE_APP_API_BASE_URL) {
    this.baseUrl = baseUrl;
    this.queue = {};// 发送队列, 格式为: {请求url: true}, 可以做一些验证之类
  }
  // 基础配置信息
  baseConfig () {
    const headers = {};
    const token = useUserStore().getToken;
    headers[ACCESS_TOKEN_NAME] = `Bearer ${token}`;
    return {
      baseURL: this.baseUrl,
      headers: headers
    };
  }
  destroy (url) {
    delete this.queue[url];
  }
  useInterceptors (instance, url, showErrorMsg, showLoading) {
    // 请求拦截
    instance.interceptors.request.use(config => {
      // 添加全局的loading...
      if (!Object.keys(this.queue).length && showLoading) {
        AgLoading.show(); // 加载中显示loading组件
      }
      this.queue[url] = true;
      return config;
    }, error => {
      AgLoading.hide()  // 报错关闭loading组件
      return Promise.reject(error)
    })
    // 响应拦截
    instance.interceptors.response.use(response => {
      this.destroy(url);
      if (showLoading) {
        AgLoading.hide(); // 报错关闭loading组件
      }
      // 根据content-type ，判断是否为 json 数据
      const contentType = response.headers['content-type'] ? response.headers['content-type'] : response.headers['Content-Type'];
      if (contentType.indexOf('application/json') === -1) {
        return Promise.resolve(response);
      }
      // 如果是json数据
      if (response.data && response.data instanceof Blob) {
        return Promise.resolve(response.data);
      }
      const resData = response.data // 接口实际返回数据 格式为：{code: '', msg: '', data: ''}， res.data 是axios封装对象的返回数据；
      if (resData.code && resData.code !== 0) { // 相应结果不为0， 说明异常
        if (showErrorMsg) {
          message.error(resData.msg); // 显示异常信息
        }
        return Promise.reject(resData);
      } else {
        return Promise.resolve(resData.data);
      }
    }, error => {
      this.destroy(url);
      if (showLoading) {
        AgLoading.hide(); // 报错关闭loading组件
      }
      let errorInfo = error.response && error.response.data && error.response.data.data;
      if (!errorInfo) {
        errorInfo = error.response.data;
      }
      if (error.response.status === 401) { // 无访问权限，会话超时， 提示用户信息 & 退出系统
        const toLoginTimeout = setTimeout(logout, 3000);
        Modal.warning({
          title: '会话超时，请重新登录',
          content: '3s后将自动退出...',
          okText: '重新登录',
          cancelText: '关闭对话',
          onOk: logout,
          onCancel() {
            clearTimeout(toLoginTimeout)
          }});
      } else {
        if (showErrorMsg) {
          message.error(JSON.stringify(errorInfo)); // 显示异常信息
        }
      }

      return Promise.reject(errorInfo);
    })
  }
  /**
   * ajax请求
   * @options options 参数
   * @interceptorsFlag 是否进行自定义拦截器处理，默认为： true
   * @showErrorMsg 发送请求出现异常是否全局提示错误信息，默认为： true
   * @showLoading 发送请求前后显示全局loading，默认为： false
   */
  request (options, interceptorsFlag = true, showErrorMsg = true, showLoading = false) {
    const instance = axios.create();
    options = Object.assign(this.baseConfig(), options);
    if (interceptorsFlag) { // 注入 req, respo 拦截器
      this.useInterceptors(instance, options.url, showErrorMsg, showLoading);
    }

    return instance(options);
  }
}

const agAxios = new AgAxios();

// ================================= 对外提供请求方法：通用请求，get， post, 下载download等 =================================

/**
 * get请求
 */
export const getRequest = (url, params) => {
  return request({ url, method: 'get', params });
};

/**
 * 通用请求封装
 *
 */
export const request = (config, interceptorsFlag = true, showErrorMsg = true, showLoading = false) => {
  return agAxios.request(config, interceptorsFlag, showErrorMsg, showLoading);
};

/**
 * post请求
 */
export const postRequest = (url, data) => {
  return request({ data, url, method: 'post' });
};

/**
 *  全系列 restful api格式, 定义通用req对象
 *
 */
export const req = {
  // 通用列表查询接口
  list: (url, params) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false);
  },

  // 通用获取数据接口
  get: (url, params) => {
    return request({ url: url, method: 'GET', params: params }, true, true, false);
  },

  // 通用列表查询统计接口
  total: (url, params) => {
    return request({ url: url + '/total', method: 'GET', params: params }, true, true, false);
  },

  // 通用列表查询统计接口
  count: (url, params) => {
    return request({ url: url + '/count', method: 'GET', params: params }, true, true, false);
  },

  // 通用列表数据导出接口
  export: (url, bizType, params) => {
    return request({ url: url + '/export/' + bizType, method: 'GET', params: params, responseType: 'blob' }, true, true, false);
  },

  // 通用Post接口
  post: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false);
  },

  // 通用新增接口
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, false);
  },

  // 通用查询单条数据接口
  getById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, false);
  },

  // 通用修改接口
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, false);
  },

  // 通用删除接口
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, false);
  }
}

/**
 *  全系列 restful api格式 (全局loading方式)
 *
 */
export const reqLoad = {

  // 通用列表查询接口
  list: (url, params) => {
    return request({ url: url, method: 'GET', params: params }, true, true, true);
  },

  // 通用新增接口
  add: (url, data) => {
    return request({ url: url, method: 'POST', data: data }, true, true, true);
  },

  // 通用查询单条数据接口
  getById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'GET' }, true, true, true);
  },

  // 通用修改接口
  updateById: (url, bizId, data) => {
    return request({ url: url + '/' + bizId, method: 'PUT', data: data }, true, true, true);
  },

  // 通用删除接口
  delById: (url, bizId) => {
    return request({ url: url + '/' + bizId, method: 'DELETE' }, true, true, true);
  }
}

/**
 * 上传图片/文件地址
 *
 */
export const upload = {
  avatar: agAxios.baseUrl + '/api/ossFiles/avatar',
  ifBG: agAxios.baseUrl + '/api/ossFiles/ifBG',
  cert: agAxios.baseUrl + '/api/ossFiles/cert',
  form: agAxios.baseUrl + '/api/ossFiles/form',
  /**
   * 获取上传表单参数
   *
   */
  getFormParams: (url, fileName, fileSize) => {
    return request({ baseURL: url, method: 'GET', params: { fileName, fileSize } });
  },
  /**
   * 上传单个文件
   *
   */
  singleFile: (url, data) => {
    const formData = new FormData();
    for (const key in data) {
      if (data.hasOwnProperty(key)) {
        formData.append(key, data[key]);
      }
    }
    return request({ baseURL: url, method: 'POST', data: formData });
  }
}

// ================================= 加密 =================================

/**
 * 加密请求参数的post请求
 */
// export const postEncryptRequest = (url, data) => {
//   return request({
//     data: { encryptData: encryptData(data) },
//     url,
//     method: 'post',
//   });
// };

// ================================= 下载 =================================

export const postDownload = function (url, data) {
  request({ method: 'post', url, data, responseType: 'blob' })
    .then((data) => {
      handleDownloadData(data);
    })
    .catch((error) => {
      handleDownloadError(error);
    });
};

/**
 * 文件下载
 */
export const getDownload = function (url, params) {
  request({ method: 'get', url, params, responseType: 'blob' })
    .then((data) => {
      handleDownloadData(data);
    })
    .catch((error) => {
      handleDownloadError(error);
    });
};

function handleDownloadError(error) {
  if (error instanceof Blob) {
    const fileReader = new FileReader();
    fileReader.readAsText(error);
    fileReader.onload = () => {
      const msg = fileReader.result;
      const jsonMsg = JSON.parse(msg);
      message.destroy();
      message.error(jsonMsg.msg);
    };
  } else {
    message.destroy();
    message.error('网络发生错误', error);
  }
}

function handleDownloadData(response) {
  if (!response) {
    return;
  }

  // 获取返回类型
  let contentType = _.isUndefined(response.headers['content-type']) ? response.headers['Content-Type'] : response.headers['content-type'];

  // 构建下载数据
  let url = window.URL.createObjectURL(new Blob([response.data], { type: contentType }));
  let link = document.createElement('a');
  link.style.display = 'none';
  link.href = url;

  // 从消息头获取文件名
  let str = _.isUndefined(response.headers['content-disposition'])
    ? response.headers['Content-Disposition'].split(';')[1]
    : response.headers['content-disposition'].split(';')[1];

  let filename = _.isUndefined(str.split('fileName=')[1]) ? str.split('filename=')[1] : str.split('fileName=')[1];
  link.setAttribute('download', decodeURIComponent(filename));

  // 触发点击下载
  document.body.appendChild(link);
  link.click();

  // 下载完释放
  document.body.removeChild(link); // 下载完成移除元素
  window.URL.revokeObjectURL(url); // 释放掉blob对象
}
