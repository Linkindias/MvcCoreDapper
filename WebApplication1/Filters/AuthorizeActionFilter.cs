﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Filters
{
    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        string[] Paths = new string[] { "/", "/LogIn", "/LogIn/In" };

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //當不包含路徑下，指檢查權限
            if (!Paths.Contains(context.HttpContext.Request.Path.Value))
            {
                ISession session = context.HttpContext.Session;

                string Account = session.GetString("Account");
                if (string.IsNullOrEmpty(Account))
                {
                    context.Result = new RedirectResult("/LogIn");
                }
            }
        }
    }
}