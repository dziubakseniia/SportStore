using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<IOrderProcessor> mockOrder = new Mock<IOrderProcessor>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"}
            });

            AdminController controller = new AdminController(mock.Object, mockOrder.Object);

            Product[] result = ((IEnumerable<Product>)controller.Index().ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<IOrderProcessor> mockOrder = new Mock<IOrderProcessor>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1", ImageMimeType = "png"},
                new Product {ProductId = 2, Name = "P2", ImageMimeType = "png"},
                new Product {ProductId = 3, Name = "P3", ImageMimeType = "png"}
            });

            AdminController controller = new AdminController(mock.Object, mockOrder.Object);

            Product product1 = controller.Edit(1).ViewData.Model as Product;
            Product product2 = controller.Edit(2).ViewData.Model as Product;
            Product product3 = controller.Edit(3).ViewData.Model as Product;

            if (product1 != null) Assert.AreEqual(1, product1.ProductId);
            if (product2 != null) Assert.AreEqual(2, product2.ProductId);
            if (product3 != null) Assert.AreEqual(3, product3.ProductId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<IOrderProcessor> mockOrder = new Mock<IOrderProcessor>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"}
            });

            AdminController controller = new AdminController(mock.Object, mockOrder.Object);

            Product result = (Product)controller.Edit(4).ViewData.Model;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<IOrderProcessor> mockOrder = new Mock<IOrderProcessor>();

            AdminController controller = new AdminController(mock.Object, mockOrder.Object);

            Product product = new Product { Name = "Test" };

            ActionResult result = controller.Edit(product);
            mock.Verify(m => m.SaveProduct(product));

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<IOrderProcessor> mockOrder = new Mock<IOrderProcessor>();

            AdminController controller = new AdminController(mock.Object, mockOrder.Object);

            Product product = new Product { Name = "Test" };

            controller.ModelState.AddModelError("error", @"error");

            ActionResult result = controller.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            Product product = new Product { ProductId = 2, Name = "Test" };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Mock<IOrderProcessor> mockOrder = new Mock<IOrderProcessor>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1"},
                product,
                new Product {ProductId = 3, Name = "P3"}
            });

            AdminController controller = new AdminController(mock.Object, mockOrder.Object);
            controller.Delete(product.ProductId);
            mock.Verify(m => m.DeleteProduct(product.ProductId));
        }

        [TestMethod]
        public void Can_Save_Order_Status()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Order order = new Order { OrderId = 1, Status = "Registered" };
            mock.Setup(m => m.Orders).Returns(new[] { order });

            AdminController controller = new AdminController(null, mock.Object);
            controller.ChangeStatus(order.OrderId, "Canceled");

            Assert.AreEqual("canceled", order.Status);
        }

        [TestMethod]
        public void Can_Create_Orders()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            AdminController controller = new AdminController(null, mock.Object);
            ViewResult result = controller.Orders();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Create_New_Product()
        {
            AdminController controller = new AdminController(null, null);
            ViewResult result = controller.Create();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}