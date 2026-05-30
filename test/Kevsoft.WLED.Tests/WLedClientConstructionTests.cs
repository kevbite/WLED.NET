using System.Net.Http;

namespace Kevsoft.WLED.Tests;

public class WLedClientConstructionTests
{
    [Fact]
    public void StringConstructorSucceeds()
    {
        var act = () => new WLedClient("http://wled.local/");

        act.Should().NotThrow();
    }

#if !NETSTANDARD2_0
    [Fact]
    public void DefaultHandlerRefreshesPooledConnectionsToAvoidStaleDns()
    {
        var handler = WLedClient.CreateDefaultHandler();

        handler.Should().BeOfType<SocketsHttpHandler>()
            .Which.PooledConnectionLifetime.Should().Be(TimeSpan.FromMinutes(2));
    }
#endif
}
