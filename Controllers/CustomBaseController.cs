using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MyDotNet9Api.DTOs;
using MyDotNet9Api.Entities;
using MyDotNet9Api.Utilities;

namespace MyDotNet9Api.Controllers;

public class CustomBaseController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly string _cacheTag;

    public CustomBaseController(ApplicationDbContext context, IMapper mapper, IOutputCacheStore outputCacheStore, string cacheTag)
    {
         _context = context;
         _mapper = mapper;
         _outputCacheStore = outputCacheStore;
         _cacheTag = cacheTag;
    }
    protected async Task<List<TDTO>> GetAll<TEntity, TDTO>( PaginationDTO pagination, Expression<Func<TEntity, object>> orderBy)
    where TEntity : class
    {
        var queryable = _context.Set<TEntity>().AsQueryable();
        await HttpContext.InsertPaginationParameterHeader(queryable);
        return await queryable
            .OrderBy(orderBy)
            .Paginate(pagination)
            .ProjectTo<TDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id)
    where TEntity: class
    where TDTO: IId
    {
        var entity = await _context.Set<TEntity>().ProjectTo<TDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
        {
            return NotFound(); 
        }

        return entity;
    }
    
    public async Task<CreatedAtRouteResult> Post<TCreation, TEntity, TRead>(TCreation creationDTO, string routName)
    where TEntity: class
    where TRead: IId
    {
        var entity = _mapper.Map<TEntity>(creationDTO);
        _context.Add(entity);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(_cacheTag, CancellationToken.None);
        var entityDTO = _mapper.Map<TRead>(entity); 
        return new CreatedAtRouteResult(routName, new { id = entityDTO.Id }, entityDTO);
    }
    
    public async Task<IActionResult> Put<TCreation, TEntity>(int id, TCreation creationDTO)
    where TEntity:class, IId
    {
        var entityExists = await _context.Set<TEntity>().AnyAsync(e => e.Id == id);
        if (!entityExists)
        {
            return NotFound();
        }
        var entity =  _mapper.Map<TEntity>(creationDTO);
        entity.Id = id;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(_cacheTag, CancellationToken.None);
        return NoContent();
    }
    
    public async Task<IActionResult> Delete<TEntity>(int id)
    where TEntity: class, IId
    {
        int deletedRecords = await _context.Set<TEntity>().Where(g => g.Id == id).ExecuteDeleteAsync();
        if (deletedRecords == 0)
        { 
            NotFound();
        }
        await _outputCacheStore.EvictByTagAsync(_cacheTag, CancellationToken.None);
        return NoContent();
    }
}