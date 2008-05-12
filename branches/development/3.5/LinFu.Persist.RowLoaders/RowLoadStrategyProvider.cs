using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.Persist.RowLoaders
{
    [Implements(typeof(IRowLoadStrategyProvider), LifecycleType.Singleton)]
    public class RowLoadStrategyProvider : IRowLoadStrategyProvider, IInitialize
    {
        private IContainer _container;

        public RowLoadStrategyProvider()
        {
            BulkLoadThreshold = 100;
        }

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
        }

        #endregion


        #region IRowLoadStrategyProvider Members

        public IRowLoadStrategy GetStrategy(string tableName, long rowCount, IEnumerable<string> keyColumns)
        {
            if (rowCount == 1)
                return _container.GetService<IRowLoadStrategy>("SingleRowLoadStrategy");

            // BUG: The BulkRowStrategy doesn't seem to be working            
            //if (rowCount > 1 && keyColumns.Count() == 1 && rowCount < BulkLoadThreshold)
            //    return _container.GetService<IRowLoadStrategy>("MultiRowLoadStrategy");
                
            //return _container.GetService<IRowLoadStrategy>("BulkLoadRowStrategy");

            return _container.GetService<IRowLoadStrategy>("MultiRowLoadStrategy");
        }

        public long BulkLoadThreshold { get; set; }


        #endregion

    }
}