namespace AGooday.AgPay.Application.Params
{
    /// <summary>
    /// 抽象类 特约商户参数定义
    /// </summary>
    public abstract class IsvSubMchParams
    {
        public static IsvSubMchParams Factory(string ifCode, string paramsStr)
        {
            return ParamsHelper.GetParams<IsvSubMchParams>(ifCode, paramsStr);
        }
    }
}
