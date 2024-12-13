using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IMchPayPassageService : IAgPayService<MchPayPassageDto, long>
    {
        IEnumerable<MchPayPassageDto> GetMchPayPassageByMchNoAndAppId(string mchNo, string appId);
        IEnumerable<MchPayPassageDto> GetByAppIdAndWayCodesAsNoTracking(string appId, List<string> wayCodes);
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
        void SetMchPassage(string mchNo, string appId, string wayCode, string ifCode, byte state);
        void SaveOrUpdateBatchSelf(List<MchPayPassageDto> mchPayPassages, string mchNo);
        MchPayPassageDto FindMchPayPassage(string mchNo, string appId, string wayCode);
        MchPayPassageDto FindMchPayPassage(string mchNo, string appId, string wayCode, long amount, string bankCardType = null);
        Task<bool> IsExistMchPayPassageUseWayCodeAsync(string wayCode);
    }
}
