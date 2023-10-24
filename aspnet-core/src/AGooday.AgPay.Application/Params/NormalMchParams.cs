namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 普通商户参数定义
    /// </summary>
    public abstract class NormalMchParams
    {
        public static NormalMchParams Factory(string ifCode, string paramsStr)
        {
            return ParamsHelper.GetParams<NormalMchParams>(ifCode, paramsStr);
        }

        /// <summary>
        /// 敏感数据脱敏
        /// </summary>
        /// <returns></returns>
        public abstract string DeSenData();
    }
}
