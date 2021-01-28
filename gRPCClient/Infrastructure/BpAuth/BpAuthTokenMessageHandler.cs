using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace gRPCClient.Infrastructure.BpAuth
{
    public class BpAuthTokenMessageHandler : DelegatingHandler
    {
        private const string BEARER = "Bearer";

        public IOptions<BpAuthOptions> BpAuthOptions { get; }
        public IBpAuthTokenHolder TokenHolder { get; }

        public BpAuthTokenMessageHandler(
            IOptions<BpAuthOptions> phoebusOptions,
            IBpAuthTokenHolder tokenHolder)
        {
            BpAuthOptions = phoebusOptions ?? throw new ArgumentNullException(nameof(phoebusOptions));
            TokenHolder = tokenHolder ?? throw new ArgumentNullException(nameof(tokenHolder));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(BEARER, TokenHolder.Token.AccessToken);
            return base.SendAsync(request, cancellationToken);
        }
    }
}