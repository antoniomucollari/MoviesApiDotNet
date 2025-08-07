using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace MyDotNet9Api.DTOs;

public class ActorCreationDTO
{
    [Required]
    [StringLength(150)]
    public required string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public IFormFile? Picture { get; set; }
}