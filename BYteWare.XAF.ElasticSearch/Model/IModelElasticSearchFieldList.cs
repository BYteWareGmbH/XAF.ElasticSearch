namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// List of ElasticSearch Field Model settings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearchFieldList : IModelNode, IModelList<IModelElasticSearchField>
    {
    }
}
