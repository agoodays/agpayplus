/**
 * 系统更新日志 api 封装
 *
 */
import { req } from '@/lib/axios';

export const API_URL_AGENT = '/api/agentInfo'

export const agentApi = {

    /**
     * 分页查询
     */
    queryPage : (param) => {
        return req.list(API_URL_AGENT, param);
    },

    /**
     * 增加
     */
    add: (param) => {
        return req.add(API_URL_AGENT, param);
    },

    /**
     * 修改
     */
    updateById: (id, param) => {
        return req.updateById(API_URL_AGENT, id, param);
    },

    /**
     * 删除
     * @id id
     */
    delById: (id) => {
        return req.delById(API_URL_AGENT, id);
    }
};
