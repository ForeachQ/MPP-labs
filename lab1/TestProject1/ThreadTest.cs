using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using MainPart;
using NUnit.Framework;
using TracerPart;

namespace TestProject1
{
    class ThreadTest
    {
        readonly ITracer _tracer = new Tracer();
        readonly int _threadsCount = 10;
        readonly List<Thread> _threads = new List<Thread>();

        public void Method()
        {
            _tracer.StartTrace();
            Thread.Sleep(150);
            _tracer.StopTrace();
        }

        [Test]
        public void ThreadCount()
        {
            for (int i = 0; i < _threadsCount; i++)
            {
                _threads.Add(new Thread(Method));
            }

            foreach (Thread thread in _threads)
            {
                thread.Start();
            }

            foreach (Thread thread in _threads)
            {
                if (thread.IsAlive)
                    thread.Join();
            }

            TraceResult traceResult = _tracer.GetTraceResult();
            ConcurrentDictionary<int, ThreadTrace> result = traceResult.GetThreadTraces();

            foreach (ThreadTrace trace in result.Values)
                Assert.AreEqual(trace.ThreadTime >= 150 && trace.ThreadTime <= 150, true, "Thread is expected to last 150ms, was: " + trace.ThreadTime);
        }

    }
}
