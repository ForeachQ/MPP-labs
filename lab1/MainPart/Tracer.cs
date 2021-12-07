using MainPart;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace TracerPart
{
    public class Tracer : ITracer
    {

        static void Main(string[] args) { }

        private readonly TraceResult _traceResult;

        public Tracer()
        {
            _traceResult = new TraceResult(new ConcurrentDictionary<int, ThreadTrace>());
        }

        public TraceResult GetTraceResult()
        {
            return _traceResult;
        }

        public void StartTrace()
        {
            var threadTrace = _traceResult.GetThreadTrace(Thread.CurrentThread.ManagedThreadId);

            var stackTrace = new StackTrace();
            var path = stackTrace.ToString().Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            path[0] = "";
            var className = stackTrace.GetFrames()[1].GetMethod().ReflectedType.Name;
            var methodName = stackTrace.GetFrames()[1].GetMethod().Name;            

            threadTrace.AddMethod(methodName, className, string.Join("", path));
        }

        public void StopTrace()
        {
            var threadTrace = _traceResult.GetThreadTrace(Thread.CurrentThread.ManagedThreadId);

            var stackTrace = new StackTrace();
            
            var path = stackTrace.ToString().Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            path[0] = "";

            threadTrace.DeleteMethod(string.Join("", path));
        }
    }
}
