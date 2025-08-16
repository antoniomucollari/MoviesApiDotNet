using System.ComponentModel.DataAnnotations;

namespace MyDotNet9Api.DTOs;

public class RatingCreationDTO
{
    public int MovieId { get; set; }
    [Range(1,5)]
    public int Punctuation { get; set; }
    public string UserEmail { get; set; }
}