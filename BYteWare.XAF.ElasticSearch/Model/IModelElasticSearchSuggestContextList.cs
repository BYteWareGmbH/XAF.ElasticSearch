namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// List of contexts settings for the suggest field
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearchSuggestContextList : IModelNode, IModelList<IModelElasticSearchSuggestContext>
    {
    }
}
