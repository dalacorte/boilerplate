using Business.Domain.Models.Others;
using MongoDB.Bson.Serialization;

namespace Business.Repository.Maps
{
    public class LogRequestMap
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<LogRequest>(map =>
            {
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.Route).SetIsRequired(true);
                map.MapMember(x => x.Controller).SetIsRequired(true);
                map.MapMember(x => x.HttpVerb).SetIsRequired(true);
                map.MapMember(x => x.QueryString).SetIsRequired(true);
                map.MapMember(x => x.RequestBody).SetIsRequired(true);
                map.MapMember(x => x.UserId).SetIsRequired(true);
                map.MapMember(x => x.UserIp).SetIsRequired(true);
                map.MapMember(x => x.HttpResponseStatusCode).SetIsRequired(true);
                map.MapMember(x => x.ExecutionTime).SetIsRequired(true);
                map.MapMember(x => x.Date).SetIsRequired(true);
            });
        }
    }
}
