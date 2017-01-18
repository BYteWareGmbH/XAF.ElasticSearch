namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// ElasticSearch Suggest Response
    /// </summary>
    [JsonObject]
    public class Suggest
    {
        /// <summary>
        /// The text that was searched
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("text")]
        public string Text
        {
            get;
            internal set;
        }

        /// <summary>
        /// The offset into the original search text
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("offset")]
        public int Offset
        {
            get;
            internal set;
        }

        /// <summary>
        /// The offset into the original search text
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("length")]
        public int Length
        {
            get;
            internal set;
        }

        /// <summary>
        /// List of suggest options for the search text
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Serialization")]
        [JsonProperty("options")]
        public IEnumerable<SuggestOption> Options
        {
            get;
            internal set;
        }
    }
}
