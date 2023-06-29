using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 普通商户参数定义
    /// </summary>
    public abstract class NormalMchParams
    {
        public static NormalMchParams Factory(string ifCode, string paramsStr)
        {
            try
            {
                Type type = typeof(NormalMchParams);
                string namespaceName = type.Namespace;
                string typeName = namespaceName + "." + ifCode + "." + ifCode + "NormalMchParams";
                Type targetType = Type.GetType(typeName, false, true);
                return JsonConvert.DeserializeObject(paramsStr, targetType) as NormalMchParams;
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
