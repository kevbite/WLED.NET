using Microsoft.Extensions.DependencyInjection;

namespace Kevsoft.WLED;

/// <summary>
/// Registers <see cref="IWLedClient"/> with an <c>IServiceCollection</c> using
/// <c>IHttpClientFactory</c> so the underlying <see cref="HttpClient"/> is pooled and managed
/// correctly.
/// </summary>
public static class WLedServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IWLedClient"/> for the WLED device at <paramref name="baseAddress"/>.
    /// </summary>
    public static IHttpClientBuilder AddWledClient(this IServiceCollection services, string baseAddress)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(baseAddress))
        {
            throw new ArgumentException("A base address is required.", nameof(baseAddress));
        }

        return services.AddWledClient(client => client.BaseAddress = new Uri(baseAddress, UriKind.Absolute));
    }

    /// <summary>
    /// Registers <see cref="IWLedClient"/>, configuring the underlying <see cref="HttpClient"/>
    /// (for example its <see cref="HttpClient.BaseAddress"/> and timeout).
    /// </summary>
    public static IHttpClientBuilder AddWledClient(this IServiceCollection services, Action<HttpClient> configureClient)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configureClient is null)
        {
            throw new ArgumentNullException(nameof(configureClient));
        }

        return services.AddHttpClient<IWLedClient, WLedClient>(configureClient);
    }
}
