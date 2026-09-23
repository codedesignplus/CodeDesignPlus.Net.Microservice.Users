using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Users.Application.Options;
using CodeDesignPlus.Net.Microservice.Users.Application.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Users.Application
{
    public class Startup : IStartup
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            MapsterConfigUsers.Configure();

            services.AddSingleton<IValidateOptions<RoleAssignmentOptions>, RoleAssignmentOptionsValidator>();

            services.AddOptions<RoleAssignmentOptions>()
                .Bind(configuration.GetSection(RoleAssignmentOptions.Section))
                .ValidateOnStart();
        }
    }
}
