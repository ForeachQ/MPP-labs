using NUnit.Framework;
using DependencyInjection.DependencyConfiguration;
using DependencyInjection.DependencyConfiguration.ImplementationData;
using DependencyInjection.DependencyProvider;
using LifeCycle = DependencyInjection.DependencyConfiguration.ImplementationData.LifeCycle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DependencyConfigTest
{
    public class Tests
    {
        [Test]
        public void RegisterTest()
        {
            var dependencies = new DependencyConfig();
            dependencies.Register<IInterface, Class>();
            dependencies.Register<IInterface, Class1>();
            dependencies.Register<InnerInterface, InnerClass>();
            bool v1 = dependencies.DependenciesDictionary.ContainsKey(typeof(IInterface));
            bool v2 = dependencies.DependenciesDictionary.ContainsKey(typeof(InnerInterface));
            var implType1 = dependencies.DependenciesDictionary[typeof(IInterface)][0].ImplementationsType;
            var implType2 = dependencies.DependenciesDictionary[typeof(IInterface)][1].ImplementationsType;
            int count = dependencies.DependenciesDictionary.Keys.Count;
            Assert.IsTrue(v1);
            Assert.IsTrue(v2);
            Assert.AreEqual(implType1, typeof(Class));
            Assert.AreEqual(implType2, typeof(Class1));
            Assert.AreEqual(count, 2);
        }

        [Test]
        public void FailedRegistrationTest()
        {
            var dependencies = new DependencyConfig();
            Assert.Throws<System.ArgumentException>(() => dependencies.Register(typeof(IInterface), typeof(SingleClass), LifeCycle.InstancePerDependency, ImplNumber.First));
        }

        [Test]
        public void GenericTest()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<IInterface, Class1>();
            dependencies.Register<IService<IInterface>, ServiceImpl<IInterface>>();

            int count = dependencies.DependenciesDictionary.Keys.Count;
            var impleType1 = dependencies.DependenciesDictionary[typeof(IInterface)][0].ImplementationsType;
            var impleType2 = dependencies.DependenciesDictionary[typeof(IService<IInterface>)][0].ImplementationsType;

            Assert.AreEqual(impleType1, typeof(Class1));
            Assert.AreEqual(impleType2, typeof(ServiceImpl<IInterface>));
            Assert.AreEqual(2, count);
        }

        [Test]
        public void EnumerableTest()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<IInterface, Class>();
            dependencies.Register<IInterface, Class1>();
            dependencies.Register<InnerInterface, InnerClass>();

            var list = provider.Resolve<IEnumerable<IInterface>>();
            var implType1 = list.ToList()[0].GetType();
            var implType2 = list.ToList()[1].GetType();

            Assert.AreEqual(implType1, typeof(Class));
            Assert.AreEqual(implType2, typeof(Class1));
        }

        [Test]
        public void TtlTest()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<IInterface, Class>(LifeCycle.InstancePerDependency, ImplNumber.First);
            dependencies.Register<IInterface, Class1>(LifeCycle.InstancePerDependency, ImplNumber.Second);
            dependencies.Register<InnerInterface, InnerClass>();

            IInterface link1 = provider.Resolve<IInterface>(ImplNumber.First);
            IInterface link2 = provider.Resolve<IInterface>(ImplNumber.First);

            Assert.IsFalse(link1 == link2);
        }

       
        [Test]
        public void SingletoneTest()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<IInterface, Class1>(LifeCycle.Singleton, ImplNumber.First);

            IInterface link1 = provider.Resolve<IInterface>(ImplNumber.First);
            IInterface link2 = provider.Resolve<IInterface>(ImplNumber.First);

            Assert.IsTrue(link1 == link2);
        }


        // A -> B -> A
        [Test]
        public void CircularTest1()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<IA, A>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IB, B>(LifeCycle.Singleton, ImplNumber.First);

            A a = (A)provider.Resolve<IA>(ImplNumber.First);
            B b = (B)provider.Resolve<IB>(ImplNumber.First);

            A ba = (A)b.ia;
            B ab = (B)a.ib;
            Assert.AreEqual(ab.ia, a);
            Assert.AreEqual(ba.ib, b);

            Assert.IsTrue(a.ib.GetType().Equals(typeof(B)));
            Assert.IsTrue(b.ia.GetType().Equals(typeof(A)));
        }

        //Q -> W -> E -> Q
        [Test]
        public void CircularTest2()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<IQ, Q>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IW, W>(LifeCycle.Singleton, ImplNumber.First);
            dependencies.Register<IE, E>(LifeCycle.Singleton, ImplNumber.First);

            Q q = (Q)provider.Resolve<IQ>(ImplNumber.First);
            W w = (W)provider.Resolve<IW>(ImplNumber.First);
            E e = (E)provider.Resolve<IE>(ImplNumber.Any);

            W qw = (W)q.iw;
            E we = (E)w.ie;
            Q eq = (Q)e.iq;

            Assert.AreEqual(qw.ie, e);
            Assert.AreEqual(we.iq, q);
            Assert.AreEqual(eq.iw, w);


            Assert.IsTrue(q.iw.GetType().Equals(typeof(W)));
            Assert.IsTrue(w.ie.GetType().Equals(typeof(E)));
            Assert.IsTrue(e.iq.GetType().Equals(typeof(Q)));
        }

        //Self -> Self
        [Test]
        public void CircularTest3()
        {
            var dependencies = new DependencyConfig();
            var provider = new DependencyProvider(dependencies);

            dependencies.Register<ISelf, Self>(LifeCycle.Singleton, ImplNumber.First);

            Self self = (Self)provider.Resolve<ISelf>(ImplNumber.First);

            Self ss = (Self)self.iself;

            Assert.AreEqual(self.iself, self);
            Assert.AreEqual(ss.iself, self);
            Assert.IsTrue(self.iself.GetType().Equals(typeof(Self)));
        }

        public interface ISelf
        {
            void met();
        }

        public class Self : ISelf
        {
            public ISelf iself { get; set; }
            public Self (ISelf self)
            {
                this.iself = self;
            }
            public void met()
            {
                throw new System.NotImplementedException();
            }
        }
        public interface IA
        {
            void met();
        }

        public class A : IA
        {
            public IB ib { get; set; }
            public A(IB ib)
            {
                this.ib = ib;
            }
            public void met()
            {
                throw new System.NotImplementedException();
            }
        }

        public interface IB
        {
            void met();
        }

        public class B : IB
        {
            public IA ia { get; set; }
            public B(IA ia)
            {
                this.ia = ia;
            }
            public void met()
            {
                throw new System.NotImplementedException();
            }
        }

        public interface IQ
        {
            void met();
        }

        public class Q : IQ
        {
            public IW iw { get; set; }
            public Q(IW iw)
            {
                this.iw = iw;
            }

            public void met()
            {
                throw new System.NotImplementedException();
            }
        }

        public interface IW
        {
            void met();
        }

        public class W : IW
        {
            public IE ie { get; set; }
            public W(IE ie)
            {
                this.ie = ie;
            }

            public void met()
            {
                throw new System.NotImplementedException();
            }
        }

        public interface IE
        {
            void met();
        }

        public class E : IE
        {
            public IQ iq { get; set; }
            public E(IQ iq)
            {
                this.iq = iq;
            }

            public void met()
            {
                throw new System.NotImplementedException();
            }
        }




        public interface IB
        {
            void met();
        }
        public interface IInterface
        {
            void met();
        }

        public class Class : IInterface
        {
            public InnerInterface inner;

            public Class(InnerInterface inner)
            {
                this.inner = inner;
            }
            public void met()
            {
                throw new System.NotImplementedException();
            }
        }

        public class Class1 : IInterface
        {
            public void met()
            {
                throw new System.NotImplementedException();
            }
        }
        public interface InnerInterface
        {
            void met();
        }

        public class InnerClass : InnerInterface
        {
            public void met()
            {
                throw new System.NotImplementedException();
            }
        }

        public interface IService<T> where T : IInterface
        {

        }

        public class ServiceImpl<T> : IService<T> where T : IInterface
        {
            public ServiceImpl(IInterface i)
            {

            }
        }


        public class SingleClass
        {

        }
    }
}