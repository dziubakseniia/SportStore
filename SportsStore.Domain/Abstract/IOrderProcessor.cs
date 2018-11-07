using System.Collections.Generic;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IOrderProcessor
    {
        IEnumerable<Order> Orders { get; }
        void ProcessOrder(Cart cart, ShippingDetails shippingDetails, Order order);
        void SaveOrder(Order order);
    }
}