﻿using CineGest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CineGest
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder
                                        .WithOrigins(Environment.GetEnvironmentVariable("ASPNETCORE_CORS_URLS"))
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                  });
            });

            services.AddControllers();

            //****************************************************************************
            // especificação do 'tipo' e 'localização' da BD
            services.AddDbContext<CineGestDB>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("ConnectionDB")));
            //****************************************************************************

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            //PUBLIC URLS
            app.UseWhen((http) =>
               !(
                    http.Request.RouteValues.Values.Contains("Login") && http.Request.RouteValues.Values.Contains("Users") ||
                    http.Request.RouteValues.Values.Contains("Signup") && http.Request.RouteValues.Values.Contains("Users") ||
                    http.Request.RouteValues.Values.Contains("GetMovies") && http.Request.RouteValues.Values.Contains("Movies") ||
                    http.Request.RouteValues.Values.Contains("GetMovie") && http.Request.RouteValues.Values.Contains("Movies") ||
                    http.Request.RouteValues.Values.Contains("GetHighlightedMovies") && http.Request.RouteValues.Values.Contains("Movies") ||
                    http.Request.RouteValues.Values.Contains("GetHighlightedMovies") && http.Request.RouteValues.Values.Contains("Sessions") ||
                    http.Request.RouteValues.Values.Contains("GetSessionsByMovie") && http.Request.RouteValues.Values.Contains("Sessions")

                ),
                (appBuilder) => appBuilder.UseVerifyToken());


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
