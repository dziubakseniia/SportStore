using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private IProductRepository _productRepository;
        private IOrderProcessor _orderProcessor;

        public AdminController(IProductRepository productRepository, IOrderProcessor orderProcessor)
        {
            _productRepository = productRepository;
            _orderProcessor = orderProcessor;
        }

        public ViewResult Index()
        {
            ViewBag.MenuType = "Products";
            return View(_productRepository.Products);
        }

        public ViewResult Edit(int productId)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                _productRepository.SaveProduct(product);
                TempData["message"] = $"{product.Name} changes has been saved";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = _productRepository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = $"{deletedProduct.Name} was deleted";
            }

            return RedirectToAction("Index");
        }

        public ViewResult Orders()
        {
            ViewBag.MenuType = "Orders";

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem {Text = @"registered", Value = "registered"},
                new SelectListItem {Text = @"paid", Value = "paid"},
                new SelectListItem {Text = @"canceled", Value = "canceled"}
            };

            ViewBag.Status = items;

            return View(_orderProcessor.Orders);
        }

        public ActionResult SaveStatus(int orderId, string status)
        {
            Order order = _orderProcessor.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                _orderProcessor.SaveOrder(order);
                TempData["message"] = "Order status was changed";
            }

            return RedirectToAction("Orders");
        }
    }
}