namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 isv oauth2参数定义
    /// </summary>
    public abstract class IsvOauth2Params
    {
        public static IsvOauth2Params Factory(string ifCode, string paramsStr)
        {
            return paramsStr.GetParams<IsvOauth2Params>(ifCode);
        }

        /// <summary>
        /// 敏感数据脱敏
        /// </summary>
        /// <returns></returns>
        public abstract string DeSenData();
    }
}
