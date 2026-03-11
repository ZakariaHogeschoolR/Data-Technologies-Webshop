namespace models
{
    /*
    my goal for this class is to give users the ability
    to keep a list of products they want to or
    are interested in, but can't buy it right now.
    M:1 relationship, Many Wishlists can be from the same 1 User
    */
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