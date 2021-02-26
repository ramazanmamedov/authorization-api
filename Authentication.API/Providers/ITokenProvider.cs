// unset

namespace Authentication.API.Providers
{
    using Data;
    using System.Threading.Tasks;

    public interface ITokenProvider
    {
        Task<AuthenticateResponse> GetTokenAsync(AuthenticateRequest request);
    }
}