namespace models
{
    public class Wishlists
    {
        public int Id{get;set;}
        public string Name{get;set;}
        public List<Products> Products;
        // public int Total{get;set;}
        public Wishlists(){}
        public DateOnly CreatedAt{get;set;}
        public DateOnly UpdatedAt{get;set;}
    }
}