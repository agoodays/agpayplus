using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace AGooday.AgPay.Infrastructure.Extensions
{
    /// <summary>
    /// 把datatable转换为对象集合列表List<T>
    /// </summary>
    public static class DataTableConvert
    {
        //把DataRow转换为对象的委托声明
        private delegate T Load<T>(DataRow dataRecord);

        //用于构造Emit的DataRow中获取字段的方法信息
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });

        //用于构造Emit的DataRow中判断是否为空行的方法信息
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });

        //使用字典存储实体的类型以及与之对应的Emit生成的转换方法
        private static readonly Dictionary<Type, Delegate> rowMapMethods = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 将DataTable转换成泛型对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();
            if (dt == null)
                return list;

            //声明 委托Load<T>的一个实例rowMap
            Load<T> rowMap = null;

            //从rowMapMethods查找当前T类对应的转换方法，没有则使用Emit构造一个。
            if (!rowMapMethods.ContainsKey(typeof(T)))
            {
                DynamicMethod method = new DynamicMethod($"DynamicCreateEntity_{typeof(T).Name}", typeof(T), new Type[] { typeof(DataRow) }, typeof(T), true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(typeof(T));
                generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);

                for (int index = 0; index < dt.Columns.Count; index++)
                {
                    string columnName = dt.Columns[index].ColumnName;
                    PropertyInfo propertyInfo = typeof(T).GetProperties()
                        .FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == columnName || p.Name == columnName);

                    Label endIfLabel = generator.DefineLabel();
                    if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                    {
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, index);
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, index);
                        generator.Emit(OpCodes.Callvirt, getValueMethod);

                        // 添加类型转换逻辑
                        Type propertyType = propertyInfo.PropertyType;
                        Type underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                        if (underlyingType.IsEnum)
                        {
                            generator.Emit(OpCodes.Unbox_Any, typeof(int));
                            generator.Emit(OpCodes.Castclass, underlyingType);
                        }
                        else if (underlyingType == typeof(Guid))
                        {
                            generator.Emit(OpCodes.Unbox_Any, typeof(Guid));
                        }
                        else if (underlyingType == typeof(byte) && dt.Columns[index].DataType == typeof(sbyte))
                        {
                            generator.Emit(OpCodes.Conv_U1);
                        }
                        else if (underlyingType.IsValueType)
                        {
                            generator.Emit(OpCodes.Unbox_Any, underlyingType);
                        }
                        else
                        {
                            generator.Emit(OpCodes.Castclass, underlyingType);
                        }

                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);

                //构造完成以后传给rowMap
                rowMap = (Load<T>)method.CreateDelegate(typeof(Load<T>));
            }
            else
            {
                rowMap = (Load<T>)rowMapMethods[typeof(T)];
            }

            //遍历Datatable的rows集合，调用rowMap把DataRow转换为对象（T）
            foreach (DataRow info in dt.Rows)
                list.Add(rowMap(info));
            return list;
        }
    }
}