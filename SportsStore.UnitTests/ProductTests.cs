using System;
using System.Collections.Generic;
using System.IO;
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
    public class ProductTests
    {
        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductId = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductId = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductId = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductId = 5, Name = "P5", Category = "Cat3"}
            });

            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };

            Product[] result = ((ProductsListViewModel)controller.List("Cat2").Model).Products.ToArray();
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"},
                new Product {ProductId = 2, Name = "P2", Category = "Apples"},
                new Product {ProductId = 3, Name = "P3", Category = "Plums"},
                new Product {ProductId = 4, Name = "P4", Category = "Oranges"}
            });

            NavController controller = new NavController(mock.Object);

            string[] results = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"},
                new Product {ProductId = 4, Name = "P2", Category = "Oranges"}
            });

            NavController controller = new NavController(mock.Object);
            string categoryToSelect = "Apples";

            string result = controller.Menu(categoryToSelect).ViewBag.SelectedCategory;
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductId = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductId = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductId = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductId = 5, Name = "P5", Category = "Cat3"}
            });

            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };

            int res1 = ((ProductsListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void Can_Create_Sorting_Partial_View()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            ProductController controller = new ProductController(mock.Object);

            PartialViewResult result = controller.Sort();

            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void Can_Return_Sorted_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            Product product1 = new Product
            { ProductId = 1, Name = "P1", Price = 1, DateOfAddition = Convert.ToDateTime("11-10-2018") };
            Product product2 = new Product
            { ProductId = 2, Name = "P2", Price = 3, DateOfAddition = Convert.ToDateTime("09-10-2018") };
            Product product3 = new Product
            { ProductId = 3, Name = "P3", Price = 4, DateOfAddition = Convert.ToDateTime("10-10-2018") };
            Product product4 = new Product
            { ProductId = 4, Name = "P4", Price = 2, DateOfAddition = Convert.ToDateTime("13-10-2018") };
            Product product5 = new Product
            { ProductId = 5, Name = "P5", Price = 5, DateOfAddition = Convert.ToDateTime("15-10-2018") };
            mock.Setup(m => m.Products).Returns(new[]
                {product1, product2, product3, product4, product5});

            ProductController controller = new ProductController(mock.Object);
            var result1 = mock.Object.Products.Where(p => p.Category == null)
                .OrderBy(p => p.Name)
                .Skip((1 - 1) * 4)
                .Take(4);

            var result2 = mock.Object.Products.Where(p => p.Category == null)
                .OrderByDescending(p => p.Name)
                .Skip((1 - 1) * 4)
                .Take(4);

            var result3 = mock.Object.Products.Where(p => p.Category == null)
                .OrderBy(p => p.Price)
                .Skip((1 - 1) * 4)
                .Take(4);

            var result4 = mock.Object.Products.Where(p => p.Category == null)
                .OrderByDescending(p => p.Price)
                .Skip((1 - 1) * 4)
                .Take(4);

            var result5 = mock.Object.Products.Where(p => p.Category == null)
                .OrderByDescending(p => p.DateOfAddition)
                .Skip((1 - 1) * 4)
                .Take(4);

            Assert.IsInstanceOfType(controller.SortedProducts(null), typeof(IEnumerable<Product>));
            CollectionAssert.AreEqual(result1.ToList(), controller.SortedProducts(null, "Sort from A to Z").ToList());
            CollectionAssert.AreEqual(result2.ToList(), controller.SortedProducts(null, "Sort from Z to A").ToList());
            CollectionAssert.AreEqual(result3.ToList(), controller.SortedProducts(null, "Price: Low to High").ToList());
            CollectionAssert.AreEqual(result4.ToList(), controller.SortedProducts(null, "Price: High to Low").ToList());
            CollectionAssert.AreEqual(result5.ToList(), controller.SortedProducts(null, "Newest Arrivals").ToList());
        }

        [TestMethod]
        public void Can_Get_Image()
        {
            byte[] productImage =
                File.ReadAllBytes(@"D:\C#_Projects\SportStore\SportsStore.WebUI\Content\download.jpg");

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductId = 1, Name = "P1", ImageData = productImage, ImageMimeType = "jpg"},
                new Product {ProductId = 2, Name = "P2"}
            });

            ProductController controller = new ProductController(mock.Object);

            Assert.IsInstanceOfType(controller.GetImage(1), typeof(FileContentResult));
            Assert.AreEqual(productImage, controller.GetImage(1).FileContents);
        }
    }
}