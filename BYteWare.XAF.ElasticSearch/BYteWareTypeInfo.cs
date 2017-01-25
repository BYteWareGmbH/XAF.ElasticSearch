namespace BYteWare.XAF
{
    using BYteWare.XAF.ElasticSearch;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Utils.Reflection;
    using DevExpress.ExpressApp.Xpo;
    using DevExpress.Xpo;
    using DevExpress.Xpo.Metadata;
    using DevExpress.Xpo.Metadata.Helpers;
    using ElasticSearch.Model;
    using Fasterflect;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Utils.Extension;

    /// <summary>
    /// Type Specific Settings
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(XAF))]
    public class BYteWareTypeInfo
    {
        private static readonly Dictionary<Type, BYteWareTypeInfo> TypeDictionary = new Dictionary<Type, BYteWareTypeInfo>();
        private MemberInfoCollection _DefaultPropertyMemberInfoCol;
        private bool _SearchedElasticSearchAttribute;
        private ElasticSearchAttribute _ElasticSearchAttribute;
        private bool isDefaultPropertyAttributeInit;
        private bool _IsESReferenceChecked;
        private List<XPMemberInfo> _ESReferences;
        private bool _IsContainingTypesChecked;
        private List<ContainingType> _ContainingTypes;
        private List<string> _ESIndexes;
        private List<string> _ESTypes;
        private List<SuggestField> _SuggestFields;
        private bool _IsSecurityFilterChecked;
        private string _ESSecurityFilter;
        private IModelClass _ModelClass;

        /// <summary>
        /// The Info type to create instances of
        /// </summary>
        public static Type InfoType
        {
            get;
            set;
        }
        = typeof(BYteWareTypeInfo);

        /// <summary>
        /// Application model settings
        /// </summary>
        [CLSCompliant(false)]
        public static IModelApplication Model
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a ByteWareTypeInfo instance with inormations about the type
        /// </summary>
        /// <param name="type">A Type instance</param>
        /// <returns>The ByteWareTypeInfo instance for type</returns>
        public static BYteWareTypeInfo GetBYteWareTypeInfo(Type type)
        {
            BYteWareTypeInfo ci;
            if (!TypeDictionary.TryGetValue(type, out ci))
            {
                ci = TypeHelper.CreateInstance(InfoType, type) as BYteWareTypeInfo;
                TypeDictionary.Add(type, ci);
            }
            return ci;
        }

        /// <summary>
        /// Clears all User dependent information, must be called on Logging Off
        /// </summary>
        public static void LoggingOff()
        {
            foreach (var byteWareType in TypeDictionary.Values)
            {
                byteWareType._IsSecurityFilterChecked = false;
                byteWareType._ESSecurityFilter = null;
            }
        }

        /// <summary>
        /// Clears all cached Model instances
        /// </summary>
        public static void ModelRefresh()
        {
            foreach (var byteWareType in TypeDictionary.Values)
            {
                byteWareType._ModelClass = null;
            }
        }

        /// <summary>
        /// Returns the element type for arrays or IEnumerables, the base type of a Nullable otherwise type
        /// </summary>
        /// <param name="type">Type to examine</param>
        /// <returns>element type for arrays or IEnumerables, the base type of a Nullable otherwise type</returns>
        public static Type GetUnderlyingType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType && type.GetGenericArguments().Length == 1 &&
                (type.GetInterfaces().Any(t => t == typeof(IEnumerable)) || Nullable.GetUnderlyingType(type) != null))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BYteWareTypeInfo" /> class.
        /// </summary>
        /// <param name="type">A Type instance</param>
        public BYteWareTypeInfo(Type type)
        {
            Type = type;
            TypeInfo = XafTypesInfo.Instance.FindTypeInfo(Type);
            ClassInfo = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.QueryClassInfo(Type);
        }

        /// <summary>
        /// The Type instance
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "MetaData")]
        public Type Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Xaf Type Information
        /// </summary>
        [CLSCompliant(false)]
        public ITypeInfo TypeInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// XPO Type Information
        /// </summary>
        public XPClassInfo ClassInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the ModelClass instance for the type
        /// </summary>
        [CLSCompliant(false)]
        public IModelClass ModelClass
        {
            get
            {
                if (_ModelClass == null)
                {
                    _ModelClass = Model?.BOModel?.GetClass(Type);
                }
                return _ModelClass;
            }
        }

        /// <summary>
        /// Returns a MemberInfoCollection instance for the defined (Xaf)Default Property of the type, null if no such attribute was defined.
        /// This helps to support a Default Property with a path to a nested property, e. g. in a ToString implementation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = nameof(XAF))]
        public MemberInfoCollection DefaultPropertyMemberInfoCol
        {
            get
            {
                if (!isDefaultPropertyAttributeInit)
                {
                    var defaultPropertyName = string.Empty;
                    var xafDefaultPropertyAttribute = TypeInfo.FindAttribute<XafDefaultPropertyAttribute>();
                    if (xafDefaultPropertyAttribute != null)
                    {
                        defaultPropertyName = xafDefaultPropertyAttribute.Name;
                    }
                    else
                    {
                        var defaultPropertyAttribute = ClassInfo.FindAttributeInfo(typeof(DefaultPropertyAttribute)) as DefaultPropertyAttribute;
                        if (defaultPropertyAttribute != null)
                        {
                            defaultPropertyName = defaultPropertyAttribute.Name;
                        }
                    }
                    if (!string.IsNullOrEmpty(defaultPropertyName))
                    {
                        _DefaultPropertyMemberInfoCol = new MemberInfoCollection(ClassInfo, defaultPropertyName, true, false);
                    }
                   isDefaultPropertyAttributeInit = true;
                }
                return _DefaultPropertyMemberInfoCol;
            }
        }

        /// <summary>
        /// Was it tried to store a mapping for the type in the corresponding ElasticSearch index
        /// </summary>
        public bool IsESMapped
        {
            get;
            set;
        }

        /// <summary>
        /// List of members who are to be reindexed on saving of instances of this tpye
        /// </summary>
        public ReadOnlyCollection<XPMemberInfo> ESReferences
        {
            get
            {
                if (!_IsESReferenceChecked)
                {
                    _IsESReferenceChecked = true;
                    if (ClassInfo != null && ((Model == null && Type.MembersWith<ElasticPropertyAttribute>(MemberTypes.All, Flags.AllMembers).Any()) || (ModelClass?.AllMembers.OfType<IModelMemberElasticSearch>().Any(t => !string.IsNullOrWhiteSpace(t.ElasticSearch?.FieldName)) ?? false)))
                    {
                        foreach (var op in ClassInfo.ObjectProperties)
                        {
                            var mi = op as XPMemberInfo;
                            if (mi != null && mi.IsAssociation)
                            {
                                var asmi = mi.GetAssociatedMember();
                                if (asmi != null && asmi.IsAggregated && GetBYteWareTypeInfo(asmi.Owner.ClassType).IsMemberESIndexed(asmi.Name))
                                {
                                    if (_ESReferences == null)
                                    {
                                        _ESReferences = new List<XPMemberInfo>();
                                    }
                                    _ESReferences.Add(mi);
                                }
                            }
                        }
                    }
                }
                return _ESReferences?.AsReadOnly();
            }
        }

        /// <summary>
        /// List of types where properties of this type's instances are used to construct their ElasticSearch document
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(XAF))]
        public ReadOnlyCollection<ContainingType> ContainingTypes
        {
            get
            {
                if (!_IsContainingTypesChecked)
                {
                    _IsContainingTypesChecked = true;
                    foreach (var ti in XafTypesInfo.Instance.PersistentTypes
                        .Where(t => !t.IsAbstract && !t.IsInterface && t.IsPersistent))
                    {
                        var ici = GetBYteWareTypeInfo(ti.Type);
                        if (ici.IsESIndexed)
                        {
                            foreach (var mi in ti.Members.Where(m => ((m.ListElementType != null && m.ListElementType.IsAssignableFrom(Type) && m.IsAssociation) || (m.IsPersistent && m.MemberType.IsAssignableFrom(Type))) && (ici.IsMemberESIndexed(m.Name) || ici.IsMemberESContaining(m.Name))))
                            {
                                if (_ContainingTypes == null)
                                {
                                    _ContainingTypes = new List<ContainingType>();
                                }
                                _ContainingTypes.Add(new ContainingType
                                {
                                    BYteWareType = ici,
                                    MemberInfo = mi,
                                    HasContainingSetting = ici.IsMemberESContaining(mi.Name)
                                });
                            }
                        }
                    }
                    if (_ContainingTypes != null)
                    {
                        for (int i = _ContainingTypes.Count - 1; i >= 0; i--)
                        {
                            if (_ContainingTypes.Any(t => t != _ContainingTypes[i] && t.MemberInfo.Name == _ContainingTypes[i].MemberInfo.Name && t.BYteWareType.Type.IsAssignableFrom(_ContainingTypes[i].BYteWareType.Type)))
                            {
                                _ContainingTypes.RemoveAt(i);
                            }
                        }
                    }
                }
                return _ContainingTypes?.AsReadOnly();
            }
        }

        /// <summary>
        /// List of all relevant ElasticSearch indexes for this type
        /// </summary>
        public ReadOnlyCollection<string> ESIndexes
        {
            get
            {
                if (IsESIndexed && _ESIndexes == null)
                {
                    _ESIndexes = new List<string>();
                    _ESTypes = new List<string>();
                    foreach (var descendant in TypeInfo.DescendantsAndSelf(t => t.Descendants))
                    {
                        var descci = GetBYteWareTypeInfo(descendant.Type);
                        if (!descci.ElasticIndexError && descci.ESIndexName.IsNotNullOrWhiteSpace() && descci.ESTypeName.IsNotNullOrWhiteSpace())
                        {
                            if (!_ESIndexes.Contains(descci.ESIndexName))
                            {
                                _ESIndexes.Add(descci.ESIndexName);
                            }
                            if (!_ESTypes.Contains(descci.ESTypeName))
                            {
                                _ESTypes.Add(descci.ESTypeName);
                            }
                        }
                        else
                        {
                            _ESIndexes.Clear();
                            _ESTypes.Clear();
                            break;
                        }
                    }
                }
                return _ESIndexes.AsReadOnly();
            }
        }

        /// <summary>
        /// List of all relevant ElasticSearch types for this type
        /// </summary>
        public ReadOnlyCollection<string> ESTypes
        {
            get
            {
                if (IsESIndexed && ESIndexes != null)
                {
                    return _ESTypes.AsReadOnly();
                }
                return null;
            }
        }

        /// <summary>
        /// Delivers only the topmost property infos
        /// </summary>
        /// <returns>list of all PropertyInfos</returns>
        public ReadOnlyCollection<PropertyInfo> GetTopPropertyInfos
        {
            get
            {
                var properties = new List<PropertyInfo>();
                var seenProps = new HashSet<string>();
                var tp = Type;
                while (tp != null)
                {
                    foreach (var prop in tp.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                    {
                        if (seenProps.Add(prop.Name))
                        {
                            properties.Add(prop);
                        }
                    }
                    tp = tp.BaseType;
                }
                return properties.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns an Enumeration of all ElasticSearch Field Names
        /// </summary>
        /// <param name="wildcards">Should possible wildcard names (only useable for search) also be returned</param>
        /// <returns>Enumeration of all ElasticSearch Field Names</returns>
        public IEnumerable<string> ESFields(bool wildcards)
        {
            if (IsESIndexed)
            {
                foreach (var p in GetTopPropertyInfos)
                {
                    var props = ESProperties(p.Name);
                    var propertyName = ElasticSearchClient.FieldName(string.IsNullOrEmpty(props?.FieldName) ? p.Name : props.FieldName);
                    if (props != null && !props.OptOut)
                    {
                        var etype = props.FieldType ?? ElasticSearchClient.GetFieldTypeFromType(p.PropertyType);
                        foreach (var item in ElasticSearchFieldsInternal(p, propertyName, etype, wildcards))
                        {
                            yield return item;
                        }
                        var multiFields = Model != null ? (props as IModelMemberElasticSearchField)?.Fields : Attribute.GetCustomAttributes(p, typeof(ElasticMultiFieldAttribute), true).OfType<IElasticSearchFieldProperties>();
                        foreach (var ga in multiFields.GroupBy(t => t.FieldName))
                        {
                            var a = ga.First();
                            foreach (var item in ElasticSearchFieldsInternal(p, a.FieldName.ToLowerInvariant(), a.FieldType ?? ElasticSearchClient.GetFieldTypeFromType(p.PropertyType), wildcards))
                            {
                                yield return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", propertyName, item);
                            }
                        }
                    }
                }
                yield return "_all";
            }
        }

        /// <summary>
        /// List of ElasticSearch suggest field informations
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(Elasticsearch))]
        public ReadOnlyCollection<SuggestField> ESSuggestFields
        {
            get
            {
                if (_SuggestFields == null)
                {
                    _SuggestFields = new List<SuggestField>();
                    foreach (var pi in GetTopPropertyInfos)
                    {
                        var props = ESProperties(pi.Name);
                        var fieldType = props?.FieldType ?? ElasticSearchClient.GetFieldTypeFromType(pi.PropertyType);
                        var fieldName = ElasticSearchClient.FieldName(string.IsNullOrEmpty(props?.FieldName) ? pi.Name : props.FieldName);
                        if (props != null && !props.OptOut)
                        {
                            if (fieldType == FieldType.completion)
                            {
                                var sf = new SuggestField
                                {
                                    FieldName = fieldName
                                };
                                _SuggestFields.Add(sf);
                                sf.Default = props.DefaultSuggestField;
                                if (!string.IsNullOrEmpty(props.WeightField))
                                {
                                    sf.WeightField = TypeInfo.FindMember(props.WeightField);
                                }
                                if (Model != null)
                                {
                                    sf.ContextSettings = new List<IElasticSearchSuggestContext>((props as IModelElasticSearchFieldProperties)?.SuggestContexts).AsReadOnly();
                                }
                                else
                                {
                                    sf.ContextSettings = new List<IElasticSearchSuggestContext>(Attribute.GetCustomAttributes(pi, typeof(ElasticSuggestContextAttribute), true).OfType<SuggestContextAttribute>()).AsReadOnly();
                                }
                            }
                            var multiFields = Model != null ? (props as IModelMemberElasticSearchField)?.Fields : Attribute.GetCustomAttributes(pi, typeof(ElasticMultiFieldAttribute), true).OfType<IElasticSearchFieldProperties>();
                            foreach (var multiField in multiFields.Where(t => t.FieldType == FieldType.completion))
                            {
                                var sf = new SuggestField
                                {
                                    FieldName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", fieldName, multiField.FieldName.ToLowerInvariant())
                                };
                                _SuggestFields.Add(sf);
                                sf.Default = multiField.DefaultSuggestField;
                                if (!string.IsNullOrEmpty(props.WeightField))
                                {
                                    sf.WeightField = TypeInfo.FindMember(props.WeightField);
                                }
                                if (Model != null)
                                {
                                    sf.ContextSettings = new List<IElasticSearchSuggestContext>((multiField as IModelElasticSearchFieldProperties)?.SuggestContexts).AsReadOnly();
                                }
                                else
                                {
                                    sf.ContextSettings = new List<IElasticSearchSuggestContext>(Attribute.GetCustomAttributes(pi, typeof(ElasticSuggestContextMultiFieldAttribute), true).OfType<ElasticSuggestContextMultiFieldAttribute>().Where(t => t.FieldName == multiField.FieldName)).AsReadOnly();
                                }
                            }
                            if (fieldType == FieldType.object_type || fieldType == FieldType.nested)
                            {
                                var nty = GetUnderlyingType(pi.PropertyType);
                                if (nty != null)
                                {
                                    var nci = GetBYteWareTypeInfo(nty);
                                    for (int i = 0; i < nci.ESSuggestFields.Count; i++)
                                    {
                                        var sf = new SuggestField();
                                        nci.ESSuggestFields[i].MapProperties(sf);
                                        sf.FieldName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", fieldName, nci.ESSuggestFields[i].FieldName);
                                        _SuggestFields.Add(sf);
                                    }
                                }
                            }
                        }
                    }
                }
                return _SuggestFields.AsReadOnly();
            }
        }

        /// <summary>
        /// Combined Filter statements in object permission entries of the security system for the Type
        /// </summary>
        public string ESSecurityFilter
        {
            get
            {
                if (!_IsSecurityFilterChecked && SecuritySystem.Instance?.User != null)
                {
                    _ESSecurityFilter = ElasticSearchClient.SecurityFilter(SecuritySystem.Instance.User, Type);
                    _IsSecurityFilterChecked = true;
                }
                return _ESSecurityFilter;
            }
        }

        /// <summary>
        /// Returns the ElasticSearchAttribute instance for the type if one was defined
        /// </summary>
        public ElasticSearchAttribute ESAttribute
        {
            get
            {
                if (_ElasticSearchAttribute == null && !_SearchedElasticSearchAttribute)
                {
                    _SearchedElasticSearchAttribute = true;
                    var attrs = Type.GetCustomAttributes(typeof(ElasticSearchAttribute), true);
                    if (attrs.Length > 0)
                    {
                        _ElasticSearchAttribute = attrs[0] as ElasticSearchAttribute;
                    }
                }
                return _ElasticSearchAttribute;
            }
        }

        /// <summary>
        /// Does the type identified by xci partake in any ElasticSearch index
        /// </summary>
        /// <returns>True if the type is part of any ElasticSearch index; otherwise False</returns>
        public bool IsPartOfAnyESIndex
        {
            get
            {
                return ((Model == null && ESAttribute != null) || (ModelClass as IModelClassElasticSearch)?.ElasticSearchIndex != null) || ESReferences != null || ContainingTypes != null;
            }
        }

        /// <summary>
        /// Is the type indexed with ElasticSearch
        /// </summary>
        /// <returns>True if the type is indexed in an ElasticSearch index; otherwise False</returns>
        public bool IsESIndexed
        {
            get
            {
                return (Model == null && ESAttribute != null) || (ModelClass as IModelClassElasticSearch)?.ElasticSearchIndex != null;
            }
        }

        /// <summary>
        /// Is the Member indexed with ElasticSearch
        /// </summary>
        /// <param name="member">Name of the member</param>
        /// <returns>True if the member is indexed in an ElasticSearch index; otherwise False</returns>
        public bool IsMemberESIndexed(string member)
        {
            return (Model == null && (ClassInfo?.FindMember(member)?.HasAttribute(typeof(ElasticPropertyAttribute)) ?? false)) || !string.IsNullOrWhiteSpace((ModelClass?.AllMembers.FirstOrDefault(t => t.Name == member) as IModelMemberElasticSearch)?.ElasticSearch.FieldName);
        }

        /// <summary>
        /// Is the Member marked as contained in an ElasticSearch index
        /// </summary>
        /// <param name="member">Name of the member</param>
        /// <returns>True if the member is marked as contained in an ElasticSearch index; otherwise False</returns>
        public bool IsMemberESContaining(string member)
        {
            return (Model == null && (ClassInfo?.FindMember(member)?.HasAttribute(typeof(ElasticContainingAttribute)) ?? false)) || (ModelClass?.AllMembers.FirstOrDefault(t => t.Name == member) as IModelMemberElasticSearch).Containing;
        }

        /// <summary>
        /// Returns the defined ElasticSearch Index Name
        /// </summary>
        /// <returns>Name for the ElasticSearch Index</returns>
        public string ESIndexName
        {
            get
            {
                return Model == null ? ESAttribute?.IndexName : (ModelClass as IModelClassElasticSearch)?.ElasticSearchIndex?.Name;
            }
        }

        /// <summary>
        /// Returns the defined ElasticSearch Type Name
        /// </summary>
        /// <returns>Name for the ElasticSearch Type</returns>
        public string ESTypeName
        {
            get
            {
                var tname = Model == null ? ESAttribute?.TypeName : (ModelClass as IModelClassElasticSearch)?.TypeName;
                return string.IsNullOrEmpty(tname) ? Type.Name : tname;
            }
        }

        /// <summary>
        /// Disables the storage of the source Json string
        /// </summary>
        /// <returns>True if the storage of the source Json string should be disabled; otherwise False</returns>
        public bool? ESSourceFieldDisabled
        {
            get
            {
                return Model == null ? ESAttribute?.SourceFieldDisabled : (ModelClass as IModelClassElasticSearch)?.SourceFieldDisabled;
            }
        }

        /// <summary>
        /// Returns the defined ElasticSearch Index settings
        /// </summary>
        /// <returns>Index Settings Json string</returns>
        public string ESIndexSettings
        {
            get
            {
                var esIndex = (ModelClass as IModelClassElasticSearch)?.ElasticSearchIndex;
                if (esIndex != null && (!string.IsNullOrWhiteSpace(esIndex.Settings) || !string.IsNullOrWhiteSpace(esIndex.Analyzers)))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("{");
                    sb.AppendLine("\"setings\": {");
                    if (!string.IsNullOrWhiteSpace(esIndex.Settings))
                    {
                        sb.AppendLine(esIndex.Settings);
                        if (!string.IsNullOrWhiteSpace(esIndex.Analyzers))
                        {
                            sb.AppendLine(",");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(esIndex.Analyzers))
                    {
                        sb.AppendLine("\"analysis\": {");
                        sb.AppendLine("\"analyzer\": [");
                        sb.AppendLine(esIndex.Analyzers);
                        sb.AppendLine("]");
                        if (!string.IsNullOrWhiteSpace(esIndex.Tokenizers))
                        {
                            sb.AppendLine(",");
                            sb.AppendLine("\"tokenizer\": [");
                            sb.AppendLine(esIndex.Tokenizers);
                            sb.AppendLine("]");
                        }
                        if (!string.IsNullOrWhiteSpace(esIndex.CharFilters))
                        {
                            sb.AppendLine(",");
                            sb.AppendLine("\"char_filter\": [");
                            sb.AppendLine(esIndex.CharFilters);
                            sb.AppendLine("]");
                        }
                        if (!string.IsNullOrWhiteSpace(esIndex.TokenFilters))
                        {
                            sb.AppendLine(",");
                            sb.AppendLine("\"filter\": [");
                            sb.AppendLine(esIndex.TokenFilters);
                            sb.AppendLine("]");
                        }
                        sb.AppendLine("}");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine("}");
                }
                return null;
            }
        }

        /// <summary>
        /// Get the ElasticSearch Field Properties
        /// </summary>
        /// <param name="member">Member Name</param>
        /// <returns>ElasticSearch Field Properties</returns>
        public IElasticProperties ESProperties(string member)
        {
            if (Model == null)
            {
                return ClassInfo?.FindMember(member)?.FindAttributeInfo(typeof(ElasticPropertyAttribute)) as IElasticProperties;
            }
            else
            {
                var props = (ModelClass?.AllMembers.FirstOrDefault(t => t.Name == member) as IModelMemberElasticSearch)?.ElasticSearch;
                return !string.IsNullOrWhiteSpace(props?.FieldName) ? props : null;
            }
        }

        /// <summary>
        /// Did an error occur with any ElasticSearch operation regarding this type
        /// </summary>
        public bool ElasticIndexError
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the Version number of XPO BusinessClass instance bo for ElasticSearch
        /// </summary>
        /// <param name="bo">XPO BusinessClass instance bo</param>
        /// <returns>Version number for ElasticSearch</returns>
        public int GetVersion(PersistentBase bo)
        {
            return (int)PersistentBase.GetCustomPropertyStore(bo).GetCustomPropertyValue(ClassInfo.OptimisticLockField) + 1;
        }

        /// <summary>
        /// Override ToString for better debug experience
        /// </summary>
        /// <returns>Name of the Type</returns>
        public override string ToString()
        {
            return Type.Name;
        }

        private IEnumerable<string> ElasticSearchFieldsInternal(PropertyInfo pi, string propertyName, FieldType etype, bool wildcards)
        {
            if (etype != FieldType.completion)
            {
                if (etype == FieldType.object_type || etype == FieldType.nested)
                {
                    var nty = GetUnderlyingType(pi.PropertyType);
                    var nestFields = ESFields(wildcards);
                    foreach (var field in nestFields)
                    {
                        yield return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", propertyName, field);
                    }
                    if (wildcards)
                    {
                        yield return string.Format(CultureInfo.InvariantCulture, "{0}.*", propertyName);
                    }
                }
                else
                {
                    yield return propertyName;
                }
            }
        }
    }
}