using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ScrapeChallenge.Processor;
using ScrapeChallenge.Repositories;
using Serilog;

namespace ScrapeChallenge
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.RegisterMongoDbRepositories();
            services.RegisterPuppeteerScraper();

            services.AddSwaggerGen(c => // Register the Swagger generator
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Scraping Challenge API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://xyz.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Nejat Serpen",
                        Email = string.Empty,
                        Url = new Uri("https://www.linkedin.com/in/nserpen/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://xyz.com/license"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwaggerUI(c => // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scraping Challenge API V1");
                c.RoutePrefix = "doc"; // root
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
