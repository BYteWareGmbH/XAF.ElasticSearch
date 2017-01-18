namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Model.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates Nodes for all ElasticSearch Indexes
    /// </summary>
    [CLSCompliant(false)]
    public class ElasticSearchIndexGenerator : ModelNodesGeneratorBase
    {
        /// <summary>
        /// Generate Nodes for all ElasticSearch Indexes
        /// </summary>
        /// <param name="node">The parent node</param>
        protected override void GenerateNodesCore(ModelNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
            var indexNames = new HashSet<string>();
            foreach (var ti in XafTypesInfo.Instance.PersistentTypes.Where(t => t.IsPersistent))
            {
                var bi = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                if (bi?.ESAttribute != null && indexNames.Add(bi.ESAttribute.IndexName))
                {
                    var modelElasticSearchIndex = node.AddNode<IModelElasticSearchIndex>();
                    modelElasticSearchIndex.Name = bi.ESAttribute.IndexName;
                }
            }
        }
    }
}
