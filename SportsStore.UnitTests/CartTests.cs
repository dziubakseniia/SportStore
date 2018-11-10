using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Cart_Add_New_Lines()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart cart = new Cart();
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);

            CartLine[] cartLines = cart.Lines.ToArray();

            Assert.AreEqual(cartLines.Length, 2);
            Assert.AreEqual(cartLines[0].Product, p1);
            Assert.AreEqual(cartLines[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_To_Existing_Lines()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart cart = new Cart();
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 10);

            CartLine[] cartLines = cart.Lines.OrderBy(p => p.Product.ProductId).ToArray();

            Assert.AreEqual(cartLines.Length, 2);
            Assert.AreEqual(cartLines[0].Quantity, 11);
            Assert.AreEqual(cartLines[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };
            Product p3 = new Product { ProductId = 3, Name = "P3" };

            Cart cart = new Cart();
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 3);
            cart.AddItem(p3, 5);
            cart.AddItem(p2, 1);

            cart.RemoveLine(p2);

            Assert.AreEqual(cart.Lines.Count(p => p.Product == p2), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Can_Remove_From_Cart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products)
                .Returns(new[]
                {
                    new Product { ProductId = 1, Name = "P1"},
                    new Product { ProductId = 2, Name = "P2"}
                }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);
            controller.AddToCart(cart, 1, null);
            controller.AddToCart(cart, 2, null);
            controller.RemoveFromCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductId, 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            Cart cart = new Cart();
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 3);

            decimal total = cart.ComputeTotalValue();

            Assert.AreEqual(total, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            Cart cart = new Cart();
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);

            cart.Clear();

            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products)
                .Returns(new[] { new Product { ProductId = 1, Name = "P1", Category = "Apples" } }.AsQueryable());

            Cart cart = new Cart();
            CartController controller = new CartController(mock.Object, null);
            controller.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductId, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products)
                .Returns(new[] { new Product { ProductId = 1, Name = "P1", Category = "Apples" } }.AsQueryable());

            Cart cart = new Cart();
            CartController controller = new CartController(mock.Object, null);
            RedirectToRouteResult result = controller.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            Cart cart = new Cart();
            CartController controller = new CartController(null, null);

            CartIndexViewModel result = (CartIndexViewModel)controller.Index(cart, "myUrl").ViewData.Model;

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            Order order = new Order();
            CartController controller = new CartController(null, mock.Object);

            ViewResult result = controller.Checkout(cart, shippingDetails, order);
            mock.Verify(m => m.SendEmail(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController controller = new CartController(null, mock.Object);
            controller.ModelState.AddModelError("error", @"error");

            ViewResult result = controller.Checkout(cart, new ShippingDetails(), new Order());
            mock.Verify(m => m.SendEmail(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Mock<IProductRepository> mockProduct = new Mock<IProductRepository>();

            Product product = new Product { ProductId = 1, Name = "P1", Quantity = 1 };
            mockProduct.Setup(m => m.Products)
                .Returns(new[] { product }.AsQueryable());

            Cart cart = new Cart();
            cart.AddItem(product, 1);

            CartController controller = new CartController(mockProduct.Object, mock.Object);

            ViewResult result = controller.Checkout(cart, new ShippingDetails(), new Order());
            mock.Verify(m => m.SendEmail(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);
            mock.Verify(m => m.CreateOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>(), It.IsAny<Order>()), Times.Once);

            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
            Assert.AreEqual(0, cart.Lines.Count());
            Assert.AreEqual(0, product.Quantity);
        }

        [TestMethod]
        public void Can_Summarize()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController controller = new CartController(mock.Object, null);
            PartialViewResult partialView = controller.Summary(cart);

            Assert.IsInstanceOfType(partialView, typeof(PartialViewResult));
        }

        [TestMethod]
        public void Can_Return_Checkout_View()
        {
            CartController controller = new CartController(null, null);

            ViewResult viewResult = controller.Checkout();

            Assert.IsInstanceOfType(viewResult, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Return_UserOrders_View()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            mock.Setup(m => m.Orders).Returns(new[] { new Order { OrderId = 1, UserId = "UserId" } });
            CartController controller = new CartController(null, mock.Object);
            ViewResult viewResult = controller.UserOrders();

            Assert.IsInstanceOfType(viewResult, typeof(ViewResult));
            Assert.AreEqual(1, mock.Object.Orders.Count());
            Assert.IsInstanceOfType(mock.Object.Orders, typeof(IEnumerable<Order>));
            Assert.AreEqual("UserId", mock.Object.Orders.ToArray()[0].UserId);
        }
    }
}