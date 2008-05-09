using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.Persist
{
    [Implements(typeof(IRowLoader),LifecycleType.OncePerRequest)]
    public class RowLoader : IRowLoader, IInitialize
    {
        private IDbConnection _connection = new SqlConnection("Data Source=.;Initial Catalog=RBUTV;Integrated Security=True");
        private IContainer _container = null;
        private IRowLoadStrategy _singleLoadStrategy = null;
        private IRowLoadStrategy _multiRowStrategy = null;

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
            _singleLoadStrategy = container.GetService<IRowLoadStrategy>("SingleRowLoadStrategy");
            _multiRowStrategy = container.GetService<IRowLoadStrategy>("MultiRowLoadStrategy");
        }

        #endregion


        #region IRowLoader Members

        public IEnumerable<IRow> CreateRows(IEnumerable<IRowTaskItem> tasks)
        {
            IRowLoadStrategy loadStrategy = null;

            var defaultResult = new IRow[0];
            int taskCount = tasks.Count();
            if (taskCount == 0)
                return defaultResult;

            loadStrategy = taskCount == 1 ? _singleLoadStrategy : _multiRowStrategy;
            
            if (loadStrategy != null)
                return loadStrategy.Load(tasks);

            return defaultResult;
        }
        #endregion
        
    }
}
