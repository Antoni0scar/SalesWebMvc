using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using SalesWebMvc.Data;
using SalesWebMvc.Services;
using SalesWebMvc.Models;

namespace SalesWebMvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Configura os servicos da aplicação
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //adicionando o dbcontext no mecanismo de injeção de dependência do aplicativo
            services.AddDbContext<SalesWebMvcContext>(
                options => options.UseMySql(Configuration.GetConnectionString("SalesWebMvcContext"), 
                builder => builder.MigrationsAssembly("SalesWebMvc"))
                );

            services.AddScoped<SeedingService>(); //Incluindo o seeding service para ser passado por injeção de dependência
            services.AddScoped<SellerService>(); //Incluindo o seller service para ser passado por injeção de dependência / O serviço agora pode ser injetado em outras classes.
            services.AddScoped<DepartmentService>(); //Incluindo o department service para ser passado por injeção de dependência
            services.AddScoped<SalesRecordService>();//Incluindo o sales record service para ser passado por injeção de dependência
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        // Método para configurar as requisições
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedingService seedingService)
        {
            //Definindo configurações de localização
            var enUS = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(enUS),
                SupportedCultures = new List<CultureInfo> { enUS },
                SupportedUICultures = new List<CultureInfo> { enUS }
            };

            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment()) //se está no perfil de desenvolvimento
            {
                app.UseDeveloperExceptionPage();
                seedingService.Seed(); //populando a base de dados
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); // perfil de produção
                app.UseHsts();
               
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"); //chamando por padrão o controller home / Index, caso não digite nada no navegador.
            });
        }
    }
}
