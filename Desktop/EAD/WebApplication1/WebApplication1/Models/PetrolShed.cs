using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class PetrolShed
    {
        public ObjectId Id { get; set; }
        public String PetrolShedId { get; set; }
        public string PetrolShedName { get; set; }
        public float AvailableQuantity { get; set; }
    }
}
