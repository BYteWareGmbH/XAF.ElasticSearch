namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Filter Field Model settings
    /// </summary>
    [KeyProperty(nameof(ElasticSearchField))]
    [CLSCompliant(false)]
    public interface IModelElasticSearchField : IModelNode
    {
        /// <summary>
        /// Name of the ElasticSearch Field
        /// </summary>
        [Required]
        [Category(nameof(ElasticSearch))]
        [Description("Name of the ElasticSearch Field")]
        [DataSourceProperty(nameof(ElasticSearchFields))]
        string ElasticSearchField
        {
            get;
            set;
        }

        /// <summary>
        /// Boost Value to change the ranking
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Boost Value to change the ranking")]
        [DefaultValue(1.0)]
        double Boost
        {
            get;
            set;
        }

        /// <summary>
        /// Enumeration of potential ElasticSearch Field Names
        /// </summary>
        [Browsable(false)]
        IEnumerable<string> ElasticSearchFields
        {
            get;
        }
    }
}
