using LibroConsoleAPI.Business;
using LibroConsoleAPI.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.XUnit
{
    public class GetSpecificAsyncMethodTests : IClassFixture<BookManagerFixture>

    {
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;
       
        public GetSpecificAsyncMethodTests(BookManagerFixture fixture)
        {
            fixture = new BookManagerFixture();
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public async Task GetSpecificAsync_WithValidIsbn_ShouldReturnBook()
        {
            // Arrange
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);
            // Act
            var result = await _bookManager.GetSpecificAsync("9780143039655");

            // Assert
            Assert.Equal("9780143039655", result.ISBN);
            Assert.Equal("The God of Small Things", result.Title);
            Assert.Equal("Arundhati Roy", result.Author);
        }

        [Fact]
        public async Task GetSpecificAsync_WithInvalidIsbn_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookManager.GetSpecificAsync("12345678912345"));
        }

        [Fact]
        public async Task GetSpecificAsync_WithEmptyISBN_ShouldThrowArgumentException()
        {
            // Arrange

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _bookManager.GetSpecificAsync(""));
            Assert.Equal("ISBN cannot be empty.", exception.Message);

        }

        [Fact]
        public async Task GetSpecificAsync_WithNullISBN_ShouldThrowArgumentException()
        {
            // Arrange

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _bookManager.GetSpecificAsync(null));
            Assert.Equal("ISBN cannot be empty.", exception.Message);
        }

    }
}
