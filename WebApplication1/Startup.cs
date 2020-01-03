using BLL.InterFace;
using BLL.Model;
using BLL.PageModel;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Filters;

namespace WebApplication1
{
    public class Startup
    {
        IConfiguration Config { get; }
        IHostingEnvironment CurrentEnv { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Config = configuration;
            this.CurrentEnv = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            string strCon = Config.GetValue<string>("NorthwindConnection");
            int cmdTimeOut = Config.GetValue<int>("CommandTimeout");

            services.AddAuthentication("BasicAuthentication")
                        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

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
            services.AddSingleton<AuthenticationRepository>(x => new AuthenticationRepository(strCon, cmdTimeOut)); //權限倉
            services.AddSingleton<CustomerRepository>(x => new CustomerRepository(strCon, cmdTimeOut)); //客戶倉
            services.AddSingleton<EmployeeRepository>(x => new EmployeeRepository(strCon, cmdTimeOut)); //員工倉
            services.AddSingleton<OrderRepository>(x => new OrderRepository(strCon, cmdTimeOut)); //訂單倉
            services.AddSingleton<MenuRepository>(x => new MenuRepository(strCon, cmdTimeOut)); //選單倉
            services.AddSingleton<RoleRepository>(x => new RoleRepository(strCon, cmdTimeOut)); //角色倉
            services.AddSingleton<ProductRepository>(x => new ProductRepository(strCon, cmdTimeOut)); //產品倉
            services.AddSingleton<CategorieRepository>(x => new CategorieRepository(strCon, cmdTimeOut)); //產品類別倉
            services.AddSingleton<OrderDetailRepository>(x => new OrderDetailRepository(strCon, cmdTimeOut)); //訂單細部倉
            services.AddSingleton<SupplierRepository>(x => new SupplierRepository(strCon, cmdTimeOut)); //供應商倉

            services.AddSession();

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); //自動动 XSRF 驗證
                options.Filters.Add(new AuthorizeActionFilter());
                options.Filters.Add(new ExceptionFilter(CurrentEnv));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

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
