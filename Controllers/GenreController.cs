using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MyDotNet9Api.Entities;

namespace MyDotNet9Api.Controllers;
[ApiController] 
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IRepository _repository;
    public GenreController(IRepository repository)
    {
        _repository = repository;
    }
    [HttpGet]
    public List<Genre> GetAll()
    {
        var repo = new InMemoryRepository();
        var genres = repo.GetAll();
        return genres;
    }
    [HttpGet("{id}")]
    [OutputCache]
    public async Task<ActionResult<Genre>> Get(int id)
    {
        var repo = new InMemoryRepository();
        var genres = await repo.GetById(id);
        if (genres is null)
        {
            return NotFound("Not Found");
        }
        return Ok(genres);
    }
    
    [HttpGet("{name}")]
    [OutputCache]
    public async Task<ActionResult<Genre>> Get(string name, [FromQuery] int id)
    {
        return new Genre { Id = id, Name = name };
    }
    
    [HttpPost]
    public async Task<ActionResult<Genre>> Post([FromBody] Genre genre)
    {
        var genreWithSameNameExists = _repository.Exist(genre.Name);
        if (genreWithSameNameExists)
        {
            return BadRequest("Threre is already a genre with that name.");}
        genre.Id = 5;
        return genre;
    }
}