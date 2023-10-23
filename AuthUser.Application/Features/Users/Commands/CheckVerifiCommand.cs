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
    public class CheckVerifiCommand : IRequest<Response<int>>
    {
        public string UserId { get; set; }

        public string VerificationCode { get; set; }
    }

    public class CheckVerifiCommandHandler : IRequestHandler<CheckVerifiCommand, Response<int>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        private readonly IUserStatusRepositoryAsync _userStatusRepository;

        public CheckVerifiCommandHandler(IUserRepositoryAsync userRepository, IUserStatusRepositoryAsync userStatusRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userStatusRepository = userStatusRepository;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CheckVerifiCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = (await _userRepository.FindByCondition(x => x.UserId.ToString() == request.UserId).ConfigureAwait(false)).AsQueryable().FirstOrDefault();

                if (request.VerificationCode != "")
                {
                    if (request.VerificationCode == user.LastVerificationCode)
                    {
                        user.UserStatuses = (await _userStatusRepository.FindByCondition(x => x.UserStatusId == 1).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                        var userObj = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
                        var resp = new Response<int>(request.VerificationCode);
                        resp.Succeeded = true;

                        return resp;
                    }
                    else
                    {
                        user.UserStatuses = (await _userStatusRepository.FindByCondition(x => x.UserStatusId == 4).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                        var userObjj = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
                        var respp = new Response<int>(request.VerificationCode);
                        respp.Succeeded = false;
                        respp.Message = "کد وارد شده صحیح نیست";
                        return respp;
                    }
                }
                else
                {
                    user.UserStatuses = (await _userStatusRepository.FindByCondition(x => x.UserStatusId == 4).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
                    var userObjj = await _userRepository.UpdateAsync(user).ConfigureAwait(false);
                    var respp = new Response<int>(request.VerificationCode);
                    respp.Succeeded = false;
                    respp.Message = "کد وارد شده کامل نیست";
                    return respp;
                }
            }
            catch(Exception ex)
            {
                var respp = new Response<int>(request.VerificationCode);
                respp.Succeeded = false;
                respp.Message = "خطا "+ ex;
                return respp;
            }
            
        }
    }
}