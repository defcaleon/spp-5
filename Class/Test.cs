using System;

namespace TestClasses
{
    public interface IService
    {
        void BestMethod();
    }

    public interface IGenericService<T>
    {
        void BestMethod();
    }

    public abstract class AbstractService
    {
        public abstract void BestMethod();
    }

    public class GenericClass<T> : IService
    {
        public void BestMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceImpl1 : IService
    {
        public void BestMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceImpl2 : IService
    {
        public void BestMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class AbstractServiceImpl : AbstractService
    {
        public override void BestMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class GenericServiceImpl<T> : IGenericService<T>
    {
        public void BestMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class Class1
    {

    }

    public class ClassWithDependencies
    {
        public IService Service;

        public IGenericService<int> GenericService;

        public ClassWithDependencies(IService service, IGenericService<int> genericService)
        {
            Service = service;
            GenericService = genericService;
        }
    }
}