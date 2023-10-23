using AutoMapper;
using AuthUser.Application.Exceptions;
using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Application.Interfaces.Services;
using AuthUser.Application.Wrappers;
using AuthUser.Domain;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUser.Application.Features.Users.Commands
{
    public partial class CreateUserCommand : IRequest<Response<Guid>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
        public string UserMobile { get; set; }
        public string UserCreditLimit { get; set; }
        public string UserTimeLimit { get; set; }
        public string UserCountLimit { get; set; }
        public string ParentUserId { get; set; }
        public string LastVerificationCode { get; set; }
        public string RegSource { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Response<Guid>>
    {
        private readonly IUserRepositoryAsync _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordService _PasswordService;
        private readonly IUserStatusRepositoryAsync _userStatusRepository;
        private readonly IUserTokenRepositoryAsync _userTokenRepository;

        public CreateUserCommandHandler(IUserRepositoryAsync userRepository, IMapper mapper, IPasswordService passwordService, IUserStatusRepositoryAsync userStatusRepository, IUserTokenRepositoryAsync userTokenRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _PasswordService = passwordService;
            _userStatusRepository = userStatusRepository;
            _userTokenRepository = userTokenRepository;
        }
        public async Task<Response<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            List<ValidationFailure> errorMessages = new List<ValidationFailure>();
            var user = _mapper.Map<User>(request);
            var userExists = (await _userRepository.FindByCondition(x => x.UserName.ToLower() == request.UserName.ToLower()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            if (userExists != null)
            {
                var Status = (await _userStatusRepository.FindByCondition(x => x.StatusValue.ToLower() == "notconfirmed").ConfigureAwait(false)).AsQueryable().FirstOrDefault();

                if (userExists.UserStatuses == Status)
                {
                    var usrTokens = (await _userTokenRepository.FindByCondition(x => x.UserId == userExists.UserId).ConfigureAwait(false)).AsQueryable();

                    foreach (var tkn in usrTokens)
                    {
                        await _userTokenRepository.DeleteAsync(tkn);
                    }


                    var res = await _userRepository.DeleteAsync(userExists).ConfigureAwait(true);
                    
                    errorMessages.Clear();
                }
                else
                {
                    errorMessages.Add(new ValidationFailure("UserExists", "User Name already exists."));
                }
            }
            if (errorMessages.Count > 0) throw new ValidationException(errorMessages);
            user.CreatedDate = DateTime.Now;
            user.UserStatuses = (await _userStatusRepository.FindByCondition(x => x.StatusValue.ToLower() == "notconfirmed").ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            string passwordHash, passwordSalt;
            _PasswordService.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            
            var userObject = await _userRepository.AddAsync(user).ConfigureAwait(false);
            return new Response<Guid>(userObject.UserId);
        }
    }

}
