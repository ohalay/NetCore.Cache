using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace NetCore.CacheMiddleware
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
            services.AddResponseCaching();

            services.AddCors(options =>
            {
                var origins = new List<string>();
                Configuration.GetSection("Origins").Bind(origins);

                options.AddPolicy(Constants.CorsPolicyName,
                        builder => builder.WithOrigins(origins.ToArray()));
            });

            services.AddMvc(options =>
            {
                //options.Filters.Add(new CorsAuthorizationFilterFactory(Constants.CorsPolicyName));

                //options.CacheProfiles.Add(Constants.CachePolicyName,
                //   new CacheProfile()
                //   {
                //       Duration = this.Configuration.GetValue<int>("CacheDuration")
                //   });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                var heared = context.Request.Headers[HeaderNames.CacheControl];
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(this.Configuration.GetValue<int>("CacheDuration"))
                };
                await next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(Constants.CorsPolicyName);
            app.UseMvc();
        }
    }
}
