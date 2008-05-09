    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.Persist
{
    [Implements(typeof(IRowLoader),LifecycleType.OncePerRequest)]
    public class RowLoader : IRowLoader, IInitialize
    {             
        private IRowLoadStrategyProvider _strategyProvider = null;

        #region IInitialize Members

        public void Initialize(IContainer container)
        {        
            _strategyProvider = container.GetService<IRowLoadStrategyProvider>();            
        }

        #endregion

        #region IRowLoader Members

        public IEnumerable<IRow> CreateRows(IEnumerable<IRowTaskItem> tasks)
        {

            if (tasks.Count() == 0)
                return null;
            
            List<IRow> rows = new List<IRow>();

            var tableGroups = tasks.GroupBy(t => t.TableName);

            foreach (var tableGroup in tableGroups)
            {
                IRowLoadStrategy loadStrategy = _strategyProvider.GetStrategy(tableGroup.Key,tableGroup.Count(),
                    tableGroup.First().PrimaryKeyValues.Select(p => p.Key));

                rows.AddRange(loadStrategy.Load(tableGroup));                
            }
            return rows;

        }

        #endregion

    }
}
