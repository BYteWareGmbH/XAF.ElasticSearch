namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Model.Core;
    using DevExpress.ExpressApp.Model.NodeGenerators;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Update Business Class Model entries
    /// </summary>
    [CLSCompliant(false)]
    public class ModelClassGeneratorUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator>
    {
        /// <summary>
        /// Update Business Class Model entries with ElasticSearch settings
        /// </summary>
        /// <param name="node">The Business Class Model node</param>
        public override void UpdateNode(ModelNode node)
        {
            var boModel = node as IModelBOModel;
            var appElasticSearch = node?.Application as IModelApplicationElasticSearch;
            if (boModel != null && appElasticSearch != null)
            {
                foreach (var modelClass in boModel)
                {
                    var modelElasticSearch = modelClass as IModelClassElasticSearch;
                    if (modelClass.TypeInfo?.Type != null)
                    {
                        var bi = BYteWareTypeInfo.GetBYteWareTypeInfo(modelClass.TypeInfo.Type);
                        if (bi?.ESAttribute != null)
                        {
                            modelElasticSearch.ElasticSearchIndex = appElasticSearch.ElasticSearch.Indexes.GetNode(bi.ESAttribute.IndexName) as IModelElasticSearchIndex;
                            modelElasticSearch.TypeName = bi.ESAttribute.TypeName;
                            modelElasticSearch.SourceFieldDisabled = bi.ESAttribute.SourceFieldDisabled;
                        }
                    }
                }
            }
        }
    }
}
