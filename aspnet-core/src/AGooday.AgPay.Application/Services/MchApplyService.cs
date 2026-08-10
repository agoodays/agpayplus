using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 商户进件表 服务实现类（纯数据访问层）
    /// 
    /// 继承 AgPayService 获得通用 CRUD 能力，
    /// 封装 MchApplyRepository 做条件查询和状态流转，
    /// 不依赖任何第三方通道服务。
    /// </summary>
    public class MchApplyService : AgPayService<MchApplyDto, MchApply>, IMchApplyService
    {
        private readonly IMchApplyRepository _mchApplyRepository;
        private readonly IMchInfoService _mchInfoService;

        public MchApplyService(IMapper mapper, IMediatorHandler bus,
            IMchApplyRepository mchApplyRepository,
            IMchInfoService mchInfoService)
            : base(mapper, bus, mchApplyRepository)
        {
            _mchApplyRepository = mchApplyRepository;
            _mchInfoService = mchInfoService;
        }

        public async Task<PaginatedResult<MchApplyDto>> GetPaginatedDataAsync(MchApplyQueryDto dto, string agentNo = null, string isvNo = null)
        {
            var query = _mchApplyRepository.GetAllAsNoTracking()
                .Where(w =>
                    (string.IsNullOrWhiteSpace(dto.ApplyId) || w.ApplyId.Equals(dto.ApplyId)) &&
                    (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo)) &&
                    (string.IsNullOrWhiteSpace(dto.AgentNo) || w.AgentNo.Equals(dto.AgentNo)) &&
                    (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo)) &&
                    (string.IsNullOrWhiteSpace(dto.IfCode) || w.IfCode.Equals(dto.IfCode)) &&
                    (!dto.State.HasValue || w.State.Equals(dto.State.Value)) &&
                    (string.IsNullOrWhiteSpace(dto.MchFullName) || EF.Functions.Like(w.MchFullName, $"%{dto.MchFullName}%")) &&
                    (!dto.MerchantType.HasValue || w.MerchantType.Equals(dto.MerchantType.Value)) &&
                    (string.IsNullOrWhiteSpace(dto.ApplyPageType) || w.ApplyPageType.Equals(dto.ApplyPageType)) &&
                    (!dto.CreatedUid.HasValue || w.CreatedUid.Equals(dto.CreatedUid.Value))
                );

            if (!string.IsNullOrEmpty(agentNo))
            {
                query = query.Where(w => w.AgentNo.Equals(agentNo) || w.TopAgentNo.Equals(agentNo));
            }
            if (!string.IsNullOrEmpty(isvNo))
            {
                query = query.Where(w => w.IsvNo.Equals(isvNo));
            }

            var result = await query.OrderByDescending(w => w.CreatedAt)
                .ToPaginatedResultAsync<MchApply, MchApplyDto>(_mapper, dto.PageNumber, dto.PageSize);

            foreach (var item in result.Items)
            {
                item.StateName = ((ApplymentState)item.State).GetDescription();
            }

            return result;
        }

        public async Task<MchApplyDto> GetByIdAsync(string applyId)
        {
            var entity = await _mchApplyRepository.GetByIdAsync(applyId);
            var dto = _mapper.Map<MchApplyDto>(entity);
            if (dto != null)
            {
                dto.StateName = ((ApplymentState)dto.State).GetDescription();
            }
            return dto;
        }

        public async Task<MchApplyDto> GetByIdAsNoTrackingAsync(string applyId)
        {
            var entity = await _mchApplyRepository.GetByIdAsNoTrackingAsync(applyId);
            var dto = _mapper.Map<MchApplyDto>(entity);
            if (dto != null)
            {
                dto.StateName = ((ApplymentState)dto.State).GetDescription();
            }
            return dto;
        }

        public async Task<MchApplyDto> GetByChannelApplyNoAsync(string channelApplyNo)
        {
            var entity = await _mchApplyRepository.GetByChannelApplyNoAsync(channelApplyNo);
            var dto = _mapper.Map<MchApplyDto>(entity);
            if (dto != null)
            {
                dto.StateName = ((ApplymentState)dto.State).GetDescription();
            }
            return dto;
        }

        public async Task<MchApplyDto> SaveDraftAsync(MchApplySaveDto dto, long? userId, string userName)
        {
            if (string.IsNullOrWhiteSpace(dto.MchNo))
            {
                throw new Exception("商户号不能为空");
            }

            var mchInfo = await _mchInfoService.GetByIdAsync(dto.MchNo);
            if (mchInfo == null)
            {
                throw new Exception($"商户不存在: {dto.MchNo}");
            }

            dto.AgentNo = mchInfo.AgentNo ?? dto.AgentNo;
            dto.TopAgentNo = mchInfo.TopAgentNo ?? dto.TopAgentNo;
            dto.IsvNo = mchInfo.IsvNo ?? dto.IsvNo;

            if (string.IsNullOrWhiteSpace(dto.ApplyPageType))
            {
                dto.ApplyPageType = "PLATFORM_WEB";
            }

            MchApply entity;
            bool isNew = string.IsNullOrEmpty(dto.ApplyId);

            if (isNew)
            {
                entity = new MchApply
                {
                    ApplyId = SeqUtil.GenApplyId(),
                    State = (byte)ApplymentState.DRAFT,
                    IsTempData = false,
                    CreatedUid = userId,
                    CreatedBy = userName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
            }
            else
            {
                entity = await _mchApplyRepository.GetByIdAsNoTrackingAsync(dto.ApplyId);
                if (entity == null) throw new Exception($"进件单不存在: {dto.ApplyId}");
            }

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.Now;
            entity.LastApplyAt = DateTime.Now;

            if (isNew) _mchApplyRepository.Add(entity);
            else _mchApplyRepository.Update(entity);

            var (result, _) = await _mchApplyRepository.SaveChangesWithResultAsync();
            if (!result) throw new Exception("保存进件草稿失败");

            return _mapper.Map<MchApplyDto>(entity);
        }

        public async Task<MchApplyDto> SubmitAsync(string applyId, long? userId, string userName)
        {
            var entity = await _mchApplyRepository.GetByIdAsync(applyId);
            if (entity == null) throw new Exception($"进件单不存在: {applyId}");

            if (entity.State != (byte)ApplymentState.DRAFT
                && entity.State != (byte)ApplymentState.PRE_AUDIT_REJECTED
                && entity.State != (byte)ApplymentState.CHANNEL_REJECTED)
            {
                throw new Exception($"当前状态不允许提交: {(ApplymentState)entity.State}");
            }

            entity.State = (byte)ApplymentState.AUDITING;
            entity.Progress = 10;
            entity.UpdatedAt = DateTime.Now;
            entity.LastApplyAt = DateTime.Now;
            _mchApplyRepository.Update(entity);

            var (result, _) = await _mchApplyRepository.SaveChangesWithResultAsync();
            if (!result) throw new Exception("提交进件失败");

            return _mapper.Map<MchApplyDto>(entity);
        }

        public async Task RemoveDraftAsync(string applyId)
        {
            var entity = await _mchApplyRepository.GetByIdAsync(applyId);
            if (entity == null) throw new Exception($"进件单不存在: {applyId}");

            if (entity.State != (byte)ApplymentState.DRAFT && entity.State != (byte)ApplymentState.PRE_AUDIT_REJECTED)
            {
                throw new Exception($"当前状态不允许删除: {(ApplymentState)entity.State}");
            }

            _mchApplyRepository.Remove(entity);
            var (result, _) = await _mchApplyRepository.SaveChangesWithResultAsync();
            if (!result) throw new Exception("删除进件单失败");
        }

        public async Task<MchApplyDto> UpdateAuditInfoAsync(string applyId, bool isPass, string remark, long? auditUid, string auditBy)
        {
            var entity = await _mchApplyRepository.GetByIdAsync(applyId);
            if (entity == null) throw new Exception($"进件单不存在: {applyId}");

            if (entity.State != (byte)ApplymentState.AUDITING)
            {
                throw new Exception($"当前状态不允许审核: {(ApplymentState)entity.State}");
            }

            entity.AuditUid = auditUid;
            entity.AuditBy = auditBy;
            entity.AuditRemark = remark;
            entity.AuditedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            if (isPass)
            {
                entity.State = (byte)ApplymentState.PRE_AUDIT_APPROVED;
                entity.Progress = 30;
            }
            else
            {
                entity.State = (byte)ApplymentState.PRE_AUDIT_REJECTED;
                entity.Progress = 0;
            }

            _mchApplyRepository.Update(entity);
            var (result, _) = await _mchApplyRepository.SaveChangesWithResultAsync();
            if (!result) throw new Exception("保存审核结果失败");

            return _mapper.Map<MchApplyDto>(entity);
        }

        public async Task<bool> PatchChannelResultAsync(string applyId, MchApplyChannelResultPatch patch)
        {
            var entity = await _mchApplyRepository.GetByIdAsync(applyId);
            if (entity == null) return false;

            if (patch.State.HasValue) entity.State = patch.State.Value;
            entity.Progress = patch.Progress;
            if (!string.IsNullOrEmpty(patch.ChannelApplyNo)) entity.ChannelApplyNo = patch.ChannelApplyNo;
            if (!string.IsNullOrEmpty(patch.ChannelMchId)) entity.ChannelMchId = patch.ChannelMchId;
            if (!string.IsNullOrEmpty(patch.ApplyErrorInfo)) entity.ApplyErrorInfo = patch.ApplyErrorInfo;
            if (!string.IsNullOrEmpty(patch.SuccResParameter)) entity.SuccResParameter = patch.SuccResParameter;
            if (!string.IsNullOrEmpty(patch.SignUrl)) entity.SignUrl = patch.SignUrl;
            if (patch.SignExpireAt.HasValue) entity.SignExpireAt = patch.SignExpireAt.Value;
            if (patch.VerifyAmount.HasValue) entity.VerifyAmount = patch.VerifyAmount.Value;
            entity.UpdatedAt = DateTime.Now;

            _mchApplyRepository.Update(entity);
            var (result, _) = await _mchApplyRepository.SaveChangesWithResultAsync();
            return result;
        }

        public async Task<MchApplyDto> MarkChannelSubmittedAsync(string applyId, MchApplyChannelSubmittedPatch patch)
        {
            var entity = await _mchApplyRepository.GetByIdAsync(applyId);
            if (entity == null) throw new Exception($"进件单不存在: {applyId}");

            entity.ChannelApplyNo = patch.ChannelApplyNo;
            entity.SuccResParameter = patch.SuccResParameter;
            entity.State = patch.State;
            entity.Progress = patch.Progress;
            if (!string.IsNullOrEmpty(patch.ApplyErrorInfo)) entity.ApplyErrorInfo = patch.ApplyErrorInfo;
            entity.SignUrl = patch.SignUrl;
            entity.SignExpireAt = patch.SignExpireAt;
            entity.VerifyAmount = patch.VerifyAmount;
            entity.UpdatedAt = DateTime.Now;

            _mchApplyRepository.Update(entity);
            var (result, _) = await _mchApplyRepository.SaveChangesWithResultAsync();
            if (!result) throw new Exception("更新渠道提交结果失败");

            return _mapper.Map<MchApplyDto>(entity);
        }
    }
}
