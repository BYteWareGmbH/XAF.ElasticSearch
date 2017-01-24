namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch Field Model settings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearchFieldProperties : IElasticSearchFieldProperties, IModelNode
    {
        /// <summary>
        /// Completion Type: Contexts to filter suggestions
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Contexts to filter suggestions.")]
        IModelMemberElasticSearchSuggestContexts SuggestContexts
        {
            get;
        }
    }
}
