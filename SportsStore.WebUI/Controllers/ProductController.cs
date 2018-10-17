using System.Web.Mvc;
using SportsStore.Domain.Abstract;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository _repository;

        public ProductController(IProductRepository productRepository)
        {
            _repository = productRepository;
        }

        public ViewResult List()
        {
            return View(_repository.Products);
        }
    }
}