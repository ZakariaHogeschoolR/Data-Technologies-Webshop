public class ShoppingCart
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public int[] ShoppingProducts{get;set;}//Items? betere naam
    // public int Quantity{get;set;}
    public float Total{get;set;}
    public ShoppingCart(int userid)
    {
        UserId = userid;
    }
    //calculate total price by multiplying 
    // (price x quantity of a product) add up per product
    public float CalculateTotal(Product[] products){}
    //get a list of products
    public Product[] Items(){}
    //make an array with the prices per product
    public float[] PricePerProduct(){}
}