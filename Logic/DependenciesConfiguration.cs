using System;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
    public class DependenciesConfiguration
    {
        public List<Record> Map { get; }

        public bool IsUsed { get; set; } = false;

        public DependenciesConfiguration()
        {
            Map = new List<Record>();
        }

        public void AddTransient<T1, T2>()
            where T1 : class
            where T2 : class
        {
            Register(typeof(T1), typeof(T2));
        }

        public void AddSingleton<T1, T2>()
            where T1 : class
            where T2 : class
        {
            Register(typeof(T1), typeof(T2), true);
        }

        public void AddTransient(Type t1, Type t2)
        {
            Register(t1, t2);
        }

        public void AddSingleton(Type t1, Type t2)
        {
            Register(t1, t2, true);
        }

        private void Register(Type dependency, Type implementation, bool isSingleton = false)
        {
            if (IsUsed)
            {
                throw new Exception("It is not possible to edit used configuration");
            }

            lock (this)
            {
                if (RecordExists(dependency, implementation) || !IsCorrect(dependency, implementation, isSingleton))
                {
                    return;
                }

                Map.Add(new Record(dependency, implementation, isSingleton));
            }
        }

        private bool RecordExists(Type dependency, Type implementation) => Map.Any(record =>
            record.DependencyType == dependency && record.ImplementationType == implementation);

        private bool IsCorrect(Type dependency, Type implementation, bool isSingleton) =>
            dependency == implementation
            || implementation.GetInterfaces().Contains(dependency)
            || (dependency.IsGenericType
                && implementation.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == dependency.GetGenericTypeDefinition())
                && !isSingleton)
            || implementation.IsSubclassOf(dependency);
    }
}