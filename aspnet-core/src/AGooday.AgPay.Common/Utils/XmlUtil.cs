using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace AGooday.AgPay.Common.Utils
{
    public class XmlUtil
    {
        public static string ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeXmlNode(json, "xml")!.InnerXml;
        }

        public static string ConvertToJson(string xml)
        {
            XElement xElement = XElement.Parse(xml);
            XCData[] array = xElement.DescendantNodes().OfType<XCData>().ToArray();
            foreach (XCData xCData in array)
            {
                xCData.Parent!.Add(xCData.Value);
                xCData.Remove();
            }

            return (JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeXNode(xElement))!.First as JProperty)?.Value?.ToString(Formatting.None) ?? "{}";
        }
    }
}
