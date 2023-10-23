using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Application.Wrappers;
using AuthUser.Domain;
using MediatR;

namespace AuthUser.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<Response<Guid>>
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; }
        public int UserStatus { get; set; }

        public string UserMobile { get; set; }
        public string UserCreditLimit { get; set; }
        public string UserTimeLimit { get; set; }
        public string UserCountLimit { get; set; }
        public string ParentUserId { get; set; }
        public string LastVerificationCode { get; set; }
        public string RegSource { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<Guid>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        private readonly IUserStatusRepositoryAsync _userStatusRepository;

        public UpdateUserCommandHandler(IUserRepositoryAsync userRepository, IUserStatusRepositoryAsync userStatusRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userStatusRepository = userStatusRepository;
            _mapper = mapper;
        }

        public async Task<Response<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = (await _userRepository.FindByCondition(x => x.UserId == request.UserId).ConfigureAwait(false)).AsQueryable().FirstOrDefault();

            if (request.LastVerificationCode != null && request.LastVerificationCode != "")
            {
                user.LastVerificationCode = request.LastVerificationCode;
                user.UserStatuses = (await _userStatusRepository.FindByCondition(x => x.UserStatusId == 4).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                var userObj = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
                return new Response<Guid>(userObj.UserId);
            }
            user.UserEmail = (request.UserEmail != null && request.UserEmail != "") ? request.UserEmail : user.UserEmail;

            user.UserMobile = (request.UserMobile != "" && request.UserMobile != null) ? request.UserMobile : user.UserMobile;
            user.UserCreditLimit = (request.UserCreditLimit != "" && request.UserCreditLimit != null) ? request.UserCreditLimit : user.UserCreditLimit;
            user.UserTimeLimit = (request.UserTimeLimit != "" && request.UserTimeLimit != null) ? request.UserTimeLimit : user.UserTimeLimit;
            user.UserCountLimit = (request.UserCountLimit != "" && request.UserCountLimit != null) ? request.UserCountLimit : user.UserCountLimit;
            user.ParentUserId = (request.ParentUserId != "" && request.ParentUserId != null) ? request.ParentUserId : user.ParentUserId;
            user.RegSource = (request.RegSource != "" && request.RegSource != null) ? request.RegSource : user.RegSource;
            user.UserStatuses = (request.UserStatus != null && request.UserStatus != 0) ? (await _userStatusRepository.FindByCondition(x => x.UserStatusId == request.UserStatus).ConfigureAwait(false)).AsQueryable().FirstOrDefault(): user.UserStatuses;

            var userObject = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
            return new Response<Guid>(userObject.UserId);
        }
    }
}