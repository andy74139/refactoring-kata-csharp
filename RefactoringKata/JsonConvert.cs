using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RefactoringKata
{
    internal class JsonConvert
    {
        public static JsonConvert Instance = new JsonConvert();

        private JsonConvert()
        {
        }

        public string Serialize(object obj)
        {
            var properties = new Dictionary<string, object>();
            var propertyInfos = obj.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                if (!ShouldSerialize(obj, propertyInfo)) continue;
                
                var propertyName = PropertyName(propertyInfo);
                var propertyValue = SelectValue(obj, propertyInfo) ?? propertyInfo.GetValue(obj);
                properties[propertyName] = propertyValue;
            }

            return '{' + string.Join(", ", properties.Select(SerializeProperty)) + '}';
        }

        private string PropertyName(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(false);
            var propertyName = propertyInfo.Name;
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof (JsonProperty))
                    propertyName = ((JsonProperty) attribute).Name;
            }
            return propertyName;
        }

        private string SelectValue(object obj, PropertyInfo propertyInfo)
        {
            var selectMethodInfo = obj.GetType().GetMethod("Select" + propertyInfo.Name);
            return selectMethodInfo == null ? null : (string) selectMethodInfo.Invoke(obj, null);
        }

        private string SerializeProperty(KeyValuePair<string, object> property)
        {
            var value = GetValueJsonString(property.Value);

            return string.Format("{0}: {1}", property.Key.ToLower().Quote(), value);
        }

        // handled types: string, primitive, List<T>, class
        private object GetValueJsonString(object obj)
        {
            var objType = obj.GetType();
            if (objType == typeof (string))
                return ((string) obj).Quote();
            if (objType.IsPrimitive)
                return obj.ToString();
            if (obj is IList && objType.IsGenericType)
                return SerializeEnumerable((dynamic) Convert.ChangeType(obj, objType));
            if(objType.IsClass)
                return Serialize(obj);

            throw new ArgumentException();
        }

        private string SerializeEnumerable<T>(IEnumerable<T> objects)
        {
            return '[' + string.Join(", ", objects.Select(o => Serialize(o))) + ']';
        }

        private bool ShouldSerialize(object obj, PropertyInfo propertyInfo)
        {
            var serializeMethodInfo = obj.GetType().GetMethod("ShouldSerialize" + propertyInfo.Name);
            return serializeMethodInfo == null || (bool)serializeMethodInfo.Invoke(obj, null);
        }
    }

    public static class JsonExtension
    {
        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.Instance.Serialize(obj);
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
