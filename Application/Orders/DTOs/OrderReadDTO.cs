using System;

namespace Application.Orders.DTOs
{
    public class OrderReadDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SellerId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
