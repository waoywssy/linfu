using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    /// <summary>
    /// Marks the propery as persistable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistableReferenceAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PersistableReferenceAttribute"/> class.       
        /// </summary>
        public PersistableReferenceAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersistableReferenceAttribute"/> class.       
        /// </summary>
        /// <param name="columnName">The name of the foreign key column name</param>
        public PersistableReferenceAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        /// <summary>
        /// Sets or gets the name of the foreign key column name.
        /// </summary>
        public string ColumnName { get; set; }
    }
}
