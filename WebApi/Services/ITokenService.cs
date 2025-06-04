using Microsoft.AspNetCore.Identity;

namespace WebApi.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IdentityUser user);
    }
}