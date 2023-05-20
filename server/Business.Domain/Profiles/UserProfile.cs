using AutoMapper;
using Business.Domain.Model;
using Business.Domain.Model.DTO;

namespace Business.Domain.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>()
                .BeforeMap((dto, u) => u.UpdateId());

            CreateMap<User, UserDTO>();
        }
    }
}
