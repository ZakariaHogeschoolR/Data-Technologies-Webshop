public class ShoppingCarts
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public List<int> ShoppingProducts{get;set;}//Items? betere naam
    // public int Quantity{get;set;}
    public float Total{get;set;}
    public DateOnly CreatedAt{get;set;}
    public DateOnly UpdatedAt{get;set;}
    public ShoppingCarts(int userid)
    {
        UserId = userid;
    }
    //calculate total price by multiplying 
    // (price x quantity of a product) add up per product
    public float CalculateTotal(List<Products> products){}

    //get a list of products
    public List<Products> Items(){}
    
    //make a list of tuples with the prices per product=> Tuple<product name, product price>
    public List<Tuple<string, float>> PricePerProduct(){}
}