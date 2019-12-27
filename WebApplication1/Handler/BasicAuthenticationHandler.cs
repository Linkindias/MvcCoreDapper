using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        string[] Paths = new string[] { "/", "/LogIn" };

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //當不包含路徑下 且不含webapi，指檢查權限
            if (!Paths.Contains(Context.Request.Path.Value) && Context.Request.Path.Value.IndexOf("api") == -1)
            {
                string Name = Context.Session.GetString("Name");
                string Id = Context.Session.GetString("Id");

                if (Name != null && Id != null)
                {
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, Id),
                        new Claim(ClaimTypes.Name, Name),
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
                    return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name));
                }
                return AuthenticateResult.Fail("Signed Out!");
            }
            return null;
        }
    }
}