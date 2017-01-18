namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Utils.Extension;

    /// <summary>
    /// Methods for the IModelMemberElasticSearchField Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelMemberElasticSearchField))]
    public static class ModelMemberElasticSearchFieldLogic
    {
        /// <summary>
        /// Returns an Enumeration of all numeric members
        /// </summary>
        /// <param name="esField">IModelMemberElasticSearchField instance</param>
        /// <returns>Enumeration of all numeric members</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        public static IEnumerable<IModelMember> Get_IntegerFields(IModelMemberElasticSearchField esField)
        {
            if (esField != null)
            {
                var member = esField.Parent as IModelMember;
                if (member?.ModelClass != null)
                {
                    return member.ModelClass.AllMembers.Where(t => t.Type.IsNumericType());
                }
            }
            return Enumerable.Empty<IModelMember>();
        }

        /// <summary>
        /// Returns the name of the WeightFieldMember
        /// </summary>
        /// <param name="esField">IModelMemberElasticSearchField instance</param>
        /// <returns>Name of the WeightFieldMember</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        public static string Get_WeightField(IModelMemberElasticSearchField esField)
        {
            return esField?.WeightFieldMember?.Name;
        }
    }
}
