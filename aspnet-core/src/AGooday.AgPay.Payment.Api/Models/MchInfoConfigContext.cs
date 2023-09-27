using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Payment.Api.Models
{
    /// <summary>
    /// 商户配置信息
    /// 放置到内存， 避免多次查询操作
    /// </summary>
    public class MchInfoConfigContext
    {
        #region 商户信息缓存
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }
        /// <summary>
        /// 商户类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte MchType { get; set; }
        /// <summary>
        /// 商户信息
        /// </summary>
        public MchInfoDto MchInfo { get; set; }
        /// <summary>
        /// 商户应用信息集
        /// </summary>
        public Dictionary<string, MchAppDto> AppMap { get; set; } = new Dictionary<string, MchAppDto>(); 
        #endregion

        /// <summary>
        /// 重置商户APP
        /// </summary>
        /// <param name="mchApp"></param>
        public void PutMchApp(MchAppDto mchApp)
        {
            AppMap.Add(mchApp.AppId, mchApp);
        }

        /// <summary>
        /// 获取商户APP
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public MchAppDto GetMchApp(string appId)
        {
            AppMap.TryGetValue(appId, out MchAppDto mchApp);

            return mchApp;
        }
    }
}
