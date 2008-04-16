using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    /// <summary>
    /// Marks the property as persistable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistablePropertyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PersistablePropertyAttribute"/> class.
        /// </summary>
        public PersistablePropertyAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersistablePropertyAttribute"/> class.
        /// </summary>
        /// <param name="columnName">The name of the corresponding <see cref="IColumn"/></param>
        public PersistablePropertyAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        /// <summary>
        /// Gets or sets the name of the corresponding <see cref="IColumn"/>
        /// </summary>
        public string ColumnName { get; set; }
    }
}
