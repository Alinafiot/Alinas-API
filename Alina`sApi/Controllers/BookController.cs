using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Alina_sApi.Model;

namespace Alina_sApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;

        public BookController(ILogger<BookController> logger)
        {
            _logger = logger;
        }

        [HttpGet("genre", Name = "SearchBooksByGenre")]
        public async Task<ActionResult> GetByGenre(string genre, int count)
        {
            if (count > 50)
            {
                return BadRequest(new { error = "Максимальна кількість книг для виведення - 50." });
            }

            if (string.IsNullOrEmpty(genre))
            {
                return BadRequest(new { error = "Назва жанру не може бути порожньою." });
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=subject:{genre}&maxResults=40"); // Отримуємо більше книг, щоб можна було перемішувати

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    if (books == null || books.Items == null)
                    {
                        return NotFound(new { error = "Книги не знайдено." });
                    }

                    var rng = new Random();
                    var shuffledBooks = books.Items.OrderBy(x => rng.Next()).ToList();

                    var bookList = new List<object>();
                    foreach (var item in shuffledBooks.Take(count))
                    {
                        bookList.Add(new
                        {
                            authors = item.VolumeInfo.Authors ?? new List<string> { "Невідомий автор" },
                            title = item.VolumeInfo.Title ?? "Невідома назва"
                        });
                    }

                    return Ok(new { books = bookList });
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, new { error = "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }
        }

        [HttpGet("title", Name = "SearchBooksByTitle")]
        public async Task<ActionResult> GetByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest(new { error = "Назва книги не може бути порожньою." });
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=intitle:{title}");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    if (books == null || books.Items == null)
                    {
                        return NotFound(new { error = "Книги не знайдено." });
                    }

                    var bookList = new List<object>();
                    var counter = 0;
                    foreach (var item in books.Items)
                    {
                        if (counter >= 20)
                            break;

                        bookList.Add(new
                        {
                            authors = item.VolumeInfo.Authors ?? new List<string> { "Невідомий автор" },
                            title = item.VolumeInfo.Title ?? "Невідома назва"
                        });

                        counter++;
                    }

                    return Ok(new { books = bookList });
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, new { error = "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }
        }

        [HttpGet("author", Name = "SearchBooksByAuthor")]
        public async Task<ActionResult> GetByAuthor(string author)
        {
            if (string.IsNullOrEmpty(author))
            {
                return BadRequest(new { error = "Ім'я автора не може бути порожнім." });
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=inauthor:{author}");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    if (books == null || books.Items == null)
                    {
                        return NotFound(new { error = "Книги не знайдено." });
                    }

                    var bookList = new List<object>();
                    var counter = 0;
                    foreach (var item in books.Items)
                    {
                        if (counter >= 20)
                            break;

                        bookList.Add(new
                        {
                            authors = item.VolumeInfo.Authors ?? new List<string> { "Невідомий автор" },
                            title = item.VolumeInfo.Title ?? "Невідома назва"
                        });

                        counter++;
                    }

                    return Ok(new { books = bookList });
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, new { error = "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }
        }

        [HttpGet("randomBooks", Name = "GetRandomBooks")]
        public async Task<ActionResult> GetRandomBooks()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://www.googleapis.com/books/v1/volumes?q=random&maxResults=10");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    if (books == null || books.Items == null)
                    {
                        return NotFound(new { error = "Випадкові книги не знайдено." });
                    }

                    var bookList = new List<object>();
                    foreach (var item in books.Items)
                    {
                        bookList.Add(new
                        {
                            authors = item.VolumeInfo.Authors ?? new List<string> { "Невідомий автор" },
                            title = item.VolumeInfo.Title ?? "Невідома назва"
                        });
                    }

                    return Ok(new { books = bookList });
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, new { error = "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }

        }

        [HttpPut("reviews/{bookName}", Name = "AddOrUpdateReviewForBook")]
        public async Task<ActionResult> AddOrUpdateReviewForBook(string bookName)
        {
            if (string.IsNullOrEmpty(bookName))
            {
                return BadRequest(new { error = "Назва книги має бути вказана." });
            }

            try
            {
                var database = new Database();
                await database.InsertBookAsync(bookName);

                var updatedReview = new
                {
                    BookName = bookName
                };

                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }
        }

        [HttpDelete("reviews/{bookName}", Name = "DeleteReviewForBook")]
        public async Task<ActionResult> DeleteReviewForBook(string bookName)
        {
            if (string.IsNullOrEmpty(bookName))
            {
                return BadRequest(new { error = "Назва книги не може бути порожньою." });
            }

            try
            {
                var database = new Database();
                var success = await database.DeleteReviewForBookAsync(bookName);
                if (success)
                {
                    _logger.LogInformation($"Review for book {bookName} successfully deleted.");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"Review for book {bookName} not found.");
                    return NotFound(new { error = "Книга не знайдена." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }
        }

        [HttpGet("reviews", Name = "GetReviewForBook")]
        public async Task<ActionResult> GetReviewForBook()
        {
            try
            {
                var database = new Database();
                var books = await database.GetBooksAsync();

                return Ok(new { Books = books });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, new { error = "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше." });
            }
        }

    }
}
