namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Contexts settings for a suggest field
    /// </summary>
    [KeyProperty(nameof(Name))]
    [CLSCompliant(false)]
    public interface IModelElasticSearchSuggestContext : IModelNode
    {
        /// <summary>
        /// ElasticSearch Context Name
        /// </summary>
        [Required]
        [Category("Filter")]
        [Description("ElasticSearch Context Name")]
        [RefreshProperties(RefreshProperties.All)]
        [DataSourceProperty(nameof(ElasticSearchSuggestFieldContexts))]
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the Parameter whose value should be used for the context that was defined through the Suggest model settings/attribute
        /// </summary>
        [Category("Filter")]
        [Description("Name of the Parameter whose value should be used for the context that was defined through the Suggest attribute")]
        string DefaultParameter
        {
            get;
        }

        /// <summary>
        /// Name of the Parameter whose value should be used for the context, overrides the DefaultParameter
        /// </summary>
        [Category("Filter")]
        [Description("Name of the Parameter whose value should be used for the context, overrides the DefaultParameter")]
        [TypeConverter(typeof(DefaultElasticSearchParameterConverter))]
        string Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// Fixed value to use for the context
        /// </summary>
        [Category("Filter")]
        [Description("Fixed value to use for the context")]
        string Value
        {
            get;
            set;
        }

        /// <summary>
        /// List of all potential Context names
        /// </summary>
        [Browsable(false)]
        IList<string> ElasticSearchSuggestFieldContexts
        {
            get;
        }

        /// <summary>
        /// ElasticSearch Suggest Field Model Settings
        /// </summary>
        [Browsable(false)]
        IModelElasticSearchSuggestField ModelElasticSearchSuggestField
        {
            get;
        }

        /// <summary>
        /// The Type Info of the Business Class
        /// </summary>
        [Browsable(false)]
        ITypeInfo TypeInfo
        {
            get;
        }
    }
}
