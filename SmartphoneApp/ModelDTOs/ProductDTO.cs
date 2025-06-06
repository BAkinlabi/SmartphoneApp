namespace SmartphoneApp.ModelDTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public double Price { get; set; }
        public double DiscountPercentage { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public List<string>? Tags { get; set; }
        public string? Brand { get; set; }
        public string? SKU { get; set; }
        public int Weight { get; set; }
        public DimensionsDTO? Dimensions { get; set; }
        public string? WarrantyInformation { get; set; }
        public string? ShippingInformation { get; set; }
        public string? AvailabilityStatus { get; set; }
        public List<ReviewDTO>? Reviews { get; set; }
        public string? ReturnPolicy { get; set; }
        public int MinimumOrderQuantity { get; set; }
        public MetaDTO? Meta { get; set; }
        public List<string>? Images { get; set; }
        public string? Thumbnail { get; set; }
    }
}
