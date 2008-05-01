using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using Simple.IoC;
using LinFu.Persist.Metadata;
using System.Data.SqlClient;
namespace LinFu.Persist.Metadata.Test
{
    /// <summary>
    /// A customizer "HACK" to procide the repository loader with a connection to the database
    /// </summary>
    [Customizer]
    public class ConnectionCustomizer : ICustomizeInstance
    {

        #region ICustomizeInstance Members

        public bool CanCustomize(string serviceName, Type serviceType, IContainer hostContainer)
        {
            return typeof(IDbTableRepositoryLoader).IsAssignableFrom(serviceType);
        }

        public void Customize(string serviceName, Type serviceType, object instance, IContainer hostContainer)
        {
            ((IDbTableRepositoryLoader)instance).Connection = new SqlConnection("Data Source=.;Initial Catalog=Northwind;Integrated Security=True");
        }

        #endregion
    }
}
