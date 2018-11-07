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
    [Authorize]
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public int PageSize = 4;

        public ProductController(IProductRepository productProductRepository)
        {
            _productRepository = productProductRepository;
        }

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

        public FileContentResult GetImage(int productId)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            byte[] defaultImage = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/Content/no-image-landscape.png"));
            if (product != null && product.ImageData != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }

            return new FileContentResult(defaultImage, "image/png");
        }
    }
}