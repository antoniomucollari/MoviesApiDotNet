using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using MyDotNet9Api.Services;

namespace MyDotNet9Api.Controllers;
[ApiController] 
[Route("api/[controller]")]
public class TheatersController: CustomBaseController
{
    private const string cacheTag = "theaters";
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    // private readonly IFileStorage _fileStorage;
    
    public TheatersController(ApplicationDbContext context, IMapper mapper,
        IOutputCacheStore outputCacheStore) : base(context, mapper, outputCacheStore, cacheTag)
    {
        _outputCacheStore = outputCacheStore;
        _context = context;
        _mapper = mapper;
        // _fileStorage = fileStorage;
    }
    
    [HttpGet]
    [OutputCache(Tags = [cacheTag])]
    public async Task<List<TheaterDTO>> Get([FromQuery] PaginationDTO pagination)
    {
        return await GetAll<Theater, TheaterDTO>(pagination,orderBy: g =>g.Name);
    }
    
    [HttpGet("{id:int}", Name="GetTheaterById")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<TheaterDTO>> Get(int id)
    {
        return await Get<Theater, TheaterDTO>(id);
    }
    
    [HttpPost]
    public async Task<CreatedAtRouteResult> Post([FromBody] TheaterCreationDTO theaterCreationDTO)
    {
        Console.WriteLine($"Latitude: {theaterCreationDTO.Latitude}");
        Console.WriteLine($"Longitude: {theaterCreationDTO.Longitude}");
        return await Post<TheaterCreationDTO, Theater, TheaterDTO>(theaterCreationDTO, routName: "GetTheaterById");
    }
    
    [HttpPut ("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] TheaterCreationDTO theaterCreationDTO)
    {
        return await Put<TheaterCreationDTO, Theater>(id, theaterCreationDTO);
    }
    [HttpDelete ("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await Delete<Theater>(id);
    }
}