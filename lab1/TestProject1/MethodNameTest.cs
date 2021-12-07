using System;
using System.Collections.Generic;
using System.Text;
using MainPart;
using TracerPart;
using NUnit.Framework;
using System.Threading;

namespace TestProject1
{
    class MethodNameTest
    {

        [Test]
        public void NamesCheck()
        {
            ITracer tracer1 = new Tracer();
            Method1(tracer1);

            ITracer tracer2 = new Tracer();
            Method1(tracer2);

            ITracer tracer3 = new Tracer();
            AnotherMethod(tracer3);


            string name1 = tracer1.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0].MethodName;
            Assert.AreEqual(name1, "Method1", "Method has another name.");

            string name2 = tracer2.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0].MethodName;
            Assert.AreEqual(name2, "Method1", "Method has another name.");

            string name3 = tracer3.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0].MethodName;
            Assert.AreEqual(name3, "AnotherMethod", "Method has another name.");
        }

        public void Method1(ITracer tracer)
        {
            tracer.StartTrace();
            tracer.StopTrace();
        }

        public void AnotherMethod(ITracer tracer)
        {
            tracer.StartTrace();
            tracer.StopTrace();
        }

    }
}
