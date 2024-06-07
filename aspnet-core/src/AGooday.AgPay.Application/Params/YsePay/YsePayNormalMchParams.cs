using Newtonsoft.Json;

namespace AGooday.AgPay.Application.Params.YsePay
{
    /// <summary>
    /// 银盛 普通商户参数定义
    /// </summary>
    public class YsePayNormalMchParams : NormalMchParams
    {
        public override string DeSenData()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
