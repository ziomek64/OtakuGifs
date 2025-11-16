using System;
using Microsoft.Extensions.DependencyInjection;
using OtakuGifs.Client;

namespace OtakuGifs.Extensions;

/// <summary>
/// Extension methods for configuring OtakuGifs services in dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OtakuGifs client to the service collection using typed HttpClient pattern
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>An IHttpClientBuilder that can be used to configure the HttpClient</returns>
    public static IHttpClientBuilder AddOtakuGifsClient(this IServiceCollection services)
    {
        return services.AddHttpClient<IOtakuGifsClient, OtakuGifsClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.otakugifs.xyz");
        });
    }

    /// <summary>
    /// Adds OtakuGifs client to the service collection using typed HttpClient pattern with custom configuration
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configureClient">Delegate to configure the HttpClient</param>
    /// <returns>An IHttpClientBuilder that can be used to configure the HttpClient</returns>
    public static IHttpClientBuilder AddOtakuGifsClient(
        this IServiceCollection services,
        Action<IServiceProvider, System.Net.Http.HttpClient> configureClient)
    {
        return services.AddHttpClient<IOtakuGifsClient, OtakuGifsClient>(configureClient);
    }
}
