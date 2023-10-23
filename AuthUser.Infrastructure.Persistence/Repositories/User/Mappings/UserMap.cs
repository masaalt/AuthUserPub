using AuthUser.Domain;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public partial class UserMap : ClassMapping<Domain.User>
    {
        public UserMap()
        {
            Table("Users");
            Lazy(true);
            Id(x => x.UserId, map =>
            {
                map.Generator(Generators.Guid);
                map.Column("UserId");
                map.UnsavedValue(Guid.Empty);
            });
            Property(x => x.UserName, map => { map.NotNullable(true); map.Length(50); });
            //Property(x => x.FirstName, map => { map.NotNullable(true); map.Length(50); });
            //Property(x => x.LastName, map => map.Length(50));
            //Property(x => x.UserEmail, map => { map.NotNullable(true); map.Length(50); });
            Property(x => x.PasswordHash, map => map.NotNullable(true));
            Property(x => x.PasswordSalt, map => map.NotNullable(true));
            Property(x => x.CreatedDate, map => map.NotNullable(true));
            //Property(x => x.UpdatedDate);
            //Property(x => x.CreatedBy, map => map.NotNullable(true));
            //Property(x => x.UpdatedBy);
            Property(x => x.UserType);
            Property(x => x.RegSource);
            Property(x => x.UserEmail);
            Property(x => x.UserMobile);
            Property(x => x.UserCreditLimit);
            Property(x => x.UserTimeLimit);
            Property(x => x.UserCountLimit);
            Property(x => x.ParentUserId);
            Property(x => x.LastVerificationCode);

            ManyToOne(x => x.UserStatuses, map => { map.Column("UserStatusId"); map.Cascade(Cascade.All); map.Fetch(FetchKind.Select); });

            Bag(x => x.UserTokens, colmap => { colmap.Key(x => x.Column("UserId")); colmap.Inverse(true); colmap.Cascade(Cascade.Persist); }, map => { map.OneToMany(a => a.Class(typeof(UserToken))); });
        }
    }
}