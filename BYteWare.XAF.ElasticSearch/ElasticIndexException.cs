namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// ElasticSearch indexing Exception
    /// </summary>
    [Serializable]
    public class ElasticIndexException : Exception
    {
        /// <summary>
        /// Constructs a new ElasticIndexException.
        /// </summary>
        public ElasticIndexException()
        {
        }

        /// <summary>
        /// Constructs a new ElasticIndexException.
        /// </summary>
        /// <param name="message">The exception message</param>
        public ElasticIndexException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new ElasticIndexException.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ElasticIndexException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">A SerializationInfo instance</param>
        /// <param name="context">The StreamingContext instance to use</param>
        protected ElasticIndexException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
