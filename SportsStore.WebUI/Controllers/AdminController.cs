using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    /// <summary>
    /// Controller for managing administrators' functions.
    /// </summary>
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private IProductRepository _productRepository;
        private IOrderProcessor _orderProcessor;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor for <c>AdminController</c>.
        /// </summary>
        /// <param name="productRepository">Products Repository for managing Products.</param>
        /// <param name="orderProcessor">Orders Repository for managing Orders.</param>
        public AdminController(IProductRepository productRepository, IOrderProcessor orderProcessor)
        {
            _productRepository = productRepository;
            _orderProcessor = orderProcessor;
        }

        /// <summary>
        /// Shows Products on Administrator's page.
        /// </summary>
        /// <returns>View of all Products.</returns>
        public ViewResult Index()
        {
            ViewBag.MenuType = "Products";
            return View(_productRepository.Products);
        }

        /// <summary>
        /// Method for editing Product.
        /// </summary>
        /// <param name="productId">int ProductId for editing.</param>
        /// <returns>View of product for editing.</returns>
        public ViewResult Edit(int productId)
        {
            Product product = _productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                return View(product);
            }

            return View("Index");
        }

        /// <summary>
        /// PostBack Method for editing Products.
        /// </summary>
        /// <param name="product"><c>Product</c> for editing.</param>
        /// <param name="image">Image of <c>Product</c> for editing.</param>
        /// <returns>Main Page for Administrator if ModelState is valid.</returns>
        /// <returns>View of Product if ModelState is not valid.</returns>
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
                _logger.Info($"{product.Name} was changed.");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        /// <summary>
        /// Creates new Product.
        /// </summary>
        /// <returns>View for creating new Product.</returns>
        public ViewResult Create()
        {
            _logger.Info("New product was created.");
            return View("Edit", new Product());
        }

        /// <summary>
        /// PostBack method for deleting Product.
        /// </summary>
        /// <param name="productId">int ProductId for deleting.</param>
        /// <returns>Main Page for Administrator.</returns>
        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = _productRepository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = $"{deletedProduct.Name} was deleted";
                _logger.Info($"{deletedProduct.Name} was deleted.");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Orders Page.
        /// </summary>
        /// <returns>View of all Orders.</returns>
        public ViewResult Orders()
        {
            ViewBag.MenuType = "Orders";

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem {Text = @"Registered", Value = "Registered"},
                new SelectListItem {Text = @"Paid", Value = "Paid"},
                new SelectListItem {Text = @"Canceled", Value = "Canceled"}
            };
            ViewBag.status = items;

            return View(_orderProcessor.Orders);
        }

        /// <summary>
        /// PostBack method that Saves Status of Order.
        /// </summary>
        /// <param name="orderId">int OrderId for saving status.</param>
        /// <param name="status">string Value of status.</param>
        /// <returns>Redirects to Action "Orders".</returns>
        [HttpPost]
        public ActionResult ChangeStatus(int orderId, string status)
        {
            Order order = _orderProcessor.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                _orderProcessor.SaveOrder(order);
                TempData["message"] = "Order status was changed";
                _logger.Info($"Orders' №{orderId} status was changed.");
            }

            return Redirect("Orders");
        }
    }
}