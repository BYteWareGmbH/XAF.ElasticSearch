namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// ElasticSearch Metadata for the shards of an index
    /// </summary>
    [JsonObject]
    public class ShardsMetaData
    {
        /// <summary>
        /// Number of shards
        /// </summary>
        [JsonProperty]
        public int Total
        {
            get;
            internal set;
        }

        /// <summary>
        /// Number of successful shards
        /// </summary>
        [JsonProperty]
        public int Successful
        {
            get;
            internal set;
        }

        /// <summary>
        /// Number of failed shards
        /// </summary>
        [JsonProperty]
        public int Failed
        {
            get;
            internal set;
        }

        /// <summary>
        /// List of ShardsFailureReason instances for the failed shards
        /// </summary>
        [JsonProperty("failures")]
        public IEnumerable<ShardsFailureReason> Failures
        {
            get;
            internal set;
        }
    }
}
