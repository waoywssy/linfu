using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace LinFu.Database
{
    /// <summary>
    /// Represents provider spesific connection information 
    /// </summary>
    public interface IConnectionInfo
    {
        /// <summary>
        /// The name of the connection   
        /// </summary>
        string Name{get;set;}
        
        /// <summary>
        /// The invariant name of the provider.
        /// </summary>        
        string ProviderName { get; set; }
        
        /// <summary>
        /// The connectionstring that is used to access the database
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Creates a new <see cref="DbProviderFactory"/> based on the <see cref="IConnectionInfo.ProviderName"/>
        /// </summary>
        /// <returns></returns>
        DbProviderFactory CreateProviderFactory();
    }
}
