using IdentityModel.Client;

namespace gRPCClient.Infrastructure.BpAuth
{
    public interface IBpAuthTokenHolder
    {
        TokenResponse Token { get; set; }
    }
}
