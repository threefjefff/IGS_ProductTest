using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public abstract class ProductBase
    {
        public string Name { get; set; }
        [JsonConverter(typeof(FloatAsStringConverter))]
        public float Price { get; set; }
    }

    public class FloatAsStringConverter: JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return float.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("F2"));
        }
    }
}