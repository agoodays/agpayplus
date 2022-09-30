using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public class SeqUtil
    {
		private const string PAY_ORDER_SEQ_PREFIX = "P";
		private const string REFUND_ORDER_SEQ_PREFIX = "R";
		private const string MHO_ORDER_SEQ_PREFIX = "M";
		private const string TRANSFER_ID_SEQ_PREFIX = "T";
		private const string DIVISION_BATCH_ID_SEQ_PREFIX = "D";

        /** 生成支付订单号 **/
        public static string GenPayOrderId()
        {
            Random rd = new Random();
            return $"{PAY_ORDER_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /** 生成退款订单号 **/
        public static string GenRefundOrderId()
        {
            Random rd = new Random();
            return $"{REFUND_ORDER_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /** 模拟生成商户订单号 **/
        public static string GenMhoOrderId()
        {
            Random rd = new Random();
            return $"{MHO_ORDER_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /** 模拟生成商户订单号 **/
        public static string GenTransferId()
        {
            Random rd = new Random();
            return $"{TRANSFER_ID_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /** 模拟生成分账批次号 **/
        public static string GenDivisionBatchId()
        {
            Random rd = new Random();
            return $"{DIVISION_BATCH_ID_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }
    }
}
