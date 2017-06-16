using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RefactoringKata
{
    public class JsonConvert
    {
        public static string Serialize(object obj)
        {
            var properties = new Dictionary<string, object>();
            var propertyInfos = obj.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                if (!ShouldSerialize(obj, propertyInfo)) continue;
                properties[propertyInfo.Name] = SelectValue(obj, propertyInfo) ?? propertyInfo.GetValue(obj);
            }

            return '{' + string.Join(", ", properties.Select(SerializeProperty)) + '}';
        }

        private static string SelectValue(object obj, PropertyInfo propertyInfo)
        {
            var selectMethodInfo = obj.GetType().GetMethod("Select" + propertyInfo.Name);
            return selectMethodInfo == null ? null : (string) selectMethodInfo.Invoke(obj, null);
        }

        private static string SerializeProperty(KeyValuePair<string, object> property)
        {
            object value;
            var valueType = property.Value.GetType();
            if (valueType == typeof(string))
                value = ((string)property.Value).Quote();
            else if (valueType.IsPrimitive)
                value = property.Value.ToString();
            else if (property.Value is IList && valueType.IsGenericType)
            {
                //var genericType = valueType.GenericTypeArguments[0];
                value = SerializeEnumerable((dynamic) Convert.ChangeType(property.Value ,valueType));
            }
            else
                value = Serialize(property.Value);

            return string.Format("{0}: {1}", property.Key.ToLower().Quote(), value);
        }

        private static string SerializeEnumerable<T>(IEnumerable<T> objects)
        {
            return '[' + string.Join(", ", objects.Select(o => Serialize(o))) + ']';
        }

        private static bool ShouldSerialize(object obj, PropertyInfo propertyInfo)
        {
            var serializeMethodInfo = obj.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
            return serializeMethodInfo == null || (bool)serializeMethodInfo.Invoke(obj, null);
        }
    }

    public static class JsonExtension
    {
        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.Serialize(obj);
        }

        public static string Quote(this string str)
        {
            return '"' + str + '"';
        }
    }

    public class JsonProperty : Attribute
    {
        public readonly string Name;

        public JsonProperty(string name)
        {
            Name = name;
        }
    }

}
