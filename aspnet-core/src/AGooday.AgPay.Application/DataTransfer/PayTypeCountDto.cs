namespace AGooday.AgPay.Application.DataTransfer
{
    public class PayTypeCountDto
    {
        public string WayCode { get; set; }
        public string TypeName { get; set; }
        public int TypeCount { get; set; }
        public decimal TypeAmount { get; set; }
    }
}
