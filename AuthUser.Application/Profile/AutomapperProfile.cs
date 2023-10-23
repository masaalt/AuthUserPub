using AutoMapper;
using AuthUser.Application.DTOs.User;
using AuthUser.Application.Features.Users.Commands;
using AuthUser.Domain;

namespace AuthUser.Application
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<CreateUserCommand, User>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<User, UserViewModel>()
                .ForMember(x => x.UserStatus, opt => opt.MapFrom(x => x.UserStatuses.StatusValue))
                .ReverseMap();
        }
    }
}
