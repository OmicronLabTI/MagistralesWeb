// <summary>
// <copyright file="CustomJsonConverter.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Orders
{
    /// <summary>
    /// Class Dynamic JsonPropertyConverter.
    /// </summary>
    public class CustomJsonConverter : Newtonsoft.Json.JsonConverter
    {
        private readonly Dictionary<Type, Dictionary<string, string>> propertyMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomJsonConverter"/> class.
        /// </summary>
        /// <param name="propertyMappings">The properties</param>
        public CustomJsonConverter(Dictionary<Type, Dictionary<string, string>> propertyMappings)
        {
            this.propertyMappings = propertyMappings ?? throw new ArgumentNullException(nameof(propertyMappings));
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return this.propertyMappings.ContainsKey(objectType);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var jsonObject = new JObject();
            var objectType = value.GetType();

            foreach (var property in objectType.GetProperties())
            {
                var jsonPropertyName = GetJsonPropertyName(property);

                var newPropertyName = this.GetPropertyChangeName(jsonPropertyName);
                var propertyValue = property.GetValue(value);
                if (propertyValue != null)
                {
                    jsonObject.Add(newPropertyName, JToken.FromObject(propertyValue, serializer));
                }
                else
                {
                    jsonObject.Add(newPropertyName, JValue.CreateNull());
                }
            }

            jsonObject.WriteTo(writer);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var instance = Activator.CreateInstance(objectType);

            foreach (var property in objectType.GetProperties())
            {
                var jsonPropertyName = GetJsonPropertyName(property);
                var jsonPropertyNameChange = this.GetPropertyChangeName(jsonPropertyName);
                if (jsonObject.TryGetValue(jsonPropertyNameChange, out JToken value))
                {
                    var propertyValue = value.ToObject(property.PropertyType, serializer);
                    property.SetValue(instance, propertyValue);
                }
            }

            return instance;
        }

        private static string GetJsonPropertyName(PropertyInfo property)
        {
            var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
            return jsonPropertyAttribute != null ? jsonPropertyAttribute.PropertyName : property.Name;
        }

        private string GetPropertyChangeName(string propertyName)
        {
            if (this.propertyMappings != null && this.propertyMappings.Count > 0)
            {
                return this.propertyMappings.FirstOrDefault().Value.TryGetValue(propertyName, out string newPropertyName) ? newPropertyName : propertyName;
            }

            return propertyName;
        }
    }
}
