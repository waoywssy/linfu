using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist.Tests.AttributeMapping.SampleDomain
{
    [Peristable(TableName="AnotherOrdersTable")]
    public class OrderMappedToSpecificTable
    {
    }
}
