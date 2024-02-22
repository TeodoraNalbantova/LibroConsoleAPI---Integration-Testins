using LibroConsoleAPI.Business;
using LibroConsoleAPI.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.XUnit
{
    public class GetAllBooksMethodTests : IClassFixture<BookManagerFixture>

    {
    
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;
        
        public GetAllBooksMethodTests(BookManagerFixture fixture)
        {
            fixture = new BookManagerFixture();
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }


        [Fact]
        public async Task GetAllAsync_WhenBooksExist_ShouldReturnAllBooks()
        {
            // Arrange
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act
            var result = await _bookManager.GetAllAsync();

            // Assert
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenNoBooksExist_ShouldThrowKeyNotFoundException()
        {

            // Act & Assert

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookManager.GetAllAsync());
            Assert.Equal("No books found.", exception.Message);


        }



    }
}
