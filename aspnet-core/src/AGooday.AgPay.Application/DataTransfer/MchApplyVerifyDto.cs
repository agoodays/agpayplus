namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 进件小额验证 DTO
    /// </summary>
    public class MchApplyVerifyDto
    {
        public string ApplyId { get; set; }

        /// <summary>验证金额（分）或验证码</summary>
        public string VerifyCode { get; set; }
    }
}
