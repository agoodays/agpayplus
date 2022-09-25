using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    public class SysUserQueryDto : PageQuery
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long SysUserId { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Realname { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        [BindNever]
        public string SysType { get; set; }
    }
}
