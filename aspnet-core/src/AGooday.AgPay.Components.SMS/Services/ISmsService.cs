using AGooday.AgPay.Components.SMS.Models;

namespace AGooday.AgPay.Components.SMS.Services
{
    public interface ISmsService
    {
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="smsBizVercodeModel"></param>
        void SendVercode(SmsBizVercodeModel smsBizVercodeModel);

        /// <summary>
        /// 发送自定义内容的短信
        /// </summary>
        /// <param name="smsBizDiyContentModel"></param>
        void SendDiyContent(SmsBizDiyContentModel smsBizDiyContentModel);

        /// <summary>
        /// 查询短信相关信息（bizQueryType：业务类型）
        /// </summary>
        /// <param name="bizQueryType">业务类型</param>
        /// <returns></returns>
        string QuerySmsInfo(string bizQueryType);
    }
}
