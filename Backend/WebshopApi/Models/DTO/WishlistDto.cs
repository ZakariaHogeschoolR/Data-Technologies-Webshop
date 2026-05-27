namespace DataTransferObject;

public class WishlistDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ProductId { get; set; }
    public int UserId { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly UpdatedAt { get; set; }
}
