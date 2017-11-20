namespace BYteWare.XAF.ElasticSearch.Response
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// ElasticSearch Suggest Response
    /// </summary>
    [JsonObject]
    [JsonConverter(typeof(SuggestResponseConverter))]
    public class SuggestResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuggestResponse"/> class.
        /// </summary>
        public SuggestResponse()
        {
            this.Suggestions = new Dictionary<string, Suggest[]>();
        }

        /// <summary>
        /// Shards Metadata Instance
        /// </summary>
        public ShardsMetaData Shards
        {
            get;
            internal set;
        }

        /// <summary>
        /// Dictionary of all Suggestions per Request
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = nameof(Newtonsoft.Json))]
        public IDictionary<string, Suggest[]> Suggestions
        {
            get;
            set;
        }
    }
}
