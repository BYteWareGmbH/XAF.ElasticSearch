namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// ElasticSearch Suggest Option
    /// </summary>
    [JsonObject]
    public class SuggestOption
    {
        /// <summary>
        /// Suggested Text
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("text")]
        public string Text
        {
            get;
            internal set;
        }

        /// <summary>
        /// Score value of the suggestion
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("_score")]
        public double Score
        {
            get;
            internal set;
        }

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
        /// Id of the search hit
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("_id")]
        public string Id
        {
            get;
            internal set;
        }

        /*[JsonProperty("_source")]
        public object Source
        {
            get;
            internal set;
        }

        [JsonProperty("contexts")]
        public IDictionary<string, IEnumerable<Context>> Contexts
        {
            get;
            internal set;
        }

        [JsonProperty("highlighted")]
        public string Highlighted
        {
            get;
            internal set;
        }

        [JsonProperty("collate_match")]
        public bool CollateMatch
        {
            get;
            internal set;
        }*/
    }
}
