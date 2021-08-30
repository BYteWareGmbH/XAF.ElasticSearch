namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Define the type of field content.
    /// </summary>
    public enum FieldType
    {
#pragma warning disable SA1300
        /// <summary>
        /// A field to index full-text values, such as the body of an email or the description of a product. These fields are analyzed, that is they are passed through an analyzer to convert the string into a list of individual terms before being indexed.
        /// </summary>
        text,

        /// <summary>
        /// A field to index structured content such as email addresses, hostnames, status codes, zip codes or tags. They are typically used for filtering (Find me all blog posts where status is published), for sorting, and for aggregations. Keyword fields are only searchable by their exact value.
        /// </summary>
        keyword,

        /// <summary>
        /// A signed 64-bit integer with a minimum value of -263 and a maximum value of 263-1.
        /// </summary>
        long_type,

        /// <summary>
        /// A signed 32-bit integer with a minimum value of -231 and a maximum value of 231-1.
        /// </summary>
        integer_type,

        /// <summary>
        /// A signed 16-bit integer with a minimum value of -32,768 and a maximum value of 32,767.
        /// </summary>
        short_type,

        /// <summary>
        /// A signed 8-bit integer with a minimum value of -128 and a maximum value of 127.
        /// </summary>
        byte_type,

        /// <summary>
        /// A double-precision 64-bit IEEE 754 floating point.
        /// </summary>
        double_type,

        /// <summary>
        /// A single-precision 32-bit IEEE 754 floating point.
        /// </summary>
        float_type,

        /// <summary>
        /// A half-precision 16-bit IEEE 754 floating point.
        /// </summary>
        half_float,

        /// <summary>
        /// A floating point that is backed by a long and a fixed scaling factor.
        /// </summary>
        scaled_float,

        /// <summary>
        /// Date type.
        /// </summary>
        date_type,

        /// <summary>
        /// Boolean type.
        /// </summary>
        boolean_type,

        /// <summary>
        /// The binary type accepts a binary value as a Base64 encoded string. The field is not stored by default and is not searchable.
        /// </summary>
        binary,

        /// <summary>
        /// JSON documents are hierarchical in nature: the document may contain inner objects which, in turn, may contain inner objects themselves.
        /// </summary>
        object_type,

        /// <summary>
        /// The nested type is a specialised version of the object datatype that allows arrays of objects to be indexed and queried independently of each other.
        /// </summary>
        nested,

        /// <summary>
        /// An ip field can index/store either IPv4 or IPv6 addresses.
        /// </summary>
        ip,

        /// <summary>
        /// Fields of type geo_point accept latitude-longitude pairs.
        /// </summary>
        geo_point,

        /// <summary>
        /// The geo_shape datatype facilitates the indexing of and searching with arbitrary geo shapes such as rectangles and polygons.
        /// </summary>
        geo_shape,

        /// <summary>
        /// The completion suggester provides auto-complete/search-as-you-type functionality.
        /// </summary>
        completion,

        /// <summary>
        /// A field of type token_count is really an integer field which accepts string values, analyzes them, then indexes the number of tokens in the string.
        /// </summary>
        token_count,

        /// <summary>
        /// Computes hashes of values at index-time and store them in the index
        /// </summary>
        murmur3,
#pragma warning restore SA1300
    }
}
