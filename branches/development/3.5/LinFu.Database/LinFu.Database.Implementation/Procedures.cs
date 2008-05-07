using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IProcedures),LifecycleType.OncePerRequest)]
    public class Procedures : IProcedures,IInitialize
    {
        private IDictionary<string, IProcedure> _procedures = 
            new Dictionary<string,IProcedure>();
        private IContainer _container = null;

        #region IProcedures Members

        public IProcedure this[string procedureName]
        {
            get 
            {
                if (_procedures.ContainsKey(procedureName))
                    return _procedures[procedureName];
                else
                {
                    IProcedure procedure = _container.GetService<IProcedure>();
                    procedure.Name = procedureName;
                    procedure.Connection = Connection;                    
                    _procedures.Add(procedureName, procedure);
                }
                return _procedures[procedureName];
            }
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
        }

        #endregion

        #region IProcedures Members

        public IConnection Connection {get;set;}        

        #endregion

        #region IProcedures Members

        public ITransaction Transaction {get;set;}
       

        #endregion
    }
}
