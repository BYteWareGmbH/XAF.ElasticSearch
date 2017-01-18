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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("_index")]
        public string Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Name of the Type
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Serialization")]
        [JsonProperty("_type")]
        public string Type
        {
            get;
            internal set;
        }

        /// <summary>
        /// Score value of the search hit
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("_score")]
        public double Score
        {
            get;
            internal set;
        }

        /// <summary>
        /// Id of the search hit
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("_id")]
        public string Id
        {
            get;
            internal set;
        }
    }
}
