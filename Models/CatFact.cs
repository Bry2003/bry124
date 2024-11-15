using Newtonsoft.Json;

namespace FoodDelivery4.Models
{
    public class CatFact
    {
        public string Fact { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }
    }
}
