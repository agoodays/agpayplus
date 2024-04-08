using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.Channel.LklPay.Enumerator
{
    public class LklPayEnum
    {
        public static TradeState ConvertTradeState(string tradeState)
        {
            Enum.TryParse(tradeState, out TradeState _tradeState);
            return _tradeState;
        }

        public static string GetAccountType(string wayCode)
        {
            string accountType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_BAR:
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                case CS.PAY_WAY_CODE.ALI_APP:
                case CS.PAY_WAY_CODE.ALI_PC:
                case CS.PAY_WAY_CODE.ALI_WAP:
                case CS.PAY_WAY_CODE.ALI_QR:
                case CS.PAY_WAY_CODE.ALI_LITE:
                    accountType = AccountType.ALIPAY.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_BAR:
                case CS.PAY_WAY_CODE.WX_APP:
                case CS.PAY_WAY_CODE.WX_JSAPI:
                case CS.PAY_WAY_CODE.WX_LITE:
                case CS.PAY_WAY_CODE.WX_H5:
                case CS.PAY_WAY_CODE.WX_NATIVE:
                    accountType = AccountType.WECHAT.ToString();
                    break;
                case CS.PAY_WAY_CODE.UP_BAR:
                case CS.PAY_WAY_CODE.UP_QR:
                case CS.PAY_WAY_CODE.YSF_BAR:
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    accountType = AccountType.UQRCODEPAY.ToString();
                    break;
                default:
                    break;
            }
            return accountType;
        }

        public static string GetTransType(string wayCode)
        {
            string transType = null;
            switch (wayCode)
            {
                case CS.PAY_WAY_CODE.ALI_QR:
                    transType = TransType.NATIVE.ToString();
                    break;
                case CS.PAY_WAY_CODE.ALI_JSAPI:
                    transType = TransType.JSAPI.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_JSAPI:
                    transType = TransType.JSAPI.ToString();
                    break;
                case CS.PAY_WAY_CODE.WX_LITE:
                    transType = TransType.WXLITE.ToString();
                    break;
                case CS.PAY_WAY_CODE.UP_QR:
                    transType = TransType.NATIVE.ToString();
                    break;
                case CS.PAY_WAY_CODE.YSF_JSAPI:
                    transType = TransType.JSAPI.ToString();
                    break;
                default:
                    break;
            }
            return transType;
        }

        /// <summary>
        /// 交易状态
        /// </summary>
        public enum TradeState
        {
            /// <summary>
            /// 初始状态
            /// </summary>
            INIT,
            /// <summary>
            /// 创建订单
            /// </summary>
            CREATE,
            /// <summary>
            /// 交易成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 交易失败
            /// </summary>
            FAIL,
            /// <summary>
            /// 交易处理中
            /// </summary>
            DEAL,
            /// <summary>
            /// 未知状态
            /// </summary>
            UNKNOWN,
            /// <summary>
            /// 订单关闭
            /// </summary>
            CLOSE,
            /// <summary>
            /// 部分退款
            /// </summary>
            PART_REFUND,
            /// <summary>
            /// 全部退款(或订单被撤销）
            /// </summary>
            REFUND
        }

        /// <summary>
        /// 钱包类型
        /// </summary>
        public interface AccountType
        {
            /// <summary>
            /// 微信
            /// </summary>
            public const string WECHAT = "WECHAT";
            /// <summary>
            /// 支付宝
            /// </summary>
            public const string ALIPAY = "ALIPAY";
            /// <summary>
            /// 银联
            /// </summary>
            public const string UQRCODEPAY = "UQRCODEPAY";
            /// <summary>
            /// 翼支付
            /// </summary>
            public const string BESTPAY = "BESTPAY";
            /// <summary>
            /// 苏宁易付宝
            /// </summary>
            public const string SUNING = "SUNING";
            /// <summary>
            /// 拉卡拉支付账户
            /// </summary>
            public const string LKLACC = "LKLACC";
            /// <summary>
            /// 网联小钱包
            /// </summary>
            public const string NUCSPAY = "NUCSPAY";
        }

        /// <summary>
        /// 接入方式	
        /// </summary>
        public interface TransType
        {
            /// <summary>
            /// NATIVE（（ALIPAY，云闪付支持）
            /// </summary>
            public const string NATIVE = "41";
            /// <summary>
            /// JSAPI（微信公众号支付，支付宝服务窗支付，银联JS支付，翼支付JS支付、拉卡拉钱包支付）
            /// </summary>
            public const string JSAPI = "51";
            /// <summary>
            /// 微信小程序支付
            /// </summary>
            public const string WXLITE = "71";
        }
    }
}
