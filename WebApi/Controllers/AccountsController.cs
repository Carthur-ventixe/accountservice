using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.CreateUserAsync(model);
        if (!result.Succeeded)
            return BadRequest();

        return Ok("User created, please verify your emailaddress");
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody]ConfirmEmailModel model)
    {
        var result = await _accountService.ConfirmEmailAsync(model);
        if (result)
            return BadRequest();

        return Ok("Email confirmed");
    }
}
