using Microsoft.AspNetCore.Mvc;
using MyDotNet9Api.Utilities;

namespace MyDotNet9Api.DTOs;

public class MovieCreationDTO
{
    public string Title { get; set; } = "example";
    public string? Trailer { get; set; }
    public DateTime ReleaseDate { get; set; }
    public IFormFile? Poster { get; set; }
    [ModelBinder(BinderType = typeof(TypeBinder))]
    public List<int>? GenresIds { get; set; }
    
    [ModelBinder(BinderType = typeof(TypeBinder))]
    public List<int>? TheatersIds { get; set; }
    
    [ModelBinder(BinderType = typeof(TypeBinder))]
    public List<ActorMovieCreationDTO>? Actors { get; set; }
}