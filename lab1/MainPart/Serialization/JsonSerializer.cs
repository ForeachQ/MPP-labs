using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TracerPart.Serialization
{
    public class JsonSerializer : ISerializer
    {

        public string Serialize(TraceResult traceResult)
        {
            var arrays = new Dictionary<string, ICollection<ThreadTrace>>
            {
                {"threads", traceResult.GetThreadTraces().Values }
            };

            return JsonConvert.SerializeObject(arrays, Formatting.Indented);
        }

        
    }
}
