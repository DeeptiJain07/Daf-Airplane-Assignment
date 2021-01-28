using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using DAF.AirplaneTrafficData.HelperClasses;
using DAF.AirplaneTrafficData.Repositories;
using DAF.AirplaneTrafficData.Repositories.Interfaces;
using DAF.AirplaneTrafficData.Services;
using DAF.AirplaneTrafficData.Services.Interfaces;
using DAF.AirplaneTrafficDataModel.Models.TrafficDataManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace DAF.AirplaneTrafficData.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.ReturnHttpNotAcceptable = true;
                });

            return services;
        }

        /// <summary>Adds the custom API behavior.</summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            services.AddScoped<IAirportService, AirportService>();
            services.AddScoped<IAircraftService, AircraftService>();

            services.AddTransient<IAirportRepository, AirportRepository>();
            services.AddTransient<IAircraftRepository, AircraftRepository>();

            var serviceProvider = services.BuildServiceProvider();
            
            services.AddMemoryCache();
            services.AddCustomOptions(); // configures custom options
            services.AddLogging();

            return services;
        }

        /// <summary>
        ///     Adds the SQL context
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:TrafficDataManagement"];

            services.AddDbContext<TrafficDataManagementContext>(o => o.UseSqlServer(connectionString
            ));
            return services;
        }

        /// <summary>
        ///     Configures custom options
        /// </summary>
        /// <param name="services">The services</param>
        private static IServiceCollection AddCustomOptions(this IServiceCollection services)
        {
            // NOTE: Commented as the options are specified in Startup for now (in the call to AddNewtonsoftJson
            // services.AddTransient<IConfigureOptions<MvcJsonOptions>, JsonOptions>();
            return services;
        }

        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1",
                        new OpenApiInfo
                        { Title = "DAF - Airplane API", Description = "DAF - Airplane Swagger API", Version = "v1" });
                    // Set the comments path for the Swagger JSON and UI.
                    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    //c.IncludeXmlComments(xmlPath);

                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    //var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                    var commentsFileName = "Comments" + ".XML";
                    if (baseDirectory != null)
                    {
                        var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                        c.IncludeXmlComments(commentsFile);
                    }
                });


            return services;
        }

        /// <summary>
        ///     Configures Cors
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var optionEndPoints = serviceProvider.GetRequiredService<IOptions<EndPoints>>().Value;
            services.AddCors(options =>
            {
                options.AddPolicy("Policy",
                    builder =>
                    {
                        builder.WithOrigins(configuration["AllowedHosts"])
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            return services;
        }

        /// <summary>
        ///     Configures HttpClient
        /// </summary>
        /// <param name="services">The services</param>
        public static IServiceCollection AddCustomHttpClient(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var optionEndPoints = serviceProvider.GetRequiredService<IOptions<EndPoints>>().Value;
            //#pragma warning disable CS0618 // Type or member is obsolete
            var loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddConsole()); // new LoggerFactory().AddConsole((_, __) => true);
            //#pragma warning restore CS0618 // Type or member is obsolete
            ILogger logger = loggerFactory.CreateLogger<Program>();

            return services;
        }

        /// <summary>
        ///     Configure unit of work
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
