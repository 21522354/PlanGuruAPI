using MediatR;
using Application.Common.Interface.Persistence;

namespace Application.Users.Queries.CheckUserExists
{
    public class CheckUserExistsQuery : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }

    public class CheckUserExistsHandler : IRequestHandler<CheckUserExistsQuery, bool>
    {
        private readonly IUserRepository _userRepository;

        public CheckUserExistsHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            return user != null;
        }
    }
}
