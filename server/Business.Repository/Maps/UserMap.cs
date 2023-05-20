using Business.Domain.Model;
using MongoDB.Bson.Serialization;

namespace Business.Repository.Maps
{
    public class UserMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id);
                map.MapMember(x => x.Username).SetIsRequired(true);
                map.MapMember(x => x.Password).SetIsRequired(true);
                map.MapMember(x => x.Email).SetIsRequired(true);
                map.MapMember(x => x.Name).SetIsRequired(true);
                map.MapMember(x => x.ProfilePicture);
                map.MapMember(x => x.Role).SetIsRequired(true);
                map.MapMember(x => x.AccessToken).SetIsRequired(true);
                map.MapMember(x => x.RefreshToken).SetIsRequired(true);
                map.MapMember(x => x.IsActive).SetIsRequired(true);
                map.MapMember(x => x.Language);
                map.MapMember(x => x.CreatedDate).SetIsRequired(true);
            });
        }
    }
}
