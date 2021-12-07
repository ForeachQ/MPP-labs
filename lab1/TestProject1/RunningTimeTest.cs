using NUnit.Framework;
using TracerPart;
using MainPart;
using System.Threading;

namespace TestProject1
{
    class RunningTimeTest
    {
        private readonly ITracer _tracer = new Tracer();

        [Test]
        public void EllapsedTime()
        {
            _tracer.StartTrace();
            Method1();
            Method2();
            Method3();
            _tracer.StopTrace();

            var methodInfoArr = _tracer.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0];
            long time1 = methodInfoArr.InnerMethods[0].GetElapsedTime();
            long time2 = methodInfoArr.InnerMethods[1].GetElapsedTime();
            long time4 = methodInfoArr.InnerMethods[1].InnerMethods[0].GetElapsedTime();
            long time3 = methodInfoArr.InnerMethods[2].GetElapsedTime();
            Assert.AreEqual(300<=time1&&time1<=400,true,"Method1 is expected to last 350ms.");
            Assert.AreEqual(1100<=time2&&time2<=1200,true,"Method2 is expected to last 450ms.");
            Assert.AreEqual(600 <= time4 && time4 <= 700, true, "Method4 is expected to last 650ms.");
            Assert.AreEqual(500 <= time3 && time3 <= 600, true, "Method3 is expected to last 550ms.");
        }

        public void Method1()
        {
            _tracer.StartTrace();
            Thread.Sleep(450);
            _tracer.StopTrace();
        }
        public void Method2()
        {
            _tracer.StartTrace();
            Thread.Sleep(450);
            Method4();
            _tracer.StopTrace();
        }
        public void Method3()
        {
            _tracer.StartTrace();
            Thread.Sleep(550);
            _tracer.StopTrace();
        }
        public void Method4()
        {
            _tracer.StartTrace();
            Thread.Sleep(650);
            _tracer.StopTrace();
        }
    }
}
