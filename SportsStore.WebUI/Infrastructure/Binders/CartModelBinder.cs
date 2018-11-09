using System.Web.Mvc;
using SportsStore.Domain.Entities;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    /// <summary>
    /// Manages Cart.
    /// </summary>
    public class CartModelBinder : IModelBinder
    {
        private const string SessionKey = "Cart";

        /// <summary>
        /// Binds Cart with current session.
        /// </summary>
        /// <param name="controllerContext"><c>ControllerContext</c>.</param>
        /// <param name="bindingContext"><c>ModelBindingContext</c> for model to bind.</param>
        /// <returns>Cart.</returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Cart cart = null;
            if (controllerContext.HttpContext.Session != null)
            {
                cart = (Cart)controllerContext.HttpContext.Session[SessionKey];
            }

            if (cart == null)
            {
                cart = new Cart();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[SessionKey] = cart;
                }
            }

            return cart;
        }
    }
}