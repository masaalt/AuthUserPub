using AuthUser.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public partial class LoginLogTypeMap : ClassMapping<LoginLogType>
    {
        public LoginLogTypeMap()
        {
            Table("LoginLogTypes");
            Lazy(true);
            Id(x => x.LoginLogTypeId, map => map.Generator(Generators.Guid));
            Property(x => x.LoginLogTypeName, map => map.Length(50));
        }
    }
}