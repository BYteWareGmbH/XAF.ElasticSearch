namespace BYteWare.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ReadOnlyCollection only works for IList so this is a real read only collection for ICollection.
    /// </summary>
    /// <typeparam name="T">Element Type.</typeparam>
    [Serializable]
    public class RealReadOnlyCollection<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        private const string NotSupportedReadOnlyException = "Collection is a read only collection.";
        private readonly ICollection<T> collection;

        /// <summary>
        /// Generates a new <see cref="RealReadOnlyCollection&lt;T&gt;"/>.
        /// </summary>
        /// <param name="collection">Internally used collection.</param>
        public RealReadOnlyCollection(ICollection<T> collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <summary>
        /// Gets the number of elements contained in the RealReadOnlyCollection&lt;T&gt; instance.
        /// </summary>
        public int Count
        {
            get
            {
                return collection.Count;
            }
        }

        /// <summary>
        /// Determines whether an element is in the RealReadOnlyCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the RealReadOnlyCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>True if value is found in the RealReadOnlyCollection&lt;T&gt;; otherwise, False.</returns>
        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        /// <summary>
        /// Copies the entire RealReadOnlyCollection&lt;T&gt; to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from RealReadOnlyCollection&lt;T&gt;. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the RealReadOnlyCollection&lt;T&gt;.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; for the RealReadOnlyCollection&lt;T&gt;.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether the ICollection&lt;T&gt; is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return ((ICollection)collection).SyncRoot;
            }
        }

        /// <summary>
        /// Returns the ICollection&lt;T&gt; that the RealReadOnlyCollection&lt;T&gt; wraps.
        /// </summary>
        protected ICollection<T> Items
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// Adds an item to the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
        /// </summary>
        /// <param name="item">The object to add to the ICollection&lt;T&gt;.</param>
        public void Add(T item)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Removes all items from the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the ICollection&lt;T&gt;. This implementation always throws NotSupportedException.
        /// </summary>
        /// <param name="item">The object to remove from the ICollection&lt;T&gt;.</param>
        /// <returns>True if value was successfully removed from the ICollection&lt;T&gt;; otherwise, False.</returns>
        public bool Remove(T item)
        {
            throw new NotSupportedException(NotSupportedReadOnlyException);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ICollection. The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array is T[] items)
            {
                collection.CopyTo(items, index);
            }
            else
            {
                throw new ArgumentException("Invalid array Type", nameof(array));
            }
        }
    }
}
