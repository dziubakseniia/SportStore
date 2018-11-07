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
    [Authorize]
    public class CartController : Controller
    {
        private IProductRepository _productRepository;
        private IOrderProcessor _orderProcessor;

        private User CurrentUserManager
        {
            get { return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); }
        }

        public CartController(IProductRepository productRepository, IOrderProcessor orderProcessor)
        {
            _productRepository = productRepository;
            _orderProcessor = orderProcessor;
        }

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

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            Product product = _productRepository.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                cart.RemoveLine(product);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

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
                cart.Clear();
                return View("Completed");
            }
            return View(shippingDetails);
        }

        public ViewResult UserOrders()
        {
            IEnumerable<Order> orders = _orderProcessor.Orders.Where(o => o.UserId == CurrentUserManager.Id);
            return View(orders);
        }
    }
}