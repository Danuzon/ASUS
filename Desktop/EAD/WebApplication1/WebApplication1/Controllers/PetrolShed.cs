using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // petrol sehed API logic implementd here
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

            var dbListPetrolShed = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").Find(s => s.PetrolShedId == id).ToList();

            var dbListUser = dbClient.GetDatabase("testdb").GetCollection<User>("User").Find(s => s.PetrolShed == id).ToList();

            var shedPetrol = dbListPetrolShed[0].AvailableQuantity;

            float QtyCountUserPetrol = 0;

            for(int i=0; i < dbListUser.Count; i++){
                QtyCountUserPetrol = QtyCountUserPetrol + dbListUser[i].PetrolFillQuantity;
            }

            float totlaAvailableQty = shedPetrol - QtyCountUserPetrol;
            //JsonResult jsonResult = new JsonResult(dbList);

            // Debug.WriteLine("Testirfan");
            // Debug.WriteLine("Testirfan2" + new JsonResult(dbList).Value);

            //return new JsonResult(dbList[0].PetrolShedName);
            // return new JsonResult(dbList);
            return new JsonResult(totlaAvailableQty);
        }

        [Route("petrolShedDetails")]
        [HttpGet]
        public JsonResult GetPetrolShedDetails()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var dbList = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").AsQueryable();

           

            var shedInfo = new ArrayList();

            foreach (var item in dbList)
            {
                PetrolShedInfo petrolShedInfo = new PetrolShedInfo();

                petrolShedInfo.PetrolShedId = item.PetrolShedId;
                petrolShedInfo.PetrolShedName = item.PetrolShedName;

                var dbListPetrolShed = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("PetrolShed").Find(s => s.PetrolShedId == item.PetrolShedId).ToList();

                var dbListUser = dbClient.GetDatabase("testdb").GetCollection<User>("User").Find(s => s.PetrolShed == item.PetrolShedId).ToList();

                var shedPetrol = dbListPetrolShed[0].AvailableQuantity;

                float QtyCountUserPetrol = 0;

                int carCount = 0;
                int motBike = 0;

                for (int i = 0; i < dbListUser.Count; i++)
                {
                    QtyCountUserPetrol = QtyCountUserPetrol + dbListUser[i].PetrolFillQuantity;
                    if (dbListUser[i].VehicleType.Equals("car") && dbListUser[i].PetrolFillStatus.Equals("join"))
                    {
                        carCount++;
                    }
                    if (dbListUser[i].VehicleType.Equals("motor bike") && dbListUser[i].PetrolFillStatus.Equals("join"))
                    {
                        motBike++;
                    }
                }

                float totlaAvailableQty = shedPetrol - QtyCountUserPetrol;
                petrolShedInfo.AvailableQuantity = totlaAvailableQty;

                int counting = dbClient.GetDatabase("testdb").GetCollection<User>("User").Find(s => s.PetrolFillStatus == "join" && s.PetrolShed == item.PetrolShedId).ToList().Count;
                petrolShedInfo.peopleCount = counting;

                petrolShedInfo.carCounting = carCount;
                petrolShedInfo.motorBikeCounting = motBike;


                shedInfo.Add(petrolShedInfo);

            };

            return new JsonResult(shedInfo);
        }


    }
}
