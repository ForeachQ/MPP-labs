using System;
using System.Collections.Generic;
using System.Text;
using MainPart;
using TracerPart;
using NUnit.Framework;
using System.Threading;

namespace TestProject1
{
    class ClassNameTest
    {

        [Test]
        public void NamesCheck()
        {
            ITracer tracer1 = new Tracer();
            Method1(tracer1);

            ITracer tracer2 = new Tracer();
            AnotherClass anotherClass = new AnotherClass(tracer2);
            anotherClass.Method();

            string name1 = tracer1.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0].ClassName;
            Assert.AreEqual(name1, "ClassNameTest", "Method from class ClassName has another class name.");

            string name2 = tracer2.GetTraceResult().GetThreadTraces()[Thread.CurrentThread.ManagedThreadId].MethodsInfo[0].ClassName;
            Assert.AreEqual(name2, "AnotherClass", "Method from class AnotherClass has another class name.");
        }

        public void Method1(ITracer tracer)
        {
            tracer.StartTrace();
            tracer.StopTrace();
        }
       
    }
    class AnotherClass
    {
        public ITracer _tracer;

        public AnotherClass(ITracer tracer)
        {
            _tracer = tracer;
        }
        public void Method()
        {
            _tracer.StartTrace();
            _tracer.StopTrace();
        }
    }
}
