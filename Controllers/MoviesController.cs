using System.Runtime.InteropServices.JavaScript;
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
public class MoviesController :ControllerBase    
{
    private const string cacheTag = "movies";
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly string container = "movies";
    private readonly IFileStorage _fileStorage;
  
    public MoviesController(IOutputCacheStore outputCacheStore, ApplicationDbContext context, 
        IMapper mapper, IFileStorage fileStorage) 
    {
        _outputCacheStore = outputCacheStore;
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }
    
    [HttpGet("postget")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<MoviesPostGetDTO>> PostGet([FromQuery] PaginationDTO pagination)
    {
        var genres = await _context.Genres.ProjectTo<GenreDTO>(_mapper.ConfigurationProvider).ToListAsync();
        var theaters = await _context.Theaters.ProjectTo<TheaterDTO>(_mapper.ConfigurationProvider).ToListAsync();
        return new MoviesPostGetDTO { Theaters = theaters, Genres = genres };
        // await _context.SaveChangesAsync();
        // await _outputCacheStore.EvictByTagAsync(_cacheTag, CancellationToken.None);
        // var entityDTO = _mapper.Map<TRead>(entity); 
        // return new CreatedAtRouteResult(routName, new { id = entityDTO.Id }, entityDTO);
    }

    [HttpGet("landing")]
    public async Task<ActionResult<LandingDTO>> Get()
    {
        var today = DateTime.Today;
        var top = 12 ;

        // Upcoming → purely date-based
        var upcomingReleases = await _context.Movies
            .Where(m => m.ReleaseDate > today)
            .OrderBy(m => m.ReleaseDate)
            .Take(top)
            .ProjectTo<MovieDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        // In Theaters → release date today or earlier, and at least one theater
        var inTheaters = await _context.Movies
            .Where(m => m.ReleaseDate <= today && m.MoviesTheaters.Any())
            .OrderBy(m => m.ReleaseDate)
            .Take(top)
            .ProjectTo<MovieDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var result = new LandingDTO
        {
            InTheaters = inTheaters,
            UpcomingReleases = upcomingReleases
        };

        return result;
    }


    
    [HttpGet("{id:int}", Name="GetMovieById")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
    { 
        var movie = await _context.Movies
        .Where(m => m.Id == id)
        .ProjectTo<MovieDetailsDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync();

        if (movie is null)
        {
            return NotFound();
        }

        return movie;
    } 
    
    [HttpPost]
    public async Task<ActionResult<Movie>> Post([FromForm] MovieCreationDTO movieCreationDTO)
    {
        var movie = _mapper.Map<Movie>(movieCreationDTO);

        if (movieCreationDTO.Poster is not null)
        {
            var url = await _fileStorage.Store(container, movieCreationDTO.Poster);
            movie.Poster = url;
        }
        AssignActorsOrder(movie);
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        var movieDTO = _mapper.Map<MovieDTO>(movie);
        return CreatedAtRoute("GetMovieById", new { id = movie.Id }, movieDTO);
    }

    private void AssignActorsOrder(Movie movie)
    {
        if (movie.MoviesActors is not null)
        {
            for (int i = 0; i < movie.MoviesActors.Count; i++)
            {
                movie.MoviesActors[i].Order = i;
            }
        }
    }


    [HttpGet("putget/{id:int}")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<ActionResult<MoviesPutGetDTO>> PutGet(int id)
    {
        var movie = await _context.Movies
            .ProjectTo<MovieDetailsDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            return NotFound();
        }

        var selectedGenresIds = movie.Genres.Select(g => g.Id).ToList();
        var nonSelectedGenres = await _context.Genres.Where(g => !selectedGenresIds.Contains(g.Id))
            .ProjectTo<GenreDTO>(_mapper.ConfigurationProvider).ToListAsync();
        
        var selectedTheatersIds = movie.Theaters.Select(t => t.Id).ToList();
        var nonSelectedTheaters = await _context.Theaters
            .Where(t => !selectedTheatersIds.Contains(t.Id))
            .ProjectTo<TheaterDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var response = new MoviesPutGetDTO();
        response.Movie= new MovieDTO(){Id = movie.Id, Title=movie.Title,Trailer=movie.Trailer,ReleaseDate =movie.ReleaseDate, Poster = movie.Poster};
        // response.Movie = _mapper.Map<MovieDTO>(movie);
        response.SelectedGenres = movie.Genres;
        response.NonSelectedGenres = nonSelectedGenres;
        response.SelectedTheaters = movie.Theaters;
        response.NonSelectedTheaters = nonSelectedTheaters;
        response.Actors = movie.Actors;
        return response;


    }

    [HttpPut("{id:int}")]
    [OutputCache(Tags = [cacheTag])]
    public async Task<IActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDto)  
    {
        var movie = await _context.Movies
            .Include(p => p.MoviesActors)
            .Include(p => p.MoviesGenres)
            .Include(p => p.MoviesTheaters)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        {
            return NotFound();
        }

        movie = _mapper.Map(movieCreationDto, movie);
        if (movieCreationDto.Poster != null)
        {
            movie.Poster = await _fileStorage.Edit(movie.Poster, container, movieCreationDto.Poster);
            
        }
        AssignActorsOrder(movie);
        await _context.SaveChangesAsync();
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        return NoContent();
    } 

   
    [HttpDelete ("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null)
        { 
            NotFound();
        }
    
        _context.Remove(movie);
        await _context.SaveChangesAsync();
        await _fileStorage.Delete(movie.Poster, container);
        await _outputCacheStore.EvictByTagAsync(cacheTag, CancellationToken.None);
        return NoContent();
    }

    [HttpGet("filter")]
    public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MoviesFilterDTO moviesFilterDTO)
    {
        var moviesQueryable = _context.Movies.AsQueryable();
        if (!string.IsNullOrEmpty(moviesFilterDTO.Title))
        {
            moviesQueryable = moviesQueryable.Where(m => m.Title.Contains((moviesFilterDTO.Title)));
        }

        if (moviesFilterDTO.InTheaters)
        {
            moviesQueryable = moviesQueryable.Where(m => m.MoviesTheaters.Select(mt => mt.MovieId).Contains(m.Id));
        }
        
        if (moviesFilterDTO.UpcomingReleases)
        {
            moviesQueryable = moviesQueryable.Where(m => m.ReleaseDate > DateTime.Today);
        }
        
        if (moviesFilterDTO.GenreId != 0)
        {
            moviesQueryable = moviesQueryable.Where(m => m.MoviesGenres.Select(mg => mg.GenreId).Contains(moviesFilterDTO.GenreId));
        }

        await HttpContext.InsertPaginationParameterHeader(moviesQueryable);
        var movies = await moviesQueryable.Paginate(moviesFilterDTO.PaginationDto)
            .ProjectTo<MovieDTO>(_mapper.ConfigurationProvider).ToListAsync();
        return movies;
    }
}