using AuthUser.Application.Interfaces.Repositories;
using AuthUser.Domain;
using NHibernate;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public class LoginLogTypeRepositoryAsync : GenericRepositoryAsync<LoginLogType>, ILoginLogTypeRepositoryAsync
    {
        public LoginLogTypeRepositoryAsync(ISession session) : base(session)
        { }
    }
}