namespace models
{
    /*
    my goal with this class is to have a dedicated 
    shopping cart for each logged in user to have.
    Why? Persistence, say their battery dies or
    they get distracted with something and
    close the browser and open the webshop again.
    they'll still have their items in the cart. neat.
    when a user is made, a shopping cart is 
    made for them as well.
    1:1 relationship, 1 ShoppingCart for 1 User.
    */
    public class ShoppingCarts
    {
        public int Id{get;set;}
        public int UserId{get;set;}
        public List<int> ShoppingProducts{get;set;}//Items? betere naam
        // public int Quantity{get;set;}
        public float Total{get;set;}
        public DateOnly CreatedAt{get;set;}
        public DateOnly UpdatedAt{get;set;}
        // public ShoppingCarts(int userid)
        // {
        //     UserId = userid;
        // }
        public ShoppingCarts(){}
        /*
        gonna ask in meeting if stuff like sum price, or all products
        should only be done with sql
        */
        //calculate total price by multiplying 
        // (price x quantity of a product) add up per product
        // public float CalculateTotal(List<Products> products){}

        //get a list of products
        // public List<Products> Items(){}
        
        //make a list of tuples with the prices per product=> Tuple<product name, product price>
        // public List<Tuple<string, float>> PricePerProduct(){}
    }
}