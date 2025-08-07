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

    public CustomBaseController(ApplicationDbContext context, IMapper mapper)
    {
         _context = context;
         _mapper = mapper;
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
}