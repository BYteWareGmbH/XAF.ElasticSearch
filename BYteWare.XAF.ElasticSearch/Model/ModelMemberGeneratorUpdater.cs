namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Model.Core;
    using DevExpress.ExpressApp.Model.NodeGenerators;
    using Fasterflect;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Update Business Class Member Model entries
    /// </summary>
    [CLSCompliant(false)]
    public class ModelMemberGeneratorUpdater : ModelNodesGeneratorUpdater<ModelBOModelMemberNodesGenerator>
    {
        /// <summary>
        /// Update Business Class Member Model entries with ElasticSearch settings
        /// </summary>
        /// <param name="node">The Business Class Member Model node</param>
        public override void UpdateNode(ModelNode node)
        {
            var member = node as IModelMember;
            var esMember = node as IModelMemberElasticSearch;
            var esProperties = esMember?.ElasticSearch;
            if (member?.MemberInfo != null && esMember != null && esProperties != null)
            {
                var ca = member.MemberInfo.FindAttribute<ElasticContainingAttribute>();
                if (ca != null)
                {
                    esMember.Containing = true;
                }
                var pa = member.MemberInfo.FindAttribute<ElasticPropertyAttribute>();
                if (pa != null)
                {
                    esProperties.IncludeInAll = pa.IncludeInAll;
                    esProperties.OptOut = pa.OptOut;
                    esProperties.CopyTo = pa.CopyTo;
                    esProperties.WeightFieldMember = member.ModelClass.AllMembers.FirstOrDefault(t => t.Name == pa.WeightField);
                    MapFieldProperties(pa, esProperties, member);
                    foreach (var sca in member.MemberInfo.FindAttributes<ElasticSuggestContextAttribute>())
                    {
                        var scaNode = esProperties.SuggestContexts.AddNode<IModelMemberElasticSearchSuggestContext>();
                        MapContextProperties(sca, scaNode);
                    }
                }
                foreach (var mf in member.MemberInfo.FindAttributes<ElasticMultiFieldAttribute>())
                {
                    var mfNode = esProperties.Fields.AddNode<IModelElasticSearchFieldProperties>();
                    MapFieldProperties(mf, mfNode, member);
                    foreach (var sca in member.MemberInfo.FindAttributes<ElasticSuggestContextMultiFieldAttribute>().Where(t => t.FieldName.Equals(mfNode.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var scaNode = mfNode.SuggestContexts.AddNode<IModelMemberElasticSearchSuggestContext>();
                        MapContextProperties(sca, scaNode);
                    }
                }
            }
        }

        private static void MapContextProperties(SuggestContextAttribute sca, IModelMemberElasticSearchSuggestContext scaNode)
        {
            sca.MapProperties(scaNode);
        }

        private static void MapFieldProperties(ElasticAttribute pa, IModelElasticSearchFieldProperties field, IModelMember member)
        {
            var fp = (IElasticSearchFieldProperties)pa;
            fp.MapProperties(field);
            if (string.IsNullOrWhiteSpace(field.Name))
            {
                field.Name = ElasticSearchClient.FieldName(member.Name);
            }
            if (!field.FieldType.HasValue)
            {
                field.FieldType = ElasticSearchClient.GetFieldTypeFromType(member.Type);
            }
        }
    }
}
