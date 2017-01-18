namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Attribute Provider for Json.Net, which always returns empty lists
    /// </summary>
    internal class EmptyAttributeProvider : IAttributeProvider
    {
        /// <summary>
        /// Returns a an empty collection of attributes
        /// </summary>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>An empty collection of <see cref="Attribute"/>s.</returns>
        public IList<Attribute> GetAttributes(bool inherit)
        {
            return new List<Attribute>();
        }

        /// <summary>
        /// Returns an empty collection of attributes, identified by type.
        /// </summary>
        /// <param name="attributeType">The type of the attributes.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>An empty collection of <see cref="Attribute"/>s.</returns>
        public IList<Attribute> GetAttributes(Type attributeType, bool inherit)
        {
            return new List<Attribute>();
        }
    }
}