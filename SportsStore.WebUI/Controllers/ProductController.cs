using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    /// <summary>
    /// Controller for Products managing.
    /// </summary>
    [Authorize]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public int PageSize = 4;

        /// <summary>
        /// Constructor for ProductController.
        /// </summary>
        /// <param name="productRepository">Product Repository for managing Products.</param>
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// View for Products List.
        /// </summary>
        /// <param name="category">String selected category.</param>
        /// <param name="sorting">String selected sorting type.</param>
        /// <param name="page">int page number from which to view List of Products.</param>
        /// <returns>View of ProductsViewModel</returns>
        public ViewResult List(string category, string sorting = null, int page = 1)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = SortedProducts(category, sorting, page),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? _productRepository.Products.Count() :
                        _productRepository.Products.Count(x => x.Category == category)
                },
                CurrentCategory = category,
                Sorting = sorting
            };

            return View(model);
        }

        /// <summary>
        /// Shows types of sorting.
        /// </summary>
        /// <param name="sorting">String selected Sorting type.</param>
        /// <param name="category">String selected sorting category.</param>
        /// <returns></returns>
        public PartialViewResult Sort(string sorting = null, string category = null)
        {
            ViewBag.Sorting = sorting;
            ViewBag.SelectedCategory = category;

            List<string> sortingTypes = new List<string>
            {
                "Sort from A to Z",
                "Sort from Z to A",
                "Price: Low to High",
                "Price: High to Low",
                "Newest Arrivals"
            };

            return PartialView("Sorting", sortingTypes);
        }

        /// <summary>
        /// Managing Products List in depend on sorting type.
        /// </summary>
        /// <param name="category">string selected category.</param>
        /// <param name="sorting">string selected Sorting type.</param>
        /// <param name="page">int page from which to view Products.</param>
        /// <returns>Sorted Products in depend on sorting type.</returns>
        /// <returns>Unsorted Products if there is no sorting type.</returns>
        public IEnumerable<Product> SortedProducts(string category, string sorting = null, int page = 1)
        {
            if (sorting == "Sort from A to Z")
            {
                return _productRepository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.Name)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize);
            }

            if (sorting == "Sort from Z to A")
            {
                return _productRepository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderByDescending(p => p.Name)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize);
            }

            if (sorting == "Price: Low to High")
            {
                return _productRepository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.Price)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize);
            }

            if (sorting == "Price: High to Low")
            {
                return _productRepository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderByDescending(p => p.Price)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize);
            }

            if (sorting == "Newest Arrivals")
            {
                return _productRepository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderByDescending(p => p.DateOfAddition)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize);
            }

            return _productRepository.Products
                 .Where(p => category == null || p.Category == category)
                 .OrderBy(p => p.ProductId)
                 .Skip((page - 1) * PageSize)
                 .Take(PageSize);
        }

        /// <summary>
        /// Gets Product image.
        /// </summary>
        /// <param name="productId">int ProductId of Product.</param>
        /// <returns>FileContentResult with Product image if it exists in the database.</returns>
        /// <returns>FileContentResult with default image if there is no image for this Product in the database.</returns>
        public FileContentResult GetImage(int productId)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/no-image-landscape.png");
            byte[] defaultImage = new byte[0];
            try
            {
                if (path != null)
                {
                    defaultImage = System.IO.File.ReadAllBytes(path);
                }

                throw new Exception();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
            }
            if (product != null && product.ImageData != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }

            return new FileContentResult(defaultImage, "image/png");
        }
    }
}