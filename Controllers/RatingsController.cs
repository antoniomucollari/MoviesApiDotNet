using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Services;

namespace MyDotNet9Api.Controllers;

[Route("api/rating")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly IUserServices _userServices;


    public UsersController(ApplicationDbContext context, IUserServices userServices)
    {
        _context = context;
        _userServices = userServices;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Post([FromBody] RatingCreationDTO ratingCreationDTO)
    {
        var userId = await _userServices.ObtainUserId();
    }
}