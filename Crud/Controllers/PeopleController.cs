using Crud.Models;
using Crud.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Crud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPeople _people;
        public PeopleController(IPeople people)
        {
            _people = people;

        }
        [HttpPost]
        public async Task<IActionResult> Create(People people)
        {
            var id = await _people.Create(people);
            return new JsonResult(id.ToString());
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var people = await _people.Get(ObjectId.Parse(id));
            return new JsonResult(people);

        }
        [HttpGet("getByName/{Name}")]
        public async Task<IActionResult> GetByName(string Name)
        {
            var people = await _people.GetByName(Name);
            return new JsonResult(people);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, People people)
        {
            var peoples = await _people.Update(ObjectId.Parse(id), people);
            return new JsonResult(peoples);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var people = await _people.Delete(ObjectId.Parse(id));
            return new JsonResult(people);
        }
        [HttpGet("Fetch")]
        public async Task<IActionResult> GetAll()
        {
            var people = await _people.GetALL();    
            return new JsonResult(people);
        }
    }
}
