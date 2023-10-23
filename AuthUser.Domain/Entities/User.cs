using System;
using System.Collections.Generic;

namespace AuthUser.Domain
{
    public class User : AuditableBaseEntity
    {
        public User()
        {
            UserTokens = new List<UserToken>();
        }

        public virtual Guid UserId { get; set; }
        public virtual UserStatuses UserStatuses { get; set; }
        public virtual string UserName { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual string UserType { get; set; }
        public virtual string RegSource { get; set; }
        public virtual string UserMobile { get; set; }
        public virtual string UserCreditLimit { get; set; }
        public virtual string UserTimeLimit { get; set; }
        public virtual string UserCountLimit { get; set; }
        public virtual string ParentUserId { get; set; }
        public virtual string LastVerificationCode { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual IList<UserToken> UserTokens { get; set; }
    }
}