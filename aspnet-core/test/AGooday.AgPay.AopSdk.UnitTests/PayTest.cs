using AGooday.AgPay.AopSdk.Exceptions;
using AGooday.AgPay.AopSdk.Models;
using AGooday.AgPay.AopSdk.Request;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.AopSdk.UnitTests
{
    [TestClass]
    public class PayTest
    {
        private AgPayClient agPayClient;

        [TestInitialize]
        public void Setup()
        {
            var appSecret = "i6tdmci8taaxis88hvxcrwyqrwy0w61wpulkusb5wn2thb1o5yjtn6dnpm1sye2tygkx92tl37jwllhhlxjszhm2aqcks6y9fkp2bdn5a5bfiyjsikjoixjpd3jgzhr1";
            agPayClient = new AgPayClient("https://localhost:9819", appSecret);
        }

        [TestMethod]
        public void UnifiedOrderTest()
        {
            Random rd = new Random();
            var now = DateTime.Now;
            PayOrderCreateRequest request = new PayOrderCreateRequest();
            PayOrderCreateReqModel model = new PayOrderCreateReqModel();
            request.SetBizModel(model);
            model.MchNo = "M1642776153";
            model.AppId = "61eac65a9bbe8a4c3c2dd637";
            //model.StoreId = null;
            model.MchOrderNo = $"PT{now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
            model.WayCode = "WX_BAR";
            model.Amount = 1;
            model.Currency = "CNY";
            model.ClientIp = "127.0.0.1";
            model.Subject = $"支付测试[{model.MchNo}商户联调]";
            model.Body = $"支付测试[{model.MchNo}商户联调]";
            model.NotifyUrl = $"https://localhost:9818/api/anon/paytestNotify/payOrder"; //回调地址
            model.DivisionMode = 0; //分账模式

            //设置扩展参数
            JObject extParams = new JObject();
            extParams.Add("authCode", "130726461201614564");
            model.ChannelExtra = extParams.ToString();

            try
            {
                var response = agPayClient.Execute(request);

                var signType = request.GetRequestOptions().GetSignType();
                var apiKey = request.GetRequestOptions().GetApiKey();
                Assert.IsTrue(response.IsSuccess(signType, apiKey));
            }
            catch (AgPayException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void QueryTest()
        {
            var now = DateTime.Now;
            PayOrderQueryRequest request = new PayOrderQueryRequest();
            PayOrderQueryReqModel model = new PayOrderQueryReqModel();
            request.SetBizModel(model);
            model.MchNo = "M1642776153";
            model.AppId = "61eac65a9bbe8a4c3c2dd637";
            model.PayOrderId = "P945002155549528064";
            model.MchOrderNo = "PT202401171422000922866";

            try
            {
                var response = agPayClient.Execute(request);

                var signType = request.GetRequestOptions().GetSignType();
                var apiKey = request.GetRequestOptions().GetApiKey();
                Assert.IsTrue(response.IsSuccess(signType, apiKey));
            }
            catch (AgPayException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void RefundOrderTest()
        {
            Random rd = new Random();
            var now = DateTime.Now;
            RefundOrderCreateRequest request = new RefundOrderCreateRequest();
            RefundOrderCreateReqModel model = new RefundOrderCreateReqModel();
            request.SetBizModel(model);
            model.MchNo = "M1642776153";
            model.AppId = "61eac65a9bbe8a4c3c2dd637";
            model.PayOrderId = "P945002155549528064";
            model.MchOrderNo = "PT202401171422000922866";
            model.MchRefundNo = $"RT{now:yyyyMMddHHmmssFFF}{rd.Next(9999):d4}";
            model.RefundAmount = 1;
            model.RefundReason = "退款测试";
            model.Currency = "CNY";
            model.ClientIp = "127.0.0.1";

            try
            {
                var response = agPayClient.Execute(request);

                var signType = request.GetRequestOptions().GetSignType();
                var apiKey = request.GetRequestOptions().GetApiKey();
                Assert.IsTrue(response.IsSuccess(signType, apiKey));
            }
            catch (AgPayException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void RefundQueryTest()
        {
            var now = DateTime.Now;
            RefundOrderQueryRequest request = new RefundOrderQueryRequest();
            RefundOrderQueryReqModel model = new RefundOrderQueryReqModel();
            request.SetBizModel(model);
            model.MchNo = "M1642776153";
            model.AppId = "61eac65a9bbe8a4c3c2dd637";
            model.RefundOrderId = "";
            model.MchRefundNo = "RT202401171424576135370";

            try
            {
                var response = agPayClient.Execute(request);

                var signType = request.GetRequestOptions().GetSignType();
                var apiKey = request.GetRequestOptions().GetApiKey();
                Assert.IsTrue(response.IsSuccess(signType, apiKey));
            }
            catch (AgPayException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}