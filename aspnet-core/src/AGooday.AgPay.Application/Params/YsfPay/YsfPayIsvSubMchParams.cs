using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.YsfPay
{
    /// <summary>
    /// 云闪付 配置信息
    /// </summary>
    public class YsfPayIsvsubMchParams : IsvSubMchParams
    {
        /// <summary>
        /// 商户编号
        /// </summary>
        public string MerId { get; set; }
    }
}
