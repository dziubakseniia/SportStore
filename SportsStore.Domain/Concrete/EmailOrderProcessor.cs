using System.Net;
using System.Net.Mail;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings _emailSettings;

        public EmailOrderProcessor(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
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
            }
        }
    }
}