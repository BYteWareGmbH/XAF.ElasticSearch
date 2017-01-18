namespace BYteWare.Utils
{
    /// <summary>
    /// Interface to report progress
    /// </summary>
    public interface IWorkerProgress
    {
        /// <summary>
        /// Name of the progress bar
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Phase of the action
        /// </summary>
        string Phase
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum of the progress bar
        /// </summary>
        int Maximum
        {
            get;
            set;
        }

        /// <summary>
        /// Position of the progress bar
        /// </summary>
        int Position
        {
            get;
            set;
        }
    }
}