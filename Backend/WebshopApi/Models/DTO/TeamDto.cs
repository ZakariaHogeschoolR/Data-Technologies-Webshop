using System.ComponentModel.DataAnnotations;

namespace DataTransferObject;

public class TeamDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }
}
