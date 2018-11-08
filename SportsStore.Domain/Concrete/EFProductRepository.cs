using System.Collections.Generic;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    /// <summary>
    /// Manages Products.
    /// </summary>
    public class EfProductRepository : IProductRepository
    {
        private EfDbContext _context = new EfDbContext();

        /// <summary>
        /// Property for Products.
        /// </summary>
        /// <returns>All Products from the "SportsStore" database.</returns>
        public IEnumerable<Product> Products
        {
            get { return _context.Products; }
        }

        /// <summary>
        /// Updates "SportsStore" database.
        /// </summary>
        public void UpdateProduct()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Saves "SportsStore" database.
        /// </summary>
        /// <param name="product">A <c>Product</c> to save.</param>
        public void SaveProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                Product dbEntry = _context.Products.Find(product.ProductId);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                    dbEntry.Description = product.Description;
                    dbEntry.DateOfAddition = product.DateOfAddition;
                    dbEntry.Quantity = product.Quantity;
                    dbEntry.ImageData = product.ImageData;
                    dbEntry.ImageMimeType = product.ImageMimeType;
                }
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes Product from "SportsStore" database.
        /// </summary>
        /// <param name="productId">ProductId for deleting.</param>
        /// <returns>Deleted Product.</returns>
        public Product DeleteProduct(int productId)
        {
            Product dbEntry = _context.Products.Find(productId);
            if (dbEntry != null)
            {
                _context.Products.Remove(dbEntry);
                _context.SaveChanges();
            }
            return dbEntry;
        }
    }
}