namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// ElasticSearch locking Exception
    /// </summary>
    [Serializable]
    public class ElasticIndexLockingException : ElasticIndexException
    {
        /// <summary>
        /// Constructs a new ElasticIndexLockingException.
        /// </summary>
        public ElasticIndexLockingException()
        {
        }

        /// <summary>
        /// Constructs a new ElasticIndexLockingException.
        /// </summary>
        /// <param name="message">The exception message</param>
        public ElasticIndexLockingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new ElasticIndexLockingException.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public ElasticIndexLockingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">A SerializationInfo instance</param>
        /// <param name="context">The StreamingContext instance to use</param>
        protected ElasticIndexLockingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
