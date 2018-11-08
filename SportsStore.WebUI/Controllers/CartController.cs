using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Identity.Concrete;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    /// <summary>
    /// Controller for Cart managing.
    /// </summary>
    [Authorize]
    public class CartController : Controller
    {
        private IProductRepository _productRepository;
        private IOrderProcessor _orderProcessor;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Property for Current User.
        /// </summary>
        private User CurrentUserManager
        {
            get { return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); }
        }

        /// <summary>
        /// Constructor for CartController.
        /// </summary>
        /// <param name="productRepository">Product Repository for managing Products.</param>
        /// <param name="orderProcessor">Order Repository for managing Orders.</param>
        public CartController(IProductRepository productRepository, IOrderProcessor orderProcessor)
        {
            _productRepository = productRepository;
            _orderProcessor = orderProcessor;
        }

        /// <summary>
        /// Adds Product to Cart.
        /// </summary>
        /// <param name="cart"><c>Cart</c> for adding to.</param>
        /// <param name="productId">int ProductId to add.</param>
        /// <param name="returnUrl">string Url for returning to.</param>
        /// <returns>Main Page of CartController.</returns>
        [Authorize(Roles = "Users")]
        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productRepository.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                cart.AddItem(product, 1);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        /// <summary>
        /// Removes Product from Cart.
        /// </summary>
        /// <param name="cart"><c>Cart</c> for removing Product from.</param>
        /// <param name="productId">ProductId for removing.</param>
        /// <param name="returnUrl">string Url for returning to.</param>
        /// <returns></returns>
        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productRepository.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                cart.RemoveLine(product);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        /// <summary>
        /// Shows Cart.
        /// </summary>
        /// <param name="cart">A <c>Cart</c> to return.</param>
        /// <param name="returnUrl">string Url for returning to.</param>
        /// <returns></returns>
        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        /// <summary>
        /// Summary of the Cart.
        /// </summary>
        /// <param name="cart"><c>Cart</c> for Summary.</param>
        /// <returns>Partial View of Cart's Summary.</returns>
        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        /// <summary>
        /// Order's checking out.
        /// </summary>
        /// <returns>View of Shipping Details.</returns>
        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        /// <summary>
        /// PostBack method for checking out.
        /// </summary>
        /// <param name="cart"><c>Cart</c> for checking out.</param>
        /// <param name="shippingDetails"><c>ShippingDetails</c> for checking out.</param>
        /// <param name="order"><c>Order</c> for checking out.</param>
        /// <returns>View of Completed Order if ModelState is valid.</returns>
        /// <returns>View of ShippingDetails if ModelState is not valid.</returns>
        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails, Order order)
        {
            if (!cart.Lines.Any())
            {
                ModelState.AddModelError("", @"Sorry, your cart is empty!");
            }

            if (ModelState.IsValid)
            {
                _orderProcessor.ProcessOrder(cart, shippingDetails, order);
                foreach (var line in cart.Lines)
                {
                    foreach (var product in _productRepository.Products)
                    {
                        if (product.ProductId == line.Product.ProductId)
                        {
                            product.Quantity -= line.Quantity;
                        }
                    }
                }
                _productRepository.UpdateProduct();
                _logger.Info($"Order №{order.OrderId} was created.");
                cart.Clear();
                return View("Completed");
            }
            return View(shippingDetails);
        }

        /// <summary>
        /// Page for User Orders.
        /// </summary>
        /// <returns>View of User Order.</returns>
        public ViewResult UserOrders()
        {
            IEnumerable<Order> orders = _orderProcessor.Orders.Where(o => o.UserId == CurrentUserManager.Id);
            return View(orders);
        }
    }
}