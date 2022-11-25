namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付接口定义表
    /// </summary>
    public class PayInterfaceDefineAddOrEditDto : PayInterfaceDefineDto
    {
        /// <summary>
        /// 支持的支付方式
        /// </summary>
        public string WayCodeStrs { get; set; }
    }
}
