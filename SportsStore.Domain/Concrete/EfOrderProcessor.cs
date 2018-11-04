﻿using System.Collections.Generic;
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
    public class EfOrderProcessor : IOrderProcessor
    {
        private EmailSettings _emailSettings;
        private EfDbContext _context = new EfDbContext();

        public EfOrderProcessor(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public IEnumerable<Order> Orders
        {
            get
            {
                return _context.Orders;
            }
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails, Order order)
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
                smtpClient.Send(mailMessage);

                body = new StringBuilder();

                body.AppendFormat("Total order value: {0:c}\n", cart.ComputeTotalValue())
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name + "\n")
                    .AppendLine("Address:\n")
                    .AppendLine(shippingDetails.Address + "\n")
                    .AppendLine(shippingDetails.City + "\n")
                    .AppendLine(shippingDetails.Country + "\n")
                    .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

                order = new Order();
                order.Status = "Registered";
                order.Info = body.ToString();
                order.UserId = CurrentUserManager.Id;

                SaveOrder(order);
            }
        }

        public User CurrentUserManager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>().FindById(HttpContext.Current.User.Identity.GetUserId()); }
        }

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