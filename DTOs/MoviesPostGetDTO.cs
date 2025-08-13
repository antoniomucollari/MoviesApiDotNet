namespace MyDotNet9Api.DTOs;

public class MoviesPostGetDTO
{
    public List<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
    public List<TheaterDTO> Theaters { get; set; } = new List<TheaterDTO>();
}   