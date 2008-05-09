?using LinFu.Persist.Metadata;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Simple.IoC.Loaders;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace LinFu.Persist.Metadata.Implementation
{
    [Implements(typeof(ITableInfoRepositoryLoader), LifecycleType.Singleton, ServiceName = "CachedTableInfoRepositoryLoader")]
    public class CachedTableInfoRepositoryLoader : ITableInfoRepositoryLoader
    {
     
        #region ITableRepositoryLoader Members

        public void Load(string repositoryName, ITableInfoRepository repository)
        {                        
            var path = Path.Combine(Path.GetDirectoryName(typeof(ITableInfoRepositoryPersister).Assembly.Location),
               string.Format("{0}.dat", repositoryName));
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    ITableInfoRepository storedRepository = (ITableInfoRepository)formatter.Deserialize(fileStream);
                    foreach (var item in storedRepository.Tables)
                    {
                        repository.Tables.Add(item);
                    }
                    repository.Name = repositoryName;
                }
            }
        }

        #endregion
    }
}
using LinFu.Persist.Metadata;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Simple.IoC.Loaders;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace LinFu.Persist.Metadata.Implementation
{
    [Implements(typeof(ITableInfoRepositoryLoader), LifecycleType.Singleton, ServiceName = "CachedTableInfoRepositoryLoader")]
    public class CachedTableInfoRepositoryLoader : ITableInfoRepositoryLoader
    {
     
        #region ITableRepositoryLoader Members

        public void Load(string repositoryName, ITableInfoRepository repository)
        {                        
            var path = Path.Combine(Path.GetDirectoryName(typeof(ITableInfoRepositoryPersister).Assembly.Location),
               string.Format("{0}.dat", repositoryName));
            if (File.Exists(path))
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    ITableInfoRepository storedRepository = (ITableInfoRepository)formatter.Deserialize(fileStream);
                    foreach (var item in storedRepository.Tables)
                    {
                        repository.Tables.Add(item);
                    }
                    repository.Name = repositoryName;
                }
            }
        }

        #endregion
    }
}
