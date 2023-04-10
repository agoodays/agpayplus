using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchPayPassageService : IDisposable
    {
        void Add(MchPayPassageDto dto);
        void Remove(long recordId);
        void Update(MchPayPassageDto dto);
        MchPayPassageDto GetById(long recordId);
        IEnumerable<MchPayPassageDto> GetMchPayPassageByAppId(string mchNo, string appId);
        IEnumerable<MchPayPassageDto> GetAll();
        IEnumerable<MchPayPassageDto> GetAll(string appId, List<string> wayCodes);
        PaginatedList<AvailablePayInterfaceDto> SelectAvailablePayInterfaceList(string wayCode, string appId, string infoType, byte mchType, int pageNumber, int pageSize);
        /// <summary>
        /// 根据支付方式查询可用的支付接口列表
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="appId"></param>
        /// <param name="infoType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<AvailablePayInterfaceDto> SelectAvailablePayInterfaceList(string wayCode, string appId, string infoType, byte type);
        void SaveOrUpdateBatchSelf(List<MchPayPassageDto> mchPayPassages, string mchNo);
        MchPayPassageDto FindMchPayPassage(string mchNo, string appId, string wayCode);
        bool IsExistMchPayPassageUseWayCode(string wayCode);
    }
}
