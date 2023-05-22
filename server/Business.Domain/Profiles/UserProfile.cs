using AutoMapper;
using Business.Domain.Model;
using Business.Domain.Model.DTO;
using Business.Domain.ViewModels;

namespace Business.Domain.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>()
                .AfterMap((dto, u) => u.UpdateId());

            CreateMap<User, UserDTO>();

            CreateMap<UserViewModel, User>();

            CreateMap<User, UserViewModel>();
        }
    }
}
