using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayOrderRepository : IRepository<PayOrder>
    {
        bool IsExistOrderByMchOrderNo(string mchNo, string mchOrderNo);
        bool IsExistOrderUseIfCode(string ifCode);
        bool IsExistOrderUseMchNo(string mchNo);
        bool IsExistOrderUseWayCode(string wayCode);
    }
}
