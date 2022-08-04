using System.Text.Json.Serialization;

namespace WebApi.Models

{
    public class Book
    { 

        [JsonPropertyName("Id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("image")]
        public string? Image { get; set; }


    }
}
