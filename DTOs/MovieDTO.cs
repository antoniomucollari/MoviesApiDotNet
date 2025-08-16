namespace MyDotNet9Api.DTOs;

public class MovieDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Trailer { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Poster { get; set; }
    public double AverageVote { get; set; }
    public int userVote { get; set; }
}