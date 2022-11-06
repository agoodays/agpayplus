using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    public class ModifyCurrentUserInfoDto
    {
        public long SysUserId { get; set; }
        public string AvatarUrl { get; set; }
        public string Realname { get; set; }
        public byte Sex { get; set; }
    }
}
