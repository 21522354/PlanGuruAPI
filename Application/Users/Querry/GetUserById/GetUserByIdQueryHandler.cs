using Application.Common.Interface.Persistence;
using Application.Users.Querry.GetUserById;
using MediatR;

namespace Application.Users.Query.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return null;
            }

            return new GetUserByIdResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Avatar = user.Avatar,
                Email = user.Email,
                IsHavePremium = user.IsHavePremium
            };
        }
    }
}
