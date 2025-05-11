using Domain.Entities.ECommerce;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interface.Persistence
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Guid orderId);
    }
}
