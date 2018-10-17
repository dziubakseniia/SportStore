using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            _kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            _kernel.Bind<IProductRepository>().To<EFProductRepository>();

            /**If there are no db
            /*Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product{Name = "Football", Price = 25},
                new Product{Name = "Surf board", Price = 179},
                new Product{Name = "Running shoes", Price = 95}
            });

            _kernel.Bind<IProductRepository>().ToConstant(mock.Object);*/
        }
    }
}