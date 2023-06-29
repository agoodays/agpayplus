using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 isv参数定义
    /// </summary>
    public abstract class IsvParams
    {
        public static IsvParams Factory(string ifCode, string paramsStr)
        {
            try
            {
                Type type = typeof(IsvParams);
                string namespaceName = type.Namespace;
                string typeName = namespaceName + "." + ifCode + "." + ifCode + "IsvParams";
                Type targetType = Type.GetType(typeName, false, true);
                return JsonConvert.DeserializeObject(paramsStr, targetType) as IsvParams;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 敏感数据脱敏
        /// </summary>
        /// <returns></returns>
        public abstract string DeSenData();
    }
}
