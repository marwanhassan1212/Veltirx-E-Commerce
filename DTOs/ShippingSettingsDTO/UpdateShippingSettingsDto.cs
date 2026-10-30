using System.ComponentModel.DataAnnotations;

namespace Veltrix.DTOs.ShippingSettingsDTO
{
    public class UpdateShippingSettingsDto
    {
        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal ShippingFee { get; set; }
    }
}
