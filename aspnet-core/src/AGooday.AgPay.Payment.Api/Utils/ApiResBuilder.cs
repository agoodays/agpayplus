using AGooday.AgPay.Payment.Api.RQRS;

namespace AGooday.AgPay.Payment.Api.Utils
{
    /// <summary>
    /// api响应结果构造器
    /// </summary>
    public class ApiResBuilder
    {
        /// <summary>
        /// 构建自定义响应对象, 默认响应成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T BuildSuccess<T>() where T : AbstractRS, new()
        {
            try
            {
                T result = new T();
                return result;
            }
            catch (Exception e) { return null; }
        }
    }
}
