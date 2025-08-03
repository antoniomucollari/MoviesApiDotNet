using Microsoft.AspNetCore.Mvc;
using MyDotNet9Api.Entities;

namespace MyDotNet9Api.Controllers;
[ApiController] 
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    [HttpGet]
    public List<Genre> GetAll()
    {
        var repo = new InMemoryRepository();
        var genres = repo.GetAll();
        return genres;
    }
}