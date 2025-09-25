using Books_Core.Models;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class BooksTests
    {
        [Fact]
        public void Books_Properties_AssignCorrectly()
        {
            var book = new Books
            {
                Id = "1",
                Title = "Test Title",
                Author = "Test Author",
                Price = 99.99M
            };

            Assert.Equal("1", book.Id);
            Assert.Equal("Test Title", book.Title);
            Assert.Equal("Test Author", book.Author);
            Assert.Equal(99.99M, book.Price);
        }

        [Fact]
        public void Books_DefaultValues_ShouldBeNullOrZero()
        {
            var book = new Books();

            Assert.Null(book.Id);
            Assert.Null(book.Title);
            Assert.Null(book.Author);
            Assert.Equal(0, book.Price);
        }

        // Added tests for boundary values and edge cases

        [Fact]
        public void Price_Zero_IsInitialValue()
        {
            var book = new Books();
            Assert.Equal(0, book.Price);
        }

        [Fact]
        public void Can_SetPrice_To_MinimumAllowed()
        {
            var book = new Books { Price = 1 };
            Assert.Equal(1, book.Price);
        }

        [Fact]
        public void Can_SetPrice_To_MaximumAllowed()
        {
            var book = new Books { Price = 5000 };
            Assert.Equal(5000, book.Price);
        }

        [Fact]
        public void Title_Can_Be_EmptyString()
        {
            var book = new Books { Title = string.Empty };
            Assert.Equal(string.Empty, book.Title);
        }

        [Fact]
        public void Author_Can_Be_EmptyString()
        {
            var book = new Books { Author = string.Empty };
            Assert.Equal(string.Empty, book.Author);
        }
    }
}
