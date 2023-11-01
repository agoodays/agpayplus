namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 mch oauth2参数定义
    /// </summary>
    public abstract class IsvSubMchOauth2Params
    {
        public static IsvSubMchOauth2Params Factory(string ifCode, string paramsStr)
        {
            return paramsStr.GetParams<IsvSubMchOauth2Params>(ifCode);
        }

        /// <summary>
        /// 敏感数据脱敏
        /// </summary>
        /// <returns></returns>
        public abstract string DeSenData();
    }
}
