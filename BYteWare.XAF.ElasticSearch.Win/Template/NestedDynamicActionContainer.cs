namespace BYteWare.XAF.ElasticSearch.Win.Template
{
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Win.SystemModule;
    using DevExpress.ExpressApp.Win.Templates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DevExpress.XtraBars;

    /// <summary>
    /// Nested Frame Template with support for dynamically added Action Containers
    /// </summary>
    [CLSCompliant(false)]
    public class NestedDynamicActionContainer : NestedFrameTemplate, IDynamicContainersTemplate, IContextMenuHolder
    {
        private readonly DevExpress.XtraBars.PopupMenu contextMenu;
        private ActionContainersManager actionsContainers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedDynamicActionContainer"/> class.
        /// </summary>
        public NestedDynamicActionContainer() : base()
        {
            contextMenu = new DevExpress.XtraBars.PopupMenu(BarManager);
            contextMenu.Name = nameof(contextMenu);
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
                    var actionsContainersField = typeof(NestedFrameTemplate).GetField("actionContainersManager", BindingFlags.NonPublic | BindingFlags.Instance);
                    actionsContainers = actionsContainersField.GetValue(this) as ActionContainersManager;
                }
                return actionsContainers;
            }
        }

        /// <summary>
        /// Ruft das dem Steuerelement zugeordnete Kontextmenü ab oder legt dieses fest.
        /// </summary>
        PopupMenu IContextMenuHolder.ContextMenu
        {
            get
            {
                return contextMenu;
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
