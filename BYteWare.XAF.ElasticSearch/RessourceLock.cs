namespace BYteWare.XAF
{
    using DevExpress.Xpo;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The lock modes
    /// </summary>
    public enum LockMode
    {
        /// <summary>
        /// Shared locks allow concurrent transactions to read a resource under pessimistic concurrency control. No other transactions can modify the data while shared (S) locks exist on the resource.
        /// </summary>
        Shared,

        /// <summary>
        /// Only one transaction can obtain an update (U) lock to a resource at a time.
        /// </summary>
        Update,

        /// <summary>
        /// Protects requested or acquired shared locks on resources lower in the hierarchy.
        /// </summary>
        IntentShared,

        /// <summary>
        /// Protects requested or acquired exclusive locks on resources lower in the hierarchy. IX is a superset of IS, and it also protects requesting shared locks on lower level resources.
        /// </summary>
        IntentExclusive,

        /// <summary>
        /// Exclusive (X) locks prevent access to a resource by concurrent transactions.
        /// </summary>
        Exclusive
    }

    /// <summary>
    /// Can place a lock on a specfific ressource, which should potentionally block all clients.
    /// Works at the moment only with SQL Server
    /// </summary>
    public class ResourceLock : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLock"/> class.
        /// </summary>
        /// <param name="session">A XPO Session</param>
        /// <param name="ressource">A name of a resource to lock</param>
        /// <param name="lockMode">The lock mode</param>
        public ResourceLock(Session session, string ressource, LockMode lockMode)
        {
            _Session = session;
            _Ressource = ressource;
            _LockMode = lockMode;
        }

        /// <summary>
        /// Releases the lock if it was acquired
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Fields...
        private readonly LockMode _LockMode;
        private readonly string _Ressource;
        private readonly Session _Session;
        private bool _Acquired;

        /// <summary>
        /// A XPO Session
        /// </summary>
        public Session Session
        {
            get
            {
                return _Session;
            }
        }

        /// <summary>
        /// The name of the Resource
        /// </summary>
        public string Ressource
        {
            get
            {
                return _Ressource;
            }
        }

        /// <summary>
        /// The lock mode
        /// </summary>
        public LockMode LockMode
        {
            get
            {
                return _LockMode;
            }
        }

        /// <summary>
        /// Was the lock already acquired
        /// </summary>
        public bool Acquired
        {
            get
            {
                return _Acquired;
            }
        }

        /// <summary>
        /// Acquires the lock on the resource
        /// </summary>
        /// <param name="timeOut">Maximum time to wait for a lock on the resource</param>
        /// <returns>True if the lock was acquired; False otherwise</returns>
        public bool Acquire(int timeOut)
        {
            if (!Acquired)
            {
                if (Session.GetDBType() == DBType.MSSql)
                {
                    _Acquired = (int)Session.ExecuteScalar(string.Format(CultureInfo.InvariantCulture, @"DECLARE @result int; EXEC @result = sp_getapplock @Resource = '{0}', @LockMode = '{1}', @LockOwner = 'Session', @LockTimeout = {2}; select @result", Ressource, Enum.GetName(typeof(LockMode), LockMode), timeOut)) >= 0;
                }
                else
                {
                    _Acquired = true;
                }
            }
            return Acquired;
        }

        /// <summary>
        /// Release the lock on the resource
        /// </summary>
        public void Release()
        {
            if (Acquired)
            {
                _Acquired = false;
                if (Session.GetDBType() == DBType.MSSql)
                {
                    Session.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, @"EXEC sp_releaseapplock @Resource = '{0}', @LockOwner = 'Session'", Ressource));
                }
            }
        }

        /// <summary>
        /// Releases the lock if it was acquired
        /// </summary>
        /// <param name="disposing">True if managed resources should be released</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Release();
            }
        }
    }
}
