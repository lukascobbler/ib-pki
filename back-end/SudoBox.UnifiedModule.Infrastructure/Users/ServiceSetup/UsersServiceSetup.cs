using Microsoft.Extensions.DependencyInjection;
using SudoBox.UnifiedModule.Application.Users.Features.Auth;
using SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;
using SudoBox.UnifiedModule.Application.Users.Features.Registration;
using SudoBox.UnifiedModule.Application.Users.Utils.Password;

namespace SudoBox.UnifiedModule.Infrastructure.Users.ServiceSetup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserFeatures(this IServiceCollection services, string? commonListPath = null)
    {
        // Common passwords
        if (!string.IsNullOrWhiteSpace(commonListPath) && File.Exists(commonListPath))
            services.AddSingleton<ICommonPasswordStore>(new FileCommonPasswordStore(commonListPath));
        else
            services.AddSingleton<ICommonPasswordStore, NullCommonPasswordStore>();

        // Feature services
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
