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
        [JsonProperty("text")]
        public string Text
        {
            get;
            internal set;
        }

        /// <summary>
        /// Score value of the suggestion
        /// </summary>
        [JsonProperty("_score")]
        public double Score
        {
            get;
            internal set;
        }

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
        /// Id of the search hit
        /// </summary>
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
