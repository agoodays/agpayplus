namespace AGooday.AgPay.Common.Utils
{
    public class SeqUtil
    {
		private const string PAY_ORDER_SEQ_PREFIX = "P";
		private const string REFUND_ORDER_SEQ_PREFIX = "R";
		private const string MHO_ORDER_SEQ_PREFIX = "M";
		private const string TRANSFER_ID_SEQ_PREFIX = "T";
		private const string DIVISION_BATCH_ID_SEQ_PREFIX = "D"; 
        
        private static bool IS_USE_MP_ID = true;

        /// <summary>
        /// 生成支付订单号
        /// </summary>
        /// <returns></returns>
        public static string GenPayOrderId()
        {
            if (IS_USE_MP_ID)
            {
                return $"{PAY_ORDER_SEQ_PREFIX}{IdWorker.Singleton.NextId()}";
            }
            Random rd = new Random();
            return $"{PAY_ORDER_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(maxValue: 9999):d4}";
        }

        /// <summary>
        /// 生成退款订单号
        /// </summary>
        /// <returns></returns>
        public static string GenRefundOrderId()
        {
            if (IS_USE_MP_ID)
            {
                return $"{REFUND_ORDER_SEQ_PREFIX}{IdWorker.Singleton.NextId()}";
            }
            Random rd = new Random();
            return $"{REFUND_ORDER_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /// <summary>
        /// 模拟生成商户订单号
        /// </summary>
        /// <returns></returns>
        public static string GenMhoOrderId()
        {
            if (IS_USE_MP_ID)
            {
                return $"{MHO_ORDER_SEQ_PREFIX}{IdWorker.Singleton.NextId()}";
            }
            Random rd = new Random();
            return $"{MHO_ORDER_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /// <summary>
        /// 模拟生成商户订单号
        /// </summary>
        /// <returns></returns>
        public static string GenTransferId()
        {
            if (IS_USE_MP_ID)
            {
                return $"{TRANSFER_ID_SEQ_PREFIX}{IdWorker.Singleton.NextId()}";
            }
            Random rd = new Random();
            return $"{TRANSFER_ID_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }

        /// <summary>
        /// 模拟生成分账批次号
        /// </summary>
        /// <returns></returns>
        public static string GenDivisionBatchId()
        {
            if (IS_USE_MP_ID)
            {
                return $"{DIVISION_BATCH_ID_SEQ_PREFIX}{IdWorker.Singleton.NextId()}";
            }
            Random rd = new Random();
            return $"{DIVISION_BATCH_ID_SEQ_PREFIX}{DateTime.Now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
        }
    }
}
