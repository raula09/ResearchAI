using Microsoft.AspNetCore.Mvc;
using ResearchCopilot.Api.Models;
using ResearchCopilot.Api.Repos;
using ResearchCopilot.Api.Services;

namespace ResearchCopilot.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserRepo _users;
    private readonly PasswordService _pwd;
    private readonly JwtService _jwt;
    public AuthController(UserRepo users, PasswordService pwd, JwtService jwt) { _users = users; _pwd = pwd; _jwt = jwt; }

    [HttpPost("register")]
    public async Task<ActionResult<TokenDto>> Register(RegisterDto dto)
    {
        var existing = await _users.FindByEmail(dto.Email);
        if (existing != null) return BadRequest("exists");
        var u = new User { Email = dto.Email, PasswordHash = _pwd.Hash(dto.Password) };
        await _users.Create(u);
        return new TokenDto { Token = _jwt.Create(u) };
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var u = await _users.FindByEmail(dto.Email);
        if (u == null) return Unauthorized();
        if (!_pwd.Verify(dto.Password, u.PasswordHash)) return Unauthorized();
        return new TokenDto { Token = _jwt.Create(u) };
    }
}
