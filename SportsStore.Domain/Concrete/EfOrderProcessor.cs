using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Identity.Concrete;

namespace SportsStore.Domain.Concrete
{
    /// <summary>
    /// Processes orders.
    /// </summary>
    public class EfOrderProcessor : IOrderProcessor
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private EmailSettings _emailSettings;
        private EfDbContext _context = new EfDbContext();

        /// <summary>
        /// Constructor for <c>EfOrderProcessor.</c>
        /// </summary>
        /// <param name="emailSettings"></param>
        public EfOrderProcessor(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        /// <summary>
        /// Property for Current User.
        /// </summary>
        /// <returns>Current user.</returns>
        private User CurrentUserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>().FindById(HttpContext.Current.User.Identity.GetUserId());
            }
        }

        /// <summary>
        /// Property for Orders.
        /// </summary>
        /// <returns>Orders from "SportsStore" database.</returns>
        public IEnumerable<Order> Orders
        {
            get
            {
                return _context.Orders;
            }
        }

        /// <summary>
        /// Dends the email message with info to client.
        /// </summary>
        /// <param name="cart">A <c>Cart</c>.</param>
        /// <param name="shippingDetails"><c>ShippingDetails</c>.</param>
        public void SendEmail(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = _emailSettings.ServerName;
                smtpClient.Port = _emailSettings.ServerPort;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.UserLogin, _emailSettings.UserPassword);

                StringBuilder body = new StringBuilder()
                    .AppendLine("New order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c})\n", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}\n", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine("Address:")
                    .AppendLine(shippingDetails.Address)
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.Country)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(_emailSettings.UserLogin,
                                                          shippingDetails.Email,
                                                          "new order submitted!",
                                                          body.ToString());
                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
                }
            }
        }

        /// <summary>
        /// Creates new Order.
        /// </summary>
        /// <param name="cart">A <c>Cart</c>.</param>
        /// <param name="shippingDetails"><c>ShippingDetails</c>.</param>
        /// <param name="order">An <c>Order</c>.</param>
        public void CreateOrder(Cart cart, ShippingDetails shippingDetails, Order order)
        {
            StringBuilder body = new StringBuilder();

            body.AppendFormat("Total order value: {0:c}\n", cart.ComputeTotalValue())
                .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

            order = new Order();
            order.Status = "Registered";
            order.Info = body.ToString();
            order.UserId = CurrentUserManager.Id;

            SaveOrder(order);
        }

        /// <summary>
        /// Saves Order in the "SportsStore" database.
        /// </summary>
        /// <param name="order">An <c>Order</c> to save.</param>
        public void SaveOrder(Order order)
        {
            if (order.OrderId == 0)
            {
                _context.Orders.Add(order);
            }
            else
            {
                Order dbEntry = _context.Orders.Find(order.OrderId);
                if (dbEntry != null)
                {
                    dbEntry.Status = order.Status;
                    dbEntry.Info = order.Info;
                }
            }
            _context.SaveChanges();
        }
    }
}