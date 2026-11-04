namespace Veltrix.DTOs.Storefront
{
    public class StorefrontOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<StorefrontOptionValueDto> Values { get; set; } = new();
    }

    public class StorefrontOptionValueDto
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}