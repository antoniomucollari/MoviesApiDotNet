using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using MyDotNet9Api.Utilities;

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
    [HttpGet]
    [OutputCache(Tags = [cacheTag])]
    public async Task<List<GenreDTO>> GetAll([FromQuery] PaginationDTO pagination)
    {
        var queryable = _context.Genres;
        await HttpContext.InsertPaginationParameterHeader(queryable);
        return await queryable.OrderBy(g=> g.Name).Paginate(pagination).ProjectTo<GenreDTO>(_mapper.ConfigurationProvider).ToListAsync();
    }
    
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