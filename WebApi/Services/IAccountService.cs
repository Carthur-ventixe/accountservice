using Microsoft.AspNetCore.Identity;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> CreateUserAsync(RegisterModel model);
        Task<bool> ConfirmEmailAsync(ConfirmEmailModel model);
        Task<SignInResult> SignInAsync(SignInModel model);
        Task SignOutAsync();
    }
}