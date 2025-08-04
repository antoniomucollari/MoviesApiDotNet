using System.ComponentModel.DataAnnotations;
using MyDotNet9Api.Validation;

namespace MyDotNet9Api.Entities;

public class Genre
{
    public int Id { get; set; }
    [Required(ErrorMessage = "You must fill the {0} field")]
    [StringLength(maximumLength:10)]
    [FirstLettercase]
    public string Name { get; set; }
}