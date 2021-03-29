using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ZeeReportingApi.Model;
using Microsoft.EntityFrameworkCore;

[assembly: FunctionsStartup(typeof(ZeeReportingApi.AppStartup))]

namespace ZeeReportingApi
{
    /// <summary>
    ///     Startup class used to initialize the dependency injection.
    /// </summary>
    /// <remarks>
    ///     See: https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
    /// </remarks>
    public class AppStartup : FunctionsStartup
    {
        /// <summary>
        ///     Configure the DI container.
        /// </summary>ß
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Intject the token service.
            builder.Services.AddSingleton<TokenIssuer>();
            builder.Services.AddDbContext<ODSContext>(
                options => options.UseSqlServer(Constants.DB_CONN_STRING));
        }
    }
}