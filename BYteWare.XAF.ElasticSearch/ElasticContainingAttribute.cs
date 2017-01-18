namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Can be used to decorate a property of a BusinessClass Type or an Association List of one.
    /// Properties of that class where used to construct the ElasticSearch document of an instance of the current class, so indexing has to happen even if only a referenced instance got changed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ElasticContainingAttribute : Attribute
    {
    }
}
