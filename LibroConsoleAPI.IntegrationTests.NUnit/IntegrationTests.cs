using LibroConsoleAPI.Business;
using LibroConsoleAPI.Business.Contracts;
using LibroConsoleAPI.Data.Models;
using LibroConsoleAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibroConsoleAPI.IntegrationTests.NUnit
{
    public class IntegrationTests
    {
        private TestLibroDbContext dbContext;
        private IBookManager bookManager;

        [SetUp]
        public void SetUp()
        {
            string dbName = $"TestDb_{Guid.NewGuid()}";
            this.dbContext = new TestLibroDbContext(dbName);
            this.bookManager = new BookManager(new BookRepository(this.dbContext));
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Dispose();
        }


        [Test]
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
            await bookManager.AddAsync(newBook);

            // Assert
            var bookInDb = await dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == newBook.ISBN);
            Assert.NotNull(bookInDb);
            Assert.That(bookInDb.Title, Is.EqualTo("Test Book"));
            Assert.That(bookInDb.Author, Is.EqualTo("John Doe"));
        }
        [Test]
        public async Task AddBookAsync_TryToAddBookWithInvalidPages_ShouldThrowException()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = -100,
                Price = 19.99
            };

            // Act
            var exception = Assert.ThrowsAsync<ValidationException>(() => bookManager.AddAsync(newBook));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Book is invalid."));

        }

        [Test]
        public async Task DeleteBookAsync_WithValidISBN_ShouldRemoveBookFromDb()
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

            await bookManager.AddAsync(newBook);
            // Act
            await bookManager.DeleteAsync(newBook.ISBN);
            // Assert
            var bookInDb = await dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == newBook.ISBN);
            Assert.IsNull(bookInDb);
        }

        [Test]
        public async Task DeleteBookAsync_TryToDeleteWithNullOrWhiteSpaceISBN_ShouldThrowException()
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

            await bookManager.AddAsync(newBook);
            // Act
            var exception = Assert.ThrowsAsync<ArgumentException>(() => bookManager.DeleteAsync(null));
            // Assert
            Assert.That(exception.Message, Is.EqualTo("ISBN cannot be empty."));

        }

        [Test]
        public async Task GetAllAsync_WhenBooksExist_ShouldReturnAllBooks()
        {
            // Arrange
            var newBook1 = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            var newBook2 = new Book
            {
                Title = "Test Book2",
                Author = "Jack King",
                ISBN = "1234567890124",
                YearPublished = 2022,
                Genre = "Drama",
                Pages = 200,
                Price = 25.00
            };

            await bookManager.AddAsync(newBook1);
            await bookManager.AddAsync(newBook2);
            // Act
            var result = await bookManager.GetAllAsync();
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            Assert.That(result, Is.EquivalentTo(new[] { newBook1, newBook2 }));
            //Use CollectionEquivalentTo for a more concise assertion.Using Is.EquivalentTo for the entire collection simplifies the assertions. It checks if the two collections contain the same elements regardless of their order. This approach is especially useful when dealing with multiple properties in your objects.


        }

        [Test]
        public async Task GetAllAsync_WhenNoBooksExist_ShouldThrowKeyNotFoundException()
        {

            // Act
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => bookManager.GetAllAsync());
            // Assert
            Assert.That(exception.Message, Is.EqualTo("No books found."));
        }

        [Test]
        public async Task SearchByTitleAsync_WithValidTitleFragment_ShouldReturnMatchingBooks()
        {
            // Arrange
            var newBook1 = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            var newBook2 = new Book
            {
                Title = "Book2",
                Author = "Jack King",
                ISBN = "1234567890124",
                YearPublished = 2022,
                Genre = "Drama",
                Pages = 200,
                Price = 25.00
            };

            await bookManager.AddAsync(newBook1);
            await bookManager.AddAsync(newBook2);
            // Act
            var result = await bookManager.SearchByTitleAsync(newBook2.Title);
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Book2"));
        }

        [Test]
        public async Task SearchByTitleAsync_WithInvalidTitleFragment_ShouldThrowKeyNotFoundException()
        {

            // Act
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => bookManager.SearchByTitleAsync("InvalidTitleFragment"));


            // Assert
            Assert.That(exception.Message, Is.EqualTo("No books found with the given title fragment."));
        }

        [Test]
        public async Task GetSpecificAsync_WithValidIsbn_ShouldReturnBook()
        {
            // Arrange
            var newBook1 = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            var newBook2 = new Book
            {
                Title = "Book2",
                Author = "Jack King",
                ISBN = "1234567890124",
                YearPublished = 2022,
                Genre = "Drama",
                Pages = 200,
                Price = 25.00
            };

            await bookManager.AddAsync(newBook1);
            await bookManager.AddAsync(newBook2);
            // Act
            var result = await bookManager.GetSpecificAsync(newBook1.ISBN);
            // Assert
            Assert.NotNull(result);
            Assert.That(result.ISBN, Is.EqualTo(newBook1.ISBN));
            Assert.That(result.Title, Is.EqualTo(newBook1.Title));

        }

        [Test]
        public async Task GetSpecificAsync_WithInvalidIsbn_ShouldThrowKeyNotFoundException()
        {

            // Act
            var exception = Assert.ThrowsAsync<KeyNotFoundException>(() => bookManager.GetSpecificAsync("125"));
            // Assert
            Assert.That(exception.Message, Is.EqualTo("No book found with ISBN: 125"));
        }

        [Test]
        public async Task UpdateAsync_WithValidBook_ShouldUpdateBook()
        {
            // Arrange
            var newBook1 = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            var newBook2 = new Book
            {
                Title = "Book2",
                Author = "Jack King",
                ISBN = "1234567890124",
                YearPublished = 2022,
                Genre = "Drama",
                Pages = 200,
                Price = 25.00
            };

            await bookManager.AddAsync(newBook1);
            await bookManager.AddAsync(newBook2);

            var bookToUpdate = newBook2;
            bookToUpdate.Title = "UPDATEDtitle";
            // Act
            await bookManager.UpdateAsync(bookToUpdate);
            // Assert
            var updatedBook = await bookManager.GetSpecificAsync(bookToUpdate.ISBN);
            Assert.NotNull(updatedBook);
            Assert.That(updatedBook.Title, Is.EqualTo(bookToUpdate.Title));



        }

        [Test]
        public async Task UpdateAsync_WithInvalidBook_ShouldThrowValidationException()
        {
            // Arrange
            var newBook1 = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            await bookManager.AddAsync(newBook1);
            var invalidBook = new Book
            {
                // Invalid data, e.g., setting YearPublished to an invalid value
                Title = "Invalid Book",
                Author = "Invalid Author",
                ISBN = "1234567890123",
                YearPublished = 1500,  // Invalid value for the test
                Genre = "Invalid Genre",
                Pages = -10,  // Invalid value for the test
                Price = -5.0  // Invalid value for the test
            };
            // Act & Assert
            var exception = Assert.ThrowsAsync<ValidationException>(() => bookManager.UpdateAsync(invalidBook));

            Assert.That(exception.Message, Is.EqualTo("Book is invalid."));

        }
    }
}
