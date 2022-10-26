using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public UserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var dbList = dbClient.GetDatabase("testdb").GetCollection<User>("User").AsQueryable();

            return new JsonResult(dbList);
        }

        [HttpPost]
        public JsonResult Post(User usr)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

           // int LastUserId = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("User").AsQueryable().Count();
           // emp.UserId = LastUserId + 1;

            dbClient.GetDatabase("testdb").GetCollection<User>("User").InsertOne(usr);

            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(User usr)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var filter = Builders<User>.Filter.Eq("UserName", usr.UserName);

          //  var update = Builders<User>.Update.Set("UserName", emp.UserName)
          //                                          .Set("PetrolShed", emp.PetrolShed)
          //                                          .Set("PetrolFillStatus", emp.PetrolFillStatus)
          //                                          .Set("PetrolFillQuantity", emp.PetrolFillQuantity);
            var update = Builders<User>.Update.Set("PetrolFillQuantity", usr.PetrolFillQuantity)
                                                .Set("PetrolFillStatus", usr.PetrolFillStatus);

            dbClient.GetDatabase("testdb").GetCollection<User>("User").UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{userName}")]
        public JsonResult Delete(String userName)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            var filter = Builders<User>.Filter.Eq("UserName", userName);


            dbClient.GetDatabase("testdb").GetCollection<User>("User").DeleteOne(filter);

            return new JsonResult("Deleted Successfully");
        }



        [Route("count/{shedId}")]
        [HttpGet]
        public JsonResult GetCountUserInQueue(String shedId)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

           // var filter = "{PetrolFillStatus:join}";

            var dbList = dbClient.GetDatabase("testdb").GetCollection<User>("User").Find(s => s.PetrolFillStatus == "join" && s.PetrolShed == shedId).ToList();

            return new JsonResult(dbList);
           // return new JsonResult(10);
        }



    }
}
