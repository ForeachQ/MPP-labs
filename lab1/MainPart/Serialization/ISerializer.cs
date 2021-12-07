using System;
using System.Collections.Generic;
using System.Text;

namespace TracerPart.Serialization
{
    interface ISerializer
    {
        string Serialize(TraceResult traceResult);
    }
}
