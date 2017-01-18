namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.ExpressApp.DC;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Informations about a suggest field of a type
    /// </summary>
    public class SuggestField
    {
        /// <summary>
        /// The ElasticSearch Field Name
        /// </summary>
        public string FieldName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Search for suggestions when no speicifc field to search in was specified (_all field search)
        /// </summary>
        public bool Default
        {
            get;
            internal set;
        }

        /// <summary>
        /// Member Info of the field which should supply weight values for the suggestions
        /// </summary>
        [CLSCompliant(false)]
        public IMemberInfo WeightField
        {
            get;
            internal set;
        }

        /// <summary>
        /// List of settings to define the context values for filtering suggestions
        /// </summary>
        public ReadOnlyCollection<IElasticSearchSuggestContext> ContextSettings
        {
            get;
            internal set;
        }
    }
}
