using System;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    [Serializable]
    [Implements(typeof(IRelationInfo),LifecycleType.OncePerRequest)]
    public class RelationInfo : IRelationInfo
    {
        #region IRelationInfo Members

        public IKeyInfo PrimaryKey {get;set;}
        
        public IKeyInfo ForeignKey{get;set;}
        
        #endregion
    }
}
