namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class to report progress
    /// </summary>
    public class WorkerProgress : IWorkerProgress
    {
        /// <summary>
        /// Name of the progress bar
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Phase of the action
        /// </summary>
        public string Phase
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum of the progress bar
        /// </summary>
        public int Maximum
        {
            get;
            set;
        }

        /// <summary>
        /// Position of the progress bar
        /// </summary>
        public int Position
        {
            get;
            set;
        }
    }
}
