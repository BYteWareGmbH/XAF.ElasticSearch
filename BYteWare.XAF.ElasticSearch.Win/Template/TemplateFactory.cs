namespace BYteWare.XAF.ElasticSearch.Win.Template
{
    using DevExpress.ExpressApp.Win;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Template Factory
    /// </summary>
    [CLSCompliant(false)]
    public class TemplateFactory : DefaultFrameTemplateFactory
    {
        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "IFrameTemplate should inherit IDisposable")]
        protected override DevExpress.ExpressApp.Templates.IFrameTemplate CreateNestedFrameTemplate()
        {
            return new NestedDynamicActionContainer();
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "IFrameTemplate should inherit IDisposable")]
        protected override DevExpress.ExpressApp.Templates.IFrameTemplate CreateLookupControlTemplate()
        {
            return new ElasticLookupControlTemplate();
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "IFrameTemplate should inherit IDisposable")]
        protected override DevExpress.ExpressApp.Templates.IFrameTemplate CreatePopupWindowTemplate()
        {
            return new PopupForm();
        }
    }
}
