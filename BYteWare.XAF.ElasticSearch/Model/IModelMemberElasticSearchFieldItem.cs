namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch Multi Field Model settings
    /// </summary>
    [CLSCompliant(false)]
    [KeyProperty(nameof(FieldName))]
    public interface IModelMemberElasticSearchFieldItem : IModelElasticSearchFieldProperties
    {
    }
}
