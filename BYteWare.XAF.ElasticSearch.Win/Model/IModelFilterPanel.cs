namespace BYteWare.XAF.ElasticSearch.Win.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Model Extension to define a Filter Panel and it's Position
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelFilterPanel : IModelNode
    {
        /// <summary>
        /// Position for a filter panel for listviews
        /// </summary>
        [Category("Behavior")]
        [Description("Displays a filter panel for listviews at the specified position")]
        DockStyle FilterPanelPosition
        {
            get;
            set;
        }
    }
}
