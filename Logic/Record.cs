using System;

namespace Business
{
    public class Record
    {
        public object SingletonImplementation { get; set; }

        public Type DependencyType { get; }

        public Type ImplementationType { get; }

        public bool IsSingleton { get;  }

        public Record(Type dependency, Type implementation, bool isSingleton)
        {
            DependencyType = dependency;
            ImplementationType = implementation;
            IsSingleton = isSingleton;
        }
    }
}