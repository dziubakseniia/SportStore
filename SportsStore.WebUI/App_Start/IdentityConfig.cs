﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SportsStore.Domain.Identity.Concrete;

namespace SportsStore.WebUI.App_Start
{
    /// <summary>
    /// Configuration Settings for Identity.
    /// </summary>
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<EfIdentityDbContext>(EfIdentityDbContext.Create);
            app.CreatePerOwinContext<EfUserManager>(EfUserManager.Create);
            app.CreatePerOwinContext<EfRoleManager>(EfRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}