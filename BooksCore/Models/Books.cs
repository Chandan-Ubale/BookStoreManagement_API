using Books_Core.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Books_Core.Models
{
    public class Books :IBooks
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("Author")]
        public string? Author { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }
    }
}
