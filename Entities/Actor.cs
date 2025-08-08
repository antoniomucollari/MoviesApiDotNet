using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyDotNet9Api.DTOs;

namespace MyDotNet9Api.Entities;

public class Actor : IId
{
          public int Id { get; set; }
          [Required]
          [StringLength(150)]
          public required string Name { get; set; }
          public DateTime DateOfBirth { get; set; }
          [Unicode(false)]
          public string? Picture { get; set; }
}