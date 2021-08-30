namespace BYteWare.XAF.ElasticSearch.Model
{
    using BYteWare.Utils.Extension;
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Methods for the IModelMemberElasticSearchSuggestContext Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelMemberElasticSearchSuggestContext))]
    public static class ModelMemberElasticSearchSuggestContextLogic
    {
        /// <summary>
        /// Returns an Enumeration of potential ElasticSearch Field Names
        /// </summary>
        /// <param name="suggestContext">IModelMemberElasticSearchSuggestContext instance</param>
        /// <returns>Enumeration of potential ElasticSearch Field Names</returns>
        public static IEnumerable<string> Get_ElasticSearchFields(IModelMemberElasticSearchSuggestContext suggestContext)
        {
            if (suggestContext?.Parent?.Parent is IModelElasticSearchFieldProperties esProperties)
            {
                if (!(esProperties.Parent is IModelMember member))
                {
                    var fields = esProperties.Parent as IModelMemberElasticSearchFields;
                    member = fields?.Parent?.Parent as IModelMember;
                }
                if (member?.ModelClass?.TypeInfo?.Type != null)
                {
                    var esFields = new HashSet<string>();
                    foreach (var ti in member.ModelClass.TypeInfo.DescendantsAndSelf(t => t.Descendants))
                    {
                        esFields.UnionWith(ElasticSearchClient.ElasticSearchFields(ti.Type, false));
                    }
                    return esFields;
                }
            }
            return Enumerable.Empty<string>();
        }
    }
}
