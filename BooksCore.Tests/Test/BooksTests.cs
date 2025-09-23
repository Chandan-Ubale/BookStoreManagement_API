using Books_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class BooksTests
    {
        [Fact]
        public void Books_Properties_AssignCorrectly()
        {
            // Arrange & Act
            var book = new Books
            {
                Id = "1",
                Title = "Test Title",
                Author = "Test Author",
                Price = 99.99M
            };

            // Assert
            Assert.Equal("1", book.Id);
            Assert.Equal("Test Title", book.Title);
            Assert.Equal("Test Author", book.Author);
            Assert.Equal(99.99M, book.Price);
        }
    }
}
