using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using Newtonsoft.Json.Serialization;
using TheWorld.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _config = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            //if (_env.IsDevelopment())
            //{
            //    services.AddScoped<IMailService, DebugMailService>();
            //}

            //if (_env.IsProduction())
            //{
            //    services.AddMvc(config =>
            //    {
            //        config.Filters.Add(new RequireHttpsAttribute());
            //    });
            //}

            services.AddDbContext<WorldContext>();

            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddTransient<GeoService>();

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = async ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        await Task.Yield();
                    }
                };


            })
            .AddEntityFrameworkStores<WorldContext>();

            services.AddTransient<WorldContextSeedData>();

            services.AddLogging();

            services.AddMvc()
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WorldContextSeedData seeder, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>().ReverseMap();
                config.CreateMap<StopViewModel, Stop>().ReverseMap();
            });

            if (env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug(LogLevel.Information);
            }
            
            app.UseStaticFiles();
            app.UseIdentity();

            //I moved this to run before app.UseMvc
            //attempting to run this after app.UseMvc will throw an AggregateException Error
            seeder.EnsureSeedData().Wait();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new {controller = "App", action = "Index"}
                    );
            });

            
        }
    }
}
