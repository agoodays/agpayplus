using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AGooday.AgPay.Payment.Api.RQRS.PayOrder
{
    /// <summary>
    /// 通用支付数据RS
    /// 根据set的值，响应不同的payDataType
    /// </summary>
    public class CommonPayDataRS : UnifiedOrderRS
    {
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string PayUrl { get; set; }

        /// <summary>
        /// 二维码地址
        /// </summary>
        public string CodeUrl { get; set; }

        /// <summary>
        /// 二维码图片地址
        /// </summary>
        public string CodeImgUrl { get; set; }

        /// <summary>
        /// 表单内容
        /// </summary>
        public string FormContent { get; set; }

        public override string BuildPayDataType()
        {

            if (!string.IsNullOrWhiteSpace(PayUrl))
            {
                return CS.PAY_DATA_TYPE.PAY_URL;
            }

            if (!string.IsNullOrWhiteSpace(CodeUrl))
            {
                return CS.PAY_DATA_TYPE.CODE_URL;
            }

            if (!string.IsNullOrWhiteSpace(CodeImgUrl))
            {
                return CS.PAY_DATA_TYPE.CODE_IMG_URL;
            }

            if (!string.IsNullOrWhiteSpace(FormContent))
            {
                return CS.PAY_DATA_TYPE.FORM;
            }

            return CS.PAY_DATA_TYPE.PAY_URL;
        }

        public override string BuildPayData()
        {

            if (!string.IsNullOrWhiteSpace(PayUrl))
            {
                return PayUrl;
            }

            if (!string.IsNullOrWhiteSpace(CodeUrl))
            {
                return CodeUrl;
            }

            if (!string.IsNullOrWhiteSpace(CodeImgUrl))
            {
                return CodeImgUrl;
            }

            if (!string.IsNullOrWhiteSpace(FormContent))
            {
                return FormContent;
            }

            return "";
        }
    }
}
