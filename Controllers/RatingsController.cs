using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using MyDotNet9Api.Services;

namespace MyDotNet9Api.Controllers;

[Route("api/rating")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly IUserServices _userServices;


    public RatingsController(ApplicationDbContext context, IUserServices userServices)
    {
        _context = context;
        _userServices = userServices;
    }
    
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Post([FromBody] RatingCreationDTO ratingCreationDTO)
    {
        var userId = await _userServices.ObtainUserId();
        var actualRating = await _context.MovieRatings.FirstOrDefaultAsync(x=>x.MovieId == ratingCreationDTO.MovieId && x.UserId == userId);

        if (actualRating is null)
        {
            var rating = new Rating()
            {
                MovieId = ratingCreationDTO.MovieId,
                Punctuation = ratingCreationDTO.Punctuation,
                UserId = userId
            };
            _context.Add(rating);
        }
        else
        {
            actualRating.Punctuation = ratingCreationDTO.Punctuation;
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }
}