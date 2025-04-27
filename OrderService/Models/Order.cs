﻿
namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public int Quantity {  get; set; }
        public OrderStatus Status { get; set; }
    }
}
