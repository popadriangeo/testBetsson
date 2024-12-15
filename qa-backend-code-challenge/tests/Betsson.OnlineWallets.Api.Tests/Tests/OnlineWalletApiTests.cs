using System.Net;
using System.Threading.Tasks;
using Betsson.OnlineWallets.Api.Tests.Fixtures;
using FluentAssertions;
using RestSharp;
using Xunit;

namespace Betsson.OnlineWallets.Api.Tests.Tests
{
    public class OnlineWalletApiTests : IClassFixture<ApiTestFixture>, IAsyncLifetime
    {
        private readonly RestClient _client;
        private readonly ApiTestFixture _fixture;

        public OnlineWalletApiTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
            _client = new RestClient(_fixture.BaseUrl);
        }

        // This method runs before each test
        public async Task InitializeAsync()
        {
            // Step 1: Get the current balance
            var getBalanceRequest = new RestRequest("/onlinewallet/balance", Method.Get);
            var getBalanceResponse = await _client.ExecuteAsync<BalanceResponse>(getBalanceRequest);

            if (getBalanceResponse.StatusCode != HttpStatusCode.OK || getBalanceResponse.Data == null)
            {
                throw new Exception("Unable to retrieve balance for reset.");
            }

            var currentBalance = getBalanceResponse.Data.Amount;

            // Step 2: Withdraw the entire balance to reset to zero, if necessary
            if (currentBalance > 0)
            {
                var withdrawRequest = new RestRequest("/onlinewallet/withdraw", Method.Post);
                withdrawRequest.AddJsonBody(new WithdrawRequest { Amount = currentBalance });

                var withdrawResponse = await _client.ExecuteAsync<BalanceResponse>(withdrawRequest);

                if (withdrawResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Unable to reset balance.");
                }
            }
        }

        public Task DisposeAsync()
        {
            // No teardown needed in this case
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetBalance_ShouldReturnOk()
        {
            var request = new RestRequest("/onlinewallet/balance", Method.Get);
            var response = await _client.ExecuteAsync<BalanceResponse>(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Deposit_ShouldReturnOk()
        {
            var request = new RestRequest("/onlinewallet/deposit", Method.Post);
            request.AddJsonBody(new DepositRequest { Amount = 100.0 });
            var response = await _client.ExecuteAsync<BalanceResponse>(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task Withdraw_ShouldReturnOk()
        {
            var request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            request.AddJsonBody(new DepositRequest { Amount = 50.0 });
            var response = await _client.ExecuteAsync<BalanceResponse>(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data.Should().NotBeNull();
        }

    }

    public class BalanceResponse
    {
        public double Amount { get; set; }
    }

    public class DepositRequest
    {
        public double Amount { get; set; }
    }

    public class WithdrawRequest
    {
        public double Amount { get; set; }
    }
}
