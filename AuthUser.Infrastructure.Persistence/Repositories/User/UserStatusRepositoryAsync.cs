﻿using AuthUser.Application.Interfaces.Repositories;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthUser.Infrastructure.Persistence.Repositories
{
    public class UserStatusRepositoryAsync : GenericRepositoryAsync<Domain.UserStatuses>, IUserStatusRepositoryAsync
    {
        public UserStatusRepositoryAsync(ISession session) : base(session)
        { }
    }
}
