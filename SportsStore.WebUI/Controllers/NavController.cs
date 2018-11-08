using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;

namespace SportsStore.WebUI.Controllers
{
    /// <summary>
    /// Controller for Nav(Menu) managing.
    /// </summary>
    [Authorize]
    public class NavController : Controller
    {
        private IProductRepository _productRepository;

        /// <summary>
        /// Constructor for NavController.
        /// </summary>
        /// <param name="productRepository">Product Repository for managing Products.</param>
        public NavController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Shows Menu PartialView.
        /// </summary>
        /// <param name="category">string selected Product category for menu</param>
        /// <param name="sorting">String selected type of sorting.</param>
        /// <returns>Partial View of Menu.</returns>
        public PartialViewResult Menu(string category = null, string sorting = null)
        {
            ViewBag.SelectedCategory = category;
            ViewBag.Sorting = sorting;

            IEnumerable<string> categories = _productRepository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);

            return PartialView("FlexMenu", categories);
        }
    }
}