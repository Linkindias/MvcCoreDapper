using BLL.Model;
using BLL.PageModel;
using DAL.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Filters;
using BLL.InterFace;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            string strCon = Configuration.GetValue<string>("NorthwindConnection");
            int cmdTimeOut = Configuration.GetValue<int>("CommandTimeout");

            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Service
            services.AddScoped<AuthService>(); //權限服務
            services.AddTransient<IMemberOfAuth, MemberService>(); //會員服務(是否有會員)
            services.AddScoped<MenuService>(); //選單服務
            services.AddTransient<IMemberOfMenu, MemberService>(); //會員服務(帳號取得角色資訊)
            services.AddSingleton<ProductService>(); //產品服務
            services.AddTransient<IMemberOfProduct, MemberService>(); //會員服務(帳號計算金額及折扣)
            services.AddSingleton<MemberService>(); //會員服務

            services.AddTransient<CustomerModel>(); //客戶
            services.AddTransient<EmployeeModel>(); //員工
            services.AddTransient<ProductModel>(); //產品
            services.AddTransient<ShopCarModel>(); //購物車

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

            services.AddSession();
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new AuthorizeActionFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //CacheHelper.CreateCache(); //建立快取
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
