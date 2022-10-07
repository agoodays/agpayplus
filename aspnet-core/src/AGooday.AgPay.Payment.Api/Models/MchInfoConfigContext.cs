using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Payment.Api.Models
{
    public class MchInfoConfigContext
    {
        /** 商户信息缓存 */
        public string MchNo { get; set; }
        public byte MchType { get; set; }
        public MchInfoDto MchInfo { get; set; }
        public Dictionary<string, MchAppDto> AppMap { get; set; } = new Dictionary<string, MchAppDto>();

        /** 重置商户APP **/
        public void PutMchApp(MchAppDto mchApp)
        {
            AppMap.Add(mchApp.AppId, mchApp);
        }

        /** get商户APP **/
        public MchAppDto GetMchApp(String appId)
        {
            AppMap.TryGetValue(appId, out MchAppDto mchApp);

            return mchApp;
        }
    }
}
