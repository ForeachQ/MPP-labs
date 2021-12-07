using System;
using TracerPart;

namespace MainPart
{
    public interface ITracer
    {
            void StartTrace();
            void StopTrace();
            TraceResult GetTraceResult();   
    }
}
