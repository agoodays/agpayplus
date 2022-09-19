using AGooday.AgPay.Application.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchPayPassageService : IDisposable
    {
        void Add(MchPayPassageDto dto);
        void Remove(long recordId);
        void Update(MchPayPassageDto dto);
        MchPayPassageDto GetById(long recordId);
        IEnumerable<MchPayPassageDto> GetAll();
    }
}
