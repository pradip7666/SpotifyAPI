using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crud.Models
{
    public class People
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

    }
}
