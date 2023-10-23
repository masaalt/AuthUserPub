using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Domain;
using NHibernate;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public class LoginLogRepositoryAsync: GenericRepositoryAsync<LoginLog>, ILoginLogRepositoryAsync
    {
        public LoginLogRepositoryAsync(ISession session) : base(session)
        { }
    }
}