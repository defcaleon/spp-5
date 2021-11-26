using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Business
{
    public class DependencyProvider
    {
        private readonly DependenciesConfiguration config;

        public DependencyProvider(DependenciesConfiguration config)
        {
            this.config = config;
            config.IsUsed = true;
            foreach (var record in config.Map.Where(r => r.IsSingleton))
            {
                record.SingletonImplementation = Resolve(record.ImplementationType);
            }
        }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).Cast<T>();
        }

        private object Resolve(Type dependency)
        {
            return config.Map.Select(record => ResolveFromRecord(record, dependency)).FirstOrDefault(result => result != null) ?? GenerateObject(dependency);
        }

        private IEnumerable<object> ResolveAll(Type dependency)
        {
            return config.Map.Select(record => ResolveFromRecord(record, dependency)).Where(result => result != null);
        }

        private object ResolveFromRecord(Record record, Type dependency)
        {
            if (dependency == record.DependencyType)
            {
                if (record.IsSingleton && record.SingletonImplementation != null)
                {
                    return record.SingletonImplementation;
                }
                return GenerateObject(record.ImplementationType);
            }
            if (record.DependencyType.IsGenericTypeDefinition && dependency.IsGenericType && dependency.GetGenericTypeDefinition() == record.DependencyType)
            {
                if (record.IsSingleton && record.SingletonImplementation != null)
                {
                    return record.SingletonImplementation;
                }
                return GenerateObject(record.ImplementationType.MakeGenericType(dependency.GetGenericArguments()));
            }

            return null;
        }

        private object GenerateObject(Type type)
        {
            var constructor = type.GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return constructor.Invoke(parameters.Select(p => p.ParameterType).Select(Resolve).ToArray());
        }
    }
}