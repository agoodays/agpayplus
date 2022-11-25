using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface IPayInterfaceConfigService : IDisposable
    {
        void Add(PayInterfaceConfigDto dto);
        void Remove(long recordId);
        void Update(PayInterfaceConfigDto dto);
        bool SaveOrUpdate(PayInterfaceConfigDto dto);
        PayInterfaceConfigDto GetById(long recordId);
        IEnumerable<PayInterfaceConfigDto> GetAll();
        bool IsExistUseIfCode(string ifCode);
        List<PayInterfaceDefineDto> SelectAllPayIfConfigListByIsvNo(byte infoType, string infoId);
        /// <summary>
        /// 根据 账户类型、账户号、接口类型 获取支付参数配置
        /// </summary>
        /// <param name="infoType">账户类型</param>
        /// <param name="infoId">账户号</param>
        /// <param name="ifCode">接口类型</param>
        /// <returns></returns>
        List<PayInterfaceDefineDto> SelectAllPayIfConfigListByAppId(string appId);
        /// <summary>
        /// 根据 账户类型、账户号 获取支付参数配置列表
        /// </summary>
        /// <param name="infoType"></param>
        /// <param name="infoId"></param>
        /// <returns></returns>
        PayInterfaceConfigDto GetByInfoIdAndIfCode(byte infoType, string infoId, string ifCode);
        IEnumerable<PayInterfaceConfigDto> GetByInfoId(byte infoType, string infoId);
        /// <summary>
        /// 查询商户app使用已正确配置了通道信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="ifCode"></param>
        /// <returns></returns>
        bool MchAppHasAvailableIfCode(string appId, string ifCode);
    }
}
