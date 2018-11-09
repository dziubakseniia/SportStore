using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    /// <summary>
    /// Resolves dependencies.
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Constructor for <c>NinjectDependencyResolver</c>.
        /// </summary>
        /// <param name="kernelParam">Parameter for kernel.</param>
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            _kernel = kernelParam;
            AddBindings();
        }

        /// <summary>
        /// Gets Service Type.
        /// </summary>
        /// <param name="serviceType"><c>Type</c> of service.</param>
        /// <returns>ServiceType.</returns>
        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        /// <summary>
        /// Gets all service types.
        /// </summary>
        /// <param name="serviceType"><c>Type</c> of service.</param>
        /// <returns>All services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Adds bindings between repositories and classes.
        /// </summary>
        private void AddBindings()
        {
            _kernel.Bind<IProductRepository>().To<EfProductRepository>();

            _kernel.Bind<IOrderProcessor>().To<EfOrderProcessor>()
                .WithConstructorArgument("settings", "emailSettings");
        }
    }
}