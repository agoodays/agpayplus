using AutoMapper;
using EmitMapper.MappingConfiguration;
using log4net;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace AGooday.AgPay.Common.Utils
{
    public static class CopyUtil
    {
        private static ILog logger = LogManager.GetLogger(typeof(CopyUtil));

        /// <summary>
        /// 深度克隆对象
        /// </summary>
        /// <param name="obj">要克隆的对象(该对象需可序列化)</param>
        /// <returns>克隆所得新对象</returns>
        public static T DeepClone<T>(T obj) where T : class
        {
            if (obj == null) return default(T);
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                var newObj = (T)formatter.Deserialize(stream);
                return newObj;
            }
        }

        /// <summary>
        /// 深度克隆对象
        /// </summary>
        /// <param name="obj">要克隆的对象(该对象需可序列化)</param>
        /// <returns>克隆所得新对象</returns>
        public static T DeepCloneByEmit<T>(T obj) where T : class
        {
            try
            {
                return EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<T, T>(new DefaultMapConfig().DeepMap()).Map(obj);
            }
            catch (Exception e)
            {
                logger.Error($"深度克隆对象方法报错，错误：{e.Message}", e);
                return default(T);
            }
        }

        /// <summary>
        /// 复制对象,并转化为指定类型
        /// </summary>
        /// <typeparam name="TSource">原实体类型</typeparam>
        /// <typeparam name="TResult">转化后的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        public static TResult CopyFrom<TSource, TResult>(TSource source) where TResult : class, new()
        {
            if (source == null)
            {
                return null;
            }

            TResult rs = new TResult();
            return CopyFrom<TSource, TResult>(source, rs);
        }

        /// <summary>
        /// 从指定类型复制对象
        /// </summary>
        /// <typeparam name="TSource">原实体类型</typeparam>
        /// <typeparam name="TResult">转化后的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="result">目标实体</param>
        /// <returns></returns>
        public static TResult CopyFrom<TSource, TResult>(TSource source, TResult result) where TResult : class
        {
            if (result == null)
            {
                return null;
            }
            //result = TinyMapper.Map<TSource,TResult>(source, result);

            //return result;
            EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TSource, TResult>().Map(source, result);
            return result;
        }

        public static void CopyProperties<T, TU>(T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    {
                        // check if the property can be set or no.
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }
            }
        }

        public static TDestination CopyProperties<TSource, TDestination>(TSource source)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
            var mapper = config.CreateMapper();
            return mapper.Map<TSource, TDestination>(source);
        }

        public static TConvert ConvertTo<TConvert>(this object entity) where TConvert : new()
        {
            var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            var convert = new TConvert();

            foreach (var entityProperty in entityProperties)
            {
                var property = entityProperty;
                var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                {
                    convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
                }
            }

            return convert;
        }
    }
}
