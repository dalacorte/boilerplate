using Business.Repository.Maps;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Business.Repository.Persistence
{
    public static class MongoPersistence
    {
        public static void Configure()
        {
            UserMap.Configure();
            LogRequestMap.Configure();

            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.CSharpLegacy));

            ConventionPack pack = new ConventionPack
                {
                    new IgnoreExtraElementsConvention(true),
                    new IgnoreIfDefaultConvention(true)
                };
            ConventionRegistry.Register("Conventions", pack, t => true);
        }
    }
}
