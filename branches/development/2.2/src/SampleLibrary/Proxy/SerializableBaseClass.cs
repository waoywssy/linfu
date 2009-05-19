using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SampleLibrary.Proxy
{
    [Serializable]
    public class SerializableBaseClass : ISerializable
    {
        public SerializableBaseClass(SerializationInfo info, StreamingContext context)
        { 
        }
        public virtual void DoSomething()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
