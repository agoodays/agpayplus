using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IMchPayPassageRepository : IRepository<MchPayPassage, long>
    {
        bool IsExistMchPayPassageUseWayCode(string wayCode);
        void RemoveByMchNo(string mchNo);
    }
}
