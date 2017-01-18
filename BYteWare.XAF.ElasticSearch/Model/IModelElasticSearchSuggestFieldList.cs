namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// List ElasticSearch Suggest settings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearchSuggestFieldList : IModelNode, IModelList<IModelElasticSearchSuggestField>
    {
    }
}
