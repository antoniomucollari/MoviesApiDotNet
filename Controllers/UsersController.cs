using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Utilities;

namespace MyDotNet9Api.Controllers;
[Route("api/users")]
[ApiController]
public class UsersController: ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        IConfiguration configuration, ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    [HttpGet("IndexUsers")]
    public async Task<ActionResult<List<UserDTO>>> IndexUsers([FromQuery] PaginationDTO paginationDto)
    {
        var queryable = _applicationDbContext.Users.AsQueryable();
        await HttpContext.InsertPaginationParameterHeader(queryable);
        var users = await queryable.ProjectTo<UserDTO>(_mapper.ConfigurationProvider).OrderBy(x => x.Email)
            .Paginate(paginationDto).ToListAsync();
        return users;
    }
 
    [HttpPost("register")]
    [AllowAnonymous]
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
    [AllowAnonymous]
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

    [HttpPost("MakeAdmin")]
    public async Task<IActionResult> MakeAdmin(EditClaimDTO editClaimDto)
    {
        var user = await _userManager.FindByEmailAsync(editClaimDto.Email);
        if (user is null)
        {
            return NotFound();
        }

        await _userManager.AddClaimAsync(user, new Claim("isadmin", "true"));
        return NoContent();
    }
    
    [HttpPost("RemoveAdmin")]
    public async Task<IActionResult> RemoveAdmin(EditClaimDTO editClaimDto)
    {
        var user = await _userManager.FindByEmailAsync(editClaimDto.Email);
        if (user is null)
        {
            return NotFound();
        }

        await _userManager.RemoveClaimAsync(user, new Claim("isadmin", "true"));
        return NoContent();
    }
}