using LibroConsoleAPI.Business;
using LibroConsoleAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.XUnit
{
    public class UpdateBookMethodTests: IClassFixture<BookManagerFixture>

    {
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;
       
        public UpdateBookMethodTests(BookManagerFixture fixture)
        {
            fixture = new BookManagerFixture();
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public async Task UpdateAsync_WithValidBook_ShouldUpdateBook()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            await _bookManager.AddAsync(newBook);
            newBook.Price = 25;

            // Act
            await _bookManager.UpdateAsync(newBook);

            // Assert
            var updatedBook = _dbContext.Books.FirstOrDefault(b => b.Price == newBook.Price);
            Assert.Equal(25,updatedBook.Price);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidBook_ShouldThrowValidationException()
        {
            // Arrange
            var invalidBook = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "123456789012345",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            // Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _bookManager.UpdateAsync(invalidBook));
            Assert.Equal("Book is invalid.", exception.Message);




        }


        [Fact]
        public async Task UpdateAsync_WithNullBook_ShouldThrowAValidationException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _bookManager.UpdateAsync(null));
            Assert.Equal("Book is invalid.", exception.Message);
        }

    }


}
