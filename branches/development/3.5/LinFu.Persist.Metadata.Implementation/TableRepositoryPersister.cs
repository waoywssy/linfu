using LinFu.Persist.Metadata;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    /// <summary>
    /// Provides a simple implementation of the <see cref="ITableRepositoryPersister"/> interface that 
    /// used binary serialization to persist the <see cref="ITableRepository"/>
    /// </summary>
    [Implements(typeof(ITableInfoRepositoryPersister),LifecycleType.Singleton)]
    public class TableRepositoryPersister : ITableInfoRepositoryPersister
    {
        #region ITableRepositoryPersister Members

        /// <summary>
        /// Saves the repository using the name of the repository as the filename.
        /// The file is stored in the same location as this assembly.
        /// </summary>
        /// <param name="repository"></param>
        public void Save(ITableInfoRepository repository)
        {
            var path = Path.Combine(Path.GetDirectoryName(typeof(ITableInfoRepositoryPersister).Assembly.Location), 
                string.Format("{0}.dat", repository.Name));
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, repository) ;
            }
        }

        #endregion
    }
}