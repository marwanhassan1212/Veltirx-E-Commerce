namespace Veltrix.DTOs.VariantOptionsDTO
{
    public class VariantOptionsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int ProductId { get; set; }

        public List<VariantOptionValueDto> Values { get; set; } = new();
    }

    public class VariantOptionValueDto
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}