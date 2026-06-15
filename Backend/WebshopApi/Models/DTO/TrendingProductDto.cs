public class TrendingProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int TeamId { get; set; } // <--- Deze is belangrijk voor je frontend .find()
    public long Purchases { get; set; }
}