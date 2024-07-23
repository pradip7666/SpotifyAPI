using Crud.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Crud.Repository
{
    public class PeopleRepo : IPeople
    {
        private readonly IMongoCollection<People> _people;

        public PeopleRepo(IMongoClient client)
        {
            var database = client.GetDatabase("MT24037_DB");
            var collection = database.GetCollection<People>(nameof(People));

            _people = collection;
        }
        public async Task<ObjectId> Create(People people)
        {
            await _people.InsertOneAsync(people);

            return people.Id;
        }

     
        public async Task<bool> Delete(ObjectId objectId)
        {
           var filter = Builders<People>.Filter.Eq(x => x.Id,objectId);
            var result = await _people.DeleteOneAsync(filter);
            return result.DeletedCount == 1;
        }

        public Task<People> Get(ObjectId id)
        {
            var filter = Builders<People>.Filter.Eq(x => x.Id, id);
            var people = _people.Find(filter).FirstOrDefaultAsync();
            return people;
        }

        public async Task<IEnumerable<People>> GetALL()
        {
            var people = await _people.Find(_ => true).ToListAsync();
            return people;
        }

        public async Task<IEnumerable<People>> GetByName(string Name)
        {
            var filter = Builders<People>.Filter.Eq(x=>x.Name, Name);
            var people = await _people.Find(filter).ToListAsync();
            return people;
        }

        public async Task<bool> Update(ObjectId objectId, People people)
        {
            var filter = Builders<People>.Filter.Eq(x => x.Id , objectId);
            var update = Builders<People>.Update
                .Set(x => x.Name, people.Name)
                .Set(x => x.Phone, people.Phone)
                .Set(x => x.Address, people.Address);
            var result = await _people.UpdateOneAsync(filter, update);

            return result.ModifiedCount == 1;
        }
    }
}
