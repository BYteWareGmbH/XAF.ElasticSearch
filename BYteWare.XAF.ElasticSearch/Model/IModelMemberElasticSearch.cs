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
    public interface IModelMemberElasticSearch
    {
        /// <summary>
        /// Properties of that class where used to construct the ElasticSearch document of an instance of the current class, so indexing has to happen even if only a referenced instance got changed.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Properties of that class where used to construct the ElasticSearch document of an instance of the current class, so indexing has to happen even if only a referenced instance got changed.")]
        [ModelBrowsable(typeof(ModelElasticSearchContainingVisibilityCalculator))]
        bool Containing
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch Mapping Settings
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch Mapping Settings")]
        IModelMemberElasticSearchField ElasticSearch
        {
            get;
        }
    }
}
