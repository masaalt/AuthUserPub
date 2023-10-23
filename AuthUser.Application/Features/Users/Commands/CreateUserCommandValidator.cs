using AuthUser.Application.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthUser.Application.Features.Users.Commands
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserRepositoryAsync userRepository;

        public CreateUserCommandValidator(IUserRepositoryAsync userRepository)
        {
            this.userRepository = userRepository;

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }

        private async Task<bool> IsUniqueUserName(string username, CancellationToken cancellationToken)
        {
            var userObject = (await userRepository.FindByCondition(x => x.UserName.ToLower() == username.ToLower()).ConfigureAwait(false)).AsQueryable().FirstOrDefault();
            if (userObject != null)
            {
                return false;
            }
            return true;
        }
    }
}
