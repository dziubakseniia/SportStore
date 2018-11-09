using System.Collections.Generic;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IOrderProcessor
    {
        IEnumerable<Order> Orders { get; }
        void SendEmail(Cart cart, ShippingDetails shippingDetails);
        void CreateOrder(Cart cart, ShippingDetails shippingDetails, Order order);
        void SaveOrder(Order order);
    }
}