namespace BYteWare.XAF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// WaitScreen Class
    /// </summary>
    public class WaitScreen
    {
#pragma warning disable SA1401 // Fields must be private
        /// <summary>
        /// WaitScreen instance
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = nameof(XAF))]
        [CLSCompliant(false)]
        protected static Lazy<WaitScreen> _Lazy = new Lazy<WaitScreen>(() => new WaitScreen());
#pragma warning restore SA1401 // Fields must be private

        /// <summary>
        /// Static instance
        /// </summary>
        public static WaitScreen Instance
        {
            get
            {
                return _Lazy.Value;
            }
        }

        /// <summary>
        /// Shows the Wait Screen
        /// </summary>
        /// <param name="caption">Caption for the Wait Screen</param>
        /// <param name="displayText">Text to show inside the Wait Screen</param>
        public virtual void Show(string caption, string displayText)
        {
        }

        /// <summary>
        /// Updates the Wait Screen
        /// </summary>
        /// <param name="caption">Caption for the Wait Screen</param>
        /// <param name="displayText">Text to show inside the Wait Screen</param>
        public virtual void Update(string caption, string displayText)
        {
        }

        /// <summary>
        /// Hides the Wait Screen
        /// </summary>
        public virtual void Hide()
        {
        }
    }
}
