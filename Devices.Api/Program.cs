
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Devices.Domain.Interfaces;
using Devices.Infrastructure.Db;
using Devices.Infrastructure.Repositorys;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Devices.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var cs = builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? throw new System.InvalidOperationException("DefaultConnection not configured");

            builder.Services.AddDbContext<DevicesDbContext>(options =>
                options.UseSqlServer(cs));
            builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

            builder.Services.AddScoped<IDeviceServices, DeviceServices>();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    var exception = context.Features
                        .Get<IExceptionHandlerFeature>()?.Error;

                    var problem = new ProblemDetails
                    {
                        Title = "Unexpected Error",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = exception?.Message
                    };

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(problem);
                });
            });
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Devices API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

         

            app.Run();
        }
    }
}
