namespace BYteWare.XAF.ElasticSearch.Win.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Model Extension to define a Filter Panel and it's Position in List Views
    /// </summary>
    [CLSCompliant(false)]
    [ModelInterfaceImplementor(typeof(IModelFilterPanel), "ModelClass")]
    public interface IModelListViewFilterPanel : IModelFilterPanel
    {
    }
}
