using BLL.InterFace;
using BLL.Model;
using DAL.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            string strCon = Configuration.GetValue<string>("NorthwindConnection");
            int cmdTimeOut = Configuration.GetValue<int>("CommandTimeout");
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Service
            services.AddSingleton<IAuthService,AuthService>(); //權限服務

            //Repository
            services.AddSingleton<AuthenticationRepository>(x => new AuthenticationRepository(strCon, cmdTimeOut)); //權限倉
            services.AddSingleton<CustomerRepository>(x => new CustomerRepository(strCon, cmdTimeOut)); //客戶倉
            services.AddSingleton<EmployeeRepository>(x => new EmployeeRepository(strCon, cmdTimeOut)); //員工倉
            services.AddSingleton<OrderRepository>(x => new OrderRepository(strCon, cmdTimeOut)); //訂單倉

            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
            app.UseCookiePolicy();

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
