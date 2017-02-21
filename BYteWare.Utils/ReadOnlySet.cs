namespace BYteWare.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Read Only Set Collection
    /// </summary>
    /// <typeparam name="T">Element Type</typeparam>
    public class ReadOnlySet<T> : IReadOnlyCollection<T>, ISet<T>
    {
        private const string NotSupportedReadOnlyException = "Set is a read only set.";
        private readonly ISet<T> _set;

        /// <summary>
        /// Generates a new <see cref="ReadOnlySet&lt;T&gt;"/>.
        /// </summary>
        /// <param name="set">Internally used Set</param>
        public ReadOnlySet(ISet<T> set)
        {
            _set = set;
        }

        /// <summary>
        /// Returns an IEnumerator for this enumerable Object.  The enumerator provides a simple way to access all the contents of a collection.
        /// </summary>
        /// <returns>IEnumerator for elements of type T</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        /// <summary>
        /// Returns an IEnumerator for this enumerable Object.  The enumerator provides a simple way to access all the contents of a collection.
        /// </summary>
        /// <returns>IEnumerator for elements</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_set).GetEnumerator();
        }

        /// <summary>
        /// Add ITEM to the set, return true if added, false if duplicate
        /// </summary>
        /// <param name="item">The item to add</param>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Transform this set into its union with other
        /// </summary>
        /// <param name="other">The IEnumerable to calculate the union with</param>
        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Transform this set into its intersection with other
        /// </summary>
        /// <param name="other">The IEnumerable to calculate the intersection with</param>
        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Transform this set so it contains no elements that are also in other
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Transform this set so it contains elements initially in this or in other, but not both
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Check if this set is a subset of other
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        /// <returns>True if other is a Subset; False otherwise</returns>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _set.IsSubsetOf(other);
        }

        /// <summary>
        /// Check if this set is a superset of other
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        /// <returns>True if other is a Superset; False otherwise</returns>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _set.IsSupersetOf(other);
        }

        /// <summary>
        /// Check if this set is a superset of other, but not the same as it
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        /// <returns>True if other is a proper Superset; False otherwise</returns>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _set.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Check if this set is a subset of other, but not the same as it
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        /// <returns>True if other is a proper Subset; False otherwise</returns>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _set.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Check if this set has any elements in common with other
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        /// <returns>True if other conatins overlapping elements; False otherwise</returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            return _set.Overlaps(other);
        }

        /// <summary>
        /// Check if this set contains the same and only the same elements as other
        /// </summary>
        /// <param name="other">The other IEnumerable</param>
        /// <returns>True if other conatins the same elements; False otherwise</returns>
        public bool SetEquals(IEnumerable<T> other)
        {
            return _set.SetEquals(other);
        }

        /// <summary>
        /// Add ITEM to the set, return true if added, false if duplicate
        /// </summary>
        /// <param name="item">item to Add</param>
        /// <returns>True if the item was added; False if it already existed</returns>
        public bool Add(T item)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Clears the content of the set
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Does the collection contain the item
        /// </summary>
        /// <param name="item">item to search</param>
        /// <returns>True if the collection contains the item; Fslse otherwise</returns>
        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        /// <summary>
        /// CopyTo copies a collection into an Array, starting at a particular index into the array.
        /// </summary>
        /// <param name="array">array to copy the values into</param>
        /// <param name="arrayIndex">starting index</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove ITEM from the set.
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>True if the item was removed; False otherwise</returns>
        public bool Remove(T item)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Number of items in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _set.Count;
            }
        }

        /// <summary>
        /// Is the Collection read only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }
    }
}
