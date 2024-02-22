using LibroConsoleAPI.Business;
using LibroConsoleAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.XUnit
{
    public class AddBookMethodTests: IClassFixture<BookManagerFixture>
    {
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;
        public AddBookMethodTests(BookManagerFixture fixture)
        {
            fixture = new BookManagerFixture();
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }

        [Fact]

        // POSITIVE CASE
        public async Task AddBookAsync_ShouldAddBook()
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

            // Act
            await _bookManager.AddAsync(newBook);
            // викаме методите 

            // Assert

            var bookInDb = await _dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == newBook.ISBN);
            Assert.NotNull(bookInDb);
            Assert.Equal(newBook.Title, bookInDb.Title);
            Assert.Equal(newBook.Author, bookInDb.Author);
        }
        // NEGATIVE CASE
        [Fact]
        public async Task AddBookAsync_TryToAddBookWithInvalidTitle_ShouldThrowException()
        {
            // Arrange
            var newBook = new Book
            {
                Title = new string('T', 500),
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            // Act
            var exception = Assert.ThrowsAsync<ValidationException>(() => _bookManager.AddAsync(newBook));


            // Assert
            Assert.Equal("Book is invalid.", exception.Result.Message);
        }
        [Fact]
        public async Task AddBookAsync_TryToAddBookWithInvalidISBNnumber_ShouldThrowException()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "My Story",
                Author = "John Doe",
                ISBN = "Invalid",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            // Act
            var exception = Assert.ThrowsAsync<ValidationException>(() => _bookManager.AddAsync(newBook));


            // Assert
            Assert.Equal("Book is invalid.", exception.Result.Message);
            var bookInDB = await _dbContext.Books.FirstOrDefaultAsync();
            Assert.Null(bookInDB);
        }


    }
}
