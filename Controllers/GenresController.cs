using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using MyDotNet9Api.Services;
using MyDotNet9Api.Utilities;

namespace MyDotNet9Api.Controllers;
[ApiController] 
[Route("api/[controller]")]
public class GenresController : CustomBaseController
{
    private const string cacheTag = "genres";
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;

    public GenresController(IOutputCacheStore outputCacheStore, ApplicationDbContext context, 
        IMapper mapper, IFileStorage fileStorage):base(context, mapper, outputCacheStore, cacheTag)
    {
        _outputCacheStore = outputCacheStore;
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }
    [HttpGet]
    [OutputCache(Tags = [cacheTag])]
    public async Task<List<GenreDTO>> Get([FromQuery] PaginationDTO pagination)
    {
        return await GetAll<Genre, GenreDTO>(pagination,orderBy: g =>g.Name);
    }
    
    [HttpGet("all")]
    [OutputCache(Tags = [cacheTag])] 
    public async Task<List<GenreDTO>> Get()
    {
        return await GetAllNoPagination<Genre, GenreDTO>(orderBy: g =>g.Name);
    }
    
    [HttpGet("{id:int}", Name="GetGenreById")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<GenreDTO>> Get(int id)
    {
        return await Get<Genre, GenreDTO>(id);
    }
    
    [HttpPost]
    public async Task<CreatedAtRouteResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
    {
        return await Post<GenreCreationDTO, Genre, GenreDTO>(genreCreationDTO, routName: "GetGenreById");
    }
    
    [HttpPut ("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
    {
        return await Put<GenreCreationDTO, Genre>(id, genreCreationDTO);
    }
    [HttpDelete ("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await Delete<Genre>(id);
    }
    
}