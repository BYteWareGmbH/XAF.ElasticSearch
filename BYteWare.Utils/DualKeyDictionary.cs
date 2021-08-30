namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Dictionary with two key levels.
    /// </summary>
    /// <typeparam name="TKey1">Type for the first key level.</typeparam>
    /// <typeparam name="TKey2">Type for the second key level.</typeparam>
    /// <typeparam name="TValue">Type for the elements.</typeparam>
    public class DualKeyDictionary<TKey1, TKey2, TValue> : Dictionary<TKey1, Dictionary<TKey2, TValue>>
    {
        /// <summary>
        /// Sets or returns the value for both key levels.
        /// </summary>
        /// <param name="key1">First level key.</param>
        /// <param name="key2">Second level key.</param>
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                var sdic = this[key1];
                return sdic[key2];
            }
            set
            {
                if (!TryGetValue(key1, out Dictionary<TKey2, TValue> sdic))
                {
                    sdic = new Dictionary<TKey2, TValue>();
                    this[key1] = sdic;
                }
                sdic[key2] = value;
            }
        }

        /// <summary>
        /// Adds a new value to the dictionary.
        /// </summary>
        /// <param name="key1">First level key.</param>
        /// <param name="key2">Second level key.</param>
        /// <param name="value">Value to Add.</param>
        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            if (!TryGetValue(key1, out Dictionary<TKey2, TValue> sdic))
            {
                sdic = new Dictionary<TKey2, TValue>();
                this[key1] = sdic;
            }
            sdic.Add(key2, value);
        }

        /// <summary>
        /// Returns true if the dictionary contains a value for both key levels; otherwise false.
        /// </summary>
        /// <param name="key1">First level key.</param>
        /// <param name="key2">Second level key.</param>
        /// <returns>Contains the dictionary a value for both key levels.</returns>
        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            if (TryGetValue(key1, out Dictionary<TKey2, TValue> sdic))
            {
                return sdic.ContainsKey(key2);
            }
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified keys.
        /// </summary>
        /// <param name="key1">First level key.</param>
        /// <param name="key2">Second level key.</param>
        /// <param name="value">Contains the value associated with the specified keys, if the keys are found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns>true if the Dictionary contains an element with the specified keys; otherwise, false.</returns>
        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            if (TryGetValue(key1, out Dictionary<TKey2, TValue> sdic))
            {
                return sdic.TryGetValue(key2, out value);
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Gets an enumeration of all values in the Dictionary.
        /// </summary>
        public new IEnumerable<TValue> Values
        {
            get
            {
                return base.Values.SelectMany(d => d.Values);
            }
        }
    }
}
