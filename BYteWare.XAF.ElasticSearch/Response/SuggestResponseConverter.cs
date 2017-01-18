namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

    /// <summary>
    /// Json Converter for an ElasticSearch Suggest Response
    /// </summary>
    public class SuggestResponseConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var response = new SuggestResponse();
            var jsonObject = JObject.Load(reader);
            foreach (var prop in jsonObject.Properties())
            {
                if (prop.Name == "_shards")
                {
                    response.Shards = prop.Value.ToObject<ShardsMetaData>();
                }
                else
                {
                    response.Suggestions.Add(prop.Name, prop.Value.ToObject<Suggest[]>());
                }
            }
            return response;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
