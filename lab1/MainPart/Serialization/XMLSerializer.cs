using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TracerPart;

namespace TracerPart.Serialization
{
    public class XMLSerializer : ISerializer
    {

        public string Serialize(TraceResult traceResult)
        {
            var data = traceResult.GetThreadTraces().Values.ToArray();
            //var xmlSerializ = new XmlSerializer();
            var xmlSerializer = new XmlSerializer(data.GetType());
            var stringWriter = new StringWriter();

            using (var writer = new System.Xml.XmlTextWriter(stringWriter))
            {
                writer.Formatting = System.Xml.Formatting.Indented;
                xmlSerializer.Serialize(writer, data);
            }
            var result = stringWriter.ToString().Replace("ArrayOfThread", "root");
            return result;
        }
    }
}
