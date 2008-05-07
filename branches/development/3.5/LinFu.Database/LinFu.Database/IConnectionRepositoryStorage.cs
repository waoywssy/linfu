using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Database
{
    public interface IConnectionRepositoryStorage
    {        
        IConnectionRepository Retrieve();
        IConnectionRepositoryLoader LoadStrategy { get; set; }
    }
}
