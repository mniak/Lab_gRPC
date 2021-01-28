using IdentityModel.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gRPCClient.Infrastructure.BpAuth
{
    public class BpAuthTokenHostedService : BackgroundService
    {
        public readonly IBpAuthTokenClient bpAuthTokenClient;
        public readonly IBpAuthTokenHolder bpAuthTokenHolder;

        private readonly ILogger<BpAuthTokenHostedService> logger;
        private readonly IOptions<BpAuthOptions> bpAuthOptions;
        private readonly AsyncRetryPolicy<bool> retryPolicy;

        private TokenResponse _token;

        public BpAuthTokenHostedService(
            IBpAuthTokenClient bpAuthTokenClient,
            IBpAuthTokenHolder bpAuthTokenHolder,
            AsyncRetryPolicy<bool> retryPolicy,
            ILogger<BpAuthTokenHostedService> logger,
            IOptions<BpAuthOptions> bpAuthOptions)
        {
            this.bpAuthTokenClient = bpAuthTokenClient ?? throw new ArgumentNullException(nameof(bpAuthTokenClient));
            this.bpAuthTokenHolder = bpAuthTokenHolder ?? throw new ArgumentNullException(nameof(bpAuthTokenHolder));
            this.retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bpAuthOptions = bpAuthOptions ?? throw new ArgumentNullException(nameof(bpAuthOptions));
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && bpAuthOptions.Value.Enabled)
            {
                await retryPolicy.ExecuteAsync(() => GetAccessTokenAsync());

                var duration = Math.Max(_token.ExpiresIn - (120), 5);
                await Task.Delay(TimeSpan.FromSeconds(duration));
            }
        }

        private async Task<bool> GetAccessTokenAsync()
        {
            try
            {
                _token = await bpAuthTokenClient.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(_token?.AccessToken))
                    return false;

                bpAuthTokenHolder.Token = _token;
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing background work item on SensidiaTokenHostedService.");
                return false;
            }
        }
    }
}
