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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("index")]
        public string Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Number of the shard
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("shard")]
        public int Shard
        {
            get;
            internal set;
        }

        /// <summary>
        /// Status code
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("status")]
        public int Status
        {
            get;
            internal set;
        }

        /// <summary>
        /// Failure Reason
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("reason")]
        public string Reason
        {
            get;
            internal set;
        }
    }
}
