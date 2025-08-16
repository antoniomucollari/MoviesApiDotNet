using Microsoft.AspNetCore.Identity;

namespace MyDotNet9Api.Entities;

public class Rating
{
    public int Id { get; set; }
    public int Punctuation { get; set; }
    public int MovieId { get; set; }
    public required string UserId { get; set; }
    public Movie Movie { get; set; }
    public IdentityUser User { get; set; }
}