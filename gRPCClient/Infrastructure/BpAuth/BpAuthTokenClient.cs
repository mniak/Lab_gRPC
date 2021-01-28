using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace gRPCClient.Infrastructure.BpAuth
{
    public class BpAuthTokenClient : IBpAuthTokenClient
    {
        public readonly HttpClient httpClient;
        public readonly IOptions<BpAuthOptions> bpAuthOptions;

        public BpAuthTokenClient(HttpClient httpClient, IOptions<BpAuthOptions> bpAuthOptions)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.bpAuthOptions = bpAuthOptions ?? throw new ArgumentNullException(nameof(bpAuthOptions));
        }

        public async Task<TokenResponse> GetAccessTokenAsync()
        {
            var requestMessage = new ClientCredentialsTokenRequest()
            {
                ClientId = bpAuthOptions.Value.ClientId,
                ClientSecret = bpAuthOptions.Value.ClientSecret,
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(requestMessage);

            if (tokenResponse.IsError)
            {
                throw new CouldNotIssueBpAuthTokenException()
                {
                    Error = tokenResponse.Error,
                };
            }

            return tokenResponse;
        }
    }
}
