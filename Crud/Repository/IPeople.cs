using Crud.Models;
using MongoDB.Bson;

namespace Crud.Repository
{
    public interface IPeople
    {
        Task<ObjectId> Create(People people);
        Task<People> Get(ObjectId id);
        Task<IEnumerable<People>> GetALL();
        Task<IEnumerable<People>> GetByName(string Name);

        Task<bool> Update(ObjectId objectId,People people);
        Task<bool> Delete(ObjectId objectId);

    }
}
