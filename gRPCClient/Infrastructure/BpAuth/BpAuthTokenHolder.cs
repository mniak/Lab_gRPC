using IdentityModel.Client;

namespace gRPCClient.Infrastructure.BpAuth
{
    public class BpAuthTokenHolder : IBpAuthTokenHolder
    {
        public TokenResponse Token { get; set; }
    }
}
