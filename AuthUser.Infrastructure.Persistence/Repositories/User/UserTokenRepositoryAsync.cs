using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Domain;
using NHibernate;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public class UserTokenRepositoryAsync : GenericRepositoryAsync<UserToken>, IUserTokenRepositoryAsync
    {
        public UserTokenRepositoryAsync(ISession session) : base(session)
        {
        }
    }
}