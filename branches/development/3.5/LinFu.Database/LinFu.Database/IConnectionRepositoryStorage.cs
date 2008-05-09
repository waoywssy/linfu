using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Database
{
    public interface IConnectionRepositoryStorage
    {
        void Load(IConnectionRepository repository);
        IConnectionRepositoryLoader LoadStrategy { get; set; }
    }
}
