public class Wishlist
{
    public int Id{get;set;}
    public string Name{get;set;}
    public List<Product> Products;
    // public int Total{get;set;}
    public Wishlist(){}
    public DateOnly CreatedAt{get;set;}
    public DateOnly UpdatedAt{get;set;}
}