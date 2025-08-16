using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyDotNet9Api.DTOs;

namespace MyDotNet9Api.Controllers;
[Route("api/users")]
[ApiController]
public class UsersController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponseDTO>> Register(UserCredentialsDTO userCredentialsDTO)
    {
        var user = new IdentityUser
        {
            Email = userCredentialsDTO.Email,
            UserName = userCredentialsDTO.Email.Split('@')[0]
        };
        var result = await _userManager.CreateAsync(user, userCredentialsDTO.Password);
        if (result.Succeeded)
        {
            return await BuildToken(user);
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponseDTO>> Login(UserCredentialsDTO userCredentialsDto)
    {
        var user = await _userManager.FindByEmailAsync(userCredentialsDto.Email);
        if (user is null)
        {
            var errors = BuildIncorrectLogin();
            return BadRequest(errors);
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, userCredentialsDto.Password, 
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return await BuildToken(user);
        }
        else
        {
            return BadRequest(BuildIncorrectLogin());
        }
    }

    private IEnumerable<IdentityError> BuildIncorrectLogin()
    {
        var identityError = new IdentityError() { Description = "Incorrect Login" };
        var errors = new List<IdentityError>();
        errors.Add(identityError);
        return errors;
    }
    private async Task<AuthenticationResponseDTO> BuildToken(IdentityUser identityUser)
    {
        var claims = new List<Claim>
        {
            new Claim("email", identityUser.Email!),
            new Claim("name", identityUser.UserName!)
        };
        var claimsDB = await _userManager.GetClaimsAsync(identityUser);

        claims.AddRange(claimsDB);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddYears(1);

        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration,
            signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return new AuthenticationResponseDTO
        {
            Token = token,
            Expiration = expiration
        };
    }
}