namespace BYteWare.XAF.ElasticSearch.Win.Template
{
    using DevExpress.ExpressApp.Templates;
    using DevExpress.ExpressApp.Win.Templates;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Popup Form Template with support for dynamically added Action Containers
    /// </summary>
    [CLSCompliant(false)]
    public class PopupForm : DevExpress.ExpressApp.Win.Templates.PopupForm, IDynamicContainersTemplate
    {
        private ActionContainersManager actionsContainers;

        /// <summary>
        /// Event that is called after the Form OnShown Event was handled
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix", Justification = "Event is really called after the Shown event")]
        public event EventHandler AfterShown;

        /// <inheritdoc/>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            OnAfterShown(e);
        }

        /// <summary>
        /// Calls the Event Handlers of ActionContainersChanged
        /// </summary>
        /// <param name="e">The ActionContainersChanged Event Arguments to pass</param>
        protected virtual void OnAfterShown(EventArgs e)
        {
            AfterShown?.Invoke(this, e);
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
                    var actionsContainersField = typeof(DevExpress.ExpressApp.Win.Templates.PopupForm).GetField("actionContainersManager", BindingFlags.NonPublic | BindingFlags.Instance);
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
