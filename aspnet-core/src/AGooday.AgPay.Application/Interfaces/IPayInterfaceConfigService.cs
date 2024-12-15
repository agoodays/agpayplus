using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceConfigService : IAgPayService<PayInterfaceConfigDto, long>
    {
        Task<bool> IsExistUseIfCodeAsync(string ifCode);
        Task<bool> SaveOrUpdateAsync(PayInterfaceConfigDto dto);
        Task<bool> RemoveAsync(string infoType, string infoId);
        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置列表
        /// </summary>
        /// <param name="infoType"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        Task<IEnumerable<PayInterfaceDefineDto>> SelectAllPayIfConfigListByIsvNoAsync(string infoType, string infoId);
        Task<IEnumerable<PayInterfaceDefineDto>> SelectAllPayIfConfigListByAppIdAsync(string appId);
        Task<List<PayInterfaceDefineDto>> PayIfConfigListAsync(string infoType, string configMode, string infoId, string ifName, string ifCode);
        Task<IEnumerable<PayInterfaceDefineDto>> GetPayIfConfigsByMchNoAsync(string mchNo);
        /// <summary>
        /// 根据 账户类型、账户号、接口类型 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <param name="ifCode">接口类型</param>
        /// <returns></returns>
        Task<PayInterfaceConfigDto> GetByInfoIdAndIfCodeAsync(string infoType, string infoId, string ifCode);
        IEnumerable<PayInterfaceConfigDto> GetByInfoIdAndIfCodes(string infoType, List<string> infoIds, string ifCode);
        IEnumerable<PayInterfaceConfigDto> GetByInfoId(string infoType, string infoId);
        IEnumerable<PayInterfaceConfigDto> GetPayOauth2ConfigByStartsWithInfoId(string infoType, string infoId);
        /// <summary>
        /// 查询商户app使用已正确配置了通道信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        Task<bool> MchAppHasAvailableIfCodeAsync(string appId, string ifCode);
    }
}
