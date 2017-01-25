namespace BYteWare.XAF.ElasticSearch.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// EventArgs Class to customize an ElasticSearch Fulltext search
    /// </summary>
    public class CustomSearchEventArgs : HandledEventArgs
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="CustomSearchEventArgs"/> class.
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="indexes">The indexes to search in</param>
        /// <param name="types">The types to search in</param>
        /// <param name="filter">The filter to be applied</param>
        /// <param name="securityFilter">The filter defined through the security system</param>
        public CustomSearchEventArgs(string searchText, string[] indexes, string[] types, string filter, string securityFilter)
        {
            SearchText = searchText;
            Indexes = indexes;
            Types = types;
            Filter = filter;
            SecurityFilter = securityFilter;
        }

        /// <summary>
        /// The text to search for
        /// </summary>
        public string SearchText
        {
            get;
            set;
        }

        /// <summary>
        /// The indexes to search in
        /// </summary>
        public IEnumerable<string> Indexes
        {
            get;
            set;
        }

        /// <summary>
        /// The types to search in
        /// </summary>
        public IEnumerable<string> Types
        {
            get;
            set;
        }

        /// <summary>
        /// The filter to be applied
        /// </summary>
        public string Filter
        {
            get;
            set;
        }

        /// <summary>
        /// The filter defined through the security system
        /// </summary>
        public string SecurityFilter
        {
            get;
            set;
        }

        /// <summary>
        /// The Json search string to send to ElasticSearch
        /// </summary>
        public string Json
        {
            get;
            set;
        }

        /// <summary>
        /// True if the search is to be retried as fuzzy search, because there were no hits
        /// </summary>
        public bool Retry
        {
            get;
            set;
        }
    }
}