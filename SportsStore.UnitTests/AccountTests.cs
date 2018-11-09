using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void Can_Return_Login_View()
        {
            AccountController controller = new AccountController();

            ActionResult result = controller.Login(null);

            Assert.IsInstanceOfType(result, typeof(ActionResult));
        }
    }
}
