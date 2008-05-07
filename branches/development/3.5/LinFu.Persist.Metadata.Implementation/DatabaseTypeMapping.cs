using System;
using System.Collections.Generic;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    /// <summary>
    /// Provides a basic database oriented implementation of the <see cref="ITypeMapper"/> interface. 
    /// </summary>
    [Implements(typeof(ITypeMapper), LifecycleType.Singleton, ServiceName = "DbTypeMapper")]
    public class DbTypeMapper : ITypeMapper
    {
        private readonly IDictionary<string, Type> _typeMap = new Dictionary<string, Type>();
        
        public DbTypeMapper()
        {
            CreateTypeMap();
        }

        public virtual Type MapType(string dataType, bool isNullable)
        {            
            string typeKey;
            
            if (isNullable)
                typeKey = string.Format("nullable {0}",dataType);
            else
                typeKey = dataType;
            
            if (_typeMap.ContainsKey(typeKey))
                return _typeMap[typeKey];
          
            //No type found, return the default
            return typeof(object);
        }

        private void CreateTypeMap()
        {
            _typeMap["nvarchar"] = typeof(string);
            _typeMap["nullable nvarchar"] = typeof(string);
            _typeMap["varchar"] = typeof(string);
            _typeMap["nullable varchar"] = typeof(string);
            _typeMap["char"] = typeof(string);
            _typeMap["nullable char"] = typeof(string);
            _typeMap["nchar"] = typeof(string);
            _typeMap["nullable nchar"] = typeof(string);
            _typeMap["datetime"] = typeof(DateTime);
            _typeMap["nullable datetime"] = typeof(DateTime?);
            _typeMap["bigint"] = typeof(long);
            _typeMap["nullable bigint"] = typeof(long?);
            _typeMap["binary"] = typeof(byte[]);
            _typeMap["nullable binary"] = typeof(byte?[]);
            _typeMap["bit"] = typeof(bool);
            _typeMap["nullable bit"] = typeof(bool?);
            _typeMap["decimal"] = typeof(decimal);
            _typeMap["nullable decimal"] = typeof(decimal?);
            _typeMap["float"] = typeof(double);
            _typeMap["nullable float"] = typeof(double?);
            _typeMap["image"] = typeof(byte[]);
            _typeMap["nullable image"] = typeof(byte?[]);
            _typeMap["int"] = typeof(int);
            _typeMap["nullable int"] = typeof(int?);
            _typeMap["money"] = typeof(decimal);
            _typeMap["nullable money"] = typeof(decimal?);
            _typeMap["real"] = typeof(Single);
            _typeMap["nullable real"] = typeof(Single?);
            _typeMap["nullable uniqueidentifier"] = typeof(Guid?);
            _typeMap["uniqueidentifier"] = typeof(Guid);
            _typeMap["smalldatetime"] = typeof(DateTime);
            _typeMap["nullable smalldatetime"] = typeof(DateTime?);
            _typeMap["smallint"] = typeof(short);
            _typeMap["nullable smallint"] = typeof(short?);
            _typeMap["smallmoney"] = typeof(decimal);
            _typeMap["nullable smallmoney"] = typeof(decimal?);
            _typeMap["text"] = typeof(decimal);
            _typeMap["smallmoney"] = typeof(decimal);
        }
    }
}
