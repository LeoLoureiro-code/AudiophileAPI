namespace AudiophileAPI.DTO
{
    public class ProductDTO
    {

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Features { get; set; } = null!;

        public decimal Price { get; set; }

        public int Stock {  get; set; }

        public string ImageURL {  get; set; } = null!;
    }
}
