using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.Third.RQRS.Transfer;
using AGooday.AgPay.Components.Third.Services;
using AGooday.AgPay.Components.Third.Utils;
using AGooday.AgPay.Payment.Api.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Transfer
{
    /// <summary>
    /// 商户转账单查询
    /// </summary>
    [ApiController]
    public class QueryTransferOrderController : ApiControllerBase
    {
        protected readonly ILogger<QueryTransferOrderController> _logger;
        private readonly ITransferOrderService _transferOrderService;

        public QueryTransferOrderController(ILogger<QueryTransferOrderController> logger,
            ITransferOrderService transferOrderService,
            RequestKit requestKit,
            ConfigContextQueryService configContextQueryService)
            : base(requestKit, configContextQueryService)
        {
            _logger = logger;
            _transferOrderService = transferOrderService;
        }

        [HttpPost, Route("api/transfer/query")]
        [PermissionAuth(PermCode.PAY.API_TRANS_ORDER_QUERY)]
        public ApiRes QueryTransferOrder()
        {
            //获取参数 & 验签
            QueryTransferOrderRQ rq = GetRQByWithMchSign<QueryTransferOrderRQ>();

            if (StringUtil.IsAllNullOrWhiteSpace(rq.MchOrderNo, rq.TransferId))
            {
                throw new BizException("mchOrderNo 和 transferId不能同时为空");
            }

            TransferOrderDto tansferOrder = _transferOrderService.QueryMchOrder(rq.MchNo, rq.MchOrderNo, rq.TransferId);
            if (tansferOrder == null)
            {
                throw new BizException("订单不存在");
            }

            QueryTransferOrderRS bizRes = QueryTransferOrderRS.BuildByRecord(tansferOrder);
            return ApiRes.OkWithSign(bizRes, rq.SignType, _configContextQueryService.QueryMchApp(rq.MchNo, rq.AppId).AppSecret);
        }
    }
}
