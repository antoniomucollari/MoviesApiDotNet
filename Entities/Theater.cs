using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace MyDotNet9Api.Entities;

public class Theater
{
    public int Id { get; set; }
    [Required]
    [StringLength(75)]
    public required string Name { get; set; }

    public required Point Loaction { get; set; }
}