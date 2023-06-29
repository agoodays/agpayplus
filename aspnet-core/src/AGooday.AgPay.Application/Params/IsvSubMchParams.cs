using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 特约商户参数定义
    /// </summary>
    public abstract class IsvSubMchParams
    {
        public static IsvSubMchParams Factory(string ifCode, string paramsStr)
        {
            try
            {
                Type type = typeof(IsvSubMchParams);
                string namespaceName = type.Namespace;
                string typeName = namespaceName + "." + ifCode + "." + ifCode + "IsvSubMchParams";
                Type targetType = Type.GetType(typeName, false, true);
                return JsonConvert.DeserializeObject(paramsStr, targetType) as IsvSubMchParams;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
