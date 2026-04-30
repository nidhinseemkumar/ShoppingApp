namespace ShoppingApp.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public ReviewDto? UserReview { get; set; }
    }
}
