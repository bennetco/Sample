using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Infrastructure;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace Sample.API
{
    public class Startup
    {
        private Container container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            SetupSimpleInjector(services);
        }

        private void SetupSimpleInjector(IServiceCollection services)
        {
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));            

            //note that we trimmed a little out of the SimpleInjector ASP.NET Core MVC boilerplate here
            //  since we're not using view components or anything requiring crosswiring

            services.UseSimpleInjectorAspNetRequestScoping(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app);            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseCors(x => {
                    //Sample.Presentation and Sample.API serve from different ports on localhost
                    //this ensures that the client can still access the API
                    x.WithOrigins("http://localhost:60686") 
                        .AllowAnyHeader()
                        .AllowAnyMethod();                    
                });
            }

            container.Verify();

            app.UseMvc(); //routing is managed by attribute on HelloWorldController
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            //not registering view components because we don't have any
            container.RegisterMvcControllers(app);

            //bulk register repos
            var repoAssembly = typeof(HelloWorldRepo).Assembly;

            var repoRegistrations = repoAssembly.GetExportedTypes()
                .Where(t => t.Namespace == "Sample.Infrastructure" && t.GetInterfaces().Any())
                .Select(t => new { Service = t.GetInterfaces().Single(), Implementation = t });
            
            foreach (var reg in repoRegistrations)
            {
                container.Register(reg.Service, reg.Implementation); //already defaulted to AsyncScopedLifestyle
            }
        }
    }
}
