using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Emails.DB;
using Emails.Filters;
using Emails.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Emails
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
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddDbContext<Context>(options => options.UseNpgsql(Configuration.GetConnectionString("context")));
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<ISentEmailsService, SentEmailsService>();
            services.AddScoped<IMailWrapperService, MailWrapperService>();
            var ignite = Ignition.Start();
            CacheConfiguration cfg = new CacheConfiguration();
            cfg.ExpiryPolicyFactory = new ExpiryPolicyFactoryImpl();
            cfg.EagerTtl = true;
            cfg.Name = "sessionCache";
            ignite.AddCacheConfiguration(cfg);
            var cache = ignite.GetOrCreateCache<int, SessionStore>("sessionCache");
            
            try
            {
                cache.Query(new Apache.Ignite.Core.Cache.Query.SqlFieldsQuery($"create table SessionStore (SessionId varchar primary key,UserId int)"));
            }
            catch { }
            services.AddSingleton(ignite);
            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
            services.AddScoped<AuthenticationFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.Use(next => context =>
            {
                string isTokenGenerated = context.Session.GetString("isTokenGenerated");
                if (string.IsNullOrEmpty(isTokenGenerated))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("CSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false });
                    context.Session.SetString("isTokenGenerated", "1");
                }
                return next(context);
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

        }
    }
}
