using System;

namespace AuthUser.Application.DTOs.User
{
    public class UserViewModel
    {
        public virtual Guid UserId { get; set; }
        public virtual string UserStatus { get; set; }
        public virtual string UserName { get; set; }
        public virtual string UserEmail { get; set; }
        public virtual string CreatedDate { get; set; }
        public virtual string UserType { get; set; }
        public virtual string RegSource { get; set; }
        public virtual string UserMobile { get; set; }
        public virtual string UserCreditLimit { get; set; }
        public virtual string UserTimeLimit { get; set; }
        public virtual string UserCountLimit { get; set; }
        public virtual string ParentUserId { get; set; }
        public virtual string LastVerificationCode { get; set; }

    }
}