namespace BYteWare.XAF.ElasticSearch.Controllers
{
    using BYteWare.Utils.Extension;
    using DevExpress.Data.Filtering;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Actions;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Xpo;
    using DevExpress.Persistent.Base;
    using ElasticSearch;
    using ElasticSearch.BusinessObjects;
    using ElasticSearch.Response;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch Filter Controller
    /// </summary>
    [CLSCompliant(false)]
    public class ElasticSearchFilterController : FilterController
    {
        /// <summary>
        /// Reason used when disabling actions if ElasticSearch wasn't available
        /// </summary>
        public const string NoElasticSearchReason = "NoElasticSearch";

        private const string NoFilterFieldsReason = "NoFilterFields";
        private const string IsInLookupReason = "IsInLookup";

        private static readonly CriteriaOperator _FalseCriteria = CriteriaOperator.Parse("1=0");
        private readonly SingleChoiceAction filterFieldsAction;
        private readonly SingleChoiceAction lookupSetFilterAction;
        private readonly SingleChoiceAction lookupFilterFieldsAction;
        private readonly Dictionary<string, int> scores = new Dictionary<string, int>();
        private bool isElasticSearchAvailable;
        private bool lastSearchElastic;
        private bool _ElasticCanFuzzy;
        private string lastSearchText;

        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticSearchFilterController"/> class.
        /// </summary>
        public ElasticSearchFilterController()
        {
#pragma warning disable CC0009 // Use object initializer
            SingleChoiceAction tempAction = null;
            try
            {
                tempAction = new SingleChoiceAction(this, "FilterFields", PredefinedCategory.FullTextSearch);
                tempAction.Caption = "Search in";
                tempAction.ImageName = "Action_ParametrizedAction";
                tempAction.Execute += FilterFieldsActionExecute;
                filterFieldsAction = tempAction;
                tempAction = null;
            }
            finally
            {
                if (tempAction != null)
                {
                    tempAction.Dispose();
                }
            }

            tempAction = null;
            try
            {
                tempAction = new SingleChoiceAction(this, "LookupFilterFields", "ElasticActionContainer");
                tempAction.Caption = "Search in";
                tempAction.ImageName = "Action_ParametrizedAction";
                tempAction.Execute += FilterFieldsActionExecute;
                lookupFilterFieldsAction = tempAction;
                tempAction = null;
            }
            finally
            {
                if (tempAction != null)
                {
                    tempAction.Dispose();
                }
            }

            tempAction = null;
            try
            {
                tempAction = new SingleChoiceAction(this, "LookupSetFilter", "ElasticActionContainer");
                tempAction.Caption = "Filter";
                tempAction.ImageName = "MenuBar_Filter";
                tempAction.Execute += LookupSetFilterAction_Execute;
                lookupSetFilterAction = tempAction;
                tempAction = null;
            }
            finally
            {
                if (tempAction != null)
                {
                    tempAction.Dispose();
                }
            }
#pragma warning restore CC0009 // Use object initializer
        }

        /// <summary>
        /// Action to select the fields to search in
        /// </summary>
        public SingleChoiceAction FilterFieldsAction
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum Number of results which are returned from an ElasticSearch query
        /// </summary>
        public int ElasticSearchResults
        {
            get;
            set;
        }
        = 200;

        /// <summary>
        /// Refresh the ObjectSpace before a search
        /// ElasticSearch acts like at ReadCommitted isolation level, which could be a newer state than the results in the current ObjectSpace if objects got loaded from prior searches
        /// </summary>
        public bool RefreshBeforeSearch
        {
            get;
            set;
        }

        /// <summary>
        /// Was the last search performed with ElasticSearch
        /// </summary>
        public bool LastSearchElastic
        {
            get
            {
                return lastSearchElastic;
            }
        }

        /// <summary>
        /// Is ElasticSearch for the type of the View avaiable
        /// </summary>
        public bool IsElasticSearchAvailable
        {
            get
            {
                return isElasticSearchAvailable;
            }
        }

        /// <summary>
        /// Can the actual search be performed as a fuzzy search
        /// </summary>
        public bool ElasticCanFuzzy
        {
            get
            {
                return _ElasticCanFuzzy;
            }
        }

        /// <summary>
        /// Event to customize the query which is sent to ElasticSearch
        /// </summary>
        public event EventHandler<CustomSearchEventArgs> CustomSearch;

        /// <summary>
        /// Initializes the FilterFieldsAction from the model settings of the view
        /// </summary>
        public virtual void SetupFilterFields()
        {
            FilterFieldsAction.BeginUpdate();
            FilterFieldsAction.Items.Clear();
            var filters = GetFilterFieldsNode();
            if (filters != null)
            {
                var currentFilterFieldsId = string.Empty;
                if (filters.Any() && filters.DefaultElasticSearchFields != null)
                {
                    currentFilterFieldsId = filters.DefaultElasticSearchFields.Id;
                }
                foreach (IModelElasticSearchFieldsItem filterItem in filters)
                {
                    var item = new ChoiceActionItem(filterItem);
                    FilterFieldsAction.Items.Add(item);
                    if (item.Id == currentFilterFieldsId)
                    {
                        FilterFieldsAction.SelectedItem = item;
                    }
                }
            }
            if (!FilterFieldsAction.Items.Any())
            {
                FilterFieldsAction.Active.SetItemValue(NoFilterFieldsReason, false);
            }
            else
            {
                FilterFieldsAction.Active.SetItemValue(NoFilterFieldsReason, true);
                if (FilterFieldsAction.SelectedIndex < 0)
                {
                    FilterFieldsAction.SelectedIndex = 0;
                }
            }
            FilterFieldsAction.EndUpdate();
        }

        /// <summary>
        /// Event Handler which is called on Execute of the FilterFieldsAction
        /// </summary>
        /// <param name="sender">The sender of the event, usually the FilterFieldsAction</param>
        /// <param name="e">A SingleChoiceActionExecuteEventArgs instance</param>
        public void FilterFieldsActionExecute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (lastSearchElastic && !string.IsNullOrWhiteSpace(lastSearchText))
            {
                FullTextSearch(lastSearchText);
            }
        }

        /// <summary>
        /// Returns an awaitable enumeration of suggestion strings for the search string
        /// </summary>
        /// <param name="search">The string to search suggestions for</param>
        /// <returns>An awaitable enumeration of suggestion strings</returns>
        public virtual async Task<IEnumerable<string>> SuggestAsync(string search)
        {
            if (IsElasticSearchAvailable && View != null)
            {
                IModelElasticSearchFieldsItem item = null;
                if (FilterFieldsAction.Active && FilterFieldsAction.SelectedItem != null && FilterFieldsAction.SelectedItem.Model != null)
                {
                    item = FilterFieldsAction.SelectedItem.Model as IModelElasticSearchFieldsItem;
                }
                return await ElasticSearchClient.Instance.SuggestAsync(search, View.ObjectTypeInfo, item).ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Filter the list view with a full text search on the string search
        /// </summary>
        /// <param name="search">The string to search for</param>
        public virtual void FullTextSearch(string search)
        {
            FullTextSearch(new ParametrizedActionExecuteEventArgs(FullTextFilterAction, FullTextFilterAction.SelectionContext, search));
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complex?")]
        protected override void FullTextSearch(ParametrizedActionExecuteEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            WaitScreen.Instance.Show(CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "FullTextSearchWaitScreenCaption"), CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "FullTextSearchWaitScreenText"));
            try
            {
                if (RefreshBeforeSearch)
                {
                    ObjectSpace.Refresh();
                }
                lastSearchElastic = false;
                if (IsElasticSearchAvailable)
                {
                    var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(View.ObjectTypeInfo.Type);
                    if (!ci.ElasticIndexError)
                    {
                        var searchText = args.ParameterCurrentValue == null ? string.Empty : args.ParameterCurrentValue.ToString();
                        bool fuzzy;
                        bool wildcard;
                        searchText = ElasticSearchClient.PrepareSearchText(searchText, out fuzzy, out wildcard);
                        var filter = string.Empty;
                        if (SetFilterAction.Active && SetFilterAction.SelectedItem != null && SetFilterAction.SelectedItem.Model != null)
                        {
                            var filterExtension = SetFilterAction.SelectedItem.Model as IModelListViewFilterItemElasticSearch;
                            if (filterExtension != null)
                            {
                                filter = filterExtension.ElasticSearchFilter ?? string.Empty;
                            }
                        }
                        var customSearchEventArgs = new CustomSearchEventArgs(
                            args.ParameterCurrentValue == null ? string.Empty : args.ParameterCurrentValue.ToString(),
                            ci.ESIndexes.ToArray(),
                            ci.ESTypes.ToArray(),
                            filter,
                            ci.ESSecurityFilter);
                        SearchOptions(searchText, fuzzy, wildcard, customSearchEventArgs);
                        lastSearchElastic = customSearchEventArgs.Handled;
                        if (!lastSearchElastic)
                        {
                            var hits = ElasticSearchClient.Instance.Search(customSearchEventArgs.Indexes, customSearchEventArgs.Types, customSearchEventArgs.Json);
                            if (hits != null)
                            {
                                _ElasticCanFuzzy = (!wildcard && !fuzzy) || (wildcard && !searchText.EveryWordContains("~"));
                                if (hits.Total == 0 && _ElasticCanFuzzy && !customSearchEventArgs.Retry)
                                {
                                    hits = DoFuzzySearch(customSearchEventArgs, wildcard, searchText);
                                }
                                if (!lastSearchElastic && hits != null)
                                {
                                    View.CollectionSource.CollectionReloaded -= CollectionSource_CollectionReloaded;
                                    View.CollectionSource.CollectionChanged -= CollectionSource_CollectionReloaded;
                                    SaveScores(hits);
                                    if (View.CollectionSource.List.Count <= 0 && _ElasticCanFuzzy && !customSearchEventArgs.Retry)
                                    {
                                        hits = DoFuzzySearch(customSearchEventArgs, wildcard, searchText);
                                        if (hits != null)
                                        {
                                            SaveScores(hits);
                                        }
                                    }
                                    if (hits != null)
                                    {
                                        if (!View.CollectionSource.IsServerMode)
                                        {
                                            CollectionSource_CollectionReloaded(View.CollectionSource, null);
                                            View.CollectionSource.CollectionReloaded += CollectionSource_CollectionReloaded;
                                            View.CollectionSource.CollectionChanged += CollectionSource_CollectionReloaded;
                                        }
                                        lastSearchElastic = true;
                                        SetFilterAction.ToolTip = GetToolTip();
                                    }
                                }
                            }
                        }
                    }
                }
                if (!lastSearchElastic)
                {
                    if (FilterFieldsAction.Active)
                    {
                        FilterFieldsAction.Active.SetItemValue(NoElasticSearchReason, false);
                    }
                    scores.Clear();
                    base.FullTextSearch(args);
                }
                else
                {
                    if (!FilterFieldsAction.Active[NoElasticSearchReason])
                    {
                        FilterFieldsAction.Active.SetItemValue(NoElasticSearchReason, true);
                    }
                }
            }
            finally
            {
                WaitScreen.Instance.Hide();
            }
        }

        /// <inheritdoc/>
        protected override string GetToolTip()
        {
            if (lastSearchElastic)
            {
                return lastSearchText;
            }
            else
            {
                return SetFilterAction.SelectedItem != null ? SetFilterAction.SelectedItem.Caption : string.Empty;
            }
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();
            var model = (IModelListViewElasticSearchFilterSettings)View.Model;
            if (model != null)
            {
                ElasticSearchResults = model.ElasticSearchResults;
                RefreshBeforeSearch = model.RefreshBeforeSearch;
            }
            if (Frame != null && Frame.Context == TemplateContext.LookupControl)
            {
                FilterFieldsAction = lookupFilterFieldsAction;
            }
            else
            {
                FilterFieldsAction = filterFieldsAction;
            }
            View.ModelChanged += View_ModelChanged;
            isElasticSearchAvailable = false;
            var ti = View.ObjectTypeInfo;
            var os = ObjectSpace as XPObjectSpace;
            if (os != null && ElasticSearchClient.Instance.IsElasticSearchAvailable(ti))
            {
                var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                if (ci.ESIndexes.All(t => ElasticSearchClient.Instance.IsIndexActive(t, os.Session)))
                {
                    isElasticSearchAvailable = true;
                }
            }
            FilterFieldsAction.Active.SetItemValue(NoElasticSearchReason, IsElasticSearchAvailable);
            SetupFilterFields();
        }

        /// <inheritdoc/>
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var model = (IModelListViewElasticSearchFilterSettings)View.Model;
            if (model != null && model.OnlyLoadWhenFullTextFilter && ReferenceEquals(View.CollectionSource.Criteria[FullTextSearchCriteriaName], null))
            {
                View.CollectionSource.Criteria[FullTextSearchCriteriaName] = _FalseCriteria;
            }
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            if (View != null)
            {
                View.ModelChanged -= View_ModelChanged;
                if (View.CollectionSource != null)
                {
                    View.CollectionSource.CollectionReloaded -= CollectionSource_CollectionReloaded;
                    View.CollectionSource.CollectionChanged -= CollectionSource_CollectionReloaded;
                }
            }
            base.OnDeactivated();
        }

        /// <summary>
        /// Invokes a Custom Search event handler
        /// </summary>
        /// <param name="args">A CustomSearchEventArgs instance</param>
        protected virtual void OnCustomSearch(CustomSearchEventArgs args)
        {
            CustomSearch?.Invoke(this, args);
        }

        /// <inheritdoc/>
        protected override void SetupFilters()
        {
            base.SetupFilters();
            lookupSetFilterAction.Items.Clear();
            lookupSetFilterAction.Items.AddRange(SetFilterAction.Items);
            lookupSetFilterAction.Active.SetItemValue("SetFilterAction", SetFilterAction.Active);
            lookupSetFilterAction.SelectedItem = SetFilterAction.SelectedItem;
        }

        /// <inheritdoc/>
        protected override void UpdateActionState()
        {
            base.UpdateActionState();
            if (Frame != null && Frame.Context == TemplateContext.LookupControl)
            {
                SetFilterAction.Active.SetItemValue("Criteria locked", true);
                lookupSetFilterAction.Active.SetItemValue("SetFilterAction", SetFilterAction.Active);
                lookupSetFilterAction.Active.SetItemValue(IsInLookupReason, true);
                lookupFilterFieldsAction.Active.SetItemValue(IsInLookupReason, true);
            }
        }

        private HitsMetaData DoFuzzySearch(CustomSearchEventArgs customSearchEventArgs, bool wildcard, string searchText)
        {
            HitsMetaData hits = null;
            customSearchEventArgs.Retry = true;
            if (wildcard)
            {
                searchText = searchText.AddStringToWordEnd("~");
            }
            SearchOptions(searchText, true, wildcard, customSearchEventArgs);
            lastSearchElastic = customSearchEventArgs.Handled;
            _ElasticCanFuzzy = false;
            if (!lastSearchElastic)
            {
                hits = ElasticSearchClient.Instance.Search(customSearchEventArgs.Indexes, customSearchEventArgs.Types, customSearchEventArgs.Json);
            }
            return hits;
        }

        private void SaveScores(HitsMetaData hits)
        {
            scores.Clear();
            var guids = new List<string>((int)hits.Total);
            var i = 1;
            foreach (var hit in hits.Hits)
            {
                if (!scores.ContainsKey(hit.Id))
                {
                    guids.Add(hit.Id);
                    scores.Add(hit.Id, i++);
                }
            }
            View.CollectionSource.Criteria[FullTextSearchCriteriaName] = new InOperator(View.ObjectTypeInfo.KeyMember.Name, guids);
        }

        private void View_ModelChanged(object sender, EventArgs e)
        {
            SetupFilterFields();
        }

        private IModelElasticSearchFieldsList GetFilterFieldsNode()
        {
            IModelElasticSearchFieldsList filters = null;
            if (View != null && View.Model != null)
            {
                var filterSettings = View.Model as IModelElasticSearchFilterSettings;
                if (filterSettings != null)
                {
                    filters = filterSettings.ElasticSearchFieldsList;
                }
                if (filters == null || !filters.Any())
                {
                    filterSettings = View.Model.ModelClass as IModelElasticSearchFilterSettings;
                    if (filterSettings != null)
                    {
                        filters = filterSettings.ElasticSearchFieldsList;
                    }
                }
            }
            return filters;
        }

        private void SearchOptions(string searchText, bool fuzzy, bool wildcard, CustomSearchEventArgs customSearchEventArgs)
        {
            IModelElasticSearchFieldsItem item = null;
            if (FilterFieldsAction.Active && FilterFieldsAction.SelectedItem != null && FilterFieldsAction.SelectedItem.Model != null)
            {
                item = FilterFieldsAction.SelectedItem.Model as IModelElasticSearchFieldsItem;
            }
            customSearchEventArgs.Json = ElasticSearchClient.Instance.SearchBody(searchText, ElasticSearchResults, fuzzy, wildcard, customSearchEventArgs.Filter, customSearchEventArgs.SecurityFilter, item);
            OnCustomSearch(customSearchEventArgs);
            lastSearchText = customSearchEventArgs.SearchText;
        }

        private void CollectionSource_CollectionReloaded(object sender, EventArgs e)
        {
            var col = sender as CollectionSourceBase;
            if (col != null && !scores.IsNullOrEmpty())
            {
                var list = new List<ISearchPosition>(col.List.OfType<ISearchPosition>());
                foreach (var bo in list)
                {
                    if (bo != null)
                    {
                        int pos;
                        if (scores.TryGetValue(bo.Session.GetKeyValue(bo).ToString(), out pos))
                        {
                            bo.SearchPosition = pos;
                        }
                    }
                }
            }
        }

        private void LookupSetFilterAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            SetFilterAction.DoExecute(e.SelectedChoiceActionItem);
        }
    }
}