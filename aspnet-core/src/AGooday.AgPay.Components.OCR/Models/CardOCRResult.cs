namespace AGooday.AgPay.Components.OCR.Models
{
    public class CardOCRResult
    {
        /// <summary>
        /// 姓名（人像面）
        /// </summary>
        public string IdCardName { get; set; }
        /// <summary>
        /// 性别（人像面）
        /// </summary>
        public string IdCardSex { get; set; }
        /// <summary>
        /// 民族（人像面）
        /// </summary>
        public string IdCardNation { get; set; }
        /// <summary>
        /// 出生日期（人像面）
        /// 格式：yyyy-MM-dd
        /// </summary>
        public string IdCardBirth { get; set; }
        /// <summary>
        /// 地址（人像面）
        /// </summary>
        public string IdCardAddress { get; set; }
        /// <summary>
        /// 身份证号（人像面）
        /// </summary>
        public string IdCardIdNum { get; set; }
        /// <summary>
        /// 发证机关（国徽面）
        /// </summary>
        public string IdCardAuthority { get; set; }
        /// <summary>
        /// 证件有效期（国徽面）
        /// 格式：yyyy.MM.dd-yyyy.MM.dd 或 yyyy.MM.dd-长期
        /// </summary>
        public string IdCardValidDate { get; set; }
        /// <summary>
        /// 签发日期
        /// 格式：yyyy-MM-dd
        /// </summary>
        public string IdCardIssueDate { get; set; }
        /// <summary>
        /// 失效日期
        /// 格式：yyyy-MM-dd 或 长期
        /// </summary>
        public string IdCardExpiringDate { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string BankCardCardNo { get; set; }
        /// <summary>
        /// 银行信息
        /// </summary>
        public string BankCardBankInfo { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string BankCardValidDate { get; set; }
        /// <summary>
        /// 卡类型
        /// </summary>
        public string BankCardCardType { get; set; }

        /// <summary>
        /// 统一社会信用代码（三合一之前为注册号）
        /// </summary>
        public string BizLicenseRegNum { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string BizLicenseName { get; set; }
        /// <summary>
        /// 注册资本
        /// </summary>
        public string BizLicenseCapital { get; set; }
        /// <summary>
        /// 法定代表人
        /// </summary>
        public string BizLicensePerson { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string BizLicenseAddress { get; set; }
        /// <summary>
        /// 经营范围
        /// </summary>
        public string BizLicenseBusiness { get; set; }
        /// <summary>
        /// 主体类型
        /// </summary>
        public string BizLicenseType { get; set; }
        /// <summary>
        /// 营业期限
        /// </summary>
        public string BizLicensePeriod { get; set; }
        /// <summary>
        /// 组成形式
        /// </summary>
        public string BizLicenseComposingForm { get; set; }
        /// <summary>
        /// 注册日期
        /// 格式：yyyy-MM-dd
        /// </summary>
        public string BizLicenseRegistrationDate { get; set; }
        /// <summary>
        /// 格式化营业期限起始日期
        /// 格式：yyyy-MM-dd
        /// </summary>
        public string BizLicenseValidFromDate { get; set; }
        /// <summary>
        /// 格式化营业期限终止日期
        /// 格式：yyyy-MM-dd
        /// </summary>
        public string BizLicenseValidToDate { get; set; }
    }
}
