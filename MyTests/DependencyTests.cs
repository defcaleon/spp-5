using System;
using System.Linq;
using Business;
using TestClasses;
using Xunit;

namespace ContainerTests
{
    public class DependencyTests
    {
        private DependenciesConfiguration dependencies { get; }

        private DependencyProvider provider { get; }

        public DependencyTests()
        {
            dependencies = new DependenciesConfiguration();
            dependencies.AddSingleton<IService, ServiceImpl1>();
            dependencies.AddTransient<Class1, Class1>();
            dependencies.AddTransient<IService, ServiceImpl2>();
            dependencies.AddTransient<IGenericService<int>, GenericServiceImpl<int>>();
            dependencies.AddTransient(typeof(IGenericService<>), typeof(GenericServiceImpl<>));
            dependencies.AddTransient<AbstractService, AbstractServiceImpl>();
            provider = new DependencyProvider(dependencies);
        }

        [Fact]
        public void ResolvingDependencies_MockedConfig_CorrectlyResolved()
        {
            var resolvedCollection = provider.ResolveAll<IService>();
            var resolvedService = provider.Resolve<IService>();
            var resolvedAbstractService = provider.Resolve<AbstractService>();
            var resolvedSelf = provider.Resolve<Class1>();
            var resolvedGenericService = provider.Resolve<IGenericService<int>>();
            var openGeneric = provider.Resolve<IGenericService<float>>();

            Assert.Equal(2, resolvedCollection.Count());
            Assert.Equal(typeof(ServiceImpl1), resolvedService.GetType());
            Assert.Equal(typeof(AbstractServiceImpl), resolvedAbstractService.GetType());
            Assert.Equal(typeof(Class1), resolvedSelf.GetType());
            Assert.Equal(typeof(GenericServiceImpl<int>), resolvedGenericService.GetType());
            Assert.Equal(typeof(GenericServiceImpl<float>), openGeneric.GetType());
        }

        [Fact]
        public void ResolvingDependenciesForAClass_MockedConfig_CorrectlyResolved()
        {
            var containerizedClass = provider.Resolve<ClassWithDependencies>();
            Assert.NotNull(containerizedClass.Service);
            Assert.NotNull(containerizedClass.GenericService);
        }

        [Fact]
        public void ChangingUsedConfig_MockedConfig_ExceptionThrown()
        {
            Assert.Throws<Exception>(() => dependencies.AddSingleton<Class1, Class1>());
        }

        [Fact]
        public void CreatingSingleton_MockedConfig_ShouldBeOneImplementation()
        {
            const int testCount = 100;
            Assert.Single(Enumerable.Range(1, testCount).Select(_ => provider.Resolve<IService>()).Distinct());
        }

        [Fact]
        public void CreatingTransient_MockedConfig_ShouldBeDifferentImplementations()
        {
            const int testCount = 100;
            Assert.Equal(testCount, Enumerable.Range(1, testCount).Select(_ => provider.Resolve<AbstractService>()).Distinct().Count());
        }
    }
}