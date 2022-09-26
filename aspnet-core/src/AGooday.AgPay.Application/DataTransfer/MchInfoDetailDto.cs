using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    public class MchInfoDetailDto : MchInfoDto
    {

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginUsername { get; set; }
    }
}
