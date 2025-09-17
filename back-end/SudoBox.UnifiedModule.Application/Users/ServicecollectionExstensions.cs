using Microsoft.Extensions.DependencyInjection;
using SudoBox.UnifiedModule.Application.Users.Utils.Password;
using SudoBox.UnifiedModule.Application.Users.Features.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;
using SudoBox.UnifiedModule.Application.Users.Features.Auth;

namespace SudoBox.UnifiedModule.Application.Users;

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
