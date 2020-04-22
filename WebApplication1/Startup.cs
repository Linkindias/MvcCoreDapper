using BLL.InterFace;
using BLL.Model;
using BLL.PageModel;
using DAL;
using DAL.Repository;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApplication1.Filters;

namespace WebApplication1
{
    public class Startup
    {
        IConfiguration Config { get; }
        IHostingEnvironment CurrentEnv { get; set; }
        IAntiforgery Antiforgery { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Config = configuration;
            this.CurrentEnv = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddCors();

            //For WebApi CSRF Token
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        //.AddJwtBearer(options =>
                        //{
                        //    options.IncludeErrorDetails = true;
                        //    options.TokenValidationParameters = new TokenValidationParameters()
                        //    {
                        //        ValidateIssuer = true,
                        //        ValidIssuer = Config["Issuer"],
                        //        ValidateAudience = false,
                        //        ValidateLifetime = true,
                        //        ValidateIssuerSigningKey = true,
                        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["TokenSec"]))
                        //    };
                        //});

            //Service
            services.AddScoped<AuthService>(); //權限服務
            services.AddTransient<IMemberOfAuth, MemberService>(); //會員服務(是否有會員)
            services.AddScoped<MenuService>(); //選單服務
            services.AddTransient<IMemberOfMenu, MemberService>(); //會員服務(帳號取得角色資訊)
            services.AddSingleton<ProductService>(); //產品服務
            services.AddTransient<IMemberOfProduct, MemberService>(); //會員服務(帳號計算金額及折扣)
            services.AddSingleton<MemberService>(); //會員服務
            services.AddTransient<IMemberOfOrder, MemberService>(); //會員服務(依會員編號取得會員資訊)
            services.AddSingleton<OrderService>(); //訂單服務

            services.AddTransient<CustomerModel>(); //客戶
            services.AddTransient<EmployeeModel>(); //員工
            services.AddTransient<ProductModel>(); //產品
            services.AddTransient<ShopCarModel>(); //購物車
            services.AddTransient<OrderModel>(); //訂單

            //Repository
            services.AddSingleton<AuthenticationRepository>(); //權限倉
            services.AddSingleton<CustomerRepository>(); //客戶倉
            services.AddSingleton<EmployeeRepository>(); //員工倉
            services.AddSingleton<OrderRepository>(); //訂單倉
            services.AddSingleton<MenuRepository>(); //選單倉
            services.AddSingleton<RoleRepository>(); //角色倉
            services.AddSingleton<ProductRepository>(); //產品倉
            services.AddSingleton<CategorieRepository>(); //產品類別倉
            services.AddSingleton<OrderDetailRepository>(); //訂單細部倉
            services.AddSingleton<SupplierRepository>(); //供應商倉

            services.AddTransient<ConnectionBase>(); //基礎Connection

            services.AddSession();

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); //自動动 XSRF 驗證
                options.Filters.Add(new AuthorizeActionFilter());
                options.Filters.Add(new ExceptionFilter(CurrentEnv));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAntiforgery antiforgery)
        {
            this.Antiforgery = antiforgery;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //For WebApi CSRF Token
            app.Use(next => context =>
            {
                if (
                    string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
                {
                    // We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false });
                }

                return next(context);
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
