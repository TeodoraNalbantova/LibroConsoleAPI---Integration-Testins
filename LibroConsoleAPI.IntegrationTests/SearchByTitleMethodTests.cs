using LibroConsoleAPI.Business;
using LibroConsoleAPI.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.XUnit
{
    public class SearchByTitleMethodTests : IClassFixture<BookManagerFixture>

    {
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;

        public SearchByTitleMethodTests(BookManagerFixture fixture)
        {
            fixture = new BookManagerFixture();
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }


        [Fact]
        public async Task SearchByTitleAsync_WithValidTitleFragment_ShouldReturnMatchingBooks()
        {
            // Arrange
            DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act
            var result = await _bookManager.SearchByTitleAsync("Pride and Prejudice");

            // Assert
            Assert.Equal("Pride and Prejudice", result.First().Title);
        }

        [Fact]
        public async Task SearchByTitleAsync_WithInvalidTitleFragment_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);



            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookManager.SearchByTitleAsync("No books found with the given title fragment."));


        }
        [Fact]
        public async Task SearchByTitleAsync_WithEmptyTitleFragment_ShouldThrowArgumentException()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookManager.SearchByTitleAsync(""));
        }

        [Fact]
        public async Task SearchByTitleAsync_WithNullTitleFragment_ShouldThrowArgumentException()
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _bookManager.SearchByTitleAsync(null));
        }

        [Fact]
        public async Task SearchByTitleAsync_WithNonexistentTitleFragment_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _bookManager.SearchByTitleAsync("No books found with the given title fragment."));
        }
        [Fact]
        public async Task SearchByTitleAsync_WithCaseInsensitiveMatching_ShouldReturnMatchingBooks()
        {
            // Arrange
            DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act
            var result = await _bookManager.SearchByTitleAsync("PRIDE AND PREJUDICE");

            // Assert
            Assert.Equal("Pride and Prejudice", result.First().Title);
        }
        [Fact]
        public async Task SearchByTitleAsync_WithPartialWordMatch_ShouldReturnMatchingBooks()
        {
            // Arrange
            DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act
            var result = await _bookManager.SearchByTitleAsync("Pride");

            // Assert
            Assert.True(result.Any()); // Ensure at least one book is returned
            Assert.All(result, book => Assert.Contains("Pride", book.Title, StringComparison.OrdinalIgnoreCase));
            //Second, you check that each returned book's title contains the partial word "Pride" (case-insensitive) using Assert.Contains.
        }
    }
}
