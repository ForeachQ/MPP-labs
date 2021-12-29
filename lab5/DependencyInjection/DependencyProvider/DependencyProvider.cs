using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependencyInjection.DependencyConfiguration;
using DependencyInjection.DependencyConfiguration.ImplementationData;
using Moq;

namespace DependencyInjection.DependencyProvider
{
    public class DependencyProvider
    {
        static object locker = new object();
        private readonly DependencyConfig configuration;
        public readonly Dictionary<Type, List<SingletonContainer>> singletonObjects;
        private readonly Stack<Type> cyclicStack = new Stack<Type>();
        private Dictionary<Type, Type> moqObjects = new Dictionary<Type, Type>();

        public DependencyProvider(DependencyConfig configuration)
        {
            ConfigValidator configValidator = new ConfigValidator(configuration);
            if (!configValidator.Validate())
            {
                throw new ArgumentException("Wrong configuration");
            }

            this.singletonObjects = new Dictionary<Type, List<SingletonContainer>>();
            this.configuration = configuration;
        }

        public TDependency Resolve<TDependency>(ImplNumber number = ImplNumber.Any)
            where TDependency : class
        {
            return (TDependency)Resolve(typeof(TDependency), number);
        }

        public object Resolve(Type dependencyType, ImplNumber number = ImplNumber.Any)
        {

            if (cyclicStack.Contains(dependencyType))
            {
                ////////////////////////////////////////////////////////////////////
                return null;
                ////////////////////////////////////////////////////////////////////
            }

            cyclicStack.Push(dependencyType);

            object result;
            if (this.IsIEnumerable(dependencyType))
            {
                result = this.CreateEnumerable(dependencyType.GetGenericArguments()[0]);
            }
            else
            {
                ImplContainer container = GetImplContainerByDependencyType(dependencyType, number);
                Type requiredType = GetGeneratedType(dependencyType, container.ImplementationsType);
                result = this.ResolveNonIEnumerable(requiredType, container.TimeToLive, dependencyType, container.ImplNumber);
            }

            return result;
        }

        private object ResolveNonIEnumerable(Type implType, LifeCycle ttl, Type dependencyType,
            ImplNumber number)
        {
            if (ttl != LifeCycle.Singleton)
            {
                return CreateInstance(dependencyType,implType);
            }

            if (!IsInSingletonObjects(dependencyType, implType, number))
            {
                lock (locker)
                {
                    if (!IsInSingletonObjects(dependencyType, implType, number))
                    {
                        var result = CreateInstance(dependencyType,implType);
                        this.AddToSingletonObjects(dependencyType, result, number);
                        cyclicStack.Pop();
                        ProcessMoqObjects(dependencyType);
                    }
                }
            }
            return this.singletonObjects[dependencyType]
                   .Find(singletonContainer => number.HasFlag(singletonContainer.ImplNumber)).Instance;
        }

        private void ProcessMoqObjects(Type replaceType)
        {
            foreach(KeyValuePair<Type, Type> keyValuePair in moqObjects)
            {
                if (replaceType.Equals(keyValuePair.Value))
                {
                    object moqObj = Resolve(keyValuePair.Key, ImplNumber.Any);
                    PropertyInfo[] propertyInfos = moqObj.GetType().GetProperties();
                    for(int i = 0; i < propertyInfos.Length; i++)
                    {
                        if (propertyInfos[i].PropertyType.Equals(keyValuePair.Value)){
                            cyclicStack.Pop();
                            object replaceObject = Resolve(replaceType, ImplNumber.Any);
                            moqObj.GetType().GetProperty(propertyInfos[i].Name).SetValue(moqObj, replaceObject);
                            break;
                        }
                    }
                }
            }
        }

        private object CreateInstance( Type dependecyType, Type implementationType)
        {
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var constructorParams = constructor.GetParameters();
                var generatedParams = new List<dynamic>();
                foreach (var parameterInfo in constructorParams)
                {
                    dynamic parameter;
                    if (parameterInfo.ParameterType.IsInterface)
                    {
                        var number = parameterInfo.GetCustomAttribute<DependencyKeyAttribute>()?.ImplNumber ?? ImplNumber.Any;
                        parameter = Resolve(parameterInfo.ParameterType, number);
                        if(parameter == null)
                        {
                            ///////////////////////////////////////////////////////////////////////
                            if (!moqObjects.ContainsKey(dependecyType))
                            {
                                moqObjects.Add(dependecyType, parameterInfo.ParameterType);
                            }
                            ///////////////////////////////////////////////////////////////////////
                        }
                    }
                    else
                    {
                        break;
                    }

                    generatedParams.Add(parameter);
                }

                return constructor.Invoke(generatedParams.ToArray());
            }

            throw new ArgumentException("Cannot create instance of class");
        }

        private ImplContainer GetImplContainerByDependencyType(Type dependencyType, ImplNumber number)
        {
            ImplContainer container;
            if (dependencyType.IsGenericType)
            {
                container = GetImplementationsContainerLast(dependencyType, number);
                container ??= GetImplementationsContainerLast(dependencyType.GetGenericTypeDefinition(), number);
            }
            else
            {
                container = GetImplementationsContainerLast(dependencyType, number);
            }

            return container;
        }

        private bool IsIEnumerable(Type dependencyType)
        {
            return dependencyType.GetInterfaces().Any(i => i.Name == "IEnumerable");
        }



        private Type GetGeneratedType(Type dependencyType, Type implementationType)
        {
            if (dependencyType.IsGenericType && implementationType.IsGenericTypeDefinition)
            {
                return implementationType.MakeGenericType(dependencyType.GetGenericArguments());
            }

            return implementationType;
        }

        private IList CreateEnumerable(Type dependencyType)
        {
            var implementationList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dependencyType));
            var implementationsContainers = this.configuration.DependenciesDictionary[dependencyType];
            foreach (var implementationContainer in implementationsContainers)
            {
                var instance = this.ResolveNonIEnumerable(implementationContainer.ImplementationsType,
                    implementationContainer.TimeToLive, dependencyType, implementationContainer.ImplNumber);
                implementationList.Add(instance);
            }

            return implementationList;
        }

        private ImplContainer GetImplementationsContainerLast(Type dependencyType, ImplNumber number)
        {
            if (this.configuration.DependenciesDictionary.ContainsKey(dependencyType))
            {
                return this.configuration.DependenciesDictionary[dependencyType]
                    .FindLast(container => number.HasFlag(container.ImplNumber));
            }

            return null;
        }

        private void AddToSingletonObjects(Type dependencyType, object implementation, ImplNumber number)
        {
            if (this.singletonObjects.ContainsKey(dependencyType))
            {
                this.singletonObjects[dependencyType].Add(new SingletonContainer(implementation, number));
            }
            else
            {
                this.singletonObjects.Add(dependencyType, new List<SingletonContainer>()
                {
                    new SingletonContainer(implementation, number)
                });
            }
        }

        private bool IsInSingletonObjects(Type dependencyType, Type implType, ImplNumber number)
        {
            var lst = this.singletonObjects.ContainsKey(dependencyType) ? this.singletonObjects[dependencyType] : null;
            return lst?.Find(container => number.HasFlag(container.ImplNumber) && container.Instance.GetType() == implType) is not null;
        }
    }
}