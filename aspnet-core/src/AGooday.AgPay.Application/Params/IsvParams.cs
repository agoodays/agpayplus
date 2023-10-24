namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 isv参数定义
    /// </summary>
    public abstract class IsvParams
    {
        public static IsvParams Factory(string ifCode, string paramsStr)
        {
            //return ParamsHelper.GetParams<IsvParams>(ifCode, paramsStr);
            return paramsStr.GetParams<IsvParams>(ifCode);
        }

        /// <summary>
        /// 敏感数据脱敏
        /// </summary>
        /// <returns></returns>
        public abstract string DeSenData();
    }
}
