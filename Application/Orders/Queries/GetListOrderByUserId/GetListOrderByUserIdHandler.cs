using MediatR;
using AutoMapper;
using Domain.Entities.ECommerce;
using Application.Common.Interface.Persistence;
using Application.Orders.DTOs;

namespace Application.Orders.Queries.GetListOrderByUserId
{
    public class GetListOrderByUserIdQuery : IRequest<List<OrderReadDTO>>
    {
        public Guid UserId { get; set; }
    }

    public class GetListOrderByUserIdHandler : IRequestHandler<GetListOrderByUserIdQuery, List<OrderReadDTO>>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public GetListOrderByUserIdHandler(IOrderRepository orderRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        public async Task<List<OrderReadDTO>> Handle(GetListOrderByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(request.UserId);
            return _mapper.Map<List<OrderReadDTO>>(orders);
        }
    }
}
