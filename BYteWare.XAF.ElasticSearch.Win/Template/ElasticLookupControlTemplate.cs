namespace BYteWare.XAF.ElasticSearch.Win.Template
{
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Win.Templates;
    using DevExpress.ExpressApp.Win.Templates.ActionContainers;
    using DevExpress.Utils;
    using DevExpress.XtraLayout;
    using DevExpress.XtraLayout.Utils;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// Template for the Lookup Control, to be able to add actions
    /// </summary>
    [CLSCompliant(false)]
    public class ElasticLookupControlTemplate : LookupControlTemplate, IDynamicContainersTemplate
    {
        private readonly LayoutControlGroup elasticActionContainerLayoutGroup;
        private readonly LayoutControlItem elasticActionContainerLayoutItem;
        private readonly ButtonsContainer elasticActionContainer;
        private ActionContainersManager actionsContainers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticLookupControlTemplate" /> class.
        /// </summary>
        public ElasticLookupControlTemplate()
            : base()
        {
            typeAndFindPanel.SuspendLayout();
            typeAndFindPanel.Root.BeginInit();
            typeAndFindPanel.Root.Remove(searchActionContainerLayoutItem);
            typeAndFindPanel.Root.Remove(typeValueLayoutItem);
            typeAndFindPanel.Height += 5;
            elasticActionContainerLayoutGroup = new LayoutControlGroup();
            elasticActionContainerLayoutItem = new LayoutControlItem();
            elasticActionContainer = new ButtonsContainer();
            elasticActionContainerLayoutGroup.BeginInit();
            elasticActionContainerLayoutItem.BeginInit();
            elasticActionContainer.BeginInit();
            elasticActionContainerLayoutGroup.TextVisible = false;
            elasticActionContainerLayoutGroup.GroupBordersVisible = false;
            elasticActionContainerLayoutGroup.DefaultLayoutType = LayoutType.Horizontal;
            elasticActionContainerLayoutItem.Control = elasticActionContainer;
            elasticActionContainerLayoutItem.Padding = new Padding(0);
            elasticActionContainerLayoutItem.Name = nameof(elasticActionContainerLayoutItem);
            elasticActionContainerLayoutItem.TextVisible = false;
            elasticActionContainerLayoutItem.ControlAlignment = ContentAlignment.MiddleRight;
            elasticActionContainer.AllowCustomization = false;
            elasticActionContainer.ContainerId = "ElasticActionContainer";
            elasticActionContainer.Name = nameof(elasticActionContainer);
            elasticActionContainer.HideItemsCompletely = false;
            elasticActionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            elasticActionContainer.PaintStyle = ActionItemPaintStyle.Caption;
            elasticActionContainer.Root.DefaultLayoutType = LayoutType.Horizontal;
            elasticActionContainer.Root.EnableIndentsWithoutBorders = DefaultBoolean.True;
            elasticActionContainer.Root.GroupBordersVisible = false;
            elasticActionContainer.Root.Location = new Point(0, 0);
            elasticActionContainer.Root.Name = "elasticActionContainerLayoutControlGroup";
            elasticActionContainer.Root.Padding = new Padding(0, 0, 0, 0);
            elasticActionContainer.Root.Spacing = new Padding(0, 0, 0, 0);
            elasticActionContainer.TabStop = false;
            ActionContainersManager.ActionContainerComponents.Add(elasticActionContainer);
            elasticActionContainerLayoutGroup.AddItem(typeValueLayoutItem);
            elasticActionContainerLayoutGroup.AddItem(elasticActionContainerLayoutItem);
            typeAndFindPanel.Root.AddItem(elasticActionContainerLayoutGroup);
            typeAndFindPanel.Root.AddItem(searchActionContainerLayoutItem);
            elasticActionContainer.EndInit();
            elasticActionContainerLayoutItem.EndInit();
            elasticActionContainerLayoutGroup.EndInit();
            typeAndFindPanel.Root.EndInit();
            typeAndFindPanel.ResumeLayout(false);
        }

        /// <summary>
        /// Returns the Action Container Manager
        /// </summary>
        public ActionContainersManager ActionContainersManager
        {
            get
            {
                if (actionsContainers == null)
                {
                    var actionsContainersField = typeof(LookupControlTemplate).GetField("actionContainersManager", BindingFlags.NonPublic | BindingFlags.Instance);
                    actionsContainers = actionsContainersField.GetValue(this) as ActionContainersManager;
                }
                return actionsContainers;
            }
        }

        /// <summary>
        /// Event that is called if an Action Container was changed
        /// </summary>
        public event EventHandler<ActionContainersChangedEventArgs> ActionContainersChanged;

        /// <summary>
        /// Registers the Enumeration of Action Containers
        /// </summary>
        /// <param name="actionContainers">Enumeration of Action Containers</param>
        public void RegisterActionContainers(IEnumerable<IActionContainer> actionContainers)
        {
            if (actionContainers != null)
            {
                ActionContainersManager.ActionContainerComponents.AddRange(actionContainers);
                OnActionContainersChanged(new ActionContainersChangedEventArgs(actionContainers, ActionContainersChangedType.Added));
            }
        }

        /// <summary>
        /// Unregisters the Enumeration of Action Containers
        /// </summary>
        /// <param name="actionContainers">Enumeration of Action Containers</param>
        public void UnregisterActionContainers(IEnumerable<IActionContainer> actionContainers)
        {
            if (actionContainers != null)
            {
                foreach (IActionContainer actionContainer in actionContainers)
                {
                    ActionContainersManager.ActionContainerComponents.Remove(actionContainer);
                }
                OnActionContainersChanged(new ActionContainersChangedEventArgs(actionContainers, ActionContainersChangedType.Removed));
            }
        }

        /// <summary>
        /// Calls the Event Handlers of ActionContainersChanged
        /// </summary>
        /// <param name="e">The ActionContainersChanged Event Arguments to pass</param>
        protected virtual void OnActionContainersChanged(ActionContainersChangedEventArgs e)
        {
            ActionContainersChanged?.Invoke(this, e);
        }
    }
}
