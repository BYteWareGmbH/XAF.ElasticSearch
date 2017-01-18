namespace BYteWare.XAF.Win
{
    using DevExpress.ExpressApp.Win;
    using DevExpress.ExpressApp.Win.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using XAF;

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public sealed class WaitScreenWin : WaitScreen
    {
        private static Lazy<ISplash> waitSplashScreen = new Lazy<ISplash>(() => new DXSplashScreen());
        private static int showCount;

        /// <summary>
        /// Wait Splash Screen Instance
        /// </summary>
        public static ISplash WaitSplashScreen
        {
            get
            {
                return waitSplashScreen.Value;
            }
            set
            {
                waitSplashScreen = new Lazy<ISplash>(() => value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "WinForms Implementation")]
        static WaitScreenWin()
        {
            _Lazy = new Lazy<WaitScreen>(() => new WaitScreenWin());
        }

        /// <summary>
        /// Registers the Winforms implementation as WaitScreen
        /// </summary>
        public static void Register()
        {
        }

        /// <inheritdoc/>
        public override void Show(string caption, string displayText)
        {
            base.Show(caption, displayText);
            if (showCount <= 0)
            {
                WaitSplashScreen.Start();
            }
            var updateSplash = WaitSplashScreen as ISupportUpdateSplash;
            if (updateSplash != null)
            {
                updateSplash.UpdateSplash(caption, displayText);
            }
            else
            {
                WaitSplashScreen.SetDisplayText(displayText);
            }
            showCount++;
        }

        /// <inheritdoc/>
        public override void Update(string caption, string displayText)
        {
            base.Update(caption, displayText);
            var updateSplash = WaitSplashScreen as ISupportUpdateSplash;
            if (updateSplash != null)
            {
                updateSplash.UpdateSplash(caption, displayText);
            }
            else
            {
                WaitSplashScreen.SetDisplayText(displayText);
            }
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            if (showCount <= 1)
            {
                WaitSplashScreen.Stop();
                showCount = 0;
            }
            else
            {
                showCount--;
            }
            base.Hide();
        }
    }
}
