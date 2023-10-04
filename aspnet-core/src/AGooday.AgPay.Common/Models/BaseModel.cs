using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Common.Models
{
    /// <summary>
    /// BaseModel 封装公共处理函数
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// ext参数, 用作扩展参数，会在转换为api数据时自动将ext全部属性放置在对象的主属性上, 并且不包含ext属性
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonExtensionData]
        private JObject ext;

        /// <summary>
        /// 获取的时候设置默认值
        /// </summary>
        [JsonIgnore]
        public JObject Ext
        {
            get { return ext ?? new JObject(); }
        }

        /// <summary>
        /// 设置扩展字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public BaseModel AddExt(string key, object value)
        {
            if (ext == null)
            {
                ext = new JObject();
            }
            ext[key] = JToken.FromObject(value);
            return this;
        }
    }
}
