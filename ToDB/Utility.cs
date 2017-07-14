using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToDB.Model;

namespace ToDB
{
    static class Utility
    {
        public static IEnumerable<string> ExtractColumnNames(object obj)
        {
            Type type = obj.GetType();
            return GetFieldsAndProperties(type)
                    .Select(p => p.Name);
        }

        public static string GetPropertyName<T>(this Expression<Func<T>> property)
        {
            MemberExpression member = property.Body as MemberExpression;
            PropertyInfo propInfo = member.Member as PropertyInfo;
            return propInfo.Name;
        }
        public static string ToSqlParameter<T>(this Expression<Func<T>> property)
        {
            
            return  "@" + GetPropertyName(property);
        }

        public static IEnumerable<FieldOrProperty> GetFieldsAndProperties(Type t)
        {
            return t
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Select(p => new FieldOrProperty(p))
                .Concat(
                    t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                        .Select(f => new FieldOrProperty(f))
                );
        }
        

    }
}