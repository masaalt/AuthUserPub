namespace AuthUser.WebApi.Controllers.V1
{
    using System.Threading.Tasks;
    using AuthUser.Application.Features.Users.Commands;
    using AuthUser.Application.Features.Users.Queries;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    
    public class UserController : BaseApiController
    {
        [HttpPost("CreateUser")]
        public async Task<IActionResult> Post(CreateUserCommand command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(GetAllUsersQuery command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPost("GetUserById")]
        public async Task<IActionResult> GetUserById(GetUserByIdQuery command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPost("GetUserByUserName")]
        public async Task<IActionResult> GetUserByUserName(GetUserByUserNameQuery command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPut("UpdateUserById")]
        public async Task<IActionResult> UpdateUserById(UpdateUserCommand command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPost("GetAllUsersByParentId")]
        public async Task<IActionResult> GetAllUsersByParentId(GetAllUsersByParentIdQuery command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPut("AddUserVerificationByUserId")]
        public async Task<IActionResult> AddUserVerificationByUserId(UpdateUserCommand command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        [Authorize]
        [HttpPost("CheckVerificationByVerificationCode")]
        public async Task<IActionResult> CheckVerificationByVerificationCode(CheckVerifiCommand command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }
    }
}
