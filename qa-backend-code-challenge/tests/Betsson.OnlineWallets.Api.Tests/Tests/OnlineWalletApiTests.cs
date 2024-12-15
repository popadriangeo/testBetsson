using System.Net;
using System.Threading.Tasks;
using Betsson.OnlineWallets.Api.Tests.Fixtures;
using FluentAssertions;
using RestSharp;
using Xunit;

namespace Betsson.OnlineWallets.Api.Tests.Tests
{
    public class OnlineWalletApiTests : IClassFixture<ApiTestFixture>
    {
        private readonly RestClient _client;

        public OnlineWalletApiTests(ApiTestFixture fixture)
        {
            _client = new RestClient(fixture.BaseUrl);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnOk()
        {
            var request = new RestRequest("/onlinewallet/balance", Method.Get);
            var response = await _client.ExecuteAsync<BalanceResponse>(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data.Should().NotBeNull();
            response.Data.Amount.Should().BeGreaterOrEqualTo(0);
        }
    }

    public class BalanceResponse
    {
        public double Amount { get; set; }
    }
}
