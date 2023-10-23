using AuthUser.Application.Interfaces.Repositories;
using NHibernate;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public class UserRepositoryAsync : GenericRepositoryAsync<Domain.User>, IUserRepositoryAsync
    {
        public UserRepositoryAsync(ISession session) : base(session)
        { }
    }
}