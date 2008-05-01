using System;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represents a type mapper that is used to map the datatype from an external datastore to a native .Net type.
    /// </summary>
    public interface ITypeMapper
    {
        /// <summary>
        /// Maps an external type description to a .Net type. 
        /// </summary>
        /// <param name="dataType">Describes the external datatype</param>
        /// <param name="isNullable">True is the external datatype should be treated as nullable, otherwise false.</param>
        /// <returns></returns>
        Type MapType(string dataType, bool isNullable);
    }
}
