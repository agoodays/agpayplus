namespace AGooday.AgPay.Components.OSS.Models
{
    public class UploadFormParams
    {
        public FormParams FormParams { get; set; }
        public string OssFileUrl { get; set; }
        public string FormActionUrl { get; set; }
    }

    public class FormParams
    {
        public string BizType { get; set; }
        public string OssAccessKeyId { get; set; }
        public int? SuccessActionStatus { get; set; }
        public string Signature { get; set; }
        public string Key { get; set; }
        public string Policy { get; set; }
    }
}
