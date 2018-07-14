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

            services.UseSimpleInjectorAspNetRequestScoping(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeContainer(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            container.Verify();

            app.UseMvc();
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
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
