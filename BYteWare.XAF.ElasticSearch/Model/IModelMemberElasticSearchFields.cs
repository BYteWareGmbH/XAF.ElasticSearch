namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;

    /// <summary>
    /// ElasticSearch Multi Fields Model settings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelMemberElasticSearchFields : IModelNode, IModelList<IModelMemberElasticSearchFieldItem>
    {
    }
}