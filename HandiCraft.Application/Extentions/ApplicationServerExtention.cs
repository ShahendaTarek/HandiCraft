using HandiCraft.Application.Mapping;
using HandiCraft.Presentation.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Extentions
{
    public static  class ApplicationServerExtention
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {

           
            Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToArray();


                    var validationErrorResponse = new ValidationError()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(validationErrorResponse);
                };

            });
            return Services;
        }

    }
}
