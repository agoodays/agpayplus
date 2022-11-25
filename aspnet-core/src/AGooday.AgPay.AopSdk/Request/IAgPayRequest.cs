using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Nets;
using AGooday.AgPay.AopSdk.Response;

namespace AGooday.AgPay.AopSdk.Request
{
    /// <summary>
    /// AgPay请求接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAgPayRequest<T> where T : AgPayResponse
    {
        /// <summary>
        /// 获取当前接口的路径
        /// </summary>
        /// <returns></returns>
        string GetApiUri();

        /// <summary>
        /// 获取当前接口的版本
        /// </summary>
        /// <returns></returns>
        string GetApiVersion();

        /// <summary>
        /// 设置当前接口的版本
        /// </summary>
        /// <param name="apiVersion"></param>
        void SetApiVersion(string apiVersion);

        RequestOptions GetRequestOptions();

        void SetRequestOptions(RequestOptions options);

        AgPayObject GetBizModel();

        void SetBizModel(AgPayObject bizModel);
    }
}
