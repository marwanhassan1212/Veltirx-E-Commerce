using System.ComponentModel.DataAnnotations;
using Veltrix.Enums;

namespace Veltrix.DTOs.OrderDTO
{
    public class UpdateOrderStatusDto
    {
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus OrderStatus { get; set; }
    }
}
