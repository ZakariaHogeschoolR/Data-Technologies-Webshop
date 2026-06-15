namespace models;

public class Orders
{
    public int Id { get; set; }
    public int WinkelwagenUsersId { get; set; }
    public decimal Total { get; set; }
    public bool PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
