using System.Web.Mvc;
using SportsStore.Domain.Entities;
using IModelBinder = System.Web.Mvc.IModelBinder;
using ModelBindingContext = System.Web.Mvc.ModelBindingContext;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string SessionKey = "Cart";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Cart cart = null;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (controllerContext.HttpContext.Session != null)
            {
                cart = (Cart)controllerContext.HttpContext.Session[SessionKey];
            }

            if (cart == null)
            {
                cart = new Cart();
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[SessionKey] = cart;
                }
            }

            return cart;
        }
    }
}