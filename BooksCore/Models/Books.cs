using Books_Core.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Books_Core.Models
{
    public class Books : IBooks
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [BsonElement("Author")]
        [Required(ErrorMessage = "Author is required.")]
        [StringLength(50, ErrorMessage = "Author cannot exceed 50 characters.")]
        public string? Author { get; set; }

        [BsonElement("Price")]
        [Range(1, 5000, ErrorMessage = "Price must be between 1 and 5000.")]
        public decimal Price { get; set; }
    }
}
