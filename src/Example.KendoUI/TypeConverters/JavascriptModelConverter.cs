using Example.KendoUI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Example.KendoUI.TypeConverters
{
    /// <summary>
    /// <see cref="JavascriptModelConverter"/> sealed class, provides a way to control the serialization of <see cref="KendoModel"/> objects.
    /// </summary>
    public sealed class JavascriptModelConverter : JsonConverter
    {
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(KendoModel))
                return true;

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var json_writer = writer as JsonTextWriter;
            json_writer.QuoteName = false;

            var model = value as KendoModel;
            if (model != null)
            {
                serializer.Serialize(json_writer, model.ToJObject());
            }
        }
    }
}
