using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AuthUser.Application.DTOs.User;
using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Application.Wrappers;
using MediatR;

namespace AuthUser.Application.Features.Users.Queries
{
    public class GetUserByUserNameQuery : IRequest<Response<UserViewModel>>
    {
        public string UserName { get; set; }
    }

    public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, Response<UserViewModel>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        public GetUserByUserNameQueryHandler(IUserRepositoryAsync userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<Response<UserViewModel>> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
        {
            var userObject = (await _userRepository.FindByCondition(x => x.UserName.Equals(request.UserName)).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            return new Response<UserViewModel>(_mapper.Map<UserViewModel>(userObject));
        }
    }
}