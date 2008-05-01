using LinFu.Persist.Metadata;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    [Implements(typeof(ITableRepositoryLoader), LifecycleType.Singleton, ServiceName = "DefaultTableRepositoryLoader")]
    public class DefaultTableRepositoryLoader : ITableRepositoryLoader
    {
     
        #region ITableRepositoryLoader Members

        public ITableRepository Load(string repositoryName)
        {                        
            var path = Path.Combine(Path.GetDirectoryName(typeof(ITableRepositoryPersister).Assembly.Location),
               string.Format("{0}.dat", repositoryName));
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (ITableRepository)formatter.Deserialize(fileStream);
            }
        }

        #endregion
    }
}
