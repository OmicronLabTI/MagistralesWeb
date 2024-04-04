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

            var objectType = value.GetType();
            if (this.propertyMappings.TryGetValue(objectType, out var propertyMappings))
            {
                var jsonObject = new JObject();
                foreach (var propertyMapping in propertyMappings)
                {
                    var propertyName = propertyMapping.Key;
                    var newPropertyName = propertyMapping.Value;
                    var propertyValue = objectType.GetProperty(propertyName)?.GetValue(value);
                    if (propertyValue == null)
                    {
                        jsonObject.Add(newPropertyName, JValue.CreateNull());
                    }
                    else
                    {
                        jsonObject.Add(newPropertyName, JToken.FromObject(propertyValue));
                    }
                }

                foreach (var property in objectType.GetProperties().Where(p => !propertyMappings.ContainsKey(p.Name)))
                {
                    var propertyValue = property.GetValue(value);
                    if (propertyValue == null)
                    {
                        jsonObject.Add(property.Name, JValue.CreateNull());
                    }
                    else
                    {
                        jsonObject.Add(property.Name, JToken.FromObject(propertyValue));
                    }
                }

                jsonObject.WriteTo(writer);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var instance = Activator.CreateInstance(objectType);

            foreach (var property in objectType.GetProperties())
            {
                if (jsonObject.TryGetValue(property.Name, out JToken value))
                {
                    var propertyName = property.Name;
                    if (this.propertyMappings.TryGetValue(objectType, out var propertyMappings) && propertyMappings.TryGetValue(property.Name, out var newPropertyName))
                    {
                        property.SetValue(instance, jsonObject[newPropertyName].ToObject(property.PropertyType, serializer));
                    }
                    else
                    {
                        property.SetValue(instance, value.ToObject(property.PropertyType, serializer));
                    }
                }
            }

            return instance;
        }
    }
}
