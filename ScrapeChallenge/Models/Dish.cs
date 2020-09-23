using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ScrapeChallenge.Models
{
    public class Dish
    {
        [BsonElement("_id")]
        [JsonProperty("_id")]
        [BsonId]
        public int Id { get; set; }
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }
    }
}
