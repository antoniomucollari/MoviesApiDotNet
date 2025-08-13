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
public class ActorsController :CustomBaseController 
{
    private const string cacheTag = "actors";
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly string container = "actors";
    private readonly IFileStorage _fileStorage;
  
    public ActorsController(IOutputCacheStore outputCacheStore, ApplicationDbContext context, 
        IMapper mapper, IFileStorage fileStorage):base(context, mapper, outputCacheStore, cacheTag)
    {
        _outputCacheStore = outputCacheStore;
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }
    
    [HttpGet]
    [OutputCache(Tags = [cacheTag])]
    public async Task<List<ActorDTO>> GetAll([FromQuery] PaginationDTO pagination)
    {
        return await GetAll<Actor, ActorDTO>(pagination,orderBy: a =>a.Name);
    }
    
    [HttpGet("{id:int}", Name="GetActorById")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<ActorDTO>> Get(int id)
    {
        return await Get<Actor, ActorDTO>(id);
    }
    
    [HttpGet("search/{name}")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<List<MovieActorDTO>>> Get(string name)
    {
        return await _context.Actors.Where(a=> a.Name.Contains(name))
            .ProjectTo<MovieActorDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
    
    [HttpPost]
    public async Task<CreatedAtRouteResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
    {
        var actor = _mapper.Map<Actor>(actorCreationDTO);

        if (actorCreationDTO.Picture is not null)
        {
            var url = await _fileStorage.Store(container, actorCreationDTO.Picture);
            actor.Picture = url;
        }
        
        _context.Actors.Add(actor); 
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        var actorDTO = _mapper.Map<ActorDTO>(actor);
        return CreatedAtRoute("GetActorById", new{id = actor.Id}, actorDTO); 
    }
    [HttpPut ("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromForm] ActorCreationDTO actorCreationDTO)
    {
        var actor = await _context.Actors.FirstOrDefaultAsync(g => g.Id == id);
        if (actor is null)
        {
            return NotFound();
        }
        actor =  _mapper.Map(actorCreationDTO, actor);
        actor.Id = id;
        if (actorCreationDTO.Picture is not null)
        {
            actor.Picture = await _fileStorage.Edit(actor.Picture, container, actorCreationDTO.Picture);
        }
        
        _context.Update(actor);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        return NoContent();
    }
    [HttpDelete ("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await _context.Actors.FirstOrDefaultAsync(g => g.Id == id);
        if (actor is null)
        { 
            NotFound();
        }

        _context.Remove(actor);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        await _fileStorage.Delete(actor.Picture, container);
        return NoContent();
    }
}