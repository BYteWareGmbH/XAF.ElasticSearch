namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch Field Model settings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelMemberElasticSearchField : IModelElasticSearchFieldProperties, IElasticProperties
    {
        /// <summary>
        /// Completion Type: A member name which returns a positive integer, which defines a weight and allows you to rank your suggestions.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("A member which returns a positive integer, which defines a weight and allows you to rank your suggestions.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        [DataSourceProperty(nameof(IntegerFields))]
        IModelMember WeightFieldMember
        {
            get;
            set;
        }

        /// <summary>
        /// It is often useful to index the same field in different ways for different purposes. This is the purpose of multi-fields.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("It is often useful to index the same field in different ways for different purposes. This is the purpose of multi-fields.")]
        IModelMemberElasticSearchFields Fields
        {
            get;
        }

        /// <summary>
        /// List of all potential Weight Field Members
        /// </summary>
        [Browsable(false)]
        IEnumerable<IModelMember> IntegerFields
        {
            get;
        }
    }
}
