using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    /// <summary>
    /// Marks the property as persistable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PersistableCollectionAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PersistableCollectionAttribute"/> class.
        /// </summary>
        public PersistableCollectionAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersistableCollectionAttribute"/> class.
        /// </summary>
        /// <param name="columnName">The name of the foreign key column name.</param>
        public PersistableCollectionAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        /// <summary>
        /// Sets or gets the name of the foreign key column name.
        /// </summary>
        public string ColumnName { get; set; }
    }
}
