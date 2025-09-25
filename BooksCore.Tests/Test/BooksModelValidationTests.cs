using Books_Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace BooksCore.Tests.Test
{
    public class BooksModelValidationTests
    {
        // Positive test: all valid properties
        [Fact]
        public void AllValid_Properties_PassesValidation()
        {
            var book = new Books { Title = "Valid Title", Author = "Valid Author", Price = 100 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.True(isValid);
            Assert.Empty(results);
        }

        // Negative test: Title required validation fails when null
        [Fact]
        public void Title_Null_FailsValidation()
        {
            var book = new Books { Title = null, Author = "Author", Price = 99 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Title is required"));
        }

        // Negative test: Title exceeding max length fails validation
        [Fact]
        public void Title_ExceedsMaxLength_FailsValidation()
        {
            var book = new Books { Title = new string('A', 101), Author = "Author", Price = 99 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Title cannot exceed 100 characters"));
        }

        // Negative test: Author null fails required validation
        [Fact]
        public void Author_Null_FailsValidation()
        {
            var book = new Books { Title = "Title", Author = null, Price = 99 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Author is required"));
        }

        // Negative test: Author exceeding max length fails validation
        [Fact]
        public void Author_ExceedsMaxLength_FailsValidation()
        {
            var book = new Books { Title = "Title", Author = new string('Z', 51), Price = 99 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Author cannot exceed 50 characters"));
        }

        // Negative test: Price below range fails validation
        [Fact]
        public void Price_BelowRange_FailsValidation()
        {
            var book = new Books { Title = "Title", Author = "Author", Price = 0 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Price must be between 1 and 5000"));
        }

        // Negative test: Price above range fails validation
        [Fact]
        public void Price_AboveRange_FailsValidation()
        {
            var book = new Books { Title = "Title", Author = "Author", Price = 6000 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Price must be between 1 and 5000"));
        }

        // Neutral test: Title minimum valid length
        [Fact]
        public void Title_MinLength_PassesValidation()
        {
            var book = new Books { Title = "A", Author = "Author", Price = 10 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.True(isValid);
        }

        // Neutral test: Author maximum valid length
        [Fact]
        public void Author_MaxLength_PassesValidation()
        {
            var book = new Books { Title = "Title", Author = new string('Z', 50), Price = 10 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.True(isValid);
        }

        // Neutral test: Price lower boundary passes validation
        [Fact]
        public void Price_LowerBoundary_PassesValidation()
        {
            var book = new Books { Title = "Title", Author = "Author", Price = 1 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.True(isValid);
        }

        // Neutral test: Price upper boundary passes validation
        [Fact]
        public void Price_UpperBoundary_PassesValidation()
        {
            var book = new Books { Title = "Title", Author = "Author", Price = 5000 };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);

            bool isValid = Validator.TryValidateObject(book, context, results, true);

            Assert.True(isValid);
        }
    }
}
