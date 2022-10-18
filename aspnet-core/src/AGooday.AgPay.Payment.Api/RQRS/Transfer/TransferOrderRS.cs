using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Payment.Api.RQRS.Refund;
using AutoMapper;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Payment.Api.RQRS.Transfer
{
    /// <summary>
    /// 创建订单(统一订单) 响应参数
    /// </summary>
    public class TransferOrderRS : AbstractRS
    {
        /// <summary>
        /// 转账单号
        /// </summary>
        public string TransferId { get; set; }

        /// <summary>
        /// 商户单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 转账金额,单位分
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款人姓名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 收款人开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 渠道退款单号
        /// </summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 渠道返回错误代码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道返回错误信息
        /// </summary>
        public string ErrMsg { get; set; }

        public static QueryTransferOrderRS BuildByRecord(TransferOrderDto record)
        {
            if (record == null)
            {
                return null;
            }

            var config = new MapperConfiguration(cfg => cfg.CreateMap<TransferOrderDto, QueryTransferOrderRS>());
            var mapper = config.CreateMapper();
            var result = mapper.Map<TransferOrderDto, QueryTransferOrderRS>(record);
            result.SuccessTime = record.SuccessTime == null ? null : new DateTimeOffset(record.SuccessTime.Value).ToUnixTimeSeconds();
            result.CreatedAt = record.CreatedAt == null ? null : new DateTimeOffset(record.CreatedAt.Value).ToUnixTimeSeconds();
            return result;
        }
    }
}
