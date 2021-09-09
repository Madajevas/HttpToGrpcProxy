using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace ProxyInterceptorTestsClient
{
    public static class BindingExtensions
    {
        public static T BindJson<T>(this RequestContext requestContext)
        {
            return JsonSerializer.Deserialize<T>(requestContext.Body);
        }

        public static T BindForm<T>(this RequestContext requestContext) where T : new()
        {
            var properties = typeof(T).GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);
            var form = HttpUtility.ParseQueryString(requestContext.Body);
            var instance = new T();

            foreach (var key in form.AllKeys)
            {
                if (!properties.TryGetValue(key.ToLower(), out var property))
                {
                    continue;
                }

                var convertedValue = Convert.ChangeType(form[key], property.PropertyType);
                property.SetValue(instance, convertedValue);
            }

            return instance;
        }

        public static T BindHeaders<T>(this RequestContext requestContext) where T : new()
        {
            var properties = typeof(T).GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);
            var instance = new T();

            BindFromDictionary(requestContext.Headers, properties, instance);

            return instance;
        }

        public static T BindQuery<T>(this RequestContext requestContext) where T : new()
        {
            var properties = typeof(T).GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);
            var instance = new T();

            BindFromDictionary(requestContext.Query, properties, instance);

            return instance;
        }

        private static void BindFromDictionary<T>(Dictionary<string, string> propertyValues, Dictionary<string, System.Reflection.PropertyInfo> properties, T instance) where T : new()
        {
            foreach (var (key, value) in propertyValues)
            {
                if (!properties.TryGetValue(key.ToLower(), out var property))
                {
                    continue;
                }

                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(instance, convertedValue);
            }
        }
    }
}
