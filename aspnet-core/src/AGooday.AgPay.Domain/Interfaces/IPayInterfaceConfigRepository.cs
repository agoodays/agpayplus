using AGooday.AgPay.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Interfaces
{
    public interface IPayInterfaceConfigRepository : IRepository<PayInterfaceConfig, long>
    {
        bool IsExistUseIfCode(string ifCode);
        bool MchAppHasAvailableIfCode(string appId, string ifCode);
        void RemoveByInfoIds(List<string> infoIds, byte infoType);
    }
}
