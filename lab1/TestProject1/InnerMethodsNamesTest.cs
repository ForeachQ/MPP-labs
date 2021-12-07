using NUnit.Framework;
using MainPart;
using System.Threading;
using TracerPart;

namespace TestProject1
{
    public class Tests
    {
            private readonly ITracer _tracer = new Tracer();

            [Test]
            public void InnerMethods()
            {
                _tracer.StartTrace();
                Method1();
                Method2();
                Method3();
                _tracer.StopTrace();

                var methodInfoArr = _tracer.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0];

                Assert.AreEqual("Method1", methodInfoArr.InnerMethods[0].MethodName);
                Assert.AreEqual("Method2", methodInfoArr.InnerMethods[1].MethodName);
                Assert.AreEqual("Method4", methodInfoArr.InnerMethods[1].InnerMethods[0].MethodName);
                Assert.AreEqual("Method3", methodInfoArr.InnerMethods[2].MethodName);
            }

            public void Method1()
            {
                _tracer.StartTrace();
                _tracer.StopTrace();
            }
            public void Method2()
            {
                _tracer.StartTrace();
                Method4();
                _tracer.StopTrace();
            }
            public void Method3()
            {
                _tracer.StartTrace();
                _tracer.StopTrace();
            }
            public void Method4()
            {
                _tracer.StartTrace();
                _tracer.StopTrace();
            }
        
    }
}