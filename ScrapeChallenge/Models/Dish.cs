using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ScrapeChallenge.Models
{
    // Dish
    public class Dish
    {
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }
        public bool HasError { get; set; }
    }
}
