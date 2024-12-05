using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.Common.Models
{
    /// <summary>
    /// 自定义的Json转换器
    /// 处理model的扩展字段 (只需要在model中设置[ext]参数， 可以实现json自动转换为外层字段。 )
    /// 比如 model为 {id:1, ext:{abc:222}}  则自动转换为： {id:1, abc:222}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseModelJsonConverter<T> : JsonConverter<T> where T : BaseModel
    {
        private const string API_EXTEND_FIELD_NAME = "ext";

        public override bool CanWrite => false;

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            // 创建实体对象
            var entity = (T)Activator.CreateInstance(objectType);

            // 填充基本属性
            foreach (var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.Name.Equals(API_EXTEND_FIELD_NAME, StringComparison.OrdinalIgnoreCase))
                    continue;

                var propertyValue = jsonObject.GetValue(property.Name, StringComparison.OrdinalIgnoreCase);
                if (propertyValue != null)
                {
                    var convertedValue = propertyValue.ToObject(property.PropertyType);
                    property.SetValue(entity, convertedValue);
                }
            }

            // 填充扩展属性
            var extValue = jsonObject.GetValue(API_EXTEND_FIELD_NAME, StringComparison.OrdinalIgnoreCase);
            if (extValue is JObject extObject)
            {
                foreach (var property in extObject.Properties())
                {
                    entity.AddExt(property.Name, property.Value.ToObject<object>());
                }
            }

            return entity;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
