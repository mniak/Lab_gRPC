using IdentityModel.Client;
using System.Threading.Tasks;

namespace gRPCClient.Infrastructure.BpAuth
{
    public interface IBpAuthTokenClient
    {
        Task<TokenResponse> GetAccessTokenAsync();
    }
}
