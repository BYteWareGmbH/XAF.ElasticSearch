namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension Methods for Task
    /// </summary>
    public static class TaskExtensions
    {
#pragma warning disable CC0061 // Async method can be terminating with 'Async' name.
        /// <summary>
        /// Fire and forget for an Async method that logs all Exceptions to the XAF Log file
        /// </summary>
        /// <param name="task">The Task to wait for its completion</param>
        public static async void Forget(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Tracing.Tracer.LogError(ex);
            }
        }
#pragma warning restore CC0061 // Async method can be terminating with 'Async' name.
    }
}
