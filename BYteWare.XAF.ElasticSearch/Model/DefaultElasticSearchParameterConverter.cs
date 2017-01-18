namespace BYteWare.XAF.ElasticSearch.Model
{
    using ElasticSearch;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// List all defined Parameter Names from the static ElasticSearchClient.Instance
    /// </summary>
    public class DefaultElasticSearchParameterConverter : StringConverter
    {
        /// <inheritdoc/>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <inheritdoc/>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(ElasticSearchClient.Instance.ParameterNames.ToList());
        }
    }
}