using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAccountService accountService, UserManager<IdentityUser> userManager, ITokenService tokenService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest("User already exists");

        var result = await _accountService.CreateUserAsync(model);
        if (!result.Succeeded)
            return BadRequest("Error creating user");

        return Ok();
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmEmailModel model)
    {
        var result = await _accountService.ConfirmEmailAsync(model);
        if (!result)
            return BadRequest();

        return Ok("Email confirmed");
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInModel model)
    {
        var result = await _accountService.SignInAsync(model);
        if (!result.Succeeded)
            return Unauthorized("Invalid email or password");

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return NotFound();

        var accessToken = _tokenService.GenerateAccessToken(user);

        return Ok(new { accessToken });
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOutAsync()
    {
        await _accountService.SignOutAsync();
        return Ok();
    }

    [HttpGet("validate")]
    [Authorize]
    public IActionResult ValidateToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            valid = true,
            UserId = userId,
        });
    }
}
