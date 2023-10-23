using System;
using System.Collections.Generic;
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
    public class GetAllUsersByParentIdQuery : IRequest<Response<IEnumerable<UserViewModel>>>
    {
        public string ParentUserId { get; set; }
    }

    public class GetAllUsersByParentIdQueryHandler : IRequestHandler<GetAllUsersByParentIdQuery, Response<IEnumerable<UserViewModel>>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        public GetAllUsersByParentIdQueryHandler(IUserRepositoryAsync userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<Response<IEnumerable<UserViewModel>>> Handle(GetAllUsersByParentIdQuery request, CancellationToken cancellationToken)
        {
            var usersList = (await _userRepository.FindByCondition(x => x.ParentUserId.Equals(request.ParentUserId)).ConfigureAwait(false)).AsQueryable().ToList();
            return new Response<IEnumerable<UserViewModel>>(_mapper.Map<IEnumerable<UserViewModel>>(usersList));
        }
    }
}