using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Params.SxfPay
{
    /// <summary>
    /// 随行付 配置信息
    /// </summary>
    public class SxfPayIsvParams : IsvParams
    {
        /// <summary>
        /// 是否沙箱环境
        /// </summary>
        public byte Sandbox { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public string OrgId { get; set; }

        public override string DeSenData()
        {
            throw new NotImplementedException();
        }
    }
}
