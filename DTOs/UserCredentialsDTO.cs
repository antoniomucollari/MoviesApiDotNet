using System.ComponentModel.DataAnnotations;

namespace MyDotNet9Api.DTOs;

public class UserCredentialsDTO
{
    [EmailAddress]
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}