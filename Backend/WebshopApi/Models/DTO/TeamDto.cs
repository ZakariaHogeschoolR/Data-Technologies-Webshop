using System.ComponentModel.DataAnnotations;

namespace DataTransferObject;

public record TeamDto(int Id, [Required] string Name, [Required] string Type);
