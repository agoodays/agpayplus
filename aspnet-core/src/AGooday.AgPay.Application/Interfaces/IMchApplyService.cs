using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Models;

namespace AGooday.AgPay.Application.Interfaces
{
    /// <summary>
    /// 商户进件表 应用服务接口（纯数据访问层）
    /// 
    /// 负责 DB CRUD、分页查询、状态流转等数据操作；
    /// 不涉及任何第三方渠道调用。渠道交互由 Components.Third 层的业务编排服务负责。
    /// </summary>
    public interface IMchApplyService : IAgPayService<MchApplyDto>
    {
        Task<PaginatedResult<MchApplyDto>> GetPaginatedDataAsync(MchApplyQueryDto dto, string agentNo = null, string isvNo = null);
        Task<MchApplyDto> GetByIdAsync(string applyId);
        Task<MchApplyDto> GetByIdAsNoTrackingAsync(string applyId);

        /// <summary>通过渠道申请单号查找（回调场景下 applyId 可能用 channelApplyNo 传来）</summary>
        Task<MchApplyDto> GetByChannelApplyNoAsync(string channelApplyNo);

        /// <summary>保存或新增一条进件草稿</summary>
        Task<MchApplyDto> SaveDraftAsync(MchApplySaveDto dto, long? userId, string userName);

        /// <summary>提交进件单（状态流转：DRAFT → AUDITING）</summary>
        Task<MchApplyDto> SubmitAsync(string applyId, long? userId, string userName);

        /// <summary>删除草稿 / 被预审拒绝的进件单</summary>
        Task RemoveDraftAsync(string applyId);

        /// <summary>更新审核信息（预审通过/拒绝）</summary>
        Task<MchApplyDto> UpdateAuditInfoAsync(string applyId, bool isPass, string remark, long? auditUid, string auditBy);

        /// <summary>
        /// 渠道返回结果后，按 applyId 批量更新通道字段
        /// 用于 MQ 消费者、定时任务轮询等异步场景写回 DB
        /// </summary>
        Task<bool> PatchChannelResultAsync(string applyId, MchApplyChannelResultPatch patch);

        /// <summary>标记进件单已提交渠道，保存渠道申请单号 / 渠道返回参数 / 进度</summary>
        Task<MchApplyDto> MarkChannelSubmittedAsync(string applyId, MchApplyChannelSubmittedPatch patch);
    }
}
