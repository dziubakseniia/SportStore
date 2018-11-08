using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Domain.Entities
{
    /// <summary>
    /// Manages cart.
    /// </summary>
    public class Cart
    {
        private List<CartLine> _lineCollection = new List<CartLine>();

        /// <summary>
        /// Property for Lines in the Cart.
        /// </summary>
        public IEnumerable<CartLine> Lines
        {
            get { return _lineCollection; }
        }

        /// <summary>
        /// Adds Products to Cart.
        /// </summary>
        /// <param name="product">A <c>Product</c> for adding.</param>
        /// <param name="quantity">A quantity of products for adding.</param>
        public void AddItem(Product product, int quantity)
        {
            CartLine line = _lineCollection.FirstOrDefault(p => p.Product.ProductId == product.ProductId);

            if (line == null)
            {
                _lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        /// <summary>
        /// Removes product from Cart.
        /// </summary>
        /// <param name="product">A <c>Product</c> in the cart.</param>
        public void RemoveLine(Product product)
        {
            _lineCollection.RemoveAll(p => p.Product.ProductId == product.ProductId);
        }

        /// <summary>
        /// Computes total value of Products in the Cart.
        /// </summary>
        /// <returns>Total Value of Products in the Cart.</returns>
        public decimal ComputeTotalValue()
        {
            return _lineCollection.Sum(p => p.Product.Price * p.Quantity);
        }

        /// <summary>
        /// Clears the Cart.
        /// </summary>
        public void Clear()
        {
            _lineCollection.Clear();
        }
    }
}