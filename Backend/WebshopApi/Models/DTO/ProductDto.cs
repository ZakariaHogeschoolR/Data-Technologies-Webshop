using System.ComponentModel.DataAnnotations;

namespace DataTransferObject;

public record ProductDto(
    int? Id,
    [Required] string ProductImage,
    [Required] string Name,
    [Required] string Description,
    [Range(0.01, 999999)] decimal Price,
    [Required] int TeamId);
