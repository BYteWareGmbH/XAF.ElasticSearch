namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;

    /// <summary>
    /// ElasticSearch Contexts for Completion field
    /// </summary>
    [CLSCompliant(false)]
    [ModelReadOnly(typeof(ModelSuggestContextsReadOnlyCalculator))]
    public interface IModelMemberElasticSearchSuggestContexts : IModelNode, IModelList<IModelMemberElasticSearchSuggestContext>
    {
    }
}