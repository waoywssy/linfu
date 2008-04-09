using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    internal static class ColumnIndexExtensions
    {
        public static bool HasColumn<T>(this IDictionary<string, T> index, string columnName)
        {
            //// Perform a case-insensitive match for the target column
            //// with the given column name
            //var matches = (from k in index.Keys
            //               where k.ToLower() == columnName.ToLower() && columnName.Length > 0
            //               select k).ToList();


            //return matches.Count > 0;
            return index.ContainsKey(columnName);
        }
    }
}
