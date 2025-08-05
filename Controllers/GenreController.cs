using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;

namespace MyDotNet9Api.Controllers;
[ApiController] 
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private const string cacheTag = "genres";
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GenreController(IOutputCacheStore outputCacheStore, ApplicationDbContext context, IMapper mapper)
    {
        _outputCacheStore = outputCacheStore;
        _context = context;
        _mapper = mapper;
    }
    // [HttpGet]
    // [OutputCache(Tags = [cacheTag])]
    // public List<Genre> GetAll()
    // {
    //     // return _repository.GetAll();
    //     return new List<Genre> { new Genre { Id = 1, Name = "Drama" }, new Genre { Id = 2, Name = "Action" } };
    // }
    
    [HttpGet("{id}", Name="GetGenreById")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<Genre>> Get(int id)
    {
        throw new InvalidOperationException();
    }
    
    [HttpPost]
    public async Task<CreatedAtRouteResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
    {
        var genre = _mapper.Map<Genre>(genreCreationDTO);
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        var genreDTO = _mapper.Map<GenreDTO>(genre);
        return new CreatedAtRouteResult("GetGenreById", new { id = genreDTO.Id }, genreDTO);
    }

    [HttpPut]
    public void Put()
    {
        
    }
    [HttpDelete]
    public void Delete()
    {
        
    }
}