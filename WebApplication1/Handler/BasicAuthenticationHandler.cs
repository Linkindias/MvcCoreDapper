using BLL.Model;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using static Base.Enums;

namespace WebApplication1
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        string[] Paths = new string[] { "/", "/LogIn" };
        IConfiguration config;
        AuthenticationRepository AuthRep;

        public BasicAuthenticationHandler(
            IConfiguration configuration,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            AuthenticationRepository authenticationRepository
            )
            : base(options, logger, encoder, clock)
        {
            this.config = configuration;
            this.AuthRep = authenticationRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string path = Context.Request.Path.Value;

            //當路徑為已登入後 且不含webapi，則檢查session是否過期
            if (!Paths.Contains(path) && path.IndexOf("api") == -1)
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
            }
            else if (Paths.Contains(path))
                return AuthenticateResult.NoResult();
            //當路徑為webapi ，則檢查jwtToken
            //if (path.IndexOf("api") > -1)
            //{
            //    if (path == "/api/Auth/In") //當為登入Api，則先給予權限
            //    {
            //        return AuthenticateResult.Success(new AuthenticationTicket(
            //            new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), Scheme.Name)), Scheme.Name));
            //    }
            //    else
            //    {
            //        StringValues Hearder = Context.Request.Headers["Authorization"];

            //        //當無Jwt Token且無特別Bearer，則無權限
            //        if (Hearder == string.Empty)   return AuthenticateResult.Fail("Lost Token");

            //        string token = Hearder.ToString().StartsWith("Bearer ")
            //                            ? Hearder.ToString().Substring(7) : Hearder.ToString();

            //        try
            //        {
            //            SecurityToken securityToken;
            //            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //            TokenValidationParameters validationParameters = new TokenValidationParameters()
            //            {
            //                ValidIssuer = config["Issuer"],
            //                ValidateAudience = false,
            //                ValidateLifetime = true,
            //                ValidateIssuerSigningKey = true,
            //                LifetimeValidator = this.LifetimeValidator,
            //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenSec"]))
            //            };
            //            Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);

            //            JwtSecurityToken jwtST = (JwtSecurityToken)securityToken;

            //            var claims = new List<Claim>() {
            //                new Claim(ClaimTypes.NameIdentifier, jwtST.Payload["nameid"].ToString()),
            //                new Claim(ClaimTypes.Name, jwtST.Payload["sub"].ToString()),
            //            };

            //            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name));
            //        }
            //        catch (SecurityTokenValidationException e)
            //        {
            //            return AuthenticateResult.Fail($"Token :{e.Message}");
            //        }
            //        catch (Exception ex)
            //        {
            //            return AuthenticateResult.Fail($"Token :{ex.Message}");
            //        }
            //    }
            //}
            return AuthenticateResult.Fail("Signed Out!");
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                bool IsCorrecnt = false;

                //當Token有效期未失效，則認證成功
                if (DateTime.UtcNow < expires) IsCorrecnt = true;

                JwtSecurityToken jwtST = (JwtSecurityToken)securityToken;

                //當有人員編號，則驗證Token及人員
                if (!string.IsNullOrEmpty(jwtST.Payload["nameid"].ToString()))
                {
                    var result = AuthRep.IsVerify(jwtST.Payload["nameid"].ToString(), jwtST.Payload["jti"].ToString(), (int)DataStatus.Enable);
                    if (result.rtn.IsSuccess)
                        return result.isVerify;
                    else
                        return result.rtn.IsSuccess;
                }
            }
            return false;
        }
    }
}