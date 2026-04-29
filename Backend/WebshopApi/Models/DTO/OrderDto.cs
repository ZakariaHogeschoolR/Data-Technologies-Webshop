using System.ComponentModel.DataAnnotations;

namespace DataTransferObject;

public class OrderDto
{
    public int Id { get; set; }

    [Required]
    public int WinkelwagenUsersId { get; set; }

    [Required]
    public decimal Total { get; set; }

    [Required]
    public bool PaymentStatus { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}
