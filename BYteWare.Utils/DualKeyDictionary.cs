namespace BYteWare.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Dictionary with two key levels
    /// </summary>
    /// <typeparam name="TKey1">Type for the first key level</typeparam>
    /// <typeparam name="TKey2">Type for the second key level</typeparam>
    /// <typeparam name="TValue">Type for the elements</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Serialization not implemented")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Multi Key Dictionary")]
    public class DualKeyDictionary<TKey1, TKey2, TValue> : Dictionary<TKey1, Dictionary<TKey2, TValue>>
    {
        /// <summary>
        /// Sets or returns the value for both key levels
        /// </summary>
        /// <param name="key1">First level key</param>
        /// <param name="key2">Second level key</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "Multi Key Dictionary")]
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get
            {
                var sdic = this[key1];
                return sdic[key2];
            }
            set
            {
                Dictionary<TKey2, TValue> sdic;
                if (!TryGetValue(key1, out sdic))
                {
                    sdic = new Dictionary<TKey2, TValue>();
                    this[key1] = sdic;
                }
                sdic[key2] = value;
            }
        }

        /// <summary>
        /// Adds a new value to the dictionary
        /// </summary>
        /// <param name="key1">First level key</param>
        /// <param name="key2">Second level key</param>
        /// <param name="value">Value to Add</param>
        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            Dictionary<TKey2, TValue> sdic;
            if (!TryGetValue(key1, out sdic))
            {
                sdic = new Dictionary<TKey2, TValue>();
                this[key1] = sdic;
            }
            sdic.Add(key2, value);
        }

        /// <summary>
        /// Returns true if the dictionary contains a value for both key levels; otherwise false
        /// </summary>
        /// <param name="key1">First level key</param>
        /// <param name="key2">Second level key</param>
        /// <returns>Contains the dictionary a value for both key levels</returns>
        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            Dictionary<TKey2, TValue> sdic;
            if (!TryGetValue(key1, out sdic))
            {
                return sdic.ContainsKey(key2);
            }
            return false;
        }

        /// <summary>
        /// Gets an enumeration of all values in the Dictionary
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
