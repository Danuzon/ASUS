using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetrolShedController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PetrolShedController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var dbList = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").AsQueryable();

            return new JsonResult(dbList);
        }

        [HttpPost]
        public JsonResult Post(PetrolShed dep)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            // int LastPetrolShedId = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").AsQueryable().Count();
            // dep.PetrolShedId = LastPetrolShedId + 1;

            dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").InsertOne(dep);

            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(PetrolShed dep)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var filter = Builders<PetrolShed>.Filter.Eq("PetrolShedId", dep.PetrolShedId);

            var update = Builders<PetrolShed>.Update.Set("AvailableQuantity", dep.AvailableQuantity);



            dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(String id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var filter = Builders<PetrolShed>.Filter.Eq("PetrolShedId", id);


            dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").DeleteOne(filter);

            return new JsonResult("Deleted Successfully");
        }

        [Route("petrolQty/{id}")]
        [HttpGet]
        public JsonResult GetCountUserInQueue(String id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            // var filter = "{PetrolFillStatus:join}";

            var dbList = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").Find(s => s.PetrolShedId == id).ToList();

            JsonResult jsonResult = new JsonResult(dbList);

            Debug.WriteLine("Testirfan");
            Debug.WriteLine("Testirfan2" + new JsonResult(dbList).Value);

            return new JsonResult(dbList[0].PetrolShedName);
            // return new JsonResult(10);
        }

         

    }
}
