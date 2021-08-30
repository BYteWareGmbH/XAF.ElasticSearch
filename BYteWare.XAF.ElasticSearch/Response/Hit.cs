namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;

    /// <summary>
    /// ElasticSearch Hit Class
    /// </summary>
    [JsonObject]
    public class Hit
    {
        /// <summary>
        /// Name of the index
        /// </summary>
        [JsonProperty("_index")]
        public string Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Name of the Type
        /// </summary>
        [JsonProperty("_type")]
        public string Type
        {
            get;
            internal set;
        }

        /// <summary>
        /// Score value of the search hit
        /// </summary>
        [JsonProperty("_score")]
        public double Score
        {
            get;
            internal set;
        }

        /// <summary>
        /// Id of the search hit
        /// </summary>
        [JsonProperty("_id")]
        public string Id
        {
            get;
            internal set;
        }
    }
}
