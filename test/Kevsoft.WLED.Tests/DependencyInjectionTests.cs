using Microsoft.Extensions.DependencyInjection;

namespace Kevsoft.WLED.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddWledClientWithBaseAddressResolvesClient()
    {
        var services = new ServiceCollection();

        services.AddWledClient("http://wled-desk/");

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IWLedClient>();

        client.Should().BeOfType<WLedClient>();
    }

    [Fact]
    public void AddWledClientWithConfigureResolvesClient()
    {
        var services = new ServiceCollection();

        services.AddWledClient(client => client.BaseAddress = new Uri("http://wled-desk/"));

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IWLedClient>();

        client.Should().BeOfType<WLedClient>();
    }

    [Fact]
    public void AddWledClientRegistersHttpClientFactory()
    {
        var services = new ServiceCollection();

        services.AddWledClient("http://wled-desk/");

        using var provider = services.BuildServiceProvider();

        provider.GetService<IHttpClientFactory>().Should().NotBeNull();
    }
}
