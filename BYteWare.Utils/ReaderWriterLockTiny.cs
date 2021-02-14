namespace BYteWare.Utils
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents a lock that is used to manage access to a resource, allowing multiple threads for reading or exclusive access for writing.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Comparison isn't needed")]
    public struct ReaderWriterLockTiny
    {
        // if lock is above this value then somebody has a write lock
        private const int _writerLock = 1000000;

        // lock state counter
        private int _lock;

        /// <summary>
        /// Tries to enter the lock in read mode.
        /// </summary>
        public void EnterReadLock()
        {
            var w = default(SpinWait);
            var tmpLock = _lock;
            while (tmpLock >= _writerLock ||
                tmpLock != Interlocked.CompareExchange(ref _lock, tmpLock + 1, tmpLock))
            {
                w.SpinOnce();
                tmpLock = _lock;
            }
        }

        /// <summary>
        /// Tries to enter the lock in write mode.
        /// </summary>
        public void EnterWriteLock()
        {
            var w = default(SpinWait);

            while (Interlocked.CompareExchange(ref _lock, _writerLock, 0) != 0)
            {
                w.SpinOnce();
            }
        }

        /// <summary>
        /// Upgrade the lock to write mode.
        /// </summary>
        public void UpgradeToWrite()
        {
            var w = default(SpinWait);

            while (Interlocked.CompareExchange(ref _lock, _writerLock + 1, 1) != 1)
            {
                w.SpinOnce();
            }
        }

        /// <summary>
        /// Downgrades the lock to read mode.
        /// </summary>
        public void DowngradeToRead()
        {
            _lock = 1;
        }

        /// <summary>
        /// Reduces the recursion count for read mode, and exits read mode if the resulting count is 0 (zero).
        /// </summary>
        public void ExitReadLock()
        {
            Interlocked.Decrement(ref _lock);
        }

        /// <summary>
        /// Reduces the recursion count for write mode, and exits write mode if the resulting count is 0 (zero).
        /// </summary>
        public void ExitWriteLock()
        {
            _lock = 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "lock counter: " + _lock;
        }
    }
}
