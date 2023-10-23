using AuthUser.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthUser.Application.Interfaces.Repositories
{
    public interface IUserStatusRepositoryAsync : IGenericRepositoryAsync<UserStatuses>
    {
    }
}
