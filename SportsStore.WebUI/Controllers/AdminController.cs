using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository _repository;
        private IOrderProcessor _orderProcessor;

        public AdminController(IProductRepository repository, IOrderProcessor orderProcessor)
        {
            _repository = repository;
            _orderProcessor = orderProcessor;
        }

        public ViewResult Index()
        {
            return View(_repository.Products);
        }

        public ViewResult Edit(int productId)
        {
            Product product = _repository.Products.FirstOrDefault(p => p.ProductId == productId);
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
                _repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} changes has been saved", product.Name);
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = _repository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
            }

            return RedirectToAction("Index");
        }

        public ViewResult Orders()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem{Text = "registered", Value = "registered"});
            items.Add(new SelectListItem{Text = "paid", Value = "paid" });
            items.Add(new SelectListItem{Text = "canceled", Value = "canceled" });

            ViewBag.Status = items;

            return View(_orderProcessor.Orders);
        }

        public ActionResult SaveStatus(int orderId, string Status)
        {
            Order order = _orderProcessor.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = Status;
                _orderProcessor.SaveOrder(order);
                TempData["message"] = "Order status was changed";
            }

            return RedirectToAction("Orders");
        }
    }
}