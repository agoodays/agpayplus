using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 商户门店表
    /// </summary>
    public class MchStoreListDto : MchStoreDto
    {

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MchName { get; set; }
    }
}
