namespace BYteWare.XAF.ElasticSearch.Controllers
{
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.Actions;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Xpo;
    using DevExpress.Persistent.Base;
    using ElasticSearch;
    using ElasticSearch.BusinessObjects;
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// View Controller for classes which implement IElasticSearch
    /// </summary>
    [CLSCompliant(false)]
    public class ElasticIndexController : ViewController
    {
        private readonly SimpleAction ReIndexAction;
        private readonly SimpleAction ElasticSearchFullReIndexAction;

        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticIndexController"/> class.
        /// </summary>
        public ElasticIndexController()
        {
            TargetObjectType = typeof(IElasticSearchIndex);

#pragma warning disable CC0009 // Use object initializer
            SimpleAction tempAction = null;
            try
            {
                tempAction = new SimpleAction(this, "ElasticSearchReIndex", PredefinedCategory.Edit);
                tempAction.Caption = "Rebuild";
                tempAction.ConfirmationMessage = "Do you want to rebuild the ElasticSearch Index?";
                tempAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
                tempAction.Execute += ReIndexAction_Execute;
                ReIndexAction = tempAction;
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
                tempAction = new SimpleAction(this, "ElasticSearchFullReIndex", PredefinedCategory.Edit);
                tempAction.Caption = "Rebuild all";
                tempAction.ConfirmationMessage = "Do you want to rebuild all ElasticSearch Indexes?";
                tempAction.TargetViewType = ViewType.ListView;
                tempAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
                tempAction.Execute += ElasticSearchFullReIndex_Execute;
                ElasticSearchFullReIndexAction = tempAction;
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

            RegisterActions(ReIndexAction, ElasticSearchFullReIndexAction);
        }

        private void ReIndexAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            var index = e.CurrentObject as IElasticSearchIndex;
            if (index != null)
            {
                try
                {
                    WaitScreen.Instance.Show(CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "ReIndexWaitScreenCaption"), CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "ReIndexWaitScreenText"));
                    var rowCount = 0;
                    using (var osp = new XPObjectSpaceProvider(Application.GetConnectionString(), null, true))
                    using (var objectspace = (XPObjectSpace)osp.CreateObjectSpace())
                    {
                        index = objectspace.GetObject(index) as IElasticSearchIndex;
                        if (index != null)
                        {
                            ElasticSearchClient.Instance.Reindex(index, (ci, i) =>
                            {
                                if (i > 0)
                                {
                                    rowCount += i;
                                    WaitScreen.Instance.Update(string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "ReIndexWaitScreenUpdateCaption"), ci.ESTypeName), string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "ReIndexWaitScreenUpdateText"), rowCount));
                                }
                            });
                        }
                    }
                }
                finally
                {
                    WaitScreen.Instance.Hide();
                }
            }
            View.Refresh();
        }

        private void ElasticSearchFullReIndex_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            try
            {
                WaitScreen.Instance.Show(CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "ReIndexWaitScreenCaption"), CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "ReIndexWaitScreenText"));
                ElasticSearchClient.Instance.RefreshIndexes(Application, p =>
                {
                    WaitScreen.Instance.Update(p.Name, string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(ElasticSearchClient.MessageGroup, "RefreshProgress"), p.Phase, p.Position, p.Maximum, p.Position / (double)p.Maximum));
                });
            }
            finally
            {
                WaitScreen.Instance.Hide();
            }
            View.Refresh();
        }
    }
}
