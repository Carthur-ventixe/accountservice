using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using WebApi.Models;

namespace WebApi.Services;

public class AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ServiceBusClient serviceBusClient) : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly ServiceBusClient _serviceBusClient = serviceBusClient;

    public async Task<IdentityResult> CreateUserAsync(RegisterModel model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return result;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        var queueName = "email-service";
        var sender = _serviceBusClient.CreateSender(queueName);

        var message = new EmailMessage { Email = model.Email, Token = encodedToken };
        var messageJson = JsonSerializer.Serialize(message);

        await sender.SendMessageAsync(new ServiceBusMessage(messageJson));

        return result;
    }

    public async Task<bool> ConfirmEmailAsync(ConfirmEmailModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return false;

        var decodedToken = Uri.UnescapeDataString(model.Token);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return false;
        }

        return true;
    }
    public async Task<SignInResult> SignInAsync(SignInModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        return result;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
