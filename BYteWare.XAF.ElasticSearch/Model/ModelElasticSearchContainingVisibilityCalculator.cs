namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Determines visibility of the Containing property
    /// </summary>
    [CLSCompliant(false)]
    public class ModelElasticSearchContainingVisibilityCalculator : IModelIsVisible
    {
        /// <summary>
        /// Determines visibility of the Containing property
        /// </summary>
        /// <param name="node">The Model node to check</param>
        /// <param name="propertyName">name of the property to be checked</param>
        /// <returns>True if the Property should be visible; False otherwise</returns>
        public bool IsVisible(IModelNode node, string propertyName)
        {
            var member = node as IModelMember;
            if (member?.MemberInfo != null)
            {
                return (member.MemberInfo.MemberTypeInfo != null && member.MemberInfo.MemberTypeInfo.IsPersistent) ||
                    (member.MemberInfo.IsAssociation && member.MemberInfo.ListElementTypeInfo != null && member.MemberInfo.ListElementTypeInfo.IsPersistent);
            }
            return false;
        }
    }
}
