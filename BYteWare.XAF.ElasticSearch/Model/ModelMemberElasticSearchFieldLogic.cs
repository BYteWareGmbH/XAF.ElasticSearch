namespace BYteWare.XAF.ElasticSearch.Model
{
    using BYteWare.Utils.Extension;
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        public static string Get_WeightField(IModelMemberElasticSearchField esField)
        {
            return esField?.WeightFieldMember?.Name;
        }

        /// <summary>
        /// Returns the name of the WeightFieldMember
        /// </summary>
        /// <param name="esField">IModelMemberElasticSearchField instance</param>
        /// <param name="value">FieldType value</param>
        public static void Set_FieldType(IModelMemberElasticSearchField esField, FieldType? value)
        {
            if (esField != null)
            {
                if (value.HasValue && esField.Parent is IModelMember member && string.IsNullOrEmpty(esField.FieldName))
                {
                    esField.FieldName = ElasticSearchClient.FieldName(member.Name);
                }
                ((ModelNode)esField).SetValue<FieldType?>(nameof(IModelMemberElasticSearchField.FieldType), value);
            }
        }
    }
}
