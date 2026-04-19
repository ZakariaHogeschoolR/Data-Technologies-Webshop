using System.ComponentModel.DataAnnotations;

namespace DataTransferObject;

public record CategoryDto(int Id, [Required] string Name);
