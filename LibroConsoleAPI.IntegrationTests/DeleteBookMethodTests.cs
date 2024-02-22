using LibroConsoleAPI.Business;
using LibroConsoleAPI.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.XUnit
{


    public class DeleteBookMethodTests : IClassFixture<BookManagerFixture>
    {
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;
        public DeleteBookMethodTests(BookManagerFixture fixture)
        {
            fixture = new BookManagerFixture();
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public async Task DeleteBookAsync_WithValidISBN_ShouldRemoveBookFromDb()
        {
            // въвеждаме една книга и после проверяваме за нея че е изтрита. 
            // Arrange
            //var newBook = new Book
            //{
            //    Title = "My Story",
            //    Author = "John Doe",
            //    ISBN = "1234567891234",
            //    YearPublished = 2021,
            //    Genre = "Fiction",
            //    Pages = 100,
            //    Price = 19.99
            //};
            //await _bookManager.AddAsync(newBook);
            // вмъкваме от JSON целия списк с 10 книги. 
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act
            await _bookManager.DeleteAsync("9780312857753");


            // Assert
            var bookInDb = _dbContext.Books.ToList();
            Assert.Equal(9, bookInDb.Count);
        }
        [Fact]
        public async Task DeleteBookAsync_TryToDeleteWithNullOrWhiteSpaceISBN_ShouldThrowException()
        {

            // Arrange
            // This part sets up the test scenario by seeding the database with 10 books, as you want to test the deletion process.
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _bookManager.DeleteAsync(" "));
            // Assert that the exception message is exactly "ISBN cannot be empty."

            Assert.Equal("ISBN cannot be empty.", exception.Message);

        }

        [Fact]
        public async Task DeleteBookAsync_WithIncorrectButValidISBN_SholdThrowArgumentNullException()
        {
            // Arrange
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Assuming you have a book with a valid ISBN like "12345678912345
            // but it is intentionally modified to be incorrect for this test
            string incorrectButValidISBN = "12345678912345";

            // Act and Assert

            await Assert.ThrowsAsync<ArgumentNullException>(async() => await _bookManager.DeleteAsync(incorrectButValidISBN));

            

        }

    }
}
    
