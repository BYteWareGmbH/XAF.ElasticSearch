namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// ElasticSearch MetaData of a Search Response
    /// </summary>
    [JsonObject]
    public class HitsMetaData
    {
        /// <summary>
        /// Total number of hits
        /// </summary>
        [JsonProperty("total")]
        public long Total
        {
            get;
            internal set;
        }

        /// <summary>
        /// Maximum Score value of all reported hits
        /// </summary>
        [JsonProperty("max_score")]
        public double MaxScore
        {
            get;
            internal set;
        }

        /// <summary>
        /// List of all hits
        /// </summary>
        [JsonProperty("hits")]
        public List<Hit> Hits
        {
            get;
            internal set;
        }
    }
}
