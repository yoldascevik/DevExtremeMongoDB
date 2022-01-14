using System;
using System.Linq;
using System.Reflection;

namespace DevExtreme.AspNet.Data.MongoDB.Helpers
{
    internal class ReflectionUtil
    {
        /// <summary>
        /// Get primary key by type
        /// Supported only EF and Linq2Sql entities
        /// </summary>
        public static string[] GetPrimaryKeysByType<T>()
        {
            var type = typeof(T);
            
            return Array.Empty<MemberInfo>()
                .Concat(type.GetRuntimeProperties())
                .Concat(type.GetRuntimeFields())
                .Where(m => m.GetCustomAttributes(true).Any(i => i.GetType().Name == "KeyAttribute")
                            || m.GetCustomAttributes(true).Any(i => i.GetType().Name == "ColumnAttribute"
                                            && Convert.ToBoolean(i.GetType().GetProperty("IsPrimaryKey")?.GetValue(i))))
                .Select(m => m.Name)
                .OrderBy(i => i)
                .ToArray();
        }
    }
}