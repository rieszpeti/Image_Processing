using Application.CSharp.Interfaces;
using Application.CSharp.ModelValidation;
using Application.Interfaces;
using Application.REPR;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DepedencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IImageProcessingService, ImageProcessingService>();
            services.AddScoped<IModelValidator, ModelValidator>();

            return services;
        }
    }
}
