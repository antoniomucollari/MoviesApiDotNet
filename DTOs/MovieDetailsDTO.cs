namespace MyDotNet9Api.DTOs;

public class MovieDetailsDTO
{
    public List<GenreDTO> Genres { get; set; } = new List<GenreDTO>();
    public List<TheaterDTO> Theaters { get; set; } = new List<TheaterDTO>();
    public List<MovieActorDTO> Actors { get; set; } = new List<MovieActorDTO>();
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Trailer { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Poster { get; set; }
    public double AverageVote { get; set; }
    public int UserVote { get; set; }
}
