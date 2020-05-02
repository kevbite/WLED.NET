using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualBasic;
using Xunit;

namespace Kevsoft.WLED.Tests
{
    public class WLedClientTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task GetsAllData()
        {
            var expected = _fixture.Create<WLedRootResponse>();
            var baseUri = $"http://{Guid.NewGuid():N}.com";
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = JsonBuilder.CreateRootResponse(expected);
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json", json);
            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.Get();

            root.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetStateData()
        {
            var expected = _fixture.Create<State>();
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = JsonBuilder.CreateStateJson(expected);
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetState();

            root.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetInformationData()
        {
            var expected = _fixture.Create<Information>();
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = JsonBuilder.CreateInformationJson(expected);
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/info", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetInformation();

            root.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public async Task GetEffectsData()
        {
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = @"[""Effect A"", ""Effect B"" ]";
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/eff", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetEffects();

            root.Should().BeEquivalentTo(new[]
                {"Effect A", "Effect B"}
            );
        }

        [Fact]
        public async Task GetPalettesData()
        {
            ;
            var baseUri = $"http://{Guid.NewGuid():N}.com";

            var mockHttpMessageHandler = new MockHttpMessageHandler();
            var json = @"[""Palette A"", ""Palette B"" ]";
            mockHttpMessageHandler.AppendResponse($"{baseUri}/json/pal", json);

            var client = new WLedClient(mockHttpMessageHandler, baseUri);

            var root = await client.GetPalettes();

            root.Should().BeEquivalentTo(new[]
                {"Palette A", "Palette B"}
            );
        }
    }
}