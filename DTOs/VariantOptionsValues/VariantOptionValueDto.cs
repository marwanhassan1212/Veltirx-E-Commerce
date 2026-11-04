namespace Veltrix.DTOs.VariantOptionValueDTO
{
    public class VariantOptionValueDto
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int VariantOptionId { get; set; }
    }
}