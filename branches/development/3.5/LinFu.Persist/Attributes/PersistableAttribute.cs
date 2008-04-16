using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    /// <summary>
    /// Marks the class as persistable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PeristableAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PeristableAttribute"/> class.
        /// </summary>
        public PeristableAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PeristableAttribute"/> class.
        /// </summary>
        /// <param name="tableName">The name of the corresponding <see cref="ITable"/></param>
        public PeristableAttribute(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// Sets of gets the name of the <see cref="ITable"/>
        /// </summary>
        public string TableName { get; set; }

    }
}
