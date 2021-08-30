namespace BYteWare.XAF.ElasticSearch.Win.Controller
{
    using BYteWare.Utils.Extension;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Actions;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Win.Templates;
    using DevExpress.ExpressApp.Win.Templates.ActionContainers;
    using DevExpress.XtraEditors;
    using DevExpress.XtraGrid;
    using ElasticSearch.Controllers;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using Forms = System.Windows.Forms;

    /// <summary>
    /// View Controller that displays a Filter Panel in ListViews
    /// </summary>
    [CLSCompliant(false)]
    public class FilterPanelListViewController : ViewController<ListView>
    {
        /// <summary>
        /// Action Container Category Name for the Filter Panel
        /// </summary>
        public const string FilterPanelGroup = "FilterPanel";

        /// <summary>
        /// Reason that is used to deactivate Actions when a fuzzy search ins't available
        /// </summary>
        public const string FuzzySearchReason = "FuzzySearch";

        /// <summary>
        /// Event that is called after the Filter Panel with it's Action Container was created
        /// </summary>
        public event EventHandler FilterControlCreated;

        private readonly SingleChoiceAction newFilterFieldsAction;
        private readonly ParametrizedAction searchTextAction;
        private readonly SimpleAction fuzzySearchAction;
        private readonly Forms.Timer searchTextTimer;
        private GridControl _GridControl;
        private PanelControl _FilterPanel;
        private ButtonsContainer buttonsContainer;
        private ElasticSearchFilterController filterController;
        private FillActionContainersController _FillActionContainersController;
        private SingleChoiceAction oldFilterFieldsAction;
        private PropertyEditor.ButtonsContainersParametrizedActionComboItem searchTextActionItem;
        private bool _canUpdate = true;
        private bool _needUpdate = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterPanelListViewController"/> class.
        /// </summary>
        public FilterPanelListViewController()
        {
            ParametrizedAction tempAction = null;
            try
            {
                tempAction = new ParametrizedAction(this, "FilterPanelSearchEdit", FilterPanelGroup, typeof(string))
                {
                    Caption = "Search",
                    ToolTip = "Search entries that contain the specified words",
                    NullValuePrompt = "Text to search",
                    PaintStyle = ActionItemPaintStyle.Image
                };
                tempAction.Execute += SearchTextActionExecute;
                searchTextAction = tempAction;
                tempAction = null;
            }
            finally
            {
                if (tempAction != null)
                {
                    tempAction.Dispose();
                }
            }

            SimpleAction tempSimpleAction = null;
            try
            {
                tempSimpleAction = new SimpleAction(this, "FilterPanelFuzzySearch", FilterPanelGroup)
                {
                    Caption = "Fuzzy Search",
                    PaintStyle = ActionItemPaintStyle.Caption
                };
                tempSimpleAction.Execute += FuzzySearchActionExecute;
                fuzzySearchAction = tempSimpleAction;
                tempSimpleAction = null;
            }
            finally
            {
                if (tempSimpleAction != null)
                {
                    tempSimpleAction.Dispose();
                }
            }

            SingleChoiceAction tempChoiceAction = null;
            try
            {
                tempChoiceAction = new SingleChoiceAction(this, "FilterPanelFilterFields", FilterPanelGroup)
                {
                    Caption = "Search in",
                    ImageName = "Action_ParametrizedAction",
                    PaintStyle = ActionItemPaintStyle.Caption
                };
                newFilterFieldsAction = tempChoiceAction;
                tempChoiceAction = null;
            }
            finally
            {
                if (tempChoiceAction != null)
                {
                    tempChoiceAction.Dispose();
                }
            }

            RegisterActions(searchTextAction, fuzzySearchAction, newFilterFieldsAction);

            Forms.Timer tempTimer = null;
            try
            {
                tempTimer = new Forms.Timer
                {
                    Interval = 500
                };
                tempTimer.Tick += SearchTextTimer_TickAsync;
                searchTextTimer = tempTimer;
                tempTimer = null;
            }
            finally
            {
                if (tempTimer != null)
                {
                    tempTimer.Dispose();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            filterController = Frame.GetController<ElasticSearchFilterController>();
            _FillActionContainersController = Frame.GetController<FillActionContainersController>();
            _FillActionContainersController.CustomFillContainers += FillActionContainersController_CustomFillContainers;
        }

        /// <inheritdoc/>
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            Active[nameof(IModelFilterPanel.FilterPanelPosition)] = view != null && view.Model is IModelFilterPanel && ((IModelFilterPanel)view.Model).FilterPanelPosition != Forms.DockStyle.None;
        }

        /// <inheritdoc/>
        protected override void OnViewChanged()
        {
            if (Active[nameof(IModelFilterPanel.FilterPanelPosition)] && View?.Editor != null && View.IsControlCreated)
            {
                if (View.Editor.Control is GridControl control)
                {
                    control.BringToFront();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();
            if (filterController != null)
            {
                filterController.FullTextFilterAction.Active.SetItemValue(FilterPanelGroup, false);
                filterController.Activated += FilterControllerActivated;
                if (filterController.Active)
                {
                    UpdateActionState();
                }
                if (Frame != null)
                {
                    if (Frame.Template is IDynamicContainersTemplate)
                    {
                        TemplateChanged();
                    }
                    else
                    {
                        Frame.TemplateChanged += Frame_TemplateChanged;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            _FillActionContainersController.CustomFillContainers -= FillActionContainersController_CustomFillContainers;
            if (View.Editor.Control is Forms.Control control)
            {
                control.HandleCreated -= GridControlHandleCreated;
            }
            if (Frame != null)
            {
                Frame.TemplateChanged -= Frame_TemplateChanged;
            }
            if (oldFilterFieldsAction != null)
            {
                oldFilterFieldsAction.Active.SetItemValue(FilterPanelGroup, true);
                filterController.FilterFieldsAction = oldFilterFieldsAction;
                oldFilterFieldsAction = null;
            }
            if (buttonsContainer != null)
            {
                if (Frame?.Template is IDynamicContainersTemplate template)
                {
                    template.UnregisterActionContainers(new IActionContainer[] { buttonsContainer });
                }
                if (!buttonsContainer.IsDisposed)
                {
                    buttonsContainer.Dispose();
                }
                buttonsContainer = null;
            }
            if (_FilterPanel != null && !_FilterPanel.IsDisposed)
            {
                _FilterPanel.Dispose();
                _FilterPanel = null;
            }
            _GridControl = null;
            newFilterFieldsAction.Active.SetItemValue(FilterPanelGroup, false);
            if (filterController != null)
            {
                filterController.Activated -= FilterControllerActivated;
            }
            base.OnDeactivated();
        }

        /// <inheritdoc/>
        protected override void OnViewControlsCreated()
        {
            if (Active)
            {
                if (View.Editor.Control is Forms.Control control)
                {
                    control.HandleCreated += GridControlHandleCreated;
                }
            }
            base.OnViewControlsCreated();
        }

        /// <summary>
        /// Calls the FilterControlCreated event handlers
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected void OnFilterControlCreated(EventArgs e)
        {
            FilterControlCreated?.Invoke(this, e);
        }

        private void FillActionContainersController_CustomFillContainers(object sender, CustomFillContainersEventArgs e)
        {
            if (e.Template is LookupControlTemplate looukupTemplate)
            {
                looukupTemplate.SearchActionContainer.ActionItemAdding += ButtonsContainer_ActionItemAdding;
            }
        }

        private void FilterControllerActivated(object sender, EventArgs e)
        {
            UpdateActionState();
        }

        private void GridControlHandleCreated(object sender, EventArgs e)
        {
            if (_GridControl != sender)
            {
                _GridControl = sender as GridControl;
                if (_GridControl != null)
                {
                    _FilterPanel = new PanelControl();
                    buttonsContainer = new ButtonsContainer();
                    _FilterPanel.BeginInit();
                    _FilterPanel.SuspendLayout();

                    // buttonsContainer
                    buttonsContainer.Location = new Point(0, 0);
                    buttonsContainer.AllowCustomization = false;
                    buttonsContainer.ContainerId = FilterPanelGroup;
                    buttonsContainer.HideItemsCompletely = false;
                    buttonsContainer.Name = "filterPanelButtonsContainer";
                    buttonsContainer.ActionItemAdding += ButtonsContainer_ActionItemAdding;

                    // _FilterPanel
                    _FilterPanel.Controls.Add(buttonsContainer);
                    _FilterPanel.Padding = new Forms.Padding(2, 6, 2, 6);
                    buttonsContainer.Dock = Forms.DockStyle.Fill;
                    _FilterPanel.Name = nameof(_FilterPanel);
                    _FilterPanel.Height = 40;
                    _FilterPanel.TabIndex = 1;
                    _FilterPanel.Dock = ((IModelFilterPanel)View.Model).FilterPanelPosition;

                    if (!_GridControl.FormsUseDefaultLookAndFeel)
                    {
                        _FilterPanel.LookAndFeel.Assign(_GridControl.LookAndFeel);
                        buttonsContainer.LookAndFeel.Assign(_GridControl.LookAndFeel);
                    }

                    _FilterPanel.EndInit();
                    _FilterPanel.ResumeLayout(false);

                    if (Frame != null && Frame.Template != null)
                    {
                        if (Frame.Template is IDynamicContainersTemplate template)
                        {
                            template.RegisterActionContainers(new IActionContainer[] { buttonsContainer });
                        }
                    }
                    var ff = buttonsContainer.ActionItems.FirstOrDefault(t => t.Key.Id == newFilterFieldsAction.Id);
                    if (ff.Value != null && ff.Value.LayoutItem != null)
                    {
                        ff.Value.LayoutItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(12, 0, 0, 0);
                    }

                    if (_GridControl.Parent != null)
                    {
                        _GridControl.Parent.Controls.Add(_FilterPanel);
                    }
                    else
                    {
                        _GridControl.ParentChanged += GridControlParentChanged;
                    }

                    UpdateActionState();
                    OnFilterControlCreated(EventArgs.Empty);
                }
            }
        }

        private void ButtonsContainer_ActionItemAdding(object sender, ActionItemEventArgs e)
        {
            if (filterController != null && (e.Item.Action == searchTextAction || e.Item.Action == filterController.FullTextFilterAction))
            {
                var buttons = (ButtonsContainer)sender;
                var oldItem = e.Item;
                buttons.BarManager.Items.Remove(oldItem.ShortcutHandler);
                searchTextActionItem = new PropertyEditor.ButtonsContainersParametrizedActionComboItem((ParametrizedAction)oldItem.Action, buttons);
                e.Item = searchTextActionItem;
                if (oldItem.Control != null)
                {
                    oldItem.Dispose();
                }
                buttons.BarManager.Items.Add(e.Item.ShortcutHandler);
                searchTextActionItem.Control.TextChanged += Control_TextChangedAsync;
                if (searchTextActionItem.Control is ComboBoxEdit be)
                {
                    be.Properties.AutoComplete = false;
                    be.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                }
            }
        }

        private void UpdateActionState()
        {
            if (filterController != null)
            {
                filterController.FilterFieldsAction.Active[ElasticSearchFilterController.NoElasticSearchReason] = filterController.IsElasticSearchAvailable;
                fuzzySearchAction.Active[ElasticSearchFilterController.NoElasticSearchReason] = filterController.IsElasticSearchAvailable;
                fuzzySearchAction.Enabled[FuzzySearchReason] = filterController.ElasticCanFuzzy;
            }
            else
            {
                fuzzySearchAction.Active[ElasticSearchFilterController.NoElasticSearchReason] = false;
            }
        }

        private void FuzzySearchActionExecute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (filterController != null && searchTextAction.Value != null)
            {
                var text = searchTextAction.Value.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    text = text.AddStringToWordEnd("~");
                    filterController.FullTextSearch(text);
                    UpdateActionState();
                }
            }
        }

        private void SearchTextActionExecute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (filterController != null && e.ParameterCurrentValue != null)
            {
                filterController.FullTextSearch(e.ParameterCurrentValue.ToString());
                UpdateActionState();
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _needUpdate = false;
        }

        private async void Control_TextChangedAsync(object sender, EventArgs e)
        {
            if (sender is Forms.Control control)
            {
                fuzzySearchAction.Enabled[FuzzySearchReason] = !string.IsNullOrWhiteSpace(control.Text);
            }
            if (searchTextActionItem != null)
            {
                if (_needUpdate)
                {
                    if (_canUpdate)
                    {
                        _canUpdate = false;
                        await UpdateSuggestDataAsync().ConfigureAwait(true);
                    }
                    else
                    {
                        RestartSearchTextTimer();
                    }
                }
                _needUpdate = true;
            }
        }

        private void RestartSearchTextTimer()
        {
            searchTextTimer.Stop();
            _canUpdate = false;
            searchTextTimer.Start();
        }

        private async void SearchTextTimer_TickAsync(object sender, EventArgs e)
        {
            _canUpdate = true;
            searchTextTimer.Stop();
            await UpdateSuggestDataAsync().ConfigureAwait(true);
        }

        private async Task UpdateSuggestDataAsync()
        {
            if (filterController != null && searchTextActionItem != null)
            {
                if (searchTextActionItem.Control is ComboBoxEdit combobox && !string.IsNullOrEmpty(combobox.Text))
                {
                    var suggest = await filterController.SuggestAsync(combobox.Text).ConfigureAwait(true);
                    var suggestList = suggest?.ToList();
                    var text = combobox.Text;
                    combobox.Properties.Items.BeginUpdate();
                    combobox.Properties.Items.Clear();

                    if (suggestList != null && suggestList.Any())
                    {
                        combobox.Properties.Items.AddRange(suggestList);
                        combobox.Properties.Items.EndUpdate();
                        if (!combobox.IsPopupOpen)
                        {
                            combobox.ShowPopup();
                        }
                        combobox.SelectionStart = text.Length;
                    }
                    else
                    {
                        combobox.Properties.Items.EndUpdate();
                        combobox.SelectionStart = text.Length;
                    }
                }
            }
        }

        private void GridControlParentChanged(object sender, EventArgs e)
        {
            var gridControl = (GridControl)sender;
            gridControl.ParentChanged -= GridControlParentChanged;
            if (gridControl.Parent != null)
            {
                gridControl.Parent.Controls.Add(_FilterPanel);
            }
        }

        private void Frame_TemplateChanged(object sender, EventArgs e)
        {
            TemplateChanged();
        }

        private void TemplateChanged()
        {
            if (Frame != null && oldFilterFieldsAction == null && Frame.Template is IDynamicContainersTemplate)
            {
                oldFilterFieldsAction = filterController.FilterFieldsAction;
                oldFilterFieldsAction.Active.SetItemValue(FilterPanelGroup, false);
                newFilterFieldsAction.Execute += filterController.FilterFieldsActionExecute;
                newFilterFieldsAction.Active.SetItemValue(FilterPanelGroup, true);
                filterController.FilterFieldsAction = newFilterFieldsAction;
                filterController.SetupFilterFields();
                UpdateActionState();
            }
        }
    }
}