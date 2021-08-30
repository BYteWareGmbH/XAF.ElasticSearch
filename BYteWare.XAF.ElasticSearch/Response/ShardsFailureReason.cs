namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;

    /// <summary>
    /// ElasticSearch Failure Reason for a shard
    /// </summary>
    [JsonObject]
    public class ShardsFailureReason
    {
        /// <summary>
        /// Name of the index
        /// </summary>
        [JsonProperty("index")]
        public string Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Number of the shard
        /// </summary>
        [JsonProperty("shard")]
        public int Shard
        {
            get;
            internal set;
        }

        /// <summary>
        /// Status code
        /// </summary>
        [JsonProperty("status")]
        public int Status
        {
            get;
            internal set;
        }

        /// <summary>
        /// Failure Reason
        /// </summary>
        [JsonProperty("reason")]
        public string Reason
        {
            get;
            internal set;
        }
    }
}
