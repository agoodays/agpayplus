using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    public class PayTypeCountDto
    {
        public string WayCode { get; set; }
        public string TypeName { get; set; }
        public int TypeCount { get; set; }
        public decimal TypeAmount { get; set; }
    }
}
