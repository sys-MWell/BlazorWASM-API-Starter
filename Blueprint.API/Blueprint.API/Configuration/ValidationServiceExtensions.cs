using Blueprint.API.Logic.Validation;
using FluentValidation;
using Template.Models.Dtos;

namespace Blueprint.API.Configuration
{
    /// <summary>
    /// Extension methods for registering validation services.
    /// </summary>
    public static class ValidationServiceExtensions
    {
        /// <summary>
        /// Adds FluentValidation validators to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();

            return services;
        }
    }
}
