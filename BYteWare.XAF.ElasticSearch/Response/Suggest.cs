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
        [JsonProperty("text")]
        public string Text
        {
            get;
            internal set;
        }

        /// <summary>
        /// The offset into the original search text
        /// </summary>
        [JsonProperty("offset")]
        public int Offset
        {
            get;
            internal set;
        }

        /// <summary>
        /// The offset into the original search text
        /// </summary>
        [JsonProperty("length")]
        public int Length
        {
            get;
            internal set;
        }

        /// <summary>
        /// List of suggest options for the search text
        /// </summary>
        [JsonProperty("options")]
        public IEnumerable<SuggestOption> Options
        {
            get;
            internal set;
        }
    }
}
