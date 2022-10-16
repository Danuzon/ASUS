using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
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
        public JsonResult Post(User emp)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("EadAppConnection"));

            int LastUserId = dbClient.GetDatabase("testdb").GetCollection<PetrolShed>("User").AsQueryable().Count();
            emp.UserId = LastUserId + 1;

            dbClient.GetDatabase("testdb").GetCollection<User>("User").InsertOne(emp);

            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(User emp)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("UserAppCon"));

            var filter = Builders<User>.Filter.Eq("UserId", emp.UserId);

            var update = Builders<User>.Update.Set("UserName", emp.UserName)
                                                    .Set("PetrolShed", emp.PetrolShed)
                                                    .Set("PetrolFillStatus", emp.PetrolFillStatus)
                                                    .Set("PetrolFillQuantity", emp.PetrolFillQuantity);

            dbClient.GetDatabase("testdb").GetCollection<User>("User").UpdateOne(filter, update);

            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("UserAppCon"));

            var filter = Builders<User>.Filter.Eq("UserId", id);


            dbClient.GetDatabase("testdb").GetCollection<User>("User").DeleteOne(filter);

            return new JsonResult("Deleted Successfully");
        }


        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }



    }
}
