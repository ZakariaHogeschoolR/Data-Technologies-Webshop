using System.ComponentModel.DataAnnotations;

namespace DataTransferObject
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        public string ProductImage { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0.01, 999999)]
        public decimal Price { get; set; }
    }
}
