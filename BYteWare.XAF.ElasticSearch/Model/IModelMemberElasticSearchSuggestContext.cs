namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// ElasticSearch Contexts for Completion field
    /// </summary>
    [CLSCompliant(false)]
    [KeyProperty(nameof(ContextName))]
    public interface IModelMemberElasticSearchSuggestContext : IModelNode, IElasticSearchSuggestContext
    {
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