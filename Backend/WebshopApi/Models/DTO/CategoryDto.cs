using System.ComponentModel.DataAnnotations;

namespace DataTransferObject;

public class CategoryDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
}
