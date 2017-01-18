namespace BYteWare.XAF.ElasticSearch
{
    using BusinessObjects;
    using Controllers;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Actions;
    using DevExpress.ExpressApp.Design;
    using DevExpress.ExpressApp.Editors;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Model.Core;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Updating;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Xpo;
    using DevExpress.Persistent.Base;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// XAF ElasticSearch Base Module
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(XAF))]
    [CLSCompliant(false)]
    [ToolboxItem(true)]
    /*[ToolboxBitmap(typeof(BYteWareModule), "Logo_16x16.bmp")]
    [ToolboxItemFilter("Xaf.Platform.Win")]*/
    [Description("BYteWare ElasticSearch XAF Base Module")]
    public sealed class ElasticSearchModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSearchModule" /> class.
        /// </summary>
        public ElasticSearchModule()
        {
        }

        /// <inheritdoc/>
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            BYteWareTypeInfo.Model = Application.Model;
            application.LoggingOff += Application_LoggingOff;
            application.Disposed += Application_Disposed;
            application.SetupComplete += Application_SetupComplete;
            application.ModelChanged += Application_ModelChanged;
        }

        /// <summary>
        /// An XPO BusinessClass Type which implements the IElasticSearchIndex interface for storing the state of the ElasticSearch indexes
        /// </summary>
        [TypeConverter(typeof(BusinessClassTypeConverter<IElasticSearchIndex>))]
        public Type ElasticSearchIndexPersistentType
        {
            get;
            set;
        }

        /// <summary>
        /// An XPO BusinessClass Type which implements the IElasticSearchIndexRefresh interface to store if an ElasticSearch index should be refreshed
        /// </summary>
        [TypeConverter(typeof(BusinessClassTypeConverter<IElasticSearchIndexRefresh>))]
        public Type ElasticSearchIndexRefreshPersistentType
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            return ModuleUpdater.EmptyModuleUpdaters;
        }

        /// <inheritdoc/>
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            if (extenders == null)
            {
                throw new ArgumentNullException(nameof(extenders));
            }
            extenders.Add<IModelApplication, IModelApplicationElasticSearch>();
            extenders.Add<IModelClass, IModelElasticSearchFilterSettings>();
            extenders.Add<IModelClass, IModelClassElasticSearch>();
            extenders.Add<IModelMember, IModelMemberElasticSearch>();
            extenders.Add<IModelListView, IModelListViewElasticSearchFilterSettings>();
            extenders.Add<IModelListViewFilterItem, IModelListViewFilterItemElasticSearch>();
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = nameof(ElasticSearch))]
        public override IList<PopupWindowShowAction> GetStartupActions()
        {
            try
            {
                ElasticSearchClient.Instance?.RefreshNeccessaryIndexes(Application, p =>
                {
                    Application.UpdateStatus("ElasticSearch RefreshIndexes", p.Name, string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "RefreshProgress"), p.Phase, p.Position, p.Maximum, p.Position / (double)p.Maximum));
                });
            }
            catch (Exception e)
            {
                Tracing.Tracer.LogError(CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "RefreshLogError"));
                Tracing.Tracer.LogError(e);
            }
            return base.GetStartupActions();
        }

        /// <inheritdoc/>
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters?.Add(new ModelClassGeneratorUpdater());
            updaters?.Add(new ModelMemberGeneratorUpdater());
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(XAF))]
        protected override IEnumerable<Type> GetRegularTypes()
        {
            return new Type[]
            {
                typeof(IModelApplicationElasticSearch),
                typeof(IModelClassElasticSearch),
                typeof(IModelElasticSearch),
                typeof(IModelElasticSearchField),
                typeof(IModelElasticSearchFieldList),
                typeof(IModelElasticSearchFieldProperties),
                typeof(IModelElasticSearchFieldsItem),
                typeof(IModelElasticSearchFieldsList),
                typeof(IModelElasticSearchFilterSettings),
                typeof(IModelElasticSearchIndex),
                typeof(IModelElasticSearchIndexes),
                typeof(IModelElasticSearchSuggestContext),
                typeof(IModelElasticSearchSuggestContextList),
                typeof(IModelElasticSearchSuggestField),
                typeof(IModelElasticSearchSuggestFieldList),
                typeof(IModelListViewElasticSearchFilterSettings),
                typeof(IModelListViewFilterItemElasticSearch),
                typeof(IModelMemberElasticSearch),
                typeof(IElasticSearchFieldProperties),
                typeof(IElasticProperties),
                typeof(IModelMemberElasticSearchField),
                typeof(IModelMemberElasticSearchFields),
                typeof(IElasticSearchSuggestContext),
                typeof(IModelMemberElasticSearchSuggestContext),
                typeof(IModelMemberElasticSearchSuggestContexts),
                typeof(ModelClassElasticSearchLogic),
                typeof(ModelElasticSearchFieldLogic),
                typeof(ModelElasticSearchFieldsItemLogic),
                typeof(ModelElasticSearchFieldsListLogic),
                typeof(ModelElasticSearchSuggestContextLogic),
                typeof(ModelElasticSearchSuggestFieldLogic),
                typeof(ModelListViewFilterItemElasticSearchLogic),
                typeof(ModelMemberElasticSearchFieldLogic),
                typeof(ModelMemberElasticSearchSuggestContextLogic),
            };
        }

        /// <inheritdoc/>
        protected override IEnumerable<Type> GetDeclaredExportedTypes()
        {
            var tl = new List<Type>();
            if (ElasticSearchIndexPersistentType != null)
            {
                tl.Add(ElasticSearchIndexPersistentType);
            }
            if (ElasticSearchIndexRefreshPersistentType != null)
            {
                tl.Add(ElasticSearchIndexRefreshPersistentType);
            }
            return tl;
        }

        /// <inheritdoc/>
        protected override IEnumerable<Type> GetDeclaredControllerTypes()
        {
            return new Type[]
            {
                typeof(ElasticIndexController),
                typeof(ElasticSearchFilterController),
                typeof(ObjectPermissionController),
            };
        }

        /// <inheritdoc/>
        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory)
        {
        }

        private static Type FindIndexType()
        {
            return XafTypesInfo.Instance.PersistentTypes.FirstOrDefault(p => p.Implements<IElasticSearchIndex>())?.Type;
        }

        private static Type FindRefreshType(Type elasticSearchIndexType)
        {
            if (elasticSearchIndexType != null)
            {
                var ti = XafTypesInfo.Instance.FindTypeInfo(elasticSearchIndexType);
                return ti?.Members.FirstOrDefault(t => t.ListElementType != null && typeof(IElasticSearchIndexRefresh).IsAssignableFrom(t.ListElementType))?.ListElementType;
            }
            return null;
        }

        private void Application_SetupComplete(object sender, EventArgs e)
        {
            var app = (XafApplication)sender;
            var et = ElasticSearchIndexPersistentType ?? FindIndexType();
            ElasticSearchClient.Instance = new ElasticSearchClient(et, ElasticSearchIndexRefreshPersistentType ?? FindRefreshType(et));
            app.ObjectSpaceCreated += ElasticSearchModule_ObjectSpaceCreated;
            var elasticSearchNodes = ConfigurationManager.AppSettings["ElasticSearchNodes"];
            var elasticSearchIndexPrefix = ConfigurationManager.AppSettings["ElasticSearchIndexPrefix"];
            ElasticSearchClient.Instance.ElasticSearchNodes.Clear();
            if (!string.IsNullOrWhiteSpace(elasticSearchNodes))
            {
                foreach (var node in elasticSearchNodes.Split(';'))
                {
                    ElasticSearchClient.Instance.ElasticSearchNodes.Add(new Uri(node));
                }
            }
            ElasticSearchClient.Instance.ElasticSearchIndexPrefix = string.Empty;
            if (elasticSearchIndexPrefix != null)
            {
                ElasticSearchClient.Instance.ElasticSearchIndexPrefix = Environment.ExpandEnvironmentVariables(elasticSearchIndexPrefix);
            }
        }

        private void ElasticSearchModule_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e)
        {
            ElasticSearchClient.Instance?.RegisterObjectSpace(e.ObjectSpace as XPObjectSpace);
        }

        private void Application_LoggingOff(object sender, LoggingOffEventArgs e)
        {
            BYteWareTypeInfo.LoggingOff();
        }

        private void Application_ModelChanged(object sender, EventArgs e)
        {
            BYteWareTypeInfo.ModelRefresh();
        }

        private void Application_Disposed(object sender, EventArgs e)
        {
            var application = sender as XafApplication;
            if (application != null)
            {
                application.Disposed -= Application_Disposed;
                application.ObjectSpaceCreated -= ElasticSearchModule_ObjectSpaceCreated;
            }
        }
    }
}
