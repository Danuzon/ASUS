using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class User
    {
        public ObjectId Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string PetrolShed { get; set; }

        public string PetrolFillStatus { get; set; }

        public float PetrolFillQuantity { get; set; }
    }
}
