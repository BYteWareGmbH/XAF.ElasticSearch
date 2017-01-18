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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("total")]
        public long Total
        {
            get;
            internal set;
        }

        /// <summary>
        /// Maximum Score value of all reported hits
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("max_score")]
        public double MaxScore
        {
            get;
            internal set;
        }

        /// <summary>
        /// List of all hits
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = nameof(Newtonsoft.Json))]
        [JsonProperty("hits")]
        public List<Hit> Hits
        {
            get;
            internal set;
        }
    }
}
