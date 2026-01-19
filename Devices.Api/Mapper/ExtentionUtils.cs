using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Devices.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Devices.Api.Mapper
{


    public static class MigrationsExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Devices.Infrastructure.Db.DevicesDbContext>();
            dbContext.Database.Migrate();
        }
    }


    public static class ResultToProblemDetailsMapperExtensions
    {

        public static IActionResult ToActionResult<T>(
            this ResultWrapper<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return result.ToProblemActionResult<T>();
        }

        public static IActionResult ToActionResult(
            this ResultWrapper result)
        {
            if (result.IsSuccess) return new OkResult();
            var problem = ToProblemDetails(result.Error!);
            return new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };
        }

        private static IActionResult ToProblemActionResult<T>(
            this ResultWrapper<T> result)
        {
            var problem = ToProblemDetails(result.Error!);

            return new ObjectResult(problem)
            {
                StatusCode = problem.Status
            };
        }

        private static ProblemDetails ToProblemDetails(Error error)
        {
            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return new ProblemDetails
            {
                Status = status,
                Title = error.Code,
                Detail = error.Message,
                Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status/{status}"
            };
        }
    }

}
